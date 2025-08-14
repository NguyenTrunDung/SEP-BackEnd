using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HOMMS.Application.Implementations
{
    /// <summary>
    /// Authentication service implementation handling JWT tokens, refresh tokens, and user permissions
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> GenerateAccessTokenAsync(ApplicationUser user, int? branchId = null, int? branchRoleId = null)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
            var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("UserId", user.Id)
            };

            // Add roles as claims (but NOT permissions)
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add current branch context if provided
            if (branchId.HasValue)
                claims.Add(new Claim("branch_id", branchId.Value.ToString()));
            if (branchRoleId.HasValue)
                claims.Add(new Claim("branch_role_id", branchRoleId.Value.ToString()));

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

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<ApplicationUser?> ValidateRefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && 
                                         u.RefreshTokenExpiryTime > DateTime.UtcNow);
            return user;
        }

        public async Task<ApiResponseBase<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            try
            {
                var principal = ValidateAccessToken(request.AccessToken);
                if (principal == null)
                    return ApiResponseBase<RefreshTokenResponseDto>.Error("Invalid access token");

                var userId = principal.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return ApiResponseBase<RefreshTokenResponseDto>.Error("Invalid token claims");

                var user = await ValidateRefreshTokenAsync(request.RefreshToken);
                if (user == null || user.Id != userId)
                    return ApiResponseBase<RefreshTokenResponseDto>.Error("Invalid refresh token");

                // Generate new tokens
                var branchIdClaim = principal.FindFirst("branch_id")?.Value;
                var branchRoleIdClaim = principal.FindFirst("branch_role_id")?.Value;
                int? branchId = int.TryParse(branchIdClaim, out var bid) ? bid : null;
                int? branchRoleId = int.TryParse(branchRoleIdClaim, out var brid) ? brid : null;

                var newAccessToken = await GenerateAccessTokenAsync(user, branchId, branchRoleId);
                var newRefreshToken = GenerateRefreshToken();
                
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");
                var refreshExpiryInDays = int.Parse(jwtSettings["RefreshExpiryInDays"] ?? "7");
                
                var tokenExpiryTime = DateTime.UtcNow.AddMinutes(expiryInMinutes);
                var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshExpiryInDays);

                // Update refresh token in database
                await UpdateUserRefreshTokenAsync(user, newRefreshToken, refreshTokenExpiryTime);

                var response = new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    TokenExpiryTime = tokenExpiryTime,
                    RefreshTokenExpiryTime = refreshTokenExpiryTime
                };

                return ApiResponseBase<RefreshTokenResponseDto>.Success(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseBase<RefreshTokenResponseDto>.Error($"Token refresh failed: {ex.Message}");
            }
        }

        public async Task<UserPermissionSummaryDto> GetUserPermissionsAsync(string userId)
        {
            var summary = new UserPermissionSummaryDto { UserId = userId };

            // Check if user is system admin
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                summary.IsSystemAdmin = roles.Contains("SystemAdmin");

                if (summary.IsSystemAdmin)
                {
                    // Get all available permissions from all branch roles
                    var allPermissions = await _context.BranchRoles
                        .Where(br => !br.IsDeleted && !string.IsNullOrEmpty(br.Permissions))
                        .Select(br => br.Permissions)
                        .ToListAsync();

                    var uniquePermissions = new HashSet<string>();
                    foreach (var permissionString in allPermissions)
                    {
                        var permissions = permissionString.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var permission in permissions)
                        {
                            uniquePermissions.Add(permission.Trim());
                        }
                    }
                    summary.GlobalPermissions = uniquePermissions.ToList();
                }
                else
                {
                    // Get permissions from branch roles
                    var userBranchRoles = await _context.BranchUserRoles
                        .Include(bur => bur.BranchRole)
                        .Include(bur => bur.Branch)
                        .Where(bur => bur.UserId == userId && 
                                     !bur.IsDeleted &&
                                     bur.BranchRole != null && !bur.BranchRole.IsDeleted &&
                                     bur.Branch != null && bur.Branch.IsActive)
                        .ToListAsync();

                    var allPermissions = new HashSet<string>();
                    foreach (var userBranchRole in userBranchRoles)
                    {
                        if (!string.IsNullOrEmpty(userBranchRole.BranchRole?.Permissions))
                        {
                            var branchPermissions = userBranchRole.BranchRole.Permissions
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => p.Trim())
                                .ToList();

                            foreach (var permission in branchPermissions)
                            {
                                allPermissions.Add(permission);
                            }

                            // Add to branch-specific permissions
                            if (userBranchRole.BranchId.HasValue)
                            {
                                summary.BranchPermissions[userBranchRole.BranchId.Value] = branchPermissions;
                            }
                        }
                    }
                    summary.GlobalPermissions = allPermissions.ToList();
                }

                summary.BranchRoles = await GetUserBranchRolesAsync(userId);
            }

            return summary;
        }

        public async Task<List<string>> GetUserBranchPermissionsAsync(string userId, int branchId)
        {
            var permissions = new List<string>();

            // Check if user is system admin
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("SystemAdmin"))
                {
                    // Get all permissions from the branch's roles
                    var branchPermissions = await _context.BranchRoles
                        .Where(br => br.BranchId == branchId && !br.IsDeleted && !string.IsNullOrEmpty(br.Permissions))
                        .Select(br => br.Permissions)
                        .ToListAsync();

                    var uniquePermissions = new HashSet<string>();
                    foreach (var permissionString in branchPermissions)
                    {
                        var perms = permissionString.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var permission in perms)
                        {
                            uniquePermissions.Add(permission.Trim());
                        }
                    }
                    return uniquePermissions.ToList();
                }
                else
                {
                    // Get user's specific permissions for this branch
                    var userBranchRole = await _context.BranchUserRoles
                        .Include(bur => bur.BranchRole)
                        .Where(bur => bur.UserId == userId && 
                                     bur.BranchId == branchId && 
                                     !bur.IsDeleted &&
                                     bur.BranchRole != null && !bur.BranchRole.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (userBranchRole?.BranchRole?.Permissions != null)
                    {
                        permissions = userBranchRole.BranchRole.Permissions
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .ToList();
                    }
                }
            }

            return permissions;
        }

        public async Task<List<UserBranchRoleDto>> GetUserBranchRolesAsync(string userId)
        {
            var userBranchRoles = await _context.BranchUserRoles
                .Include(bur => bur.Branch)
                .Include(bur => bur.BranchRole)
                .Where(bur => bur.UserId == userId && 
                             !bur.IsDeleted &&
                             bur.Branch != null && bur.Branch.IsActive &&
                             bur.BranchRole != null && !bur.BranchRole.IsDeleted)
                .ToListAsync();

            return userBranchRoles.Select(bur => new UserBranchRoleDto
            {
                BranchId = bur.BranchId ?? 0,
                BranchName = bur.Branch!.Name,
                BranchCode = bur.Branch.Code,
                BranchRoleId = bur.BranchRoleId,
                BranchRoleName = bur.BranchRole!.Name,
                BranchPermissions = !string.IsNullOrEmpty(bur.BranchRole.Permissions) 
                    ? bur.BranchRole.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim()).ToList()
                    : new List<string>(),
                IsDefault = false, // You might want to implement default branch logic
                AssignedAt = bur.CreatedAt
            }).ToList();
        }

        public async Task<BranchDto?> GetUserDefaultBranchAsync(string userId)
        {
            // Get user's first branch or implement default branch logic
            var branchUserRole = await _context.BranchUserRoles
                .Include(bur => bur.Branch)
                .ThenInclude(b => b!.Manager)
                .Where(bur => bur.UserId == userId && 
                             !bur.IsDeleted &&
                             bur.Branch != null && bur.Branch.IsActive)
                .FirstOrDefaultAsync();

            if (branchUserRole?.Branch == null)
                return null;

            var branch = branchUserRole.Branch;
            return new BranchDto
            {
                Id = branch.Id,
                Name = branch.Name,
                Code = branch.Code,
                Address = branch.Address,
                Phone = branch.Phone,
                Email = branch.Email,
                Description = branch.Description,
                IsActive = branch.IsActive,
                ManagerName = branch.Manager != null ? $"{branch.Manager.FirstName} {branch.Manager.LastName}" : null,
                CreatedAt = branch.CreatedAt
            };
        }

        public async Task<bool> UpdateUserRefreshTokenAsync(ApplicationUser user, string refreshToken, DateTime expiryTime)
        {
            try
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = expiryTime;
                user.LastModifiedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RevokeRefreshTokenAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    user.LastModifiedAt = DateTime.UtcNow;

                    var result = await _userManager.UpdateAsync(user);
                    return result.Succeeded;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public ClaimsPrincipal? ValidateAccessToken(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured");
                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, // Don't validate lifetime for refresh scenarios
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task<LoginResponseDto> BuildLoginResponseAsync(
            ApplicationUser user, 
            string accessToken, 
            string refreshToken, 
            DateTime tokenExpiryTime, 
            DateTime refreshTokenExpiryTime)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userPermissions = await GetUserPermissionsAsync(user.Id);
            var userBranches = await GetUserBranchRolesAsync(user.Id);
            var defaultBranch = await GetUserDefaultBranchAsync(user.Id);

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiryTime = tokenExpiryTime,
                RefreshTokenExpiryTime = refreshTokenExpiryTime,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = userRoles.ToList()
                },
                Permissions = userPermissions.GlobalPermissions,
                UserBranches = userBranches,
                DefaultBranch = defaultBranch,
                IsSystemAdmin = userPermissions.IsSystemAdmin
            };

            return response;
        }

        /// <summary>
        /// Validates if a user has access to a specific branch
        /// </summary>
        public async Task<bool> ValidateUserBranchAccessAsync(string userId, int? branchId)
        {
            try
            {
                if (!branchId.HasValue)
                    return false;

                // Check if user is system admin (can access all branches)
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("SystemAdmin"))
                    {
                        return true; // System admin can access any branch
                    }
                }

                // Check if user has access to the specific branch
                var userBranchRole = await _context.BranchUserRoles
                    .Include(bur => bur.Branch)
                    .Include(bur => bur.BranchRole)
                    .Where(bur => bur.UserId == userId &&
                                 bur.BranchId == branchId &&
                                 !bur.IsDeleted &&
                                 bur.Branch != null && bur.Branch.IsActive &&
                                 bur.BranchRole != null && !bur.BranchRole.IsDeleted)
                    .FirstOrDefaultAsync();

                return userBranchRole != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates if a user has access to any branch (for internal login)
        /// </summary>
        public async Task<bool> ValidateUserHasAnyBranchAccessAsync(string userId)
        {
            try
            {
                // Check if user is system admin (can access all branches)
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("SystemAdmin"))
                    {
                        return true; // System admin can access any branch
                    }
                }

                // Check if user has access to any branch
                var hasAnyBranchAccess = await _context.BranchUserRoles
                    .Include(bur => bur.Branch)
                    .Include(bur => bur.BranchRole)
                    .Where(bur => bur.UserId == userId &&
                                 !bur.IsDeleted &&
                                 bur.Branch != null && bur.Branch.IsActive &&
                                 bur.BranchRole != null && !bur.BranchRole.IsDeleted)
                    .AnyAsync();

                return hasAnyBranchAccess;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Gets all active branches (for system admin)
        /// </summary>
        private async Task<List<BranchDto>> GetAllActiveBranchesAsync()
        {
            var branches = await _context.Branches
                .Where(b => b.IsActive && !b.IsDeleted)
                .Select(b => new BranchDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Code = b.Code,
                    Address = b.Address,
                    Phone = b.Phone,
                    Email = b.Email,
                    Description = b.Description,
                    IsActive = b.IsActive,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return branches;
        }
    }
} 