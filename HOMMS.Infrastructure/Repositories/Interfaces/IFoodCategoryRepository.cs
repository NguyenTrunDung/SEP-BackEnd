using HOMMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for food category operations
    /// </summary>
    public interface IFoodCategoryRepository : IRepository<FoodCategory, int>
    {
        /// <summary>
        /// Gets categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Categories for the branch</returns>
        Task<IEnumerable<FoodCategory>> GetCategoriesByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets active categories for a specific branch ordered by sort value
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Active categories for the branch</returns>
        Task<IEnumerable<FoodCategory>> GetActiveCategoriesByBranchAsync(int branchId);
        
        /// <summary>
        /// Gets category with its foods
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>Category with foods</returns>
        Task<FoodCategory> GetCategoryWithFoodsAsync(int categoryId);
        
        /// <summary>
        /// Gets categories with their foods for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Categories with foods</returns>
        Task<IEnumerable<FoodCategory>> GetCategoriesWithFoodsByBranchAsync(int branchId);
        
        /// <summary>
        /// Adds a food category and saves changes immediately
        /// </summary>
        /// <param name="category">Food category to add</param>
        /// <returns>Task representing the async operation</returns>
        Task AddAndSaveAsync(FoodCategory category);
        
        /// <summary>
        /// Finds a food category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Food category or null if not found</returns>
        Task<FoodCategory?> FindByIdAsync(int id);
        
        /// <summary>
        /// Updates a food category and saves changes immediately
        /// </summary>
        /// <param name="category">Food category to update</param>
        /// <returns>Task representing the async operation</returns>
        Task UpdateAndSaveAsync(FoodCategory category);

        /// <summary>
        /// Gets the maximum sort value for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Maximum sort value, or 0 if no categories exist</returns>
        Task<int> GetMaxSortValueByBranchAsync(int branchId);

        /// <summary>
        /// Updates multiple categories with new sort orders
        /// </summary>
        /// <param name="categoryUpdates">List of categories with their new sort values</param>
        /// <returns>Task representing the async operation</returns>
        Task UpdateSortOrdersAsync(IEnumerable<(int CategoryId, int Sort)> categoryUpdates);

        /// <summary>
        /// Gets categories by branch ordered by sort value with tracking enabled
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Categories for the branch with tracking enabled</returns>
        Task<IEnumerable<FoodCategory>> GetCategoriesByBranchWithTrackingAsync(int branchId);
    }
} 