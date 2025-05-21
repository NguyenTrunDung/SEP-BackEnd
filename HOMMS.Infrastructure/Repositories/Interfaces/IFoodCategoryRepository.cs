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
    }
} 