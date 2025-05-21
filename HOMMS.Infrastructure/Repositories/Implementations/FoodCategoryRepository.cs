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
    /// Implementation of food category repository
    /// </summary>
    public class FoodCategoryRepository : Repository<FoodCategory, int>, IFoodCategoryRepository
    {
        public FoodCategoryRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<FoodCategory>> GetCategoriesByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(c => c.BranchId == branchId)
                .OrderBy(c => c.Sort)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<FoodCategory>> GetActiveCategoriesByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(c => c.BranchId == branchId && c.Active)
                .OrderBy(c => c.Sort)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<FoodCategory> GetCategoryWithFoodsAsync(int categoryId)
        {
            return await DbSet
                .Include(c => c.Foods)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<FoodCategory>> GetCategoriesWithFoodsByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(c => c.BranchId == branchId && c.Active)
                .Include(c => c.Foods.Where(f => !f.IsDeleted))
                .OrderBy(c => c.Sort)
                .ToListAsync();
        }
    }
} 