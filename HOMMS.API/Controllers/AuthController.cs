using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HOMMS.Infrastructure.Data;
using HOMMS.Common.Constants;

namespace HOMMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
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
            await _userManager.AddToRoleAsync(user, "User");

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // In a real application, send email with confirmation link
            // For demo purposes, we'll just return the token
            return Ok(new { Message = "User registered successfully! Please confirm your email.", Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Invalid login attempt.");

            // Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email not confirmed. Please confirm your email before logging in.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return BadRequest("Invalid login attempt.");

            // --- Gather all permissions for the user ---
            var userRoles = await _userManager.GetRolesAsync(user);
            var allPermissions = new HashSet<string>();

            if (userRoles.Contains("Admin"))
            {
                // Admin System: add ALL permissions in the system
                allPermissions = new HashSet<string>(PermissionConstants.All);
            }
            else
            {
                var userBranchRoles = _context.BranchUserRoles
                    .Where(bur => bur.UserId == user.Id)
                    .Select(bur => bur.BranchRole)
                    .ToList();

                foreach (var role in userBranchRoles)
                {
                    if (!string.IsNullOrEmpty(role.Permissions))
                    {
                        foreach (var perm in role.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            allPermissions.Add(perm.Trim());
                        }
                    }
                }
            }

            // Generate JWT token with all permissions
            var token = await GenerateJwtToken(user, null, null, null, string.Join(",", allPermissions));

            return Ok(new { Token = token });
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
            
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
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


     








        [Authorize]
        [HttpPost("select-branch")]
        public async Task<IActionResult> SelectBranch([FromBody] SelectBranchModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Check if user is Admin System (global or for any branch)
            var isAdminSystem = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdminSystem)
            {
                return BadRequest("Admin System should use the dashboard dropdown to switch branches. This endpoint is not required.");
            }

            string permissions = null;
            string branchRoleName = null;
            int? branchRoleId = null;

            // Check if user has a BranchUserRole for the selected branch
            var branchUserRole = _context.BranchUserRoles
                .Where(bur => bur.UserId == user.Id && bur.BranchId == model.BranchId)
                .Select(bur => new
                {
                    bur.BranchId,
                    bur.BranchRoleId,
                    bur.BranchRole.Name,
                    bur.BranchRole.Permissions
                })
                .FirstOrDefault();

            if (branchUserRole == null)
                return Forbid();

            branchRoleId = branchUserRole.BranchRoleId;
            branchRoleName = branchUserRole.Name;
            permissions = branchUserRole.Permissions;

            var token = await GenerateJwtToken(user, model.BranchId, branchRoleId, branchRoleName, permissions);
            return Ok(new { Token = token });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user, int? branchId = null, int? branchRoleId = null, string branchRoleName = null, string permissions = null)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // Add roles as claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add branch and permission claims if provided
            if (branchId.HasValue)
                claims.Add(new Claim("branch_id", branchId.Value.ToString()));
            if (branchRoleId.HasValue)
                claims.Add(new Claim("branch_role_id", branchRoleId.Value.ToString()));
            if (!string.IsNullOrEmpty(branchRoleName))
                claims.Add(new Claim("branch_role_name", branchRoleName));
            if (!string.IsNullOrEmpty(permissions))
            {
                var perms = permissions.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var perm in perms)
                {
                    claims.Add(new Claim("permission", perm.Trim()));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(expiryInMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ConfirmEmailModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class UserProfileModel
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }

    public class SelectBranchModel
    {
        public int BranchId { get; set; }
    }
} 