using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
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

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _authService = authService;
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

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailModel model)
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