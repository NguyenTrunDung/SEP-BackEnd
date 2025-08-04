using HOMMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for login response including tokens, user info, permissions, and branches
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// JWT access token for authentication
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
        
        /// <summary>
        /// Refresh token for obtaining new access tokens
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
        
        /// <summary>
        /// Access token expiry time
        /// </summary>
        public DateTime TokenExpiryTime { get; set; }
        
        /// <summary>
        /// Refresh token expiry time
        /// </summary>
        public DateTime RefreshTokenExpiryTime { get; set; }
        
        /// <summary>
        /// User information
        /// </summary>
        public UserInfoDto User { get; set; } = new UserInfoDto();
        
        /// <summary>
        /// User's permissions across all branches
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();
        
        /// <summary>
        /// User's branches and roles
        /// </summary>
        public List<UserBranchRoleDto> UserBranches { get; set; } = new List<UserBranchRoleDto>();
        
        /// <summary>
        /// Default/current branch for the user
        /// </summary>
        public BranchDto? DefaultBranch { get; set; }
        
        /// <summary>
        /// Whether user is a system admin (has access to all permissions)
        /// </summary>
        public bool IsSystemAdmin { get; set; }
    }
    
    /// <summary>
    /// DTO for user information in auth responses
    /// </summary>
    public class UserInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
    
    /// <summary>
    /// DTO for user's branch and role information
    /// </summary>
    public class UserBranchRoleDto
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public int BranchRoleId { get; set; }
        public string BranchRoleName { get; set; } = string.Empty;
        public List<string> BranchPermissions { get; set; } = new List<string>();
        public bool IsDefault { get; set; }
        public DateTime AssignedAt { get; set; }
    }
    
    /// <summary>
    /// DTO for refresh token request
    /// </summary>
    public class RefreshTokenRequestDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;
        
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// DTO for refresh token response
    /// </summary>
    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiryTime { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
    
    /// <summary>
    /// DTO for branch selection with updated permissions
    /// </summary>
    public class SelectBranchResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public BranchDto SelectedBranch { get; set; } = new BranchDto();
        public UserBranchRoleDto BranchRole { get; set; } = new UserBranchRoleDto();
        public List<string> AvailablePermissions { get; set; } = new List<string>();
    }
    
    /// <summary>
    /// DTO for user permission summary
    /// </summary>
    public class UserPermissionSummaryDto
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsSystemAdmin { get; set; }
        public List<string> GlobalPermissions { get; set; } = new List<string>();
        public Dictionary<int, List<string>> BranchPermissions { get; set; } = new Dictionary<int, List<string>>();
        public List<UserBranchRoleDto> BranchRoles { get; set; } = new List<UserBranchRoleDto>();
    }

    /// <summary>
    /// DTO for branch information
    /// </summary>
    //public class BranchDto
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public string Code { get; set; } = string.Empty;
    //    public string Address { get; set; } = string.Empty;
    //    public string? Phone { get; set; }
    //    public string? Email { get; set; }
    //    public string? Description { get; set; }
    //    public bool IsActive { get; set; }
    //    public string? ManagerName { get; set; }
    //    public DateTime CreatedAt { get; set; }
    //}


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
        public int? Department { get; set; }
    }

    public class SelectBranchModel
    {
        public int BranchId { get; set; }
    }
} 