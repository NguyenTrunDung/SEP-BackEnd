using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    /// <summary>
    /// Implementation of specialized menu repository
    /// </summary>
    public class MenuRepository : Repository<Menu, int>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Menu>> GetMenusByBranchAndDateAsync(int branchId, DateTime date)
        {
            return await DbSet
                .Where(m => m.BranchId == branchId && m.Date.Date == date.Date)
                .OrderBy(m => m.TimeOfDay)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<Dictionary<Branch, IEnumerable<Menu>>> GetMenusForAllBranchesAsync(DateTime date)
        {
            // We need to bypass the branch filter to get menus from all branches
            // This requires using the DbContext directly with IgnoreQueryFilters()
            
            var branchesWithMenus = await DbContext.Set<Branch>()
                .Where(b => b.IsActive)
                .Include(b => DbContext.Set<Menu>()
                    .Where(m => m.BranchId == b.Id && m.Date.Date == date.Date)
                    .OrderBy(m => m.TimeOfDay))
                .ToListAsync();
                
            var result = new Dictionary<Branch, IEnumerable<Menu>>();
            
            foreach (var branch in branchesWithMenus)
            {
                // Clone the branch to avoid modification issues
                var branchCopy = new Branch
                {
                    Id = branch.Id,
                    Name = branch.Name,
                    Code = branch.Code,
                    Address = branch.Address,
                    Phone = branch.Phone,
                    Email = branch.Email,
                    IsActive = branch.IsActive
                };
                
                // Extract the menus
                var menus = DbContext.Set<Menu>()
                    .IgnoreQueryFilters() // This bypasses the global branch filter
                    .Where(m => m.BranchId == branch.Id && m.Date.Date == date.Date)
                    .OrderBy(m => m.TimeOfDay)
                    .ToList();
                
                result.Add(branchCopy, menus);
            }
            
            return result;
        }
        
        /// <inheritdoc/>
        public async Task<Menu> GetMenuWithDetailsAsync(int menuId)
        {
            var menu = await DbSet
                .Include(m => m.MenuDetails)
                    .ThenInclude(md => md.Food)
                        .ThenInclude(f => f.Category)
                .Include(m => m.Branch)
                .FirstOrDefaultAsync(m => m.Id == menuId);
                
            return menu;
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Menu>> GetMenusWithDetailsByBranchAndDateAsync(int branchId, DateTime date)
        {
            return await DbSet
                .Where(m => m.BranchId == branchId && m.Date.Date == date.Date)
                .Include(m => m.MenuDetails)
                    .ThenInclude(md => md.Food)
                        .ThenInclude(f => f.Category)
                .Include(m => m.Branch)
                .OrderBy(m => m.TimeOfDay)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Menu>> GetMenusByBranchDateAndTimeOfDayAsync(int branchId, DateTime date, string timeOfDay)
        {
            return await DbSet
                .Where(m => m.BranchId == branchId && 
                           m.Date.Date == date.Date && 
                           m.TimeOfDay == timeOfDay)
                .OrderBy(m => m.TimeOfDay)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<Dictionary<FoodCategory, IEnumerable<MenuDetail>>> GetMenuDetailsByCategoryAsync(int menuId)
        {
            var menu = await GetMenuWithDetailsAsync(menuId);
            
            if (menu == null)
                return new Dictionary<FoodCategory, IEnumerable<MenuDetail>>();
                
            return menu.MenuDetails
                .Where(md => md.Status == true && md.Food?.Category != null)
                .GroupBy(md => md.Food.Category)
                .OrderBy(g => g.Key.Sort)
                .ToDictionary(
                    g => g.Key,
                    g => g.AsEnumerable()
                );
        }
    }
} 