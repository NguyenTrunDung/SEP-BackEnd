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

            // Seed all branches as in BranchConfiguration, but without explicit Ids
            var branches = new[]
            {
                new Branch
                {
                    Name = "Coteccons",
                    Code = "coteccons",
                    Address = "Coteccons",
                    Phone = "0919000000",
                    IsActive = true,
                    BranchId = 1,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Branch
                {
                    Name = "Bệnh viện Hoàn Mỹ Cửu Long Canteen",
                    Code = "cthoanmy",
                    Address = "Cần Thơ",
                    Phone = "0123456789",
                    IsActive = true,
                    BranchId = 2,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Branch
                {
                    Name = "BV Nhi Đồng Canteen",
                    Code = "bvnhi",
                    Address = "TP Hồ Chí Minh",
                    Phone = "0123456789",
                    IsActive = true,
                    BranchId = 3,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Branch
                {
                    Name = "Becamex",
                    Code = "becamex",
                    Address = "Đại lộ Bình Dương, khu Gò Cát, Lái Thiêu, Thuận An, Bình Dương",
                    Phone = "0919111111",
                    IsActive = true,
                    BranchId = 4,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            foreach (var branch in branches)
            {
                if (!dbContext.Branches.Any(b => b.Code == branch.Code))
                {
                    dbContext.Branches.Add(branch);
                }
            }
            await dbContext.SaveChangesAsync();

            // Return the Id of the first branch (by code)
            var firstBranch = dbContext.Branches.FirstOrDefault(b => b.Code == branches[0].Code);
            return firstBranch?.Id ?? 0;
        }
    }
} 