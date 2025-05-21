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
    }
} 