using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace HOMMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This is a fallback if the context is created without explicit configuration
                // For example, when running migrations from the command line
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                    .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply entity configurations
            ApplyEntityConfigurations(builder);

            // Custom model configurations
            CustomizeIdentityModel(builder);
        }

        private void ApplyEntityConfigurations(ModelBuilder builder)
        {
            // Register all entity configurations
            // Example:
            // builder.ApplyConfiguration(new OrderConfiguration());
            // builder.ApplyConfiguration(new ProductConfiguration());
        }

        private void CustomizeIdentityModel(ModelBuilder builder)
        {
            // Customize the ASP.NET Identity model
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                // Customize properties, indexes, etc.
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("Roles");
                // Customize properties, indexes, etc.
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }

        // Add your DbSet properties here
        // Example:
        // public DbSet<Order> Orders { get; set; }
        // public DbSet<Product> Products { get; set; }
    }
} 