using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System.Security.Claims;

namespace HOMMS.Application.Interfaces
{
    /// <summary>
    /// Interface for authentication service handling JWT tokens, refresh tokens, and user permissions
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Generates JWT access token for the user without embedded permissions
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="branchId">Current branch ID (optional)</param>
        /// <param name="branchRoleId">Current branch role ID (optional)</param>
        /// <returns>JWT token</returns>
        Task<string> GenerateAccessTokenAsync(ApplicationUser user, int? branchId = null, int? branchRoleId = null);
        
        /// <summary>
        /// Generates refresh token for the user
        /// </summary>
        /// <returns>Refresh token string</returns>
        string GenerateRefreshToken();
        
        /// <summary>
        /// Validates refresh token and returns user if valid
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>User if token is valid, null otherwise</returns>
        Task<ApplicationUser?> ValidateRefreshTokenAsync(string refreshToken);
        
        /// <summary>
        /// Refreshes access token using refresh token
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <returns>New tokens</returns>
        Task<ApiResponseBase<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        
        /// <summary>
        /// Gets user's permissions from database (BranchRole.Permissions)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User permission summary</returns>
        Task<UserPermissionSummaryDto> GetUserPermissionsAsync(string userId);
        
        /// <summary>
        /// Gets user's permissions for a specific branch
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>List of permissions for the branch</returns>
        Task<List<string>> GetUserBranchPermissionsAsync(string userId, int branchId);
        
        /// <summary>
        /// Gets user's branch and role information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user branch roles</returns>
        Task<List<UserBranchRoleDto>> GetUserBranchRolesAsync(string userId);
        
        /// <summary>
        /// Gets user's default branch
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Default branch DTO</returns>
        Task<BranchDto?> GetUserDefaultBranchAsync(string userId);
        
        /// <summary>
        /// Updates user's refresh token in database
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="refreshToken">New refresh token</param>
        /// <param name="expiryTime">Token expiry time</param>
        /// <returns>Success status</returns>
        Task<bool> UpdateUserRefreshTokenAsync(ApplicationUser user, string refreshToken, DateTime expiryTime);
        
        /// <summary>
        /// Revokes user's refresh token (logout)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success status</returns>
        Task<bool> RevokeRefreshTokenAsync(string userId);
        
        /// <summary>
        /// Validates JWT token and extracts claims
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Claims principal if valid</returns>
        ClaimsPrincipal? ValidateAccessToken(string token);
        
        /// <summary>
        /// Builds complete login response with tokens, user info, and permissions
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="accessToken">Generated access token</param>
        /// <param name="refreshToken">Generated refresh token</param>
        /// <param name="tokenExpiryTime">Access token expiry</param>
        /// <param name="refreshTokenExpiryTime">Refresh token expiry</param>
        /// <returns>Complete login response</returns>
        Task<LoginResponseDto> BuildLoginResponseAsync(
            ApplicationUser user, 
            string accessToken, 
            string refreshToken, 
            DateTime tokenExpiryTime, 
            DateTime refreshTokenExpiryTime);
    }
} 