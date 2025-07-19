using HOMMS.Domain.Dtos;

namespace HOMMS.Application.Interfaces
{
    /// <summary>
    /// Service interface for Disease Category management
    /// </summary>
    public interface IDiseaseCategoryService
    {
        /// <summary>
        /// Gets all disease categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of disease categories</returns>
        Task<IEnumerable<DiseaseCategoryDto>> GetByBranchIdAsync(int branchId);
        
        /// <summary>
        /// Gets active disease categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of active disease categories</returns>
        Task<IEnumerable<DiseaseCategoryDto>> GetActiveByBranchIdAsync(int branchId);
        
        /// <summary>
        /// Gets a disease category by ID
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <returns>Disease category or null if not found</returns>
        Task<DiseaseCategoryDto?> GetByIdAsync(int id);
        
        /// <summary>
        /// Gets a disease category by code and branch
        /// </summary>
        /// <param name="code">Disease category code</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Disease category or null if not found</returns>
        Task<DiseaseCategoryDto?> GetByCodeAndBranchAsync(string code, int branchId);
        
        /// <summary>
        /// Creates a new disease category
        /// </summary>
        /// <param name="dto">Create disease category DTO</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="userId">User ID creating the category</param>
        /// <returns>Created disease category</returns>
        Task<DiseaseCategoryDto> CreateAsync(CreateDiseaseCategoryDto dto, int branchId, string userId);
        
        /// <summary>
        /// Updates an existing disease category
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <param name="dto">Update disease category DTO</param>
        /// <param name="userId">User ID updating the category</param>
        /// <returns>Updated disease category or null if not found</returns>
        Task<DiseaseCategoryDto?> UpdateAsync(int id, UpdateDiseaseCategoryDto dto, string userId);
        
        /// <summary>
        /// Soft deletes a disease category
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <param name="userId">User ID deleting the category</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(int id, string userId);
        
        /// <summary>
        /// Gets disease categories with patient statistics
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of disease categories with patient counts</returns>
        Task<IEnumerable<DiseaseCategoryDto>> GetWithPatientCountsAsync(int branchId);
        
        /// <summary>
        /// Gets disease categories with food restriction statistics
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of disease categories with food restriction counts</returns>
        Task<IEnumerable<DiseaseCategoryDto>> GetWithFoodRestrictionCountsAsync(int branchId);
        
        /// <summary>
        /// Checks if a disease category code is available for use
        /// </summary>
        /// <param name="code">Disease category code</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if code is available</returns>
        Task<bool> IsCodeAvailableAsync(string code, int branchId, int? excludeId = null);
    }
}