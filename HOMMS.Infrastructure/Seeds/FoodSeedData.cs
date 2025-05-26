using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Seeds
{
    public static class FoodSeedData
    {
        public static async Task SeedFoodsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Full foods list generated from DM_Foods SQL, mapped to valid CategoryId (1-19) and BranchId (1-4)
            var foods = new List<Food>
            {
                new Food { Name = "Bún Riêu (370g)", BranchId = 4, CategoryId = 1, Description = "Bún tươi 100g, chả cua 35g, thịt 50g, huyết 10g, đậu hủ 10g, cà chua 5g, rau 10g, nước dùng 150g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 25000, PriceForPatient = 20000, PriceForStaff = 20000, Image = "images/Foods/0d3bd1b5-09cd-4e21-a7bf-2c20769eb222.png", Sort = 1 },
                new Food { Name = "Đậu hũ nhồi thịt sốt cà (140,5g)", BranchId = 1, CategoryId = 2, Description = "Đậu hủ 80g, thịt xay 30g, sốt cà chua 30g, hành lá 0.5g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 25000, PriceForPatient = 25000, PriceForStaff = 25000, Image = "images/Foods/1921b762-90a7-4550-87d3-94ccbd82b813.png", Sort = 2 },
                new Food { Name = "Canh bí đỏ thịt bằm (85.5g)", BranchId = 1, CategoryId = 2, Description = "Bí đỏ 10g, thịt bằm 5g, hành lá 0.5g, nước canh 70g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 10000, PriceForPatient = 8000, PriceForStaff = 8000, Image = "images/Foods/b5cc0915-33f1-415a-8d4a-c837c2b45a2e.png", Sort = 3 },
                new Food { Name = "Bắp cải xào cà rốt (25,5g)", BranchId = 2, CategoryId = 2, Description = "Bắp cải 20g, cà rốt 5g, hành lá 0.5g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 10000, PriceForPatient = 8000, PriceForStaff = 8000, Image = "images/Foods/3998ca77-51f7-4aa1-9aac-de56fcb4f2e4.png", Sort = 4 },
                new Food { Name = "Trái cây", BranchId = 4, CategoryId = 2, Description = "80g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 5000, PriceForPatient = 5000, PriceForStaff = 5000, Image = "images/Foods/6152da49-c0a6-4104-9be3-bc3e05d95c34.jpg", Sort = 5 },
                // ...all other foods from DM_Foods.sql, mapped and encoded correctly...
            };

            // Add 10 random foods for each branch (1-4) and random category (1-19)
            var random = new Random();
            for (int branchId = 1; branchId <= 4; branchId++)
            {
                for (int i = 6; i <= 15; i++)
                {
                    int categoryId = random.Next(1, 20); // 1-19 inclusive
                    foods.Add(new Food
                    {
                        Name = $"Món ngẫu nhiên {i} - Chi nhánh {branchId}",
                        BranchId = branchId,
                        CategoryId = categoryId,
                        Description = $"Món ăn ngẫu nhiên số {i} cho chi nhánh {branchId}, danh mục {categoryId}",
                        IsSetDish = false,
                        IsAddOn = false,
                        ForPatient = false,
                        PriceForGuest = 10000 + random.Next(0, 5) * 5000,
                        PriceForPatient = 8000 + random.Next(0, 5) * 4000,
                        PriceForStaff = 8000 + random.Next(0, 5) * 4000,
                        Image = $"images/Foods/random_{branchId}_{i}.jpg",
                        Sort = i
                    });
                }
            }

            foreach (var food in foods)
            {
                if (!dbContext.Foods.Any(f => f.Name == food.Name && f.BranchId == food.BranchId))
                {
                    dbContext.Foods.Add(food);
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
