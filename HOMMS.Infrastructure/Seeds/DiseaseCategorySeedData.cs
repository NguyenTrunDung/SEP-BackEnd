using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HOMMS.Infrastructure.Seeds
{
    public static class DiseaseCategorySeedData
    {
        public static void SeedDiseaseCategories(ModelBuilder modelBuilder)
        {
            // Seed Disease Categories for Vietnamese hospitals
            modelBuilder.Entity<DiseaseCategory>().HasData(
                new DiseaseCategory
                {
                    Id = 1,
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
                    Id = 2,
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
                    Id = 3,
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
                    Id = 4,
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
                    Id = 5,
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
                }
            );
        }

        public static void SeedFoodRestrictions(ModelBuilder modelBuilder)
        {
            // Seed Food Restrictions - assuming some basic food IDs exist
            modelBuilder.Entity<DiseaseCategoryFoodRestriction>().HasData(
                // Diabetes Type 2 Restrictions
                new DiseaseCategoryFoodRestriction
                {
                    Id = 1,
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
                    Id = 2,
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
                    Id = 3,
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
                    Id = 4,
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
                    Id = 5,
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
                    Id = 6,
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
                    Id = 7,
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
                    Id = 8,
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

        public static void SeedPatientDiseaseCategories(ModelBuilder modelBuilder)
        {
            // Seed sample patient assignments using actual Patient IDs from PatientSeedData
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
                    AssignedByPhysician = "BS. Trần Thị Hoa",
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "BS. Trần Thị Hoa"
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
                    AssignedByPhysician = "BS. Nguyễn Minh Đức",
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "BS. Nguyễn Minh Đức"
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
                    AssignedByPhysician = "BS. Lê Văn Thành",
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "BS. Lê Văn Thành"
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
                    AssignedByPhysician = "PGS.TS. Lương Văn Khải",
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "PGS.TS. Lương Văn Khải"
                },
                // Multi-condition patient example
                new PatientDiseaseCategory
                {
                    Id = 5,
                    BranchId = 1,
                    PatientId = "CTH-P001", // Same patient (Nguyễn Văn An) with multiple conditions
                    DiseaseCategoryId = 2, // Hypertension (secondary to diabetes)
                    DiagnosedDate = DateTime.UtcNow.AddDays(-85),
                    PatientSeverityLevel = 2,
                    PatientSpecificNotes = "Tăng huyết áp thứ phát do biến chứng tiểu đường",
                    IsActive = true,
                    AssignedByPhysician = "BS. Trần Thị Hoa",
                    ExpiryDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "BS. Trần Thị Hoa"
                }
            );
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