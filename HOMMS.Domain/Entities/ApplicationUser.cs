using HOMMS.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Application user for Identity framework with additional properties
    /// </summary>
    public class ApplicationUser : IdentityUser, IAuditableEntity
    {
        /// <summary>
        /// Gets or sets the first name of the user
        /// </summary>
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the last name of the user
        /// </summary>
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets the full name of the user
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();
        
        /// <summary>
        /// Gets or sets the profile picture URL
        /// </summary>
        [StringLength(2000)]
        public string? ProfilePictureUrl { get; set; }
        
        /// <summary>
        /// Gets or sets whether the user is active
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the refresh token for JWT authentication
        /// </summary>
        public string? RefreshToken { get; set; }
        
        /// <summary>
        /// Gets or sets the refresh token expiry date/time
        /// </summary>
        public System.DateTime? RefreshTokenExpiryTime { get; set; }
        
        /// <summary>
        /// Gets or sets the address of the user
        /// </summary>
        [StringLength(500)]
        public string? Address { get; set; }
        
        /// <summary>
        /// Gets or sets the branch-user relationships for this user
        /// </summary>
        public virtual ICollection<BranchUser> BranchUsers { get; set; } = new List<BranchUser>();
        
        #region Audit Properties
        
        /// <summary>
        /// Gets or sets the created datetime
        /// </summary>
        public System.DateTime CreatedAt { get; set; } = System.DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the created by user ID
        /// </summary>
        public string? CreatedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the last modified datetime
        /// </summary>
        public System.DateTime? LastModifiedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the last modified by user ID
        /// </summary>
        public string? LastModifiedBy { get; set; }
        
        #endregion
    }
} 