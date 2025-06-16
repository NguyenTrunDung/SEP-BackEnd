using HOMMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for location operations
    /// </summary>
    public interface ILocationRepository : IRepository<Location, int>
    {
        /// <summary>
        /// Gets locations for a specific area
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <returns>Locations for the area</returns>
        Task<IEnumerable<Location>> GetLocationsByAreaAsync(int areaId);
        
        /// <summary>
        /// Gets active locations for a specific area ordered by sort value
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <returns>Active locations for the area</returns>
        Task<IEnumerable<Location>> GetActiveLocationsByAreaAsync(int areaId);
        
        /// <summary>
        /// Gets locations for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Locations for the branch</returns>
        Task<IEnumerable<Location>> GetLocationsByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets active locations for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Active locations for the branch</returns>
        Task<IEnumerable<Location>> GetActiveLocationsByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets location with its area information
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <returns>Location with area</returns>
        Task<Location?> GetLocationWithAreaAsync(int locationId);
        
        /// <summary>
        /// Checks if location name is unique within an area
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <param name="name">Location name</param>
        /// <param name="excludeId">Location ID to exclude from check (for updates)</param>
        /// <returns>True if name is unique</returns>
        Task<bool> IsLocationNameUniqueAsync(int areaId, string name, int? excludeId = null);
        
        /// <summary>
        /// Checks if room number is unique within a branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="roomNumber">Room number</param>
        /// <param name="excludeId">Location ID to exclude from check (for updates)</param>
        /// <returns>True if room number is unique</returns>
        Task<bool> IsRoomNumberUniqueAsync(int branchId, string roomNumber, int? excludeId = null);
    }
} 