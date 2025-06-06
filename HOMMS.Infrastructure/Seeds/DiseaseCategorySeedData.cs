using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using HOMMS.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace HOMMS.Infrastructure.Seeds
{
    public static class DiseaseCategorySeedData
    {
        /// <summary>
        /// Migration-time seeding is now handled by runtime seeding in SeedDiseaseCategoriesAsync.
        /// This method is kept for reference but should not be called from ModelBuilder configuration.
        /// Use SeedDiseaseCategoriesAsync instead for better flexibility and maintenance.
        /// </summary>
        [Obsolete("Use SeedDiseaseCategoriesAsync for runtime seeding instead")]
        public static void SeedDiseaseCategories(ModelBuilder modelBuilder)
        {
            // NOTE: This method is obsolete. Use SeedDiseaseCategoriesAsync for runtime seeding.
            // Migration-time seeding with HasData() creates maintenance issues:
            // 1. Requires hardcoded IDs that may conflict
            // 2. Data is baked into migration files
            // 3. Cannot handle conditional logic or existence checks
            // 4. Creates code duplication with runtime seeding
        }

        /// <summary>
        /// Migration-time seeding is now handled by runtime seeding.
        /// Use runtime seeding methods for better flexibility and maintenance.
        /// </summary>
        [Obsolete("Use runtime seeding for food restrictions instead")]
        public static void SeedFoodRestrictions(ModelBuilder modelBuilder)
        {
            // Seed Food Restrictions - assuming some basic food IDs exist
            modelBuilder.Entity<DiseaseCategoryFoodRestriction>().HasData(
                // Diabetes Type 2 Restrictions
                new DiseaseCategoryFoodRestriction
                {
                    BranchId = 1,
                    DiseaseCategoryId = 1, // Diabetes
                    FoodId = 15, // Chocolate Cake
                    RestrictionLevel = 4, // Dangerous
                    Reason = "Contains very high sugar content that can cause dangerous blood glucose spikes",
                    AlternativeRecommendations = "Sugar-free pudding, fresh fruit salad, low-sugar desserts",
                    IsActive = true,
                    RequiresPhysicianOverride = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new DiseaseCategoryFoodRestriction
                {
                    BranchId = 1,
                    DiseaseCategoryId = 1, // Diabetes
                    FoodId = 23, // White Rice
                    RestrictionLevel = 2, // Warning
                    Reason = "High glycemic index can cause rapid blood sugar elevation",
                    AlternativeRecommendations = "Brown rice, quinoa, cauliflower rice",
                    IsActive = true,
                    RequiresPhysicianOverride = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new DiseaseCategoryFoodRestriction
                {
                    BranchId = 1,
                    DiseaseCategoryId = 1, // Diabetes
                    FoodId = 31, // Sweet Che (Vietnamese Dessert)
                    RestrictionLevel = 4, // Dangerous
                    Reason = "Traditional Vietnamese dessert with very high sugar content",
                    AlternativeRecommendations = "Sugar-free che, fresh fruit, unsweetened pudding",
                    IsActive = true,
                    RequiresPhysicianOverride = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                
                // Hypertension Restrictions
                new DiseaseCategoryFoodRestriction
                {
                    BranchId = 1,
                    DiseaseCategoryId = 2, // Hypertension
                    FoodId = 45, // Pho Bo (Traditional Vietnamese Soup)
                    RestrictionLevel = 2, // Warning
                    Reason = "High sodium content in traditional broth preparation",
                    AlternativeRecommendations = "Low-sodium pho, clear vegetable broth, steamed dishes",
                    IsActive = true,
                    RequiresPhysicianOverride = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new DiseaseCategoryFoodRestriction
                {
                    BranchId = 1,
                    DiseaseCategoryId = 2, // Hypertension
                    FoodId = 52, // Pickled Vegetables
                    RestrictionLevel = 3, // Prohibited
                    Reason = "Very high sodium content from pickling process",
                    AlternativeRecommendations = "Fresh vegetables, steamed vegetables, low-sodium alternatives",
                    IsActive = true,
                    RequiresPhysicianOverride = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },

                // Cardiovascular Disease Restrictions
                new DiseaseCategoryFoodRestriction
                {
                    BranchId = 1,
                    DiseaseCategoryId = 3, // CVD
                    FoodId = 67, // Fried Spring Rolls
                    RestrictionLevel = 3, // Prohibited
                    Reason = "High saturated fat content from deep frying",
                    AlternativeRecommendations = "Fresh spring rolls, steamed dumplings, grilled options",
                    IsActive = true,
                    RequiresPhysicianOverride = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                
                // Kidney Disease Restrictions
                new DiseaseCategoryFoodRestriction
                {
                    BranchId = 1,
                    DiseaseCategoryId = 4, // CKD
                    FoodId = 78, // Banana
                    RestrictionLevel = 3, // Prohibited
                    Reason = "High potassium content dangerous for kidney patients",
                    AlternativeRecommendations = "Apples, pears, berries (low potassium fruits)",
                    IsActive = true,
                    RequiresPhysicianOverride = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },

                // Gastritis Restrictions
                new DiseaseCategoryFoodRestriction
                {
                    BranchId = 1,
                    DiseaseCategoryId = 5, // Gastritis
                    FoodId = 89, // Spicy Bun Bo Hue
                    RestrictionLevel = 3, // Prohibited
                    Reason = "Spicy and acidic ingredients can irritate stomach lining",
                    AlternativeRecommendations = "Plain rice noodles, chicken broth, steamed dishes",
                    IsActive = true,
                    RequiresPhysicianOverride = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
        }

        /// <summary>
        /// Migration-time seeding is now handled by runtime seeding in SeedPatientDiseaseCategoriesAsync.
        /// Use SeedPatientDiseaseCategoriesAsync for better flexibility and maintenance.
        /// </summary>
        [Obsolete("Use SeedPatientDiseaseCategoriesAsync for runtime seeding instead")]
        public static void SeedPatientDiseaseCategories(ModelBuilder modelBuilder)
        {
            // Note: This seed data uses placeholder user emails that will need to be resolved to actual user IDs
            // during database seeding process. See SeedPatientDiseaseCategoriesAsync method for runtime seeding.
            modelBuilder.Entity<PatientDiseaseCategory>().HasData(
                new PatientDiseaseCategory
                {
                    Id = 1,
                    BranchId = 1,
                    PatientId = "CTH-P001", // Nguyễn Văn An - Diabetes patient
                    DiseaseCategoryId = 1, // Diabetes Type 2
                    DiagnosedDate = DateTime.UtcNow.AddDays(-30),
                    PatientSeverityLevel = 3,
                    PatientSpecificNotes = "Bệnh nhân khó kiểm soát đường huyết, cần theo dõi chặt chẽ",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-nurse-1" // Placeholder - will be updated by async seeding
                },
                new PatientDiseaseCategory
                {
                    Id = 2,
                    BranchId = 1,
                    PatientId = "CTH-P002", // Lê Thị Bình - Hypertension patient
                    DiseaseCategoryId = 2, // Hypertension
                    DiagnosedDate = DateTime.UtcNow.AddDays(-60),
                    PatientSeverityLevel = 2,
                    PatientSpecificNotes = "Cao huyết áp nhẹ, đáp ứng tốt với thay đổi chế độ ăn",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-nurse-2" // Placeholder - will be updated by async seeding
                },
                new PatientDiseaseCategory
                {
                    Id = 3,
                    BranchId = 1,
                    PatientId = "CTH-P004", // Trần Thị Dung - Food allergy patient
                    DiseaseCategoryId = 6, // Food Allergies
                    DiagnosedDate = DateTime.UtcNow.AddDays(-90),
                    PatientSeverityLevel = 3,
                    PatientSpecificNotes = "Dị ứng nghiêm trọng với hải sản, tránh hoàn toàn tôm, cua, cá",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-nurse-3" // Placeholder - will be updated by async seeding
                },
                new PatientDiseaseCategory
                {
                    Id = 4,
                    BranchId = 2,
                    PatientId = "SGH-P001", // Nguyễn Thị Phương - Kidney disease patient
                    DiseaseCategoryId = 4, // Chronic Kidney Disease
                    DiagnosedDate = DateTime.UtcNow.AddDays(-120),
                    PatientSeverityLevel = 4,
                    PatientSpecificNotes = "Bệnh thận mạn giai đoạn cuối, hạn chế protein và phospho",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-doctor-1" // Placeholder - will be updated by async seeding
                },
                // Multi-condition patient example
                new PatientDiseaseCategory
                {
                    Id = 5,
                    BranchId = 1,
                    PatientId = "CTH-P001", // Nguyễn Văn An also has hypertension
                    DiseaseCategoryId = 2, // Hypertension
                    DiagnosedDate = DateTime.UtcNow.AddDays(-45),
                    PatientSeverityLevel = 2,
                    PatientSpecificNotes = "Cao huyết áp thứ phát do tiểu đường",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-nurse-1" // Placeholder - will be updated by async seeding
                }
            );
        }

        /// <summary>
        /// Seeds disease categories asynchronously
        /// </summary>
        public static async Task SeedDiseaseCategoriesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DiseaseCategorySeeding");

            try
            {
                // Get all disease categories to seed
                var diseaseCategories = GetDiseaseCategoriesData();
                
                // Only add categories that don't already exist
                foreach (var category in diseaseCategories)
                {
                    var exists = await dbContext.DiseaseCategories
                        .AnyAsync(dc => dc.Code == category.Code && dc.BranchId == category.BranchId);
                    
                    if (!exists)
                    {
                        dbContext.DiseaseCategories.Add(category);
                    }
                }

                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Successfully seeded disease categories.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding disease categories.");
                throw;
            }
        }

        /// <summary>
        /// Seeds patient disease categories with actual user IDs after users are created
        /// This method should be called after IdentitySeedData.SeedRolesAndAdminAsync
        /// </summary>
        public static async Task SeedPatientDiseaseCategoriesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("PatientDiseaseCategorySeeding");

            try
            {
                // Check if we already have data with real user IDs
                var existingAssignment = await dbContext.PatientDiseaseCategories
                    .FirstOrDefaultAsync(x => x.CreatedBy != null && !x.CreatedBy.StartsWith("temp-"));
                
                if (existingAssignment != null)
                {
                    // Already seeded with real user IDs
                    logger.LogInformation("Patient disease categories already seeded with real user IDs.");
                    return;
                }

                // Get nurse and doctor users
                var nurseTran = await userManager.FindByEmailAsync("nurse.tran@hospital.com");
                var nurseDuc = await userManager.FindByEmailAsync("nurse.duc@hospital.com");
                var nurseThanh = await userManager.FindByEmailAsync("nurse.thanh@hospital.com");
               // var doctorKhai = await userManager.FindByEmailAsync("doctor.khai@hospital.com");

                if (nurseTran == null || nurseDuc == null || nurseThanh == null)
                {
                    // Users not yet created, create manual assignments with actual data
                    logger.LogInformation("Hospital staff users not found, seeding patient disease categories with manual assignments...");
                    
                    var assignments = GetPatientDiseaseCategoryData();
                    foreach (var assignment in assignments)
                    {
                        var exists = await dbContext.PatientDiseaseCategories
                            .AnyAsync(pdc => pdc.PatientId == assignment.PatientId && 
                                           pdc.DiseaseCategoryId == assignment.DiseaseCategoryId);
                        
                        if (!exists)
                        {
                            assignment.CreatedBy = "system"; // Use system as fallback
                            dbContext.PatientDiseaseCategories.Add(assignment);
                        }
                    }
                }
                else
                {
                    // Update placeholder CreatedBy values with actual user IDs or create new assignments
                    var assignmentsToUpdate = await dbContext.PatientDiseaseCategories
                        .Where(x => x.CreatedBy != null && x.CreatedBy.StartsWith("temp-"))
                        .ToListAsync();

                    if (assignmentsToUpdate.Any())
                    {
                        // Update existing placeholder assignments
                        foreach (var assignment in assignmentsToUpdate)
                        {
                            switch (assignment.CreatedBy)
                            {
                                case "temp-nurse-1":
                                    assignment.CreatedBy = nurseTran.Id;
                                    break;
                                case "temp-nurse-2":
                                    assignment.CreatedBy = nurseDuc.Id;
                                    break;
                                case "temp-nurse-3":
                                    assignment.CreatedBy = nurseThanh.Id;
                                    break;
                                //case "temp-doctor-1":
                                //    assignment.CreatedBy = doctorKhai.Id;
                                //break;
                            }
                        }
                    }
                    else
                    {
                        // Create new assignments with real user IDs
                        var assignments = GetPatientDiseaseCategoryData();
                        var userMap = new Dictionary<string, string>
                        {
                            { "temp-nurse-1", nurseTran.Id },
                            { "temp-nurse-2", nurseDuc.Id },
                            { "temp-nurse-3", nurseThanh.Id },
                          //  { "temp-doctor-1", doctorKhai.Id }
                        };

                        foreach (var assignment in assignments)
                        {
                            var exists = await dbContext.PatientDiseaseCategories
                                .AnyAsync(pdc => pdc.PatientId == assignment.PatientId && 
                                               pdc.DiseaseCategoryId == assignment.DiseaseCategoryId);
                            
                            if (!exists)
                            {
                                // Map placeholder to real user ID
                                if (userMap.ContainsKey(assignment.CreatedBy))
                                {
                                    assignment.CreatedBy = userMap[assignment.CreatedBy];
                                }
                                else
                                {
                                    assignment.CreatedBy = nurseTran.Id; // Default fallback
                                }
                                
                                dbContext.PatientDiseaseCategories.Add(assignment);
                            }
                        }
                    }
                }

                await dbContext.SaveChangesAsync();
                logger.LogInformation("Successfully seeded patient disease categories.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding patient disease categories.");
                throw;
            }
        }

        /// <summary>
        /// Gets the disease categories data for seeding
        /// </summary>
        private static List<DiseaseCategory> GetDiseaseCategoriesData()
        {
            return new List<DiseaseCategory>
            {
                new DiseaseCategory
                {
                    BranchId = 1, // Can Tho Hospital
                    Name = "Diabetes Type 2",
                    Code = "DM2",
                    Description = "Type 2 diabetes mellitus requiring dietary carbohydrate management",
                    DietaryRestrictions = "Avoid high sugar foods, limit simple carbohydrates, avoid sugary drinks, limit white rice",
                    RecommendedFoods = "Brown rice, whole grains, lean proteins, vegetables, low-glycemic fruits",
                    IsActive = true,
                    SeverityLevel = 3,
                    RequiresApproval = true,
                    ColorCode = "#FF6B6B",
                    SortOrder = 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new DiseaseCategory
                {
                    BranchId = 1,
                    Name = "Hypertension",
                    Code = "HTN",
                    Description = "High blood pressure requiring low sodium diet",
                    DietaryRestrictions = "Avoid high sodium foods, limit salt, avoid processed foods, limit soy sauce",
                    RecommendedFoods = "Fresh fruits, vegetables, low-sodium broths, grilled/steamed foods",
                    IsActive = true,
                    SeverityLevel = 2,
                    RequiresApproval = false,
                    ColorCode = "#4ECDC4",
                    SortOrder = 2,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new DiseaseCategory
                {
                    BranchId = 1,
                    Name = "Cardiovascular Disease",
                    Code = "CVD",
                    Description = "Heart disease requiring low fat and low cholesterol diet",
                    DietaryRestrictions = "Avoid fried foods, limit saturated fats, avoid high cholesterol foods",
                    RecommendedFoods = "Fish, lean poultry, vegetables, fruits, whole grains, low-fat dairy",
                    IsActive = true,
                    SeverityLevel = 3,
                    RequiresApproval = true,
                    ColorCode = "#45B7D1",
                    SortOrder = 3,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new DiseaseCategory
                {
                    BranchId = 1,
                    Name = "Chronic Kidney Disease",
                    Code = "CKD",
                    Description = "Kidney disease requiring protein and potassium restriction",
                    DietaryRestrictions = "Limit protein, avoid high potassium foods (bananas, oranges), limit phosphorus",
                    RecommendedFoods = "White rice, apples, cabbage, low-protein foods, controlled portions",
                    IsActive = true,
                    SeverityLevel = 4,
                    RequiresApproval = true,
                    ColorCode = "#F7DC6F",
                    SortOrder = 4,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new DiseaseCategory
                {
                    BranchId = 1,
                    Name = "Gastritis",
                    Code = "GAST",
                    Description = "Stomach inflammation requiring bland diet",
                    DietaryRestrictions = "Avoid spicy foods, acidic foods, alcohol, coffee, carbonated drinks",
                    RecommendedFoods = "Bland foods, rice porridge, steamed vegetables, lean proteins",
                    IsActive = true,
                    SeverityLevel = 2,
                    RequiresApproval = false,
                    ColorCode = "#BB8FCE",
                    SortOrder = 5,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new DiseaseCategory
                {
                    BranchId = 1,
                    Name = "Food Allergies",
                    Code = "ALLERGY",
                    Description = "Various food allergies requiring complete avoidance of specific foods",
                    DietaryRestrictions = "Complete avoidance of allergen foods, read all ingredient labels carefully",
                    RecommendedFoods = "Safe alternative foods verified to be allergen-free",
                    IsActive = true,
                    SeverityLevel = 4,
                    RequiresApproval = true,
                    ColorCode = "#E74C3C",
                    SortOrder = 6,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };
        }

        /// <summary>
        /// Gets the patient disease category data for seeding
        /// </summary>
        private static List<PatientDiseaseCategory> GetPatientDiseaseCategoryData()
        {
            return new List<PatientDiseaseCategory>
            {
                new PatientDiseaseCategory
                {
                    BranchId = 1,
                    PatientId = "CTH-P001", // Nguyễn Văn An - Diabetes patient
                    DiseaseCategoryId = 1, // Diabetes Type 2
                    DiagnosedDate = DateTime.UtcNow.AddDays(-30),
                    PatientSeverityLevel = 3,
                    PatientSpecificNotes = "Bệnh nhân khó kiểm soát đường huyết, cần theo dõi chặt chẽ",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-nurse-1" // Will be updated to real user ID
                },
                new PatientDiseaseCategory
                {
                    BranchId = 1,
                    PatientId = "CTH-P002", // Lê Thị Bình - Hypertension patient
                    DiseaseCategoryId = 2, // Hypertension
                    DiagnosedDate = DateTime.UtcNow.AddDays(-60),
                    PatientSeverityLevel = 2,
                    PatientSpecificNotes = "Cao huyết áp nhẹ, đáp ứng tốt với thay đổi chế độ ăn",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-nurse-2" // Will be updated to real user ID
                },
                new PatientDiseaseCategory
                {
                    BranchId = 1,
                    PatientId = "CTH-P004", // Trần Thị Dung - Food allergy patient
                    DiseaseCategoryId = 6, // Food Allergies
                    DiagnosedDate = DateTime.UtcNow.AddDays(-90),
                    PatientSeverityLevel = 3,
                    PatientSpecificNotes = "Dị ứng nghiêm trọng với hải sản, tránh hoàn toàn tôm, cua, cá",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-nurse-3" // Will be updated to real user ID
                },
                new PatientDiseaseCategory
                {
                    BranchId = 2,
                    PatientId = "SGH-P001", // Nguyễn Thị Phương - Kidney disease patient
                    DiseaseCategoryId = 4, // Chronic Kidney Disease
                    DiagnosedDate = DateTime.UtcNow.AddDays(-120),
                    PatientSeverityLevel = 4,
                    PatientSpecificNotes = "Bệnh thận mạn giai đoạn cuối, hạn chế protein và phospho",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-doctor-1" // Will be updated to real user ID
                },
                new PatientDiseaseCategory
                {
                    BranchId = 1,
                    PatientId = "CTH-P001", // Nguyễn Văn An also has hypertension
                    DiseaseCategoryId = 2, // Hypertension
                    DiagnosedDate = DateTime.UtcNow.AddDays(-45),
                    PatientSeverityLevel = 2,
                    PatientSpecificNotes = "Cao huyết áp thứ phát do tiểu đường",
                    IsActive = true,
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "temp-nurse-1" // Will be updated to real user ID
                }
            };
        }

        public static void SeedVietnameseTestData(ModelBuilder modelBuilder)
        {
            // This method would be called to seed Vietnamese-specific food items
            // and their disease category relationships for comprehensive testing
            
            // Note: This assumes Food entities exist with these IDs
            // In a real implementation, you would coordinate with your Food seeding
            
            var vietnameseFoodRestrictions = new List<DiseaseCategoryFoodRestriction>
            {
                // Vietnamese Traditional Foods with Disease Restrictions
                new DiseaseCategoryFoodRestriction
                {
                    Id = 100,
                    BranchId = 1,
                    DiseaseCategoryId = 1, // Diabetes
                    FoodId = 101, // Banh Chung (Traditional sticky rice cake)
                    RestrictionLevel = 3, // Prohibited
                    Reason = "Bánh chưng chứa nhiều tinh bột và đường, có thể làm tăng đường huyết nguy hiểm",
                    AlternativeRecommendations = "Bánh tét không đường, cơm gạo lứt, rau củ luộc",
                    IsActive = true,
                    RequiresPhysicianOverride = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new DiseaseCategoryFoodRestriction
                {
                    Id = 101,
                    BranchId = 1,
                    DiseaseCategoryId = 2, // Hypertension
                    FoodId = 102, // Nuoc Mam (Fish sauce)
                    RestrictionLevel = 4, // Dangerous
                    Reason = "Nước mắm chứa hàm lượng natrium cực cao, nguy hiểm cho bệnh nhân tăng huyết áp",
                    AlternativeRecommendations = "Nước mắm ít muối, gia vị tự nhiên, nước tương nhạt",
                    IsActive = true,
                    RequiresPhysicianOverride = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };

            modelBuilder.Entity<DiseaseCategoryFoodRestriction>().HasData(vietnameseFoodRestrictions);
        }
    }
} 