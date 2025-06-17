using HOMMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for area operations
    /// </summary>
    public interface IAreaRepository : IRepository<Area, int>
    {
        /// <summary>
        /// Gets areas for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Areas for the branch</returns>
        Task<IEnumerable<Area>> GetAreasByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets active areas for a specific branch ordered by sort value
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Active areas for the branch</returns>
        Task<IEnumerable<Area>> GetActiveAreasByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets area with its locations
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <returns>Area with locations</returns>
        Task<Area?> GetAreaWithLocationsAsync(int areaId);
        
        /// <summary>
        /// Gets areas with their locations for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Areas with locations</returns>
        Task<IEnumerable<Area>> GetAreasWithLocationsByBranchAsync(int branchId);
        
        /// <summary>
        /// Checks if area name is unique within a branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="name">Area name</param>
        /// <param name="excludeId">Area ID to exclude from check (for updates)</param>
        /// <returns>True if name is unique</returns>
        Task<bool> IsAreaNameUniqueAsync(int branchId, string name, int? excludeId = null);
    }
} 