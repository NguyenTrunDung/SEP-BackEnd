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

                logger.LogInformation("Seeding identity data (roles and admin user)...");
                await IdentitySeedData.SeedRolesAndAdminAsync(services);
                logger.LogInformation("Identity data seeded successfully.");

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