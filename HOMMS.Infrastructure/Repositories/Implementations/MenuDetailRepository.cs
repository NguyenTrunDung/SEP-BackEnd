using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class MenuDetailRepository : Repository<Menu, int>, IMenuDetailRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MenuDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Menu?> GetMenuWithDetailsAsync(int id)
        {
            return await _dbContext.Menus
                .Include(m => m.MenuDetails)
                    .ThenInclude(md => md.Food)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Menu>> GetAllMenusWithDetailsAsync(int branchId)
        {
            return await _dbContext.Menus
                .Where(f => f.BranchId == branchId)
                .Include(m => m.MenuDetails)
                    .ThenInclude(md => md.Food)
                .OrderByDescending(m => m.Date)
                .ThenByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetMenuTemplatesAsync(int branchId)
        {
            return await _dbContext.Menus
                .Where(m => m.BranchId == branchId)
                .Include(m => m.MenuDetails)
                    .ThenInclude(md => md.Food)
                        .ThenInclude(f => f.Category)
                .OrderByDescending(m => m.Date)
                .ThenByDescending(m => m.CreatedAt)
                .Take(20) // Limit to recent 20 menus for performance
                .ToListAsync();
        }

        public async Task<bool> UpdateMenuWithDetailsAsync(Menu menu)
        {
            _dbContext.Menus.Update(menu);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddMenuWithDetailsAsync(Menu menu)
        {
            await _dbContext.Menus.AddAsync(menu);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Menu?> GetMenuByDateAsync(int branchId, DateTime date, string? timeOfDay)
        {
            return await _dbContext.Menus
                .Include(m => m.MenuDetails)
                .FirstOrDefaultAsync(m => m.BranchId == branchId && m.Date == date && m.TimeOfDay == timeOfDay);
        }
    }
}
