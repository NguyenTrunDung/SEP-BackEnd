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
            // Global filter applies BranchId and IsDeleted
            return await DbSet
                .OrderBy(a => a.Sort)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Area>> GetActiveAreasByBranchAsync(int branchId)
        {
            // Global filter applies BranchId and IsDeleted
            return await DbSet
                .Where(a => a.IsActive)
                .OrderBy(a => a.Sort)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<Area?> GetAreaWithLocationsAsync(int areaId)
        {
            // Global filter applies BranchId and IsDeleted
            return await DbSet
                .Include(a => a.Locations.Where(l => !l.IsDeleted))
                .FirstOrDefaultAsync(a => a.Id == areaId);
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Area>> GetAreasWithLocationsByBranchAsync(int branchId)
        {
            // Global filter applies to Area, but not to navigation property Locations
            return await DbSet
                .Where(a => a.BranchId == branchId && a.IsActive && !a.IsDeleted)
                .Select(a => new Area
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Sort = a.Sort,
                    IsActive = a.IsActive,
                    BranchId = a.BranchId,
                    Locations = a.Locations
                        .Where(l => l.IsActive && !l.IsDeleted && l.BranchId == a.BranchId)
                        .ToList()
                })
                .OrderBy(a => a.Sort)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<bool> IsAreaNameUniqueAsync(int branchId, string name, int? excludeId = null)
        {
            // Global filter applies BranchId and IsDeleted
            var query = DbSet.Where(a => a.Name.ToLower() == name.ToLower());
            
            if (excludeId.HasValue)
            {
                query = query.Where(a => a.Id != excludeId.Value);
            }
            
            return !await query.AnyAsync();
        }
    }
} 