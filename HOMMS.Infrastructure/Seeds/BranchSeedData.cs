using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Seeds
{
    public static class BranchSeedData
    {
        public static async Task<int> SeedDefaultBranchAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed all branches with only required fields: Name, Address, Phone
            var branches = new[]
            {
                new Branch
                {
                    Name = "Coteccons",
                    Address = "Coteccons Head Office",
                    Phone = "0919000000",
                    Code = "coteccons", // Optional field
                    IsActive = true,
                    BranchId = 1,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Branch
                {
                    Name = "Bệnh viện Hoàn Mỹ Cửu Long Canteen",
                    Address = "Cần Thơ",
                    Phone = "0123456789",
                    Code = "cthoanmy", // Optional field
                    IsActive = true,
                    BranchId = 2,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Branch
                {
                    Name = "BV Nhi Đồng Canteen",
                    Address = "TP Hồ Chí Minh",
                    Phone = "0123456789",
                    Code = "bvnhi", // Optional field
                    IsActive = true,
                    BranchId = 3,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Branch
                {
                    Name = "Becamex",
                    Address = "Đại lộ Bình Dương, khu Gò Cát, Lái Thiêu, Thuận An, Bình Dương",
                    Phone = "0919111111",
                    Code = "becamex", // Optional field
                    IsActive = true,
                    BranchId = 4,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            foreach (var branch in branches)
            {
                // Check by Name instead of Code since Name is unique now
                if (!dbContext.Branches.Any(b => b.Name == branch.Name))
                {
                    dbContext.Branches.Add(branch);
                }
            }
            await dbContext.SaveChangesAsync();

            // Return the Id of the first branch (by name)
            var firstBranch = dbContext.Branches.FirstOrDefault(b => b.Name == branches[0].Name);
            return firstBranch?.Id ?? 0;
        }
    }
}