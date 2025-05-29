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
                        Date = DateTime.Today,
                        TimeOfDay = "Breakfast",
                        IsTime = false,
                        TimeFrom = null,
                        TimeTo = null,
                        BranchId = 1,
               },
                 new Menu
               {
                        Date = DateTime.Today,
                        TimeOfDay = "Dinner",
                        IsTime = false,
                        TimeFrom = null,
                        TimeTo = null,
                        BranchId = 1,
               },
                   new Menu
               {
                        Date = DateTime.Today,
                        TimeOfDay = "Lunch",
                        IsTime = false,
                        TimeFrom = null,
                        TimeTo = null,
                        BranchId = 1,
               },
                     new Menu
               {
                        Date = DateTime.Today,
                        TimeOfDay = "Breakfast",
                        IsTime = false,
                        TimeFrom = null,
                        TimeTo = null,
                        BranchId = 1,
               },
                       new Menu
               {
                        Date = DateTime.Today,
                        TimeOfDay = "Dinner",
                        IsTime = false,
                        TimeFrom = null,
                        TimeTo = null,
                        BranchId = 1,
               },
                        new Menu
               {
                        Date = DateTime.Today,
                        TimeOfDay = "Breakfast",
                        IsTime = false,
                        TimeFrom = null,
                        TimeTo = null,
                        BranchId = 1,
               },
                       new Menu
               {
                        Date = DateTime.Today,
                        TimeOfDay = "Lunch",
                        IsTime = false,
                        TimeFrom = null,
                        TimeTo = null,
                        BranchId = 1,
               },
                          new Menu
               {
                        Date = DateTime.Today,
                        TimeOfDay = "Breakfast",
                        IsTime = false,
                        TimeFrom = null,
                        TimeTo = null,
                        BranchId = 1,
               },
                       new Menu
               {
                        Date = DateTime.Today,
                        TimeOfDay = "Lunch",
                        IsTime = false,
                        TimeFrom = null,
                        TimeTo = null,
                        BranchId = 1,
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