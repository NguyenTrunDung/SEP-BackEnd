using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HOMMS.Domain.Entities.Base;

namespace HOMMS.Domain.Entities
{
    public class Branch : BaseAuditableEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign key to a user who manages this branch (optional)
        public string? ManagerId { get; set; }
        public ApplicationUser? Manager { get; set; }

        // Navigation properties
        public virtual ICollection<Menu> Menus { get; set; } = new HashSet<Menu>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<BranchUser> BranchUsers { get; set; } = new HashSet<BranchUser>();
    }
} 