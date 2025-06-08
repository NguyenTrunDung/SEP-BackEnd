using HOMMS.Domain.Entities.Base;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Junction entity to manage user-branch-role relationships
    /// </summary>
    public class BranchUserRole : BaseAuditableEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public int? BranchId { get; set; } // Make BranchId nullable for optional relationship
        public int BranchRoleId { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual BranchRole? BranchRole { get; set; }
        public virtual Branch? Branch { get; set; }
    }
} 