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

                // Only seed branch roles if none exist for this branch
                if (!await context.BranchRoles.AnyAsync(r => r.BranchId == branchId))
                {
                    logger.LogInformation("Seeding branch roles and permissions...");
                    await HOMMS.Infrastructure.Seeds.BranchRoleSeedData.SeedBranchRolesAsync(services, branchId);
                    logger.LogInformation("Branch roles and permissions seeded successfully.");
                }
                else
                {
                    logger.LogInformation($"Branch roles already exist for branchId {branchId}. Skipping branch role seeding.");
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
    }
} 