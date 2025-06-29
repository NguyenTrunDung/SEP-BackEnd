using HOMMS.Domain.Entities;
using Asp.Versioning;

using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using System.Threading.Tasks;
using HOMMS.Infrastructure.Data;
using HOMMS.Common.Constants;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using HOMMS.Infrastructure.Seeds;
using Microsoft.Extensions.DependencyInjection;

namespace HOMMS.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IEmailVerifyService _emailVerifyService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IAuthService authService,IEmailVerifyService emailVerifyService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _authService = authService;
            _emailVerifyService = emailVerifyService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign default role
            await _userManager.AddToRoleAsync(user, "Staff");

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

             
            //Send Email
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new { token, email = user.Email }, Request.Scheme);
            var message = new MessageDto(  user.Email , "Confirmation email link", $"<h2>Xác nhận tài khoản</h2><p>Nhấn vào link dưới đây để xác nhận:</p><a href='{confirmationLink}'>Xác nhận email</a>");

            await _emailVerifyService.SendEmailAsync(message);



            // In a real application, send email with confirmation link
            // For demo purposes, we'll just return the token
            return Ok(new { Message = "User registered successfully! Please confirm your email.", Token = token });
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseBase<LoginResponseDto>>> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseBase<LoginResponseDto>.Error("Invalid request data"));

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(ApiResponseBase<LoginResponseDto>.Error("Invalid login attempt."));

            // Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest(ApiResponseBase<LoginResponseDto>.Error("Email not confirmed. Please confirm your email before logging in."));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return BadRequest(ApiResponseBase<LoginResponseDto>.Error("Invalid login attempt."));

            // Generate tokens (without embedded permissions)
            var accessToken = await _authService.GenerateAccessTokenAsync(user);
            var refreshToken = _authService.GenerateRefreshToken();
            
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");
            var refreshExpiryInDays = int.Parse(jwtSettings["RefreshExpiryInDays"] ?? "7");
            
            var tokenExpiryTime = DateTime.UtcNow.AddMinutes(expiryInMinutes);
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshExpiryInDays);

            // Update user's refresh token in database
            await _authService.UpdateUserRefreshTokenAsync(user, refreshToken, refreshTokenExpiryTime);

            // Build complete login response with permissions from database
            var loginResponse = await _authService.BuildLoginResponseAsync(
                user, accessToken, refreshToken, tokenExpiryTime, refreshTokenExpiryTime);

            return Ok(ApiResponseBase<LoginResponseDto>.Success(loginResponse, "Login successful"));
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
                return BadRequest("User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);

            if (!result.Succeeded)
                return BadRequest("Email confirmation failed.");
             
            return Ok("Email confirmed successfully. You can now login.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return Ok("If your email is registered and confirmed, you will receive a password reset link.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // In a real application, send email with reset link
            // For demo purposes, we'll just return the token
            return Ok(new { Message = "Password reset token generated successfully.", Token = token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
                return BadRequest("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password reset successful. You can now login with your new password.");
        }

        //-------------------------------------------------------------//

        //Login with google
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        [AllowAnonymous]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded || result.Principal == null)
                return Unauthorized("Google login failed");

            var claims = result.Principal.Claims;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? "";
            var lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? "";

            if (string.IsNullOrEmpty(email))
                return BadRequest("Unable to retrieve email from Google.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return BadRequest("Failed to create user from Google login.");

                await _userManager.AddToRoleAsync(user, "Customer");
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            // Lấy permission
            var userRoles = await _userManager.GetRolesAsync(user);
            var allPermissions = new HashSet<string>();

            if (userRoles.Contains("Manager"))
            {
                allPermissions = new HashSet<string>(PermissionConstants.All);
            }
            else
            {
                var userBranchRoles = await _context.BranchUserRoles
                    .Where(bur => bur.UserId == user.Id)
                    .Include(bur => bur.BranchRole)
                    .ToListAsync();

                foreach (var role in userBranchRoles)
                {
                    if (!string.IsNullOrEmpty(role.BranchRole?.Permissions))
                    {
                        foreach (var perm in role.BranchRole.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            allPermissions.Add(perm.Trim());
                        }
                    }
                }
            }

            // Tạo access token
            var accessToken = await _authService.GenerateAccessTokenAsync(user);
            var refreshToken = _authService.GenerateRefreshToken();

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");
            var refreshExpiryInDays = int.Parse(jwtSettings["RefreshExpiryInDays"] ?? "7");

            var tokenExpiryTime = DateTime.UtcNow.AddMinutes(expiryInMinutes);
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshExpiryInDays);

            // Cập nhật refresh token
            await _authService.UpdateUserRefreshTokenAsync(user, refreshToken, refreshTokenExpiryTime);

            // Xây dựng response giống login thường
            var loginResponse = await _authService.BuildLoginResponseAsync(
                user, accessToken, refreshToken, tokenExpiryTime, refreshTokenExpiryTime);

            return Ok(ApiResponseBase<LoginResponseDto>.Success(loginResponse, "Login with Google successful"));
        }

        //edit profile
        [Authorize]
        [HttpPost("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User not found.");

            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (!string.IsNullOrEmpty(model.Address))
                user.Address = model.Address;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(model.ProfilePictureUrl))
                user.ProfilePictureUrl = model.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Profile updated successfully.");
        }

        public class EditProfileModel
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Address { get; set; }
            public string? PhoneNumber { get; set; }
            public string? ProfilePictureUrl { get; set; }
        }

        //-------------------------------------------------//

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            if (User.Identity?.Name == null)
                return Unauthorized("User is not authenticated.");

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return NotFound();

            return Ok(new UserProfileModel
            {
                Email = user.Email ?? string.Empty, // Fix for CS8601: Ensure non-null value
                FirstName = user.FirstName ?? string.Empty, // Fix for CS8601: Ensure non-null value
                LastName = user.LastName ?? string.Empty, // Fix for CS8601: Ensure non-null value
                Address = user.Address, // Nullable, no fix needed
                PhoneNumber = user.PhoneNumber, // Nullable, no fix needed
                ProfilePictureUrl = user.ProfilePictureUrl // Nullable, no fix needed
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponseBase<RefreshTokenResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseBase<RefreshTokenResponseDto>.Error("Invalid request data"));

            var result = await _authService.RefreshTokenAsync(model);
            if (result.Status == "error")
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponseBase<object>>> Logout()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest(ApiResponseBase<object>.Error("Invalid user"));

            var success = await _authService.RevokeRefreshTokenAsync(userId);
            if (!success)
                return BadRequest(ApiResponseBase<object>.Error("Logout failed"));

            return Ok(ApiResponseBase<object>.Success(null, "Logout successful"));
        }

        [Authorize]
        [HttpPost("select-branch")]
        public async Task<ActionResult<ApiResponseBase<SelectBranchResponseDto>>> SelectBranch([FromBody] SelectBranchModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(ApiResponseBase<SelectBranchResponseDto>.Error("User not found"));

            // Check if user is Admin System (global or for any branch)
            var isAdminSystem = await _userManager.IsInRoleAsync(user, "SystemAdmin");
            if (isAdminSystem)
            {
                return BadRequest(ApiResponseBase<SelectBranchResponseDto>.Error("Admin System should use the dashboard dropdown to switch branches. This endpoint is not required."));
            }

            // Check if user has a BranchUserRole for the selected branch
            var branchUserRole = await _context.BranchUserRoles
                .Include(bur => bur.Branch)
                .Include(bur => bur.BranchRole)
                .Where(bur => bur.UserId == user.Id && 
                             bur.BranchId == model.BranchId &&
                             !bur.IsDeleted &&
                             bur.Branch != null && bur.Branch.IsActive &&
                             bur.BranchRole != null && !bur.BranchRole.IsDeleted)
                .FirstOrDefaultAsync();

            if (branchUserRole == null)
                return Forbid("User does not have access to this branch");

            // Generate new token with branch context
            var accessToken = await _authService.GenerateAccessTokenAsync(user, model.BranchId, branchUserRole.BranchRoleId);
            
            // Get branch permissions
            var branchPermissions = await _authService.GetUserBranchPermissionsAsync(user.Id, model.BranchId);

            var response = new SelectBranchResponseDto
            {
                AccessToken = accessToken,
                SelectedBranch = new BranchDto
                {
                    Id = branchUserRole.Branch!.Id,
                    Name = branchUserRole.Branch.Name,
                    Code = branchUserRole.Branch.Code,
                    Address = branchUserRole.Branch.Address,
                    Phone = branchUserRole.Branch.Phone,
                    Email = branchUserRole.Branch.Email,
                    Description = branchUserRole.Branch.Description,
                    IsActive = branchUserRole.Branch.IsActive,
                    CreatedAt = branchUserRole.Branch.CreatedAt
                },
                BranchRole = new UserBranchRoleDto
                {
                    BranchId = branchUserRole.BranchId ?? 0,
                    BranchName = branchUserRole.Branch.Name,
                    BranchCode = branchUserRole.Branch.Code,
                    BranchRoleId = branchUserRole.BranchRoleId,
                    BranchRoleName = branchUserRole.BranchRole!.Name,
                    BranchPermissions = branchPermissions,
                    AssignedAt = branchUserRole.CreatedAt
                },
                AvailablePermissions = branchPermissions
            };

            return Ok(ApiResponseBase<SelectBranchResponseDto>.Success(response, "Branch selected successfully"));
        }
    }
}
