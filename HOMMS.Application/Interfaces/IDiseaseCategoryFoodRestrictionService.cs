using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;

namespace HOMMS.Application.Interfaces
{
    /// <summary>
    /// Service interface for Disease Category Food Restriction management
    /// </summary>
    public interface IDiseaseCategoryFoodRestrictionService
    {
        /// <summary>
        /// Gets all food restrictions for a specific disease category
        /// </summary>
        /// <param name="diseaseCategoryId">Disease category ID</param>
        /// <returns>Collection of food restriction DTOs</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetByDiseaseCategoryIdAsync(int diseaseCategoryId);
        
        /// <summary>
        /// Gets all food restrictions for a specific food item
        /// </summary>
        /// <param name="foodId">Food ID</param>
        /// <returns>Collection of food restriction DTOs</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetByFoodIdAsync(int foodId);
        
        /// <summary>
        /// Gets all food restrictions for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of food restriction DTOs</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetByBranchIdAsync(int branchId);
        
        /// <summary>
        /// Gets a food restriction by ID
        /// </summary>
        /// <param name="id">Food restriction ID</param>
        /// <returns>Food restriction DTO or null if not found</returns>
        Task<DiseaseCategoryFoodRestrictionDto?> GetByIdAsync(int id);
        
        /// <summary>
        /// Gets allowed foods for a patient based on their disease categories
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="diseaseCategoryId">Optional specific disease category ID to filter by</param>
        /// <returns>Collection of allowed food DTOs</returns>
        Task<IEnumerable<FoodDto>> GetAllowedFoodsForPatientAsync(string patientId, int branchId, int? diseaseCategoryId = null);
        
        /// <summary>
        /// Gets restricted foods for a patient based on their disease categories
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of restricted food restriction DTOs</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetRestrictedFoodsForPatientAsync(string patientId, int branchId);
        
        /// <summary>
        /// Validates if a food item is allowed for a patient
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <param name="foodId">Food ID</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Validation result with restriction details</returns>
        Task<FoodValidationResultDto> ValidateFoodForPatientAsync(string patientId, int foodId, int branchId);
        
        /// <summary>
        /// Validates if multiple food items are allowed for a patient (bulk validation)
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <param name="foodIds">List of food IDs to validate</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Validation results for each food item</returns>
        Task<IEnumerable<FoodValidationResultDto>> ValidateFoodsForPatientAsync(string patientId, IEnumerable<int> foodIds, int branchId);
        
        /// <summary>
        /// Creates a new food restriction
        /// </summary>
        /// <param name="dto">Create food restriction DTO</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="userId">User ID creating the restriction</param>
        /// <returns>Created food restriction DTO</returns>
        Task<DiseaseCategoryFoodRestrictionDto> CreateAsync(CreateDiseaseCategoryFoodRestrictionDto dto, int branchId, string userId);
        
        /// <summary>
        /// Updates an existing food restriction
        /// </summary>
        /// <param name="id">Food restriction ID</param>
        /// <param name="dto">Update food restriction DTO</param>
        /// <param name="userId">User ID updating the restriction</param>
        /// <returns>Updated food restriction DTO or null if not found</returns>
        Task<DiseaseCategoryFoodRestrictionDto?> UpdateAsync(int id, UpdateDiseaseCategoryFoodRestrictionDto dto, string userId);
        
        /// <summary>
        /// Soft deletes a food restriction
        /// </summary>
        /// <param name="id">Food restriction ID</param>
        /// <param name="userId">User ID deleting the restriction</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(int id, string userId);
        
        /// <summary>
        /// Gets food restrictions by restriction level
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="restrictionLevel">Restriction level (1=Advisory, 2=Warning, 3=Prohibited, 4=Dangerous)</param>
        /// <returns>Collection of food restriction DTOs</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetByRestrictionLevelAsync(int branchId, int restrictionLevel);
        
        /// <summary>
        /// Gets active food restrictions for a branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of active food restriction DTOs</returns>
        Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetActiveByBranchIdAsync(int branchId);
        
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
    
    /// <summary>
    /// DTO for food validation results
    /// </summary>
    public class FoodValidationResultDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public bool IsAllowed { get; set; }
        public List<DiseaseCategoryFoodRestrictionDto> Restrictions { get; set; } = new();
        public string? ValidationMessage { get; set; }
        public int HighestRestrictionLevel { get; set; }
    }
} 