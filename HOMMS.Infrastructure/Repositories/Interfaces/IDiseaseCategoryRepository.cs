using HOMMS.Domain.Entities;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Disease Category operations
    /// </summary>
    public interface IDiseaseCategoryRepository : IRepository<DiseaseCategory, int>
    {
        /// <summary>
        /// Gets all disease categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of disease categories</returns>
        Task<IEnumerable<DiseaseCategory>> GetByBranchIdAsync(int branchId);
        
        /// <summary>
        /// Gets a disease category by code and branch
        /// </summary>
        /// <param name="code">Disease category code</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Disease category if found</returns>
        Task<DiseaseCategory?> GetByCodeAndBranchAsync(string code, int branchId);
        
        /// <summary>
        /// Gets active disease categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of active disease categories</returns>
        Task<IEnumerable<DiseaseCategory>> GetActiveByBranchIdAsync(int branchId);
        
        /// <summary>
        /// Checks if a disease category code exists in a branch
        /// </summary>
        /// <param name="code">Disease category code</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if code exists</returns>
        Task<bool> CodeExistsAsync(string code, int branchId, int? excludeId = null);
        
        /// <summary>
        /// Gets disease categories with patient counts
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Disease categories with patient statistics</returns>
        Task<IEnumerable<DiseaseCategory>> GetWithPatientCountsAsync(int branchId);
        
        /// <summary>
        /// Gets disease categories with food restriction counts
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Disease categories with food restriction statistics</returns>
        Task<IEnumerable<DiseaseCategory>> GetWithFoodRestrictionCountsAsync(int branchId);
    }
}