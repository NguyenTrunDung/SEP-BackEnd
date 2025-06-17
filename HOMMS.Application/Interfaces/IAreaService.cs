using HOMMS.Domain.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    /// <summary>
    /// Service interface for area operations
    /// </summary>
    public interface IAreaService
    {
        /// <summary>
        /// Gets areas for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Areas for the branch</returns>
        Task<IEnumerable<AreaDto>> GetAreasByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets active areas for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Active areas for the branch</returns>
        Task<IEnumerable<AreaDto>> GetActiveAreasByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets areas with their locations for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Areas with locations</returns>
        Task<IEnumerable<AreaDto>> GetAreasWithLocationsByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets area by ID
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Area DTO</returns>
        Task<AreaDto?> GetByIdAsync(int id);
        
        /// <summary>
        /// Gets area with locations by ID
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Area DTO with locations</returns>
        Task<AreaDto?> GetWithLocationsAsync(int id);
        
        /// <summary>
        /// Creates a new area
        /// </summary>
        /// <param name="dto">Area creation DTO</param>
        /// <returns>Created area DTO</returns>
        Task<AreaDto> CreateAsync(CreateAreaDto dto);
        
        /// <summary>
        /// Updates an existing area
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <param name="dto">Area update DTO</param>
        /// <returns>Updated area DTO</returns>
        Task<AreaDto?> UpdateAsync(int id, UpdateAreaDto dto);
        
        /// <summary>
        /// Deletes an area (soft delete)
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Success status</returns>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Validates if area name is unique within a branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="name">Area name</param>
        /// <param name="excludeId">Area ID to exclude from check</param>
        /// <returns>True if name is unique</returns>
        Task<bool> IsNameUniqueAsync(int branchId, string name, int? excludeId = null);
    }
} 