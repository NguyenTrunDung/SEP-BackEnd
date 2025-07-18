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
    public class AreaSeedData
    {
        public static async Task AreaSeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var areas = new List<Area>
            {
                new Area
                {
                    Name = "Khu hành chính",
                    Sort = 1,
                    IsActive = true,
                    BranchId = 1
                },
                new Area
                {
                    Name = "Khoa nội",
                    Sort = 2,
                    IsActive = true,
                    BranchId = 1
                },
                new Area
                {
                    Name = "Khoa cấp cứu",
                    Sort = 3,
                    IsActive = true,
                    BranchId = 1
                },
                new Area
                {
                    Name = "Khoa ngoại",
                    Sort = 4,
                    IsActive = true,
                    BranchId = 1
                }
            };

            foreach (var area in areas)
            {
                // Check if the area already exists based on its name
                if (!dbContext.Areas.Any(a => a.Name == area.Name))
                {
                    dbContext.Areas.Add(area);
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}