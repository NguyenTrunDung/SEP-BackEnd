using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class LocationRepository : Repository<Location, int>, ILocationRepository
    {
        private readonly ApplicationDbContext _context;
        public LocationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            return await DbSet.OrderBy(l => l.Sort)
                .ToListAsync();
        }

        public async Task<bool> CreateLocationAsync(Location location)
        {
            // Check if the location name already exists (case-sensitive) in area
            var existingLocation = await GetLocationInAreaAsync(location.Name, location.AreaId);
            if (existingLocation != null) return false;     // Name already exists

            await DbSet.AddAsync(location);
            await _context.SaveChangesAsync();

            return true;        // Location created successfully
        }

        public async Task<Location?> GetLocationInAreaAsync(string name, int areaId)
        {
            return await DbSet.Where(l => l.Name == name && l.AreaId == areaId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateLocationAsync(int id, string newName, int areaId)
        {
            var existingLocation = await DbSet.FindAsync(id);
            if (existingLocation == null) return false;     // Location not found

            // Check if the new name already exists (case-sensitive) in area
            var nameExists = await GetLocationInAreaAsync(newName, areaId);
            if (nameExists != null && nameExists.Id != id) return false;       // Name conflict

            //Update the location details
            existingLocation.Name = newName;
            existingLocation.AreaId = areaId;

            DbSet.Update(existingLocation);
            await _context.SaveChangesAsync();
            return true;        // Location updated successfully
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var existingLocation = await DbSet.FindAsync(id);
            if (existingLocation == null) return false;     // Location not found

            existingLocation.IsActive = false;
            DbSet.Update(existingLocation);
            await _context.SaveChangesAsync();
            return true;        // Location deleted successfully
        }
    }
}
