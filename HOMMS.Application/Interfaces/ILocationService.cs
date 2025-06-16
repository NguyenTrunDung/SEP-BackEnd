using HOMMS.Domain.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    /// <summary>
    /// Service interface for location operations
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Gets locations for a specific area
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <returns>Locations for the area</returns>
        Task<IEnumerable<LocationDto>> GetLocationsByAreaAsync(int areaId);
        
        /// <summary>
        /// Gets active locations for a specific area
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <returns>Active locations for the area</returns>
        Task<IEnumerable<LocationDto>> GetActiveLocationsByAreaAsync(int areaId);
        
        /// <summary>
        /// Gets locations for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Locations for the branch</returns>
        Task<IEnumerable<LocationDto>> GetLocationsByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets active locations for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Active locations for the branch</returns>
        Task<IEnumerable<LocationDto>> GetActiveLocationsByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets location by ID
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <returns>Location DTO</returns>
        Task<LocationDto?> GetByIdAsync(int id);
        
        /// <summary>
        /// Gets location with area information by ID
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <returns>Location DTO with area</returns>
        Task<LocationDto?> GetWithAreaAsync(int id);
        
        /// <summary>
        /// Creates a new location
        /// </summary>
        /// <param name="dto">Location creation DTO</param>
        /// <returns>Created location DTO</returns>
        Task<LocationDto> CreateAsync(CreateLocationDto dto);
        
        /// <summary>
        /// Updates an existing location
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <param name="dto">Location update DTO</param>
        /// <returns>Updated location DTO</returns>
        Task<LocationDto?> UpdateAsync(int id, UpdateLocationDto dto);
        
        /// <summary>
        /// Deletes a location (soft delete)
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <returns>Success status</returns>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Validates if location name is unique within an area
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <param name="name">Location name</param>
        /// <param name="excludeId">Location ID to exclude from check</param>
        /// <returns>True if name is unique</returns>
        Task<bool> IsNameUniqueAsync(int areaId, string name, int? excludeId = null);
        
        /// <summary>
        /// Validates if room number is unique within a branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="roomNumber">Room number</param>
        /// <param name="excludeId">Location ID to exclude from check</param>
        /// <returns>True if room number is unique</returns>
        Task<bool> IsRoomNumberUniqueAsync(int branchId, string roomNumber, int? excludeId = null);
    }
} 