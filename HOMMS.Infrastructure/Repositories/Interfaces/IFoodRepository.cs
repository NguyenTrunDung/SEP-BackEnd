using HOMMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for food operations
    /// </summary>
    public interface IFoodRepository : IRepository<Food, int>
    {
        /// <summary>
        /// Gets foods for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Foods for the branch</returns>
        Task<IEnumerable<Food>> GetFoodsByBranchAsync(int branchId);

        /// <summary>
        /// Gets foods for a specific branch and category
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="categoryId">Category ID</param>
        /// <returns>Foods for the branch and category</returns>
        Task<IEnumerable<Food>> GetFoodsByBranchAndCategoryAsync(int branchId, int categoryId);
        
        /// <summary>
        /// Gets food with its category
        /// </summary>
        /// <param name="foodId">Food ID</param>
        /// <returns>Food with category</returns>
        Task<Food> GetFoodWithCategoryAsync(int foodId);
        
        /// <summary>
        /// Gets foods with their categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Foods with categories</returns>
        Task<IEnumerable<Food>> GetFoodsWithCategoriesByBranchAsync(int branchId);

        /// <summary>
        /// Gets foods for a specific branch and date
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param>
        /// <returns>Foods for the branch and date</returns>
        Task<IEnumerable<Food>> GetFoodsByBranchAndDateAsync(int branchId, DateTime date);

        /// <summary>
        /// Gets categories for a specific branch and date
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param>
        /// <returns>Categories for the branch and date</returns>
        Task<IEnumerable<FoodCategory>> GetCategoriesByBranchAndDateAsync(int branchId, DateTime date);

        /// <summary>
        /// Gets foods for a specific branch, category, and date
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="categoryId">Category ID</param>
        /// <param name="date">Date</param>
        /// <returns>Foods for the branch, category, and date</returns>
        Task<IEnumerable<Food>> GetFoodsByBranchCategoryAndDateAsync(int branchId, int categoryId, DateTime date);
    }
} 