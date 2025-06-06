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
    public static class OrderSeedData
    {

        public static async Task OrderSeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var rev = new List<Order>
            {
                new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2025, 5, 30), // test theo ngày
                    Total = 100000,
                    Status = "Completed",
                    CustomerName = "Nguyen Van A",
                    Code = "ORD-001"
                },
                new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2025, 5, 30), // cùng ngày
                    Total = 150000,
                    Status = "Completed",
                    CustomerName = "Tran Thi B",
                    Code = "ORD-002"
                },
                new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2025, 5, 28), // tuần trước
                    Total = 120000,
                    Status = "Completed",
                    CustomerName = "Le Van C",
                    Code = "ORD-003"
                },
                 new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2025, 5, 28), // tuần trước
                    Total = 10000,
                    Status = "Completed",
                    CustomerName = "Le Van D",
                    Code = "ORD-007"
                },
                new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2025, 5, 15), // cùng tháng
                    Total = 200000,
                    Status = "Completed",
                    CustomerName = "Pham Thi D",
                    Code = "ORD-004"
                },
                new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2025, 4, 10), // tháng trước
                    Total = 170000,
                    Status = "Completed",
                    CustomerName = "Hoang Van E",
                    Code = "ORD-005"
                },
                new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2024, 12, 25), // năm trước
                    Total = 190000,
                    Status = "Completed",
                    CustomerName = "Vu Thi F",
                    Code = "ORD-006"
                },
                new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2024, 6, 6, 17, 0, 0),
                    Total = 310000,
                    Status = "Preparing",
                    CustomerName = "Vu Thi T",
                    Code = "ORD-008"
                },
                new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2024, 6, 6, 14, 30, 0),
                    Total = 250000,
                    Status = "Completed",
                    CustomerName = "Vu Thi S",
                    Code = "ORD-009"
                },
                new Order
                {
                    BranchId = 1,
                    OrderDate = new DateTime(2024, 6, 6),
                    Total = 310000,
                    Status = "Preparing",
                    CustomerName = "Vu Thi G",
                    Code = "ORD-010"
                },
            };

            foreach (var Order in rev)
            {
                if (!dbContext.Orders.Any(r => r.Id == Order.Id && r.BranchId == Order.BranchId))
                {
                    dbContext.Orders.Add(Order);
                }
            }


            await dbContext.SaveChangesAsync();
        }
    }
}
