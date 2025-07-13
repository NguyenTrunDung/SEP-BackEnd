using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HOMMS.Infrastructure.Seeds
{
    /// <summary>
    /// Seed data for DiseaseCategoryFoodRestriction entities
    /// Provides food restrictions for different disease categories
    /// </summary>
    public static class DiseaseCategoryFoodRestrictionSeedData
    {
        /// <summary>
        /// Seeds disease category food restrictions asynchronously
        /// This method should be called after Disease Categories and Foods are seeded
        /// </summary>
        /// <param name="serviceProvider">Service provider for dependency injection</param>
        public static async Task SeedDiseaseCategoryFoodRestrictionsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DiseaseCategoryFoodRestrictionSeeding");

            try
            {
                // Check if we already have food restrictions data
                var existingRestrictions = await dbContext.DiseaseCategoryFoodRestrictions.AnyAsync();
                if (existingRestrictions)
                {
                    logger.LogInformation("Disease category food restrictions already exist. Skipping seeding.");
                    return;
                }

                // Get disease categories and foods for mapping
                var diseaseCategories = await dbContext.DiseaseCategories.ToListAsync();
                var foods = await dbContext.Foods.ToListAsync();

                if (!diseaseCategories.Any() || !foods.Any())
                {
                    logger.LogWarning("Disease categories or foods not found. Skipping food restrictions seeding.");
                    return;
                }

                // Create mappings for easier lookup
                var diseaseCategoryMap = diseaseCategories.ToDictionary(dc => dc.Code, dc => dc.Id);
                var foodMap = foods.GroupBy(f => f.Name.ToLower()).ToDictionary(g => g.Key, g => g.First().Id);

                // Get the restrictions to seed
                var restrictions = GetDiseaseCategoryFoodRestrictionsData(diseaseCategoryMap, foodMap);

                // Add only restrictions that don't already exist
                foreach (var restriction in restrictions)
                {
                    var exists = await dbContext.DiseaseCategoryFoodRestrictions
                        .AnyAsync(r => r.DiseaseCategoryId == restriction.DiseaseCategoryId &&
                                     r.FoodId == restriction.FoodId &&
                                     r.BranchId == restriction.BranchId);

                    if (!exists)
                    {
                        dbContext.DiseaseCategoryFoodRestrictions.Add(restriction);
                    }
                }

                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Successfully seeded {restrictions.Count} disease category food restrictions.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding disease category food restrictions.");
                throw;
            }
        }

        /// <summary>
        /// Gets the disease category food restrictions data for seeding
        /// </summary>
        /// <param name="diseaseCategoryMap">Dictionary mapping disease category codes to IDs</param>
        /// <param name="foodMap">Dictionary mapping food names to IDs</param>
        /// <returns>List of DiseaseCategoryFoodRestriction entities</returns>
        private static List<DiseaseCategoryFoodRestriction> GetDiseaseCategoryFoodRestrictionsData(
            Dictionary<string, int> diseaseCategoryMap, 
            Dictionary<string, int> foodMap)
        {
            var restrictions = new List<DiseaseCategoryFoodRestriction>();

            // Helper method to get disease category ID safely
            int GetDiseaseCategoryId(string code)
            {
                return diseaseCategoryMap.ContainsKey(code) ? diseaseCategoryMap[code] : 0;
            }

            // Helper method to get food ID safely - try multiple variations
            int GetFoodId(string name)
            {
                var variations = new[]
                {
                    name.ToLower(),
                    name.ToLower().Replace("(", "").Replace(")", "").Trim(),
                    name.ToLower().Replace("(", "").Replace(")", "").Replace("g", "").Trim(),
                    name.ToLower().Replace("(", "").Replace(")", "").Replace(" ", "").Trim()
                };

                Console.WriteLine($"🔍 Searching for food: '{name}'");
                Console.WriteLine($"🔍 Available food names: [{string.Join(", ", foodMap.Keys)}]");

                foreach (var variation in variations)
                {
                    Console.WriteLine($"🔍 Trying variation: '{variation}'");
                    if (foodMap.ContainsKey(variation))
                    {
                        Console.WriteLine($"✅ Found food '{name}' with ID: {foodMap[variation]}");
                        return foodMap[variation];
                    }
                }
                Console.WriteLine($"❌ Food '{name}' not found in any variation");
                return 0;
            }

            // === DIABETES TYPE 2 RESTRICTIONS ===
            var diabetesId = GetDiseaseCategoryId("DM2");
            if (diabetesId > 0)
            {
                // Bún Riêu - high sugar content from broth
                var bunRieuId = GetFoodId("bún riêu (370g)");
                if (bunRieuId > 0)
                {
                    restrictions.Add(new DiseaseCategoryFoodRestriction
                    {
                        BranchId = 1,
                        DiseaseCategoryId = diabetesId,
                        FoodId = bunRieuId,
                        RestrictionLevel = 2, // Warning
                        Reason = "Bún riêu có nước dùng chứa đường và tinh bột cao, cần kiểm soát khẩu phần",
                        AlternativeRecommendations = "Bún rau củ, canh rau không đường, cháo tôm nhạt",
                        IsActive = true,
                        RequiresPhysicianOverride = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }

                // Fruit - high sugar content
                var fruitId = GetFoodId("trái cây");
                if (fruitId > 0)
                {
                    restrictions.Add(new DiseaseCategoryFoodRestriction
                    {
                        BranchId = 1,
                        DiseaseCategoryId = diabetesId,
                        FoodId = fruitId,
                        RestrictionLevel = 1, // Advisory
                        Reason = "Trái cây chứa đường tự nhiên, cần hạn chế lượng và chọn loại ít đường",
                        AlternativeRecommendations = "Táo, ổi, dưa leo, trái cây ít ngọt với khẩu phần nhỏ",
                        IsActive = true,
                        RequiresPhysicianOverride = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }
            }

            // === HYPERTENSION RESTRICTIONS ===
            var hypertensionId = GetDiseaseCategoryId("HTN");
            if (hypertensionId > 0)
            {
                // Bún riêu - high sodium from broth
                var bunRieuId = GetFoodId("bún riêu (370g)");
                if (bunRieuId > 0)
                {
                    restrictions.Add(new DiseaseCategoryFoodRestriction
                    {
                        BranchId = 1,
                        DiseaseCategoryId = hypertensionId,
                        FoodId = bunRieuId,
                        RestrictionLevel = 3, // Prohibited
                        Reason = "Nước dùng bún riêu có hàm lượng natrium cao từ nước mắm và gia vị",
                        AlternativeRecommendations = "Bún rau củ nước trong, canh rau không muối, món hấp",
                        IsActive = true,
                        RequiresPhysicianOverride = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }
            }

            // === CARDIOVASCULAR DISEASE RESTRICTIONS ===
            var cvdId = GetDiseaseCategoryId("CVD");
            if (cvdId > 0)
            {
                // Đậu hũ nhồi thịt - contains meat fat
                var dauHuId = GetFoodId("đậu hũ nhồi thịt sốt cà (140,5g)");
                if (dauHuId > 0)
                {
                    restrictions.Add(new DiseaseCategoryFoodRestriction
                    {
                        BranchId = 1,
                        DiseaseCategoryId = cvdId,
                        FoodId = dauHuId,
                        RestrictionLevel = 2, // Warning
                        Reason = "Thịt nhồi có thể chứa chất béo bão hòa, cần hạn chế cho bệnh nhân tim mạch",
                        AlternativeRecommendations = "Đậu hũ hấp, rau củ luộc, protein thực vật",
                        IsActive = true,
                        RequiresPhysicianOverride = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }
            }

            // === CHRONIC KIDNEY DISEASE RESTRICTIONS ===
            var ckdId = GetDiseaseCategoryId("CKD");
            if (ckdId > 0)
            {
                // Canh bí đỏ thịt bằm - protein restriction
                var canhBiId = GetFoodId("canh bí đỏ thịt bằm (85.5g)");
                if (canhBiId > 0)
                {
                    restrictions.Add(new DiseaseCategoryFoodRestriction
                    {
                        BranchId = 1,
                        DiseaseCategoryId = ckdId,
                        FoodId = canhBiId,
                        RestrictionLevel = 2, // Warning
                        Reason = "Thịt bằm chứa protein cao, cần hạn chế lượng protein cho bệnh nhân thận",
                        AlternativeRecommendations = "Canh bí đỏ không thịt, rau củ luộc, khẩu phần protein được kiểm soát",
                        IsActive = true,
                        RequiresPhysicianOverride = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }
            }

            // === GASTRITIS RESTRICTIONS ===
            var gastritisId = GetDiseaseCategoryId("GAST");
            if (gastritisId > 0)
            {
                // Simple restriction for acidic foods
                var dauHuId = GetFoodId("đậu hũ nhồi thịt sốt cà (140,5g)");
                if (dauHuId > 0)
                {
                    restrictions.Add(new DiseaseCategoryFoodRestriction
                    {
                        BranchId = 1,
                        DiseaseCategoryId = gastritisId,
                        FoodId = dauHuId,
                        RestrictionLevel = 3, // Prohibited
                        Reason = "Sốt cà chua có tính axit cao gây kích ứng niêm mạc dạ dày",
                        AlternativeRecommendations = "Đậu hũ hấp nhạt, cháo, rau củ luộc",
                        IsActive = true,
                        RequiresPhysicianOverride = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }
            }

            // === FOOD ALLERGIES RESTRICTIONS ===
            var allergyId = GetDiseaseCategoryId("ALLERGY");
            if (allergyId > 0)
            {
                // Bún riêu - contains seafood
                var bunRieuId = GetFoodId("bún riêu (370g)");
                if (bunRieuId > 0)
                {
                    restrictions.Add(new DiseaseCategoryFoodRestriction
                    {
                        BranchId = 1,
                        DiseaseCategoryId = allergyId,
                        FoodId = bunRieuId,
                        RestrictionLevel = 4, // Dangerous
                        Reason = "Bún riêu chứa chả cua và hải sản có thể gây dị ứng nghiêm trọng",
                        AlternativeRecommendations = "Bún rau củ, phở gà, cháo thịt (nếu không dị ứng)",
                        IsActive = true,
                        RequiresPhysicianOverride = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }
            }

            // Return only restrictions where both disease category and food exist
            return restrictions.Where(r => r.DiseaseCategoryId > 0 && r.FoodId > 0).ToList();
        }
    }
}