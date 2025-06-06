using HOMMS.Domain.Entities.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents an area within a hospital branch (e.g., Administrative Zone, ICU Zone, Emergency Zone)
    /// </summary>
    public class Area : BaseAuditableEntity<int>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the name of the area
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the area
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the sort order for this area
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// Gets or sets whether this area is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the branch ID this area belongs to
        /// </summary>
        public int BranchId { get; set; }

        /// <summary>
        /// Gets or sets the branch navigation property
        /// </summary>
        public virtual Branch? Branch { get; set; }

        /// <summary>
        /// Gets or sets the locations in this area
        /// </summary>
        public virtual ICollection<Location> Locations { get; set; } = new HashSet<Location>();
    }
} 