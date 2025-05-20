using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Extensions
{
    public static class SeedExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            // Create a new scope to get scoped services
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                // Get the DB context
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Apply pending migrations
                await context.Database.MigrateAsync();

                // Seed identity data (roles and admin user)
                await IdentitySeedData.SeedRolesAndAdminAsync(services);

                // Add other seed methods here
                // Example: await ProductSeedData.SeedAsync(context);
            }
            catch (Exception ex)
            {
                // Log the error - use a proper logging mechanism
                var logger = services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
} 