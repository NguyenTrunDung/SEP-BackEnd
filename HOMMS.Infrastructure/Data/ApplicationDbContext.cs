using HOMMS.Application.Interfaces;
using HOMMS.Domain.Entities;
using HOMMS.Domain.Entities.Base;
using HOMMS.Infrastructure.Configurations;
using HOMMS.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly bool _multiTenancyEnabled;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IConfiguration configuration,
            IBranchContext branchContext,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _configuration = configuration;
            _branchContext = branchContext ?? throw new ArgumentNullException(nameof(branchContext));
            _httpContextAccessor = httpContextAccessor;
            _multiTenancyEnabled = branchContext != null;
        }

        // Add your DbSet properties here
        public DbSet<Branch> Branches { get; set; }
        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuDetail> MenuDetails { get; set; }
        public DbSet<BranchRole> BranchRoles { get; set; }
        public DbSet<BranchUser> BranchUsers { get; set; }
        public DbSet<BranchUserRole> BranchUserRoles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<UserWalletTransaction> UserWalletTransactions { get; set; }

 
        public DbSet<Comment> Comment { get; set; }

 

        public DbSet<UserWallet> UserWallets { get; set; }


        // Disease Category Management DbSets
        public DbSet<DiseaseCategory> DiseaseCategories { get; set; }
        public DbSet<PatientDiseaseCategory> PatientDiseaseCategories { get; set; }
        public DbSet<DiseaseCategoryFoodRestriction> DiseaseCategoryFoodRestrictions { get; set; }
        
        // Patient Management DbSet
        public DbSet<Patient> Patients { get; set; }
        
        // Area and Location Management DbSets
        public DbSet<Area> Areas { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DbSet<Department> Department { get; set; }

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

            // Remove direct configuration for BranchRole and BranchUserRole relationships here

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

            // Apply global query filter for soft delete (ISoftDeletable)
            var softDeleteEntityTypes = builder.Model.GetEntityTypes()
                .Where(e => typeof(ISoftDeletable).IsAssignableFrom(e.ClrType));
            foreach (var entityType in softDeleteEntityTypes)
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(ApplySoftDeleteFilter), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.MakeGenericMethod(entityType.ClrType);
                if (method != null)
                {
                    var entityBuilder = builder.Entity(entityType.ClrType);
                    method.Invoke(this, new object[] { entityBuilder });
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
            builder.ApplyConfiguration(new BranchRoleConfiguration());
            builder.ApplyConfiguration(new BranchUserConfiguration());
            builder.ApplyConfiguration(new BranchUserRoleConfiguration());
            builder.ApplyConfiguration(new OrdersConfiguration());
            builder.ApplyConfiguration(new OrderDetailsConfiguration());
            builder.ApplyConfiguration(new UserWalletTransactionConfiguration());
            builder.ApplyConfiguration(new SystemLogsConfigurations());
 
            builder.ApplyConfiguration(new CommentConfiguration());

 
            // Disease Category configurations
            builder.ApplyConfiguration(new DiseaseCategoryConfiguration());
            builder.ApplyConfiguration(new PatientDiseaseCategoryConfiguration());
            builder.ApplyConfiguration(new DiseaseCategoryFoodRestrictionConfiguration());
            
            // Patient configurations
            builder.ApplyConfiguration(new PatientConfiguration());
            
            // Area and Location configurations
            builder.ApplyConfiguration(new AreaConfiguration());
            builder.ApplyConfiguration(new LocationConfiguration());
            builder.ApplyConfiguration(new DepartmentConfiguration());

        }

        private void CustomizeIdentityModel(ModelBuilder builder)
        {
            // Customize the ASP.NET Identity model
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                
                // Configure Patient relationship
                entity.HasOne(u => u.Patient)
                    .WithOne(p => p.User)
                    .HasForeignKey<ApplicationUser>(u => u.PatientId)
                    .OnDelete(DeleteBehavior.SetNull);
                    
                // Add index for PatientId
                entity.HasIndex(u => u.PatientId)
                    .HasDatabaseName("IX_Users_PatientId");
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
        
        private void ApplySoftDeleteFilter<TEntity>(dynamic builder)
            where TEntity : class, ISoftDeletable
        {
            builder.HasQueryFilter((System.Linq.Expressions.Expression<System.Func<TEntity, bool>>)(e => !e.IsDeleted));
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
            // Apply audit information before saving changes
            ApplyAuditInformation();
            
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
            // Apply audit information before saving changes
            ApplyAuditInformation();
            
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

        /// <summary>
        /// Applies audit information to entities that implement IAuditableEntity or ISoftDeletable
        /// </summary>
        private void ApplyAuditInformation()
        {
            var currentUserId = GetCurrentUserId();
            var currentTime = DateTime.UtcNow;

            // Debug logging - remove in production
            System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] ApplyAuditInformation called. CurrentUserId: {currentUserId ?? "NULL"}, CurrentTime: {currentTime}");

            var auditableEntries = ChangeTracker.Entries().Where(e => 
                e.Entity is IAuditableEntity || e.Entity is ISoftDeletable).ToList();
            
            System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Found {auditableEntries.Count} auditable entities");

            foreach (var entry in auditableEntries)
            {
                System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Processing entity: {entry.Entity.GetType().Name}, State: {entry.State}");
                
                switch (entry.State)
                {
                    case EntityState.Added:
                        // Handle auditable entities on creation
                        if (entry.Entity is IAuditableEntity auditableEntity)
                        {
                            System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Setting creation audit for {entry.Entity.GetType().Name}");
                            auditableEntity.CreatedAt = currentTime;
                            if (string.IsNullOrEmpty(auditableEntity.CreatedBy))
                                auditableEntity.CreatedBy = currentUserId;
                            auditableEntity.LastModifiedAt = null; // Clear on creation
                            auditableEntity.LastModifiedBy = null; // Clear on creation
                            System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Set CreatedAt: {auditableEntity.CreatedAt}, CreatedBy: {auditableEntity.CreatedBy ?? "NULL"}");
                        }
                        break;

                    case EntityState.Modified:
                        // Handle auditable entities on update
                        if (entry.Entity is IAuditableEntity modifiedAuditableEntity)
                        {
                            System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Setting modification audit for {entry.Entity.GetType().Name}");
                            
                            // Preserve original creation values
                            entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                            entry.Property(nameof(IAuditableEntity.CreatedBy)).IsModified = false;
                            
                            // Update modification values
                            modifiedAuditableEntity.LastModifiedAt = currentTime;
                            modifiedAuditableEntity.LastModifiedBy = currentUserId;
                            System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Set LastModifiedAt: {modifiedAuditableEntity.LastModifiedAt}, LastModifiedBy: {modifiedAuditableEntity.LastModifiedBy ?? "NULL"}");
                        }

                        // Handle soft deletable entities
                        if (entry.Entity is ISoftDeletable softDeletableEntity)
                        {
                            // Check if IsDeleted property was changed to true
                            var isDeletedProperty = entry.Property(nameof(ISoftDeletable.IsDeleted));
                            if (isDeletedProperty.IsModified && 
                                isDeletedProperty.CurrentValue is true && 
                                isDeletedProperty.OriginalValue is false)
                            {
                                // This is a soft delete operation
                                System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Setting soft delete audit for {entry.Entity.GetType().Name}");
                                softDeletableEntity.DeletedAt = currentTime;
                                softDeletableEntity.DeletedBy = currentUserId;
                                System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Set DeletedAt: {softDeletableEntity.DeletedAt}, DeletedBy: {softDeletableEntity.DeletedBy ?? "NULL"}");
                            }
                            else if (isDeletedProperty.IsModified && 
                                     isDeletedProperty.CurrentValue is false && 
                                     isDeletedProperty.OriginalValue is true)
                            {
                                // This is a soft undelete operation
                                System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Setting soft undelete audit for {entry.Entity.GetType().Name}");
                                softDeletableEntity.DeletedAt = null;
                                softDeletableEntity.DeletedBy = null;
                            }
                        }
                        break;

                    case EntityState.Deleted:
                        // Handle entities being hard deleted
                        if (entry.Entity is ISoftDeletable hardDeletedEntity)
                        {
                            System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Converting hard delete to soft delete for {entry.Entity.GetType().Name}");
                            // Convert hard delete to soft delete
                            entry.State = EntityState.Modified;
                            hardDeletedEntity.IsDeleted = true;
                            hardDeletedEntity.DeletedAt = currentTime;
                            hardDeletedEntity.DeletedBy = currentUserId;
                            System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Set IsDeleted: true, DeletedAt: {hardDeletedEntity.DeletedAt}, DeletedBy: {hardDeletedEntity.DeletedBy ?? "NULL"}");
                        }
                        break;
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] ApplyAuditInformation completed");
        }

        /// <summary>
        /// Gets the current user ID from the HTTP context
        /// </summary>
        /// <returns>Current user ID or null if not authenticated</returns>
        private string? GetCurrentUserId()
        {
            try
            {
                var httpContext = _httpContextAccessor?.HttpContext;
                
                // Debug logging
                System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] GetCurrentUserId - HttpContext available: {httpContext != null}");
                
                if (httpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] User is authenticated: {httpContext.User.Identity.Name ?? "Unknown"}");
                    
                    // Log all claims for debugging
                    foreach (var claim in httpContext.User.Claims)
                    {
                        System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Claim - Type: {claim.Type}, Value: {claim.Value}");
                    }
                    
                    // Try to get user ID from claims (standard Identity claim)
                    var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Found NameIdentifier claim: {userIdClaim.Value}");
                        return userIdClaim.Value;
                    }

                    // Fallback to other possible claim types
                    var subClaim = httpContext.User.FindFirst("sub");
                    if (subClaim != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Found sub claim: {subClaim.Value}");
                        return subClaim.Value;
                    }

                    var idClaim = httpContext.User.FindFirst("id");
                    if (idClaim != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Found id claim: {idClaim.Value}");
                        return idClaim.Value;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] No suitable user ID claim found");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] User not authenticated or HttpContext not available");
                }

                // Return null for unauthenticated users or system operations
                return null;
            }
            catch (Exception ex)
            {
                // Handle any exceptions during user ID extraction
                // This can happen during data seeding or system operations
                System.Diagnostics.Debug.WriteLine($"[AUDIT DEBUG] Exception in GetCurrentUserId: {ex.Message}");
                return null;
            }
        }
    }
} 