using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    /// <summary>
    /// Implementation of food repository
    /// </summary>
    public class FoodRepository : Repository<Food, int>, IFoodRepository
    {
        public FoodRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Food>> GetFoodsByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(f => f.BranchId == branchId)
                .OrderBy(f => f.Sort)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Food>> GetFoodsByBranchAndCategoryAsync(int branchId, int categoryId)
        {
            return await DbSet
                .Where(f => f.BranchId == branchId && f.CategoryId == categoryId)
                .OrderBy(f => f.Sort)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<Food> GetFoodWithCategoryAsync(int foodId)
        {
            return await DbSet
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.Id == foodId);
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Food>> GetFoodsWithCategoriesByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(f => f.BranchId == branchId)
                .Include(f => f.Category)
                .OrderBy(f => f.Category.Sort)
                .ThenBy(f => f.Sort)
                .ToListAsync();
        }
    }
} 