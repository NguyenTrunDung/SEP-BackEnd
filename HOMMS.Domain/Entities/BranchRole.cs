using HOMMS.Domain.Entities.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a role with permissions that can be associated with branches through BranchUserRole
    /// </summary>
    public class BranchRole : BaseAuditableEntity<int>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
        public string Permissions { get; set; } = string.Empty; // Comma-separated

        public virtual ICollection<BranchUserRole> BranchUserRoles { get; set; } = new List<BranchUserRole>();
    }
} 