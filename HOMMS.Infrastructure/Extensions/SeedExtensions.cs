using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Extensions
{
    public static class SeedExtensions
    {
        public static async Task SeedDatabaseAsync(this Microsoft.AspNetCore.Builder.WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeding");
            try
            {
                logger.LogInformation("Applying database migrations...");
                var context = services.GetRequiredService<ApplicationDbContext>();
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");

                // Only seed branches if none exist
                int branchId = 0;
                if (!await context.Branches.AnyAsync())
                {
                    logger.LogInformation("Seeding branches...");
                    branchId = await HOMMS.Infrastructure.Seeds.BranchSeedData.SeedDefaultBranchAsync(services);
                    logger.LogInformation("Branches seeded successfully.");
                }
                else
                {
                    branchId = context.Branches.Select(b => b.Id).FirstOrDefault();
                    logger.LogInformation("Branches already exist. Skipping branch seeding.");
                }

                // Only seed roles and admin if no roles exist
                if (!await context.Roles.AnyAsync())
                {
                    logger.LogInformation("Seeding identity data (roles and admin user)...");
                    await IdentitySeedData.SeedRolesAndAdminAsync(services, branchId);
                    logger.LogInformation("Identity data seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Roles already exist. Skipping identity seeding.");
                }

                // Only seed branch roles if none exist for this branch (check via BranchUserRole associations)
                var hasExistingRoles = await context.BranchUserRoles.AnyAsync(bur => 
                    bur.BranchId == branchId && !bur.IsDeleted);
                if (!hasExistingRoles)
                {
                    logger.LogInformation("Seeding branch roles and permissions...");
                    await HOMMS.Infrastructure.Seeds.BranchRoleSeedData.SeedBranchRolesAsync(services, branchId);
                    logger.LogInformation("Branch roles and permissions seeded successfully.");
                }
                else
                {
                    logger.LogInformation($"Branch roles already exist for branchId {branchId}. Skipping branch role seeding.");
                }

                // Only seed branch user roles if none exist for this branch
                if (!await context.BranchUserRoles.AnyAsync(bur => bur.BranchId == branchId))
                {
                    logger.LogInformation("Seeding branch user role relationships...");
                    await BranchUserRoleSeedData.SeedBranchUserRolesAsync(services, branchId);
                    logger.LogInformation("Branch user role relationships seeded successfully.");
                }
                else
                {
                    logger.LogInformation($"Branch user roles already exist for branchId {branchId}. Skipping branch user role seeding.");
                }

                // Seed Food Categories if none exist
                if (!await context.FoodCategories.AnyAsync())
                {
                    logger.LogInformation("Seeding food categories...");
                    await FoodCategorySeedData.SeedCateAsync(services).ConfigureAwait(false);
                    logger.LogInformation("Food categories seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Food categories already exist. Skipping food category seeding.");
                }

                ///Seed Foods if none exist
                if (!await context.Foods.AnyAsync())
                {
                    logger.LogInformation("Seeding foods...");
                    await FoodSeedData.SeedFoodsAsync(services);
                    logger.LogInformation("Foods seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Foods already exist. Skipping food seeding.");
                }

                ///Seed Menus if none exist
                if (!await context.Menus.AnyAsync())
                {
                    logger.LogInformation("Seeding menus...");
                    await MenuSeedData.MenuSeedDataAsync(services);
                    logger.LogInformation("Menus seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Menus already exist. Skipping menu seeding.");
                }

                ///Seed Menus Detail if none exist
                if (!await context.MenuDetails.AnyAsync())
                {
                    logger.LogInformation("Seeding menus detail...");
                    await MenuDetailSeedData.MenuDetailDataAsync(services);
                    logger.LogInformation("Menus detail seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Menus detail already exist. Skipping menu seeding.");
                }

                ///Seed System Log if none exist
                if (!await context.SystemLogs.AnyAsync())
                {
                    logger.LogInformation("Seeding System Logl...");
                    await SystemLogSeedData.SystemLogSeedDataAsync(services);
                    logger.LogInformation("System Log seeded successfully.");
                }
                else
                {
                    logger.LogInformation("System Log already exist. Skipping menu seeding.");
                }

                ///Seed Order if none exist
                if (!await context.Orders.AnyAsync())
                {
                    logger.LogInformation("Seeding order detail...");
                    await OrderSeedData.OrderSeedDataAsync(services);
                    logger.LogInformation("Order seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Order already exist. Skipping order seeding.");
                }

                ///Seed Order Detail if none exist
                if (!await context.OrderDetails.AnyAsync())
                {
                    logger.LogInformation("Seeding order detail...");
                    await OrderDetailsSeedData.OrderDetailsSeedDataAsync(services);
                    logger.LogInformation("Order detail seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Order detail already exist. Skipping order detail seeding.");
                }

                ///Seed Patients if none exist
                if (!await context.Patients.AnyAsync())
                {
                    logger.LogInformation("Seeding patients...");
                    await PatientSeedData.SeedPatientsAsync(services);
                    logger.LogInformation("Patients seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Patients already exist. Skipping patient seeding.");
                }

                ///Seed Disease Categories if none exist
                if (!await context.DiseaseCategories.AnyAsync())
                {
                    logger.LogInformation("Seeding disease categories...");
                    await DiseaseCategorySeedData.SeedDiseaseCategoriesAsync(services);
                    logger.LogInformation("Disease categories seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Disease categories already exist. Skipping disease category seeding.");
                }

                ///Seed Patient Disease Categories if none exist
                if (!await context.PatientDiseaseCategories.AnyAsync())
                {
                    logger.LogInformation("Seeding patient disease categories...");
                    await DiseaseCategorySeedData.SeedPatientDiseaseCategoriesAsync(services);
                    logger.LogInformation("Patient disease categories seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Patient disease categories already exist. Skipping patient disease category seeding.");
                }

                ///Seed Disease Category Food Restrictions if none exist
                if (!await context.DiseaseCategoryFoodRestrictions.AnyAsync())
                {
                    logger.LogInformation("Seeding disease category food restrictions...");
                    await DiseaseCategoryFoodRestrictionSeedData.SeedDiseaseCategoryFoodRestrictionsAsync(services);
                    logger.LogInformation("Disease category food restrictions seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Disease category food restrictions already exist. Skipping disease category food restriction seeding.");
                }

                // Seed Areas if none exist
                if (!await context.Areas.AnyAsync())
                {
                    logger.LogInformation("Seeding areas...");
                    await AreaSeedData.AreaSeedDataAsync(services);
                    logger.LogInformation("Areas seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Areas already exist. Skipping area seeding.");
                }
                //Seed Locations if none exist
                if (!await context.Locations.AnyAsync())
                {
                    logger.LogInformation("Seeding locations...");
                    await LocationSeedData.LocationSeedDataAsync(services);
                    logger.LogInformation("Locations seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Locations already exist. Skipping location seeding.");
                }

                // Add other seed methods here if needed
                // logger.LogInformation("Seeding additional data...");
                // await ProductSeedData.SeedAsync(context);
                // logger.LogInformation("Additional data seeded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public static async Task ClearAllDataAsync(this ApplicationDbContext context)
        {
            // Remove all data from all tables except __EFMigrationsHistory and Branches
            // Order matters due to FK constraints, so delete child tables first
            // Adjust the order below as needed for your schema

            // Remove child/relationship tables first
            await context.UserWalletTransactions.ExecuteDeleteAsync();
            await context.OrderDetails.ExecuteDeleteAsync();
            await context.Orders.ExecuteDeleteAsync();
            await context.MenuDetails.ExecuteDeleteAsync();
            await context.Menus.ExecuteDeleteAsync();
            await context.Foods.ExecuteDeleteAsync();
            await context.FoodCategories.ExecuteDeleteAsync();
            await context.PatientDiseaseCategories.ExecuteDeleteAsync();
            await context.DiseaseCategoryFoodRestrictions.ExecuteDeleteAsync();
            await context.DiseaseCategories.ExecuteDeleteAsync();
            await context.Patients.ExecuteDeleteAsync();
            await context.BranchUserRoles.ExecuteDeleteAsync();
            await context.BranchUsers.ExecuteDeleteAsync();
            await context.BranchRoles.ExecuteDeleteAsync();
            await context.SystemLogs.ExecuteDeleteAsync();
            // Delete Locations before Areas to avoid FK constraint violation
            await context.Locations.ExecuteDeleteAsync();
            await context.Areas.ExecuteDeleteAsync();
            // Identity tables
            await context.Users.ExecuteDeleteAsync();
            await context.Roles.ExecuteDeleteAsync();
            await context.Set<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ExecuteDeleteAsync();
            await context.Set<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ExecuteDeleteAsync();
            await context.Set<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ExecuteDeleteAsync();
            await context.Set<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ExecuteDeleteAsync();
            await context.Set<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ExecuteDeleteAsync();
            // Do NOT delete Branches to keep seed data consistent
            // await context.Branches.ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Reseed identity columns for all tables with int PKs (except Branches)
            var tablesToReseed = new[]
            {
                "Areas",
                "BranchRoles",
                "BranchUserRoles",
                "BranchUsers",
                "DiseaseCategories",
                "DiseaseCategoryFoodRestrictions",
                "Food_Categories",
                "Foods",
                "Locations",
                "Menu_Details",
                "Menus",
                "OrderDetails",
                "Orders",
                "PatientDiseaseCategories",
                "SystemLogs",
                "UserWalletTransactions"
            };

            foreach (var table in tablesToReseed)
            {
                await context.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('[dbo].[{table}]', RESEED, 0);");
            }
        }

        //public static async Task SeedDatabaseWithClearAsync(this Microsoft.AspNetCore.Builder.WebApplication app)
        //{
        //    using var scope = app.Services.CreateScope();
        //    var services = scope.ServiceProvider;
        //    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeding");
        //    try
        //    {
        //        logger.LogInformation("Applying database migrations...");
        //        var context = services.GetRequiredService<ApplicationDbContext>();
        //        await context.Database.MigrateAsync();
        //        logger.LogInformation("Database migrations applied successfully.");

        //        // Clear all data before seeding
        //        logger.LogInformation("Clearing all data from all tables...");
        //        await context.ClearAllDataAsync();
        //        logger.LogInformation("All data cleared.");

        //        // Now seed as usual
        //        await app.SeedDatabaseAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "An error occurred while clearing and seeding the database.");
        //        throw;
        //    }
        //}
    }
}