using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Seeds
{
    public static class FoodCategorySeedData
    {
        public static async Task SeedCateAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var categories = new List<FoodCategory>
            {
                new FoodCategory { Name = "Điểm tâm", BranchId = 3, Image = "images/FoodCategories/d1f55a13-0e73-499f-8f9a-29a7beb0901e.jpg", Sort = 1, Active = true },
                new FoodCategory { Name = "Món chính", BranchId = 3, Image = "images/FoodCategories/e9f42583-dc3a-4c42-aa74-c13012974998.jpg", Sort = 2, Active = true },
                new FoodCategory { Name = "Món Khác", BranchId = 3, Image = "images/FoodCategories/c687fa81-a057-4980-9589-fd93e571563e.jpg", Sort = 5, Active = true },
                new FoodCategory { Name = "Nước giải khát", BranchId = 4, Image = "images/FoodCategories/757836be-c454-456b-86d5-8c127732720a.jpg", Sort = 6, Active = true },
                new FoodCategory { Name = "Tráng miệng", BranchId = 4, Image = "images/FoodCategories/d632a123-49fa-498b-a279-6bdb2332b138.jpg", Sort = 4, Active = true },
                new FoodCategory { Name = "Món Chay", BranchId = 4, Image = "images/FoodCategories/dbbd2851-b779-439c-86c3-3bf05fe4db9d.png", Sort = 3, Active = true },
                new FoodCategory { Name = "Điểm Tâm", BranchId = 1, Image = "images/FoodCategories/3c80715d-9c1d-4d19-a8a5-1aecd5005d43.png", Sort = 1, Active = true },
                new FoodCategory { Name = "Điểm Tâm", BranchId = 3, Image = "images/FoodCategories/a7e2f138-2eaa-4bc0-a03e-48b6260720ab.png", Sort = 1, Active = true },
                new FoodCategory { Name = "Món Chính", BranchId = 2, Sort = 2, Active = true },
                new FoodCategory { Name = "Món Chay", BranchId = 2, Sort = 3, Active = true },
                new FoodCategory { Name = "Tráng Miệng", BranchId = 2, Sort = 4, Active = true },
                new FoodCategory { Name = "Món Khác", BranchId = 2, Sort = 5, Active = true },
                new FoodCategory { Name = "Nước Giải Khát", BranchId = 2, Sort = 6, Active = true },
                new FoodCategory { Name = "Điểm tâm", BranchId = 1, Image = "images/FoodCategories/275a3044-a4f5-4f22-bc82-42dcda2ad2a2.jpg", Sort = 1, Active = true },
                new FoodCategory { Name = "Món chính", BranchId = 1, Image = "images/FoodCategories/06c40819-5e98-4e4e-abc0-1928395e85d6.jpg", Sort = 2, Active = true },
                new FoodCategory { Name = "Tráng miệng", BranchId = 1, Image = "images/FoodCategories/fbad326c-44e0-46c5-acee-879a9777d01f.jpg", Sort = 4, Active = true },
                new FoodCategory { Name = "Món chay", BranchId = 1, Image = "images/FoodCategories/1a2bae3b-09ad-4051-87e4-3a22e543d661.jpg", Sort = 3, Active = true },
                new FoodCategory { Name = "Món canh", BranchId = 3, Image = "images/FoodCategories/7da5ac6e-2c5e-4b0f-adc9-a10b0f4433ee.jpg", Sort = 5, Active = true },
                new FoodCategory { Name = "Món xào", BranchId = 3, Image = "images/FoodCategories/cbdd8998-2ea2-472b-bcd1-2b72fa34d48b.jpg", Sort = 6, Active = true }
            };

            foreach (var category in categories)
            {
                if (!dbContext.FoodCategories.Any(c => c.Name == category.Name && c.BranchId == category.BranchId))
                {
                    dbContext.FoodCategories.Add(category);
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
