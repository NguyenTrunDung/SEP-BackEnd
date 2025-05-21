using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HOMMS.Domain.Entities.Base;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a hospital branch (tenant) in the system
    /// </summary>
    public partial class Branch : BaseAuditableEntity, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the name of the branch
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the code of the branch (short identifier)
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the address of the branch
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets whether this branch is active
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the contact phone number for the branch
        /// </summary>
        [StringLength(20)]
        public string? Phone { get; set; }
        
        /// <summary>
        /// Gets or sets the contact email for the branch
        /// </summary>
        [StringLength(100)]
        public string? Email { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the branch
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }
        
        /// <summary>
        /// Gets or sets the branch ID for IBranchEntity implementation (self-reference in this case)
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the foreign key to a user who manages this branch (optional)
        /// </summary>
        public string? ManagerId { get; set; }
        public ApplicationUser? Manager { get; set; }
        
        /// <summary>
        /// Gets or sets the navigation property for menus belonging to this branch
        /// </summary>
        public virtual ICollection<Menu> Menus { get; set; } = new HashSet<Menu>();
        
        /// <summary>
        /// Gets or sets the navigation property for users belonging to this branch
        /// </summary>
        public virtual ICollection<BranchUser> BranchUsers { get; set; } = new HashSet<BranchUser>();
        
        /// <summary>
        /// Gets or sets the navigation property for foods belonging to this branch
        /// </summary>
        public virtual ICollection<Food> Foods { get; set; } = new HashSet<Food>();
        
        /// <summary>
        /// Gets or sets the navigation property for food categories belonging to this branch
        /// </summary>
        public virtual ICollection<FoodCategory> FoodCategories { get; set; } = new HashSet<FoodCategory>();
    }
} 