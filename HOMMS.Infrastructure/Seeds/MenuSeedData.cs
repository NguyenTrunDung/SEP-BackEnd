using HOMMS.Domain.Dtos;
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
    public static class MenuSeedData
    {
        public static async Task MenuSeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var menu = new List<Menu> {

               new Menu
               {


                        Date = new DateTime(2025-01-21),
                        TimeOfDay = "Breakfast",
                        IsTime = true,
                        TimeFrom = new TimeSpan(6, 0, 0),
                        TimeTo = new TimeSpan(9, 0, 0),
                        BranchId = 1,


               },

                 new Menu
               {


                        Date = new DateTime(2025-02-21),
                        TimeOfDay = "Dinner",
                        IsTime = true,
                        TimeFrom = new TimeSpan(6, 0, 0),
                        TimeTo = new TimeSpan(9, 0, 0),
                        BranchId = 1,


               },
                   new Menu
               {


                        Date = new DateTime(2025-01-22),
                        TimeOfDay = "Lunch",
                        IsTime = true,
                        TimeFrom = new TimeSpan(6, 0, 0),
                        TimeTo = new TimeSpan(9, 0, 0),
                        BranchId = 1,


               },
                     new Menu
               {


                        Date = new DateTime(2025-4-14),
                        TimeOfDay = "Breakfast",
                        IsTime = true,
                        TimeFrom = new TimeSpan(6, 0, 0),
                        TimeTo = new TimeSpan(9, 0, 0),
                        BranchId = 2,


               },
                       new Menu
               {


                        Date = new DateTime(2025-01-21),
                        TimeOfDay = "Dinner",
                        IsTime = true,
                        TimeFrom = new TimeSpan(6, 0, 0),
                        TimeTo = new TimeSpan(9, 0, 0),
                        BranchId = 2,


               },
                        new Menu
               {


                        Date = new DateTime(2025-4-14),
                        TimeOfDay = "Breakfast",
                        IsTime = true,
                        TimeFrom = new TimeSpan(6, 0, 0),
                        TimeTo = new TimeSpan(9, 0, 0),
                        BranchId = 3,


               },
                       new Menu
               {


                        Date = new DateTime(2025-01-21),
                        TimeOfDay = "Lunch",
                        IsTime = true,
                        TimeFrom = new TimeSpan(6, 0, 0),
                        TimeTo = new TimeSpan(9, 0, 0),
                        BranchId = 3,


               },
                          new Menu
               {


                        Date = new DateTime(2025-4-14),
                        TimeOfDay = "Breakfast",
                        IsTime = true,
                        TimeFrom = new TimeSpan(6, 0, 0),
                        TimeTo = new TimeSpan(9, 0, 0),
                        BranchId = 4,


               },
                       new Menu
               {


                        Date = new DateTime(2025-01-21),
                        TimeOfDay = "Lunch",
                        IsTime = true,
                        TimeFrom = new TimeSpan(6, 0, 0),
                        TimeTo = new TimeSpan(9, 0, 0),
                        BranchId = 4,


               },




                };





            foreach (var Menu in menu)
            {
                if (!dbContext.Menus.Any(f => f.Name == Menu.Name && f.BranchId == Menu.BranchId))
                {
                    dbContext.Menus.Add(Menu);
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}