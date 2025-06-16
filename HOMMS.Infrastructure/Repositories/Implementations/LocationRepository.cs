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
    /// Implementation of location repository
    /// </summary>
    public class LocationRepository : Repository<Location, int>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Location>> GetLocationsByAreaAsync(int areaId)
        {
            return await DbSet
                .Where(l => l.AreaId == areaId && !l.IsDeleted)
                .OrderBy(l => l.Sort)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Location>> GetActiveLocationsByAreaAsync(int areaId)
        {
            return await DbSet
                .Where(l => l.AreaId == areaId && l.IsActive && !l.IsDeleted)
                .OrderBy(l => l.Sort)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Location>> GetLocationsByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(l => l.BranchId == branchId && !l.IsDeleted)
                .Include(l => l.Area)
                .OrderBy(l => l.Area!.Sort)
                .ThenBy(l => l.Area!.Name)
                .ThenBy(l => l.Sort)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Location>> GetActiveLocationsByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(l => l.BranchId == branchId && l.IsActive && !l.IsDeleted)
                .Include(l => l.Area)
                .Where(l => l.Area!.IsActive && !l.Area.IsDeleted)
                .OrderBy(l => l.Area!.Sort)
                .ThenBy(l => l.Area!.Name)
                .ThenBy(l => l.Sort)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<Location?> GetLocationWithAreaAsync(int locationId)
        {
            return await DbSet
                .Include(l => l.Area)
                .FirstOrDefaultAsync(l => l.Id == locationId && !l.IsDeleted);
        }
        
        /// <inheritdoc/>
        public async Task<bool> IsLocationNameUniqueAsync(int areaId, string name, int? excludeId = null)
        {
            var query = DbSet.Where(l => l.AreaId == areaId && 
                                        l.Name.ToLower() == name.ToLower() && 
                                        !l.IsDeleted);
            
            if (excludeId.HasValue)
            {
                query = query.Where(l => l.Id != excludeId.Value);
            }
            
            return !await query.AnyAsync();
        }
        
        /// <inheritdoc/>
        public async Task<bool> IsRoomNumberUniqueAsync(int branchId, string roomNumber, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(roomNumber))
                return true; // Room number is optional
                
            var query = DbSet.Where(l => l.BranchId == branchId && 
                                        l.RoomNumber != null &&
                                        l.RoomNumber.ToLower() == roomNumber.ToLower() && 
                                        !l.IsDeleted);
            
            if (excludeId.HasValue)
            {
                query = query.Where(l => l.Id != excludeId.Value);
            }
            
            return !await query.AnyAsync();
        }
    }
} 