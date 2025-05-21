using HOMMS.Domain.Entities.Base;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Junction entity to manage many-to-many relationship between branches and users
    /// </summary>
    public class BranchUser : BaseEntity<int>
    {
        /// <summary>
        /// Gets or sets the branch ID
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the user ID
        /// </summary>
        public string? UserId { get; set; }
        
        /// <summary>
        /// Gets or sets whether this is the default branch for the user
        /// </summary>
        public bool IsDefault { get; set; }
        
        /// <summary>
        /// Gets or sets the branch navigation property
        /// </summary>
        public virtual Branch? Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the user navigation property
        /// </summary>
        public virtual ApplicationUser? User { get; set; }
    }
} 