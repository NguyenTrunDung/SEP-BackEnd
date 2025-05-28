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
    public static class MenuDetailSeedData
    {
        public static async Task MenuDetailDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            var menuD = new List<MenuDetail>
            {

                new MenuDetail
                {
                    MenuId = 1,
                    FoodId = 1,
                    Qty = 10,
                    Sold = 10,
                    CreatedAt = DateTime.Now,
                    PriceForGuest=25000,
                    PriceForPatient=20000,
                    PriceForStaff = 20000,
                    DiscountPrice= 0,
                    Status = true,
                    IsQty = true,
                },



                 new MenuDetail
                {
                    MenuId = 1,
                    FoodId = 2,
                    Qty = 410,
                    Sold = 510,
                    CreatedAt = DateTime.Now,
                    PriceForGuest=27000,
                    PriceForPatient=25000,
                    PriceForStaff = 26000,
                    DiscountPrice= 0,
                    Status = true,
                    IsQty = true,
                },
                  new MenuDetail
                {
                    MenuId = 1,
                    FoodId = 3,
                    Qty = 180,
                    Sold = 120,
                    CreatedAt = DateTime.Now,
                    PriceForGuest=75000,
                    PriceForPatient=30000,
                    PriceForStaff = 50000,
                    DiscountPrice= 0,
                    Status = true,
                    IsQty = true,
                },


            };


            foreach (var MenuDetail in menuD)
            {
                if (!dbContext.MenuDetails.Any(f => f.MenuId == MenuDetail.MenuId && f.FoodId == MenuDetail.FoodId))
                {
                    dbContext.MenuDetails.Add(MenuDetail);
                }
            }
            await dbContext.SaveChangesAsync();

        }
    }
}
