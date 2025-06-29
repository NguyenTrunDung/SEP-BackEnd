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
    public static class SystemLogSeedData
    {
        public static async Task SystemLogSeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var Slog = new List<SystemLog>
            {
                new SystemLog

                       {

                           BranchId = 1,
                           UserId = "SystemAdmin",
                           Note = "Đã thêm món ăn Canh bí đỏ thịt bằm",
                           CreatedAt = DateTime.Parse("2025-01-21T11:39:49.190"),
                           LastModifiedAt = DateTime.Parse("2025-01-21T11:39:49.190")
                       },
                       new SystemLog
                       {

                           BranchId = 1,
                           UserId = "SystemAdmin",
                           Note = "Đã thêm món ăn Bắp cải xào cà rốt",
                           CreatedAt = DateTime.Parse("2025-01-21T11:40:17.723"),
                           LastModifiedAt = DateTime.Parse("2025-01-21T11:40:17.723")
                       },
                       new SystemLog
                       {

                           BranchId = 1,
                           UserId = "SystemAdmin",
                           Note = "Đã thêm món ăn Trái cây",
                           CreatedAt = DateTime.Parse("2025-01-21T11:40:51.253"),
                           LastModifiedAt = DateTime.Parse("2025-01-21T11:40:51.253")
                       },
                       new SystemLog
                       {

                           BranchId = 1,
                           UserId = "SystemAdmin",
                           Note = "Đã thêm món ăn Thịt bò xào",
                           CreatedAt = DateTime.Parse("2025-01-21T11:41:25.493"),
                           LastModifiedAt = DateTime.Parse("2025-01-21T11:41:25.493")
                       },
                       new SystemLog
                       {

                           BranchId = 1,
                           UserId = "SystemAdmin",
                           Note = "Đã thêm món ăn Cá lóc kho tiêu",
                           CreatedAt = DateTime.Parse("2025-01-21T11:41:54.093"),
                           LastModifiedAt = DateTime.Parse("2025-01-21T11:41:54.093")
                       },
                       new SystemLog
                       {

                           BranchId = 1,
                           UserId = "SystemAdmin",
                           Note = "Đã thêm món ăn Đậu hũ nhồi thịt sốt cà (Suất ăn)",
                           CreatedAt = DateTime.Parse("2025-01-21T11:43:08.123"),
                           LastModifiedAt = DateTime.Parse("2025-01-21T11:43:08.123")
                       }




            };




            foreach (var Log in Slog)
            {
                if (!dbContext.SystemLogs.Any(f => f.BranchId == Log.BranchId &&
                                                   f.Note == Log.Note &&
                                                   f.CreatedAt == Log.CreatedAt))
                {
                    dbContext.SystemLogs.Add(Log);
                }
            }
            await dbContext.SaveChangesAsync();

        }
    }
}
