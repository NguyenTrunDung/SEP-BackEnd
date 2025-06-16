namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for displaying location information
    /// </summary>
    public class LocationDto
    {
        /// <summary>
        /// Gets or sets the location ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the location name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the room number or identifier
        /// </summary>
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
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the area ID this location belongs to
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// Gets or sets the branch ID this location belongs to
        /// </summary>
        public int BranchId { get; set; }

        /// <summary>
        /// Gets or sets the area information
        /// </summary>
        public AreaDto? Area { get; set; }
    }

    /// <summary>
    /// DTO for creating a location
    /// </summary>
    public class CreateLocationDto
    {
        /// <summary>
        /// Gets or sets the location name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the room number or identifier
        /// </summary>
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
        public int AreaId { get; set; }

        /// <summary>
        /// Gets or sets the branch ID this location belongs to
        /// </summary>
        public int BranchId { get; set; }
    }

    /// <summary>
    /// DTO for updating a location
    /// </summary>
    public class UpdateLocationDto : CreateLocationDto
    {
        /// <summary>
        /// Gets or sets the location ID
        /// </summary>
        public int Id { get; set; }
    }
} 