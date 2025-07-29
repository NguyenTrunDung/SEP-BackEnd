using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Seeds
{
    public class DepartmentSeedData
    {
        public static async Task SeedDepartmentsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var dep = new List<Department>

            {
                new Department {Name = " Khoa ngoại", IsActive = true, BranchId= 1, Sort = 1 , LocationId= 1},
                new Department { Name = " Khoa nội", IsActive = true, BranchId= 1, Sort = 2 , LocationId= 1}

            };

            foreach (var par in dep)
            {
                if (!dbContext.Department.Any(f => f.BranchId == par.BranchId &&
                                                   f.Name == par.Name &&
                                                   f.CreatedAt == par.CreatedAt))
                {
                    dbContext.Department.Add(par);
                }
            }
            await dbContext.SaveChangesAsync();

        }

    }






}

