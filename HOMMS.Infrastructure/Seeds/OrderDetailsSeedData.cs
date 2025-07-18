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
    public static class OrderDetailsSeedData
    {

        public static async Task OrderDetailsSeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var rev = new List<OrderDetails>
            {
              new OrderDetails
    {
       
        OrderId = 1,
        FoodId = 1,
        MenuId = 1,
        Qty = 2,
        Price = 25000,
        Total = 50000,
        Note = "Ít cay",
        CreatedAt = DateTime.Now
    },
    new OrderDetails
    {
       
        OrderId = 1,
        FoodId = 2,
        MenuId = 1,
        Qty = 1,
        Price = 30000,
        Total = 30000,
        Note = null,
        CreatedAt = DateTime.Now
    },
    new OrderDetails
    {
        
        OrderId = 2,
        FoodId = 3,
        MenuId = 1,
        Qty = 3,
        Price = 20000,
        Total = 60000,
        Note = "Không hành",
        CreatedAt = DateTime.Now
    }



            };

            foreach (var OrderDetails in rev)
            {
                if (!dbContext.OrderDetails.Any(r => r.Id == OrderDetails.Id))
                {
                    dbContext.OrderDetails.Add(OrderDetails);
                }
            }


            await dbContext.SaveChangesAsync();
        }











    }
}
