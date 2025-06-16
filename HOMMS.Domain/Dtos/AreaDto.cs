using System.Collections.Generic;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for displaying area information
    /// </summary>
    public class AreaDto
    {
        /// <summary>
        /// Gets or sets the area ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the area name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the area description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the sort order for this area
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// Gets or sets whether this area is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the branch ID this area belongs to
        /// </summary>
        public int BranchId { get; set; }

        /// <summary>
        /// Gets or sets the locations in this area
        /// </summary>
        public ICollection<LocationDto>? Locations { get; set; }
    }

    /// <summary>
    /// DTO for creating or updating an area
    /// </summary>
    public class CreateAreaDto
    {
        /// <summary>
        /// Gets or sets the area name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the area description
        /// </summary>
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
    }

    /// <summary>
    /// DTO for updating an area
    /// </summary>
    public class UpdateAreaDto : CreateAreaDto
    {
        /// <summary>
        /// Gets or sets the area ID
        /// </summary>
        public int Id { get; set; }
    }
} 