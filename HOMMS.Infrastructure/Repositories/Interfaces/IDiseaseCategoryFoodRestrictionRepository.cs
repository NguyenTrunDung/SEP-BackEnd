using HOMMS.Domain.Entities;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Disease Category Food Restriction operations
    /// </summary>
    public interface IDiseaseCategoryFoodRestrictionRepository : IRepository<DiseaseCategoryFoodRestriction, int>
    {
        /// <summary>
        /// Gets all food restrictions for a specific disease category
        /// </summary>
        /// <param name="diseaseCategoryId">Disease category ID</param>
        /// <returns>Collection of food restrictions</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetByDiseaseCategoryIdAsync(int diseaseCategoryId);
        
        /// <summary>
        /// Gets all food restrictions for a specific food item
        /// </summary>
        /// <param name="foodId">Food ID</param>
        /// <returns>Collection of food restrictions</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetByFoodIdAsync(int foodId);
        
        /// <summary>
        /// Gets all food restrictions for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of food restrictions</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetByBranchIdAsync(int branchId);
        
        /// <summary>
        /// Gets allowed foods for a patient based on their disease categories
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="diseaseCategoryId">Optional specific disease category ID to filter by</param>
        /// <returns>Collection of allowed foods</returns>
        Task<IEnumerable<Food>> GetAllowedFoodsForPatientAsync(string patientId, int branchId, int? diseaseCategoryId = null);
        
        /// <summary>
        /// Gets restricted foods for a patient based on their disease categories
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of restricted foods with restriction details</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetRestrictedFoodsForPatientAsync(string patientId, int branchId);
        
        /// <summary>
        /// Validates if a food item is allowed for a patient
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <param name="foodId">Food ID</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Validation result with restriction details</returns>
        Task<(bool IsAllowed, IEnumerable<DiseaseCategoryFoodRestriction> Restrictions)> ValidateFoodForPatientAsync(string patientId, int foodId, int branchId);
        
        /// <summary>
        /// Gets food restrictions by restriction level
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="restrictionLevel">Restriction level</param>
        /// <returns>Collection of food restrictions</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetByRestrictionLevelAsync(int branchId, int restrictionLevel);
        
        /// <summary>
        /// Gets active food restrictions for a branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of active food restrictions</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetActiveByBranchIdAsync(int branchId);
        
        /// <summary>
        /// Checks if a food restriction exists for a disease category and food combination
        /// </summary>
        /// <param name="diseaseCategoryId">Disease category ID</param>
        /// <param name="foodId">Food ID</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if restriction exists</returns>
        Task<bool> RestrictionExistsAsync(int diseaseCategoryId, int foodId, int branchId, int? excludeId = null);
    }
} 