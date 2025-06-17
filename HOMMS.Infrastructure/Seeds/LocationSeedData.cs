using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Seeds
{
    public class LocationSeedData
    {
        public static async Task LocationSeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Get all Areas for mapping
            var areas = dbContext.Areas.ToList();
            var areaMap = areas.ToDictionary(a => a.Name.ToLower(), a => a.Id);

            var locations = new List<Location>
            {
                new Location
                {
                    Name = "Phòng khám A",
                    Sort = 1,
                    IsActive = true,
                    AreaId = areaMap.ContainsKey("khu hành chính") ? areaMap["khu hành chính"] : areas.First().Id
                },
                new Location
                {
                    Name = "Phòng khám B",
                    Sort = 2,
                    IsActive = false,
                    AreaId = areaMap.ContainsKey("khu hành chính") ? areaMap["khu hành chính"] : areas.First().Id
                },
                new Location
                {
                    Name = "Phòng khám C",
                    Sort = 3,
                    IsActive = true,
                    AreaId = areaMap.ContainsKey("khoa nội") ? areaMap["khoa nội"] : areas.First().Id
                },
            };

            foreach (var location in locations)
            {
                // Check if the location already exists based on its name and area ID
                if (!dbContext.Locations.Any(l => l.Name == location.Name && l.AreaId == location.AreaId))
                {
                    dbContext.Locations.Add(location);
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}