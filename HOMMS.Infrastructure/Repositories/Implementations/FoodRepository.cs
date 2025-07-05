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
                .ThenByDescending(f => f.CreatedAt)
                .ThenBy(f => f.Sort)
                .ToListAsync();
        }

        public async Task<IEnumerable<Food>> GetFoodsByBranchAndDateAsync(int branchId, DateTime date)
        {
            // Get all foods available in menus for the branch and date
            var menus = await DbContext.Set<Menu>()
                .Where(m => m.BranchId == branchId && m.Date.Date == date.Date)
                .Include(m => m.MenuDetails)
                    .ThenInclude(md => md.Food)
                .ToListAsync();

            var foods = menus
                .SelectMany(m => m.MenuDetails)
                .Where(md => md.Status == true && md.Food != null)
                .Select(md => md.Food)
                .Distinct()
                .ToList();

            return foods;
        }

        public async Task<IEnumerable<FoodCategory>> GetCategoriesByBranchAndDateAsync(int branchId, DateTime date)
        {
            // Get all categories of foods available in menus for the branch and date
            var menus = await DbContext.Set<Menu>()
                .Where(m => m.BranchId == branchId && m.Date.Date == date.Date)
                .Include(m => m.MenuDetails)
                    .ThenInclude(md => md.Food)
                        .ThenInclude(f => f.Category)
                .ToListAsync();

            var categories = menus
                .SelectMany(m => m.MenuDetails)
                .Where(md => md.Status == true && md.Food != null && md.Food.Category != null)
                .Select(md => md.Food.Category)
                .Distinct()
                .ToList();

            return categories;
        }

        public async Task<IEnumerable<Food>> GetFoodsByBranchCategoryAndDateAsync(int branchId, int categoryId, DateTime date)
        {
            // Get all foods in a category available in menus for the branch and date
            var menus = await DbContext.Set<Menu>()
                .Where(m => m.BranchId == branchId && m.Date.Date == date.Date)
                .Include(m => m.MenuDetails)
                    .ThenInclude(md => md.Food)
                .ToListAsync();

            var foods = menus
                .SelectMany(m => m.MenuDetails)
                .Where(md => md.Status == true && md.Food != null && md.Food.CategoryId == categoryId)
                .Select(md => md.Food)
                .Distinct()
                .ToList();

            return foods;
        }


        public async Task<IEnumerable<Food>> GetFoodWithDiseaseCategoryFoodRestrictionAsync(int branchId, int categoryId)
        {
            var foods = await DbContext.Set<DiseaseCategoryFoodRestriction>()
                .Where(fo => fo.DiseaseCategoryId == categoryId && fo.IsActive&& fo.BranchId == branchId)
                .Select(r => r.Food)
                .Where(f => !f.IsDeleted)
                .Distinct()
                .ToListAsync();



            return foods;
        }


        public async Task AddAndSaveAsync(Food food)
        {
            await DbSet.AddAsync(food);
            await DbContext.SaveChangesAsync();
        }
        public async Task<Food?> FindByIdAsync(int id)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task UpdateAndSaveAsync(Food food)
        {
            DbSet.Update(food);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetMaxSortValueByBranchAsync(int branchId)
        {
            var maxSort = await DbSet
                .Where(f => f.BranchId == branchId)
                .MaxAsync(f => (int?)f.Sort);
            
            return maxSort ?? 0;
        }

    }
}