using HOMMS.Application.Interfaces;
using HOMMS.Domain.Entities;
using HOMMS.Domain.Entities.Base;
using HOMMS.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IConfiguration _configuration;
        private readonly IBranchContext _branchContext;
        private readonly bool _multiTenancyEnabled;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IConfiguration configuration,
            IBranchContext branchContext)
            : base(options)
        {
            _configuration = configuration;
            _branchContext = branchContext ?? throw new ArgumentNullException(nameof(branchContext));
            _multiTenancyEnabled = branchContext != null;
        }

        // Add your DbSet properties here
        public DbSet<Branch> Branches { get; set; }
        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuDetail> MenuDetails { get; set; }

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

            // Apply global query filter for multi-tenancy
            if (_multiTenancyEnabled)
            {
                // Get all entity types that implement IBranchEntity
                var branchEntityTypes = builder.Model.GetEntityTypes()
                    .Where(e => typeof(IBranchEntity).IsAssignableFrom(e.ClrType));

                foreach (var entityType in branchEntityTypes)
                {
                    // Skip the Branch entity itself from filtering
                    if (entityType.ClrType == typeof(Branch))
                        continue;

                    // Use the non-generic Entity(Type) method to avoid ambiguity
                    var entityBuilder = builder.Entity(entityType.ClrType);

                    // Use dynamic to avoid invalid cast exception
                    var filterMethod = typeof(ApplicationDbContext)
                        .GetMethod(nameof(ApplyBranchFilter), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                        ?.MakeGenericMethod(entityType.ClrType);

                    if (filterMethod == null)
                    {
                        throw new InvalidOperationException($"The ApplyBranchFilter method could not be found for type {entityType.ClrType.Name}.");
                    }

                    // Pass as dynamic to avoid cast issues
                    filterMethod.Invoke(this, new object[] { entityBuilder });
                }
            }
        }

        private void ApplyEntityConfigurations(ModelBuilder builder)
        {
            // Register all entity configurations
            builder.ApplyConfiguration(new FoodCategoryConfiguration());
            builder.ApplyConfiguration(new FoodConfiguration());
            builder.ApplyConfiguration(new MenuConfiguration());
            builder.ApplyConfiguration(new MenuDetailConfiguration());
            builder.ApplyConfiguration(new BranchConfiguration());

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

        private void ApplyBranchFilter<TEntity>(dynamic builder)
            where TEntity : class, IBranchEntity
        {
            builder.HasQueryFilter((System.Linq.Expressions.Expression<System.Func<TEntity, bool>>)(e => !_multiTenancyEnabled || e.BranchId == GetCurrentBranchId()));
        }
        
        // Gets the current branch ID from the branch context or returns the default
        private int GetCurrentBranchId()
        {
            try
            {
                return _branchContext?.GetCurrentBranchId() ?? 1; // Default to 1 (main branch)
            }
            catch
            {
                // For data seeding or when context is not available
                return 1; // Default to 1 (main branch)
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Apply branch filtering for new entities if multi-tenancy is enabled
            if (_multiTenancyEnabled)
            {
                var branchId = GetCurrentBranchId();
                
                foreach (var entry in ChangeTracker.Entries<IBranchEntity>()
                    .Where(e => e.State == EntityState.Added && e.Entity.BranchId == 0))
                {
                    entry.Entity.BranchId = branchId;
                }
            }
            
            return base.SaveChangesAsync(cancellationToken);
        }
        
        public override int SaveChanges()
        {
            // Apply branch filtering for new entities if multi-tenancy is enabled
            if (_multiTenancyEnabled)
            {
                var branchId = GetCurrentBranchId();
                
                foreach (var entry in ChangeTracker.Entries<IBranchEntity>()
                    .Where(e => e.State == EntityState.Added && e.Entity.BranchId == 0))
                {
                    entry.Entity.BranchId = branchId;
                }
            }
            
            return base.SaveChanges();
        }
    }
} 