using HOMMS.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a specific location within an area (e.g., Admin Room 1, Admin Room 2, Conference Room)
    /// </summary>
    public class Location : BaseAuditableEntity<int>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the name of the location
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the location
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the room number or identifier
        /// </summary>
        [StringLength(20)]
        public string? RoomNumber { get; set; }

        /// <summary>
        /// Gets or sets the capacity of the location
        /// </summary>
        public int? Capacity { get; set; }

        /// <summary>
        /// Gets or sets the sort order for this location within the area
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// Gets or sets whether this location is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the area ID this location belongs to
        /// </summary>
        [Required]
        public int AreaId { get; set; }

        /// <summary>
        /// Gets or sets the branch ID this location belongs to
        /// </summary>
        public int BranchId { get; set; }

        /// <summary>
        /// Gets or sets the area navigation property
        /// </summary>
        public virtual Area? Area { get; set; }

        /// <summary>
        /// Gets or sets the branch navigation property
        /// </summary>
        public virtual Branch? Branch { get; set; }
    }
} 