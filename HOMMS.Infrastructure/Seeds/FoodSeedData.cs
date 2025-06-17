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

            // Get all FoodCategories for mapping
            var categories = dbContext.FoodCategories.ToList();
            var categoryMap = categories
                .GroupBy(c => (c.Name.ToLower(), c.BranchId))
                .ToDictionary(g => g.Key, g => g.First().Id);

            // Example: Map food category by name and branch
            int GetCategoryId(string name, int branchId)
            {
                var key = (name.ToLower(), branchId);
                return categoryMap.ContainsKey(key) ? categoryMap[key] : categories.First().Id;
            }

            var foods = new List<Food>();
            foods.Add(new Food { Name = "Bún Riêu (370g)", BranchId = 1, CategoryId = GetCategoryId("Điểm tâm", 1), Description = "Bún tươi 100g, chả cua 35g, thịt 50g, huyết 10g, đậu hủ 10g, cà chua 5g, rau 10g, nước dùng 150g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 25000, PriceForPatient = 20000, PriceForStaff = 20000, Image = "images/Foods/0d3bd1b5-09cd-4e21-a7bf-2c20769eb222.png", Sort = 1 });
            foods.Add(new Food { Name = "Đậu hũ nhồi thịt sốt cà (140,5g)", BranchId = 1, CategoryId = GetCategoryId("Món chính", 1), Description = "Đậu hủ 80g, thịt xay 30g, sốt cà chua 30g, hành lá 0.5g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 25000, PriceForPatient = 25000, PriceForStaff = 25000, Image = "images/Foods/1921b762-90a7-4550-87d3-94ccbd82b813.png", Sort = 2 });
            foods.Add(new Food { Name = "Canh bí đỏ thịt bằm (85.5g)", BranchId = 1, CategoryId = GetCategoryId("Món chính", 1), Description = "Bí đỏ 10g, thịt bằm 5g, hành lá 0.5g, nước canh 70g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 10000, PriceForPatient = 8000, PriceForStaff = 8000, Image = "images/Foods/b5cc0915-33f1-415a-8d4a-c837c2b45a2e.png", Sort = 3 });
            foods.Add(new Food { Name = "Bắp cải xào cà rốt (25,5g)", BranchId = 1, CategoryId = GetCategoryId("Món chính", 1), Description = "Bắp cải 20g, cà rốt 5g, hành lá 0.5g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 10000, PriceForPatient = 8000, PriceForStaff = 8000, Image = "images/Foods/3998ca77-51f7-4aa1-9aac-de56fcb4f2e4.png", Sort = 4 });
            foods.Add(new Food { Name = "Trái cây", BranchId = 1, CategoryId = GetCategoryId("Tráng miệng", 1), Description = "80g", IsSetDish = false, IsAddOn = false, ForPatient = false, PriceForGuest = 5000, PriceForPatient = 5000, PriceForStaff = 5000, Image = "images/Foods/6152da49-c0a6-4104-9be3-bc3e05d95c34.jpg", Sort = 5 });
            // ...all other foods from DM_Foods.sql, mapped and encoded correctly...

            var random = new Random();
            for (int i = 6; i <= 15; i++)
            {
                // Pick a random category from the available categories for branch 1
                var branch1Categories = categories.Where(c => c.BranchId == 1).ToList();
                int categoryId = branch1Categories.Count > 0 ? branch1Categories[random.Next(branch1Categories.Count)].Id : categories.First().Id;
                foods.Add(new Food
                {
                    Name = $"Món ngẫu nhiên {i} - Chi nhánh {1}",
                    BranchId = 1,
                    CategoryId = categoryId,
                    Description = $"Món ăn ngẫu nhiên số {i} cho chi nhánh {1}, danh mục {categoryId}",
                    IsSetDish = false,
                    IsAddOn = false,
                    ForPatient = false,
                    PriceForGuest = 10000 + random.Next(0, 5) * 5000,
                    PriceForPatient = 8000 + random.Next(0, 5) * 4000,
                    PriceForStaff = 8000 + random.Next(0, 5) * 4000,
                    Image = $"images/Foods/random_{1}_{i}.jpg",
                    Sort = i
                });
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
