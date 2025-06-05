using HOMMS.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        #region Customer-specific Properties
        
        /// <summary>
        /// Gets or sets whether this user is a customer account (created by admin/manager)
        /// </summary>
        public bool IsCustomerAccount { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the wallet balance for customer accounts in VND (Vietnamese Dong)
        /// VND doesn't use decimal places, stored as whole numbers
        /// </summary>
        [Column(TypeName = "bigint")]
        public long WalletBalance { get; set; } = 0;
        
        /// <summary>
        /// Gets or sets the customer code (unique identifier for customer accounts)
        /// </summary>
        [StringLength(20)]
        public string? CustomerCode { get; set; }
        
        /// <summary>
        /// Gets or sets additional notes about the customer (allergies, dietary restrictions, etc.)
        /// Supports Vietnamese text
        /// </summary>
        [StringLength(1000)]
        public string? CustomerNotes { get; set; }
        
        /// <summary>
        /// Gets or sets whether the customer account is currently enabled for ordering
        /// </summary>
        public bool IsCustomerEnabled { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the patient room number (for hospitalized patients)
        /// </summary>
        [StringLength(20)]
        public string? RoomNumber { get; set; }
        
        /// <summary>
        /// Gets or sets the patient bed number (for hospitalized patients)
        /// </summary>
        [StringLength(10)]
        public string? BedNumber { get; set; }
        
        /// <summary>
        /// Gets or sets the patient admission date (for hospitalized patients)
        /// </summary>
        public DateTime? AdmissionDate { get; set; }
        
        /// <summary>
        /// Gets or sets the patient discharge date (for discharged patients)
        /// </summary>
        public DateTime? DischargeDate { get; set; }
        
        /// <summary>
        /// Gets or sets the attending physician name
        /// </summary>
        [StringLength(100)]
        public string? AttendingPhysician { get; set; }
        
        /// <summary>
        /// Gets or sets whether the patient requires special dietary supervision
        /// </summary>
        public bool RequiresDietarySupervision { get; set; } = false;
        
        #endregion
        
        /// <summary>
        /// Gets or sets the branch-user relationships for this user
        /// </summary>
        public virtual ICollection<BranchUser> BranchUsers { get; set; } = new List<BranchUser>();
        
        /// <summary>
        /// Gets or sets the wallet transactions for this user
        /// </summary>
        public virtual ICollection<UserWalletTransaction> WalletTransactions { get; set; } = new List<UserWalletTransaction>();
        
        /// <summary>
        /// Gets or sets the orders placed by this user
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        /// <summary>
        /// Gets or sets the patient ID if this user is linked to a patient record (optional)
        /// </summary>
        [StringLength(450)]
        public string? PatientId { get; set; }
        
        /// <summary>
        /// Gets or sets the patient record associated with this user (optional)
        /// </summary>
        public virtual Patient? Patient { get; set; }

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