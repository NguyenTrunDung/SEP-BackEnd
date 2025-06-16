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
    public class AreaRepository : Repository<Area, int>, IAreaRepository
    {
        private readonly ApplicationDbContext _context;

        public AreaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Area>> GetAllAreasAsync()
        {
            return await DbSet.OrderBy(a => a.Sort)
                .ToListAsync();
        }

        public async Task<bool> CreateAreaAsync(Area area)
        {
            //Check if the name already exists (case-sensitive)
            var existingArea = await DbSet.Where(a => a.Name == area.Name)
                .FirstOrDefaultAsync();

            if (existingArea != null) return false;     //Name already exists

            await DbSet.AddAsync(area);
            await _context.SaveChangesAsync();

            return true;    //Area created successfully
        }

        public async Task<bool> UpdateAreaAsync(int id, string newName)
        {
            var existingArea = await DbSet.FindAsync(id);
            if (existingArea == null) return false;     //Area not found

            //Check if the new name already exists (case-sensitive)
            var nameExists = await DbSet.Where(a => a.Name == newName && a.Id != id).FirstOrDefaultAsync();
            if (nameExists != null) return false;       //Name conflict

            existingArea.Name = newName;
            DbSet.Update(existingArea);
            await _context.SaveChangesAsync();
            return true;    //Area updated successfully
        }

        public async Task<bool> DeleteAreaAsync(int id)
        {
            var existingArea = await DbSet.FindAsync(id);
            if (existingArea == null) return false;     //Area not found

            existingArea.IsActive = false;
            DbSet.Update(existingArea);
            await _context.SaveChangesAsync();
            return true;    //Area deleted successfully
        }
    }
}