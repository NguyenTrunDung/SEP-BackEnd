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
    /// Implementation of area repository
    /// </summary>
    public class AreaRepository : Repository<Area, int>, IAreaRepository
    {
        public AreaRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Area>> GetAreasByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(a => a.BranchId == branchId && !a.IsDeleted)
                .OrderBy(a => a.Sort)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Area>> GetActiveAreasByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(a => a.BranchId == branchId && a.IsActive && !a.IsDeleted)
                .OrderBy(a => a.Sort)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<Area?> GetAreaWithLocationsAsync(int areaId)
        {
            return await DbSet
                .Include(a => a.Locations.Where(l => !l.IsDeleted))
                .FirstOrDefaultAsync(a => a.Id == areaId && !a.IsDeleted);
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Area>> GetAreasWithLocationsByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(a => a.BranchId == branchId && a.IsActive && !a.IsDeleted)
                .Include(a => a.Locations.Where(l => l.IsActive && !l.IsDeleted))
                .OrderBy(a => a.Sort)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<bool> IsAreaNameUniqueAsync(int branchId, string name, int? excludeId = null)
        {
            var query = DbSet.Where(a => a.BranchId == branchId && 
                                        a.Name.ToLower() == name.ToLower() && 
                                        !a.IsDeleted);
            
            if (excludeId.HasValue)
            {
                query = query.Where(a => a.Id != excludeId.Value);
            }
            
            return !await query.AnyAsync();
        }
    }
} 