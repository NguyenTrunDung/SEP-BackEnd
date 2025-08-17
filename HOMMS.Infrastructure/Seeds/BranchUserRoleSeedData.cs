using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Seeds
{
    public static class BranchUserRoleSeedData
    {
        public static async Task SeedBranchUserRolesAsync(IServiceProvider serviceProvider, int branchId)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var branchRoleRepo = scope.ServiceProvider.GetRequiredService<IRepository<BranchRole, int>>();
            var branchUserRoleRepo = scope.ServiceProvider.GetRequiredService<IRepository<BranchUserRole, int>>();

            // Define user-branch role mappings based on the seeded users and roles
            var userBranchRoleMappings = new Dictionary<string, List<string>>
            {
                // System Administrator - has Admin System role
                { "admin@homms.com", new List<string> { "Admin System" } },
                
                // Branch Management
                { "branch.manager@homms.com", new List<string> { "Quản lý chi nhánh" } },
                { "manager@homms.com", new List<string> { "Quản lý" } },
                
                // Cashier Operations
                { "cashier@homms.com", new List<string> { "Thu Ngân" } },
                { "cashier2@homms.com", new List<string> { "Thu Ngân" } },
                
                // General Staff
                { "staff@homms.com", new List<string> { "Nhân viên" } },
                { "staff2@homms.com", new List<string> { "Nhân viên" } },
                
                // Kitchen Operations
                { "kitchen@homms.com", new List<string> { "Nhà bếp" } },
                { "kitchen2@homms.com", new List<string> { "Nhà bếp" } },
                
                // Nursing Staff
                { "nurse@homms.com", new List<string> { "Y tá" } },
                { "nurse2@homms.com", new List<string> { "Y tá" } }
            };

            // Additional test scenarios - users with multiple roles
            var multiRoleUsers = new Dictionary<string, List<string>>
            {
                // Some users might have multiple roles for testing purposes
                // Branch manager can also act as regular manager
                { "branch.manager@homms.com", new List<string> { "Quản lý chi nhánh", "Quản lý" } },
                
                // Senior staff member with both staff and cashier capabilities
                { "staff@homms.com", new List<string> { "Nhân viên", "Thu Ngân" } },
                
                // Head nurse with both nursing and management capabilities
                { "nurse@homms.com", new List<string> { "Y tá", "Nhân viên" } }
            };

            // Process single role mappings
            await ProcessUserRoleMappings(userManager, branchRoleRepo, branchUserRoleRepo, dbContext, branchId, userBranchRoleMappings);
            
            // Process multi-role mappings (optional - for testing complex scenarios)
            // Uncomment the line below if you want users with multiple roles
            // await ProcessUserRoleMappings(userManager, branchRoleRepo, branchUserRoleRepo, dbContext, branchId, multiRoleUsers);

            await dbContext.SaveChangesAsync();
        }

        private static async Task ProcessUserRoleMappings(
            UserManager<ApplicationUser> userManager,
            IRepository<BranchRole, int> branchRoleRepo,
            IRepository<BranchUserRole, int> branchUserRoleRepo,
            ApplicationDbContext dbContext,
            int branchId,
            Dictionary<string, List<string>> userRoleMappings)
        {
            foreach (var mapping in userRoleMappings)
            {
                var userEmail = mapping.Key;
                var branchRoleNames = mapping.Value;

                // Find user
                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    Console.WriteLine($"Warning: User with email {userEmail} not found. Skipping BranchUserRole seeding for this user.");
                    continue;
                }

                // Ensure BranchUser relationship exists
                await EnsureBranchUserExists(dbContext, user.Id, branchId);

                // Process each branch role for this user
                foreach (var branchRoleName in branchRoleNames)
                {
                    // Find branch role (roles are now global, not branch-specific)
                    var branchRole = (await branchRoleRepo.GetByAsync(br => br.Name == branchRoleName)).FirstOrDefault();
                    if (branchRole == null)
                    {
                        Console.WriteLine($"Warning: Branch role '{branchRoleName}' not found. Skipping.");
                        continue;
                    }

                    // Check if BranchUserRole relationship already exists
                    var existingBranchUserRole = await dbContext.BranchUserRoles
                        .FirstOrDefaultAsync(bur => bur.UserId == user.Id && 
                                                   bur.BranchId == branchId && 
                                                   bur.BranchRoleId == branchRole.Id);

                    if (existingBranchUserRole == null)
                    {
                        // Create BranchUserRole relationship
                        var branchUserRole = new BranchUserRole
                        {
                            UserId = user.Id,
                            BranchId = branchId,
                            BranchRoleId = branchRole.Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        };

                        await branchUserRoleRepo.AddAsync(branchUserRole);
                        Console.WriteLine($"Created BranchUserRole: {userEmail} -> {branchRoleName}");
                    }
                    else
                    {
                        Console.WriteLine($"BranchUserRole already exists: {userEmail} -> {branchRoleName}");
                    }
                }
            }
        }

        private static async Task EnsureBranchUserExists(ApplicationDbContext dbContext, string userId, int branchId)
        {
            var existingBranchUser = await dbContext.BranchUsers
                .FirstOrDefaultAsync(bu => bu.UserId == userId && bu.BranchId == branchId);

            if (existingBranchUser == null)
            {
                // Create BranchUser relationship
                var branchUser = new BranchUser
                {
                    BranchId = branchId,
                    UserId = userId,
                    IsDefault = true, // First branch is default
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };

                await dbContext.BranchUsers.AddAsync(branchUser);
                Console.WriteLine($"Created BranchUser relationship for user {userId} and branch {branchId}");
            }
        }

        /// <summary>
        /// Seeds test BranchUserRole data for specific testing scenarios
        /// </summary>
        public static async Task SeedTestBranchUserRoleDataAsync(IServiceProvider serviceProvider, int branchId)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var branchRoleRepo = scope.ServiceProvider.GetRequiredService<IRepository<BranchRole, int>>();
            var branchUserRoleRepo = scope.ServiceProvider.GetRequiredService<IRepository<BranchUserRole, int>>();

            // Additional test scenarios for different user types
            var testScenarios = new Dictionary<string, List<string>>
            {
                // Test user with default manager role
                { "manager@homms.com", new List<string> { "Quản lý" } },
                
                // Test different combinations for comprehensive testing
                { "cashier@homms.com", new List<string> { "Thu Ngân" } },
                { "kitchen@homms.com", new List<string> { "Nhà bếp" } },
                { "nurse@homms.com", new List<string> { "Y tá" } }
            };

            await ProcessUserRoleMappings(userManager, branchRoleRepo, branchUserRoleRepo, dbContext, branchId, testScenarios);
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Cleans up BranchUserRole data for testing purposes
        /// </summary>
        public static async Task CleanupBranchUserRoleDataAsync(IServiceProvider serviceProvider, int branchId)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Remove all BranchUserRole relationships for the specified branch
            var branchUserRoles = await dbContext.BranchUserRoles
                .Where(bur => bur.BranchId == branchId)
                .ToListAsync();
                
            dbContext.BranchUserRoles.RemoveRange(branchUserRoles);
            await dbContext.SaveChangesAsync();
            
            Console.WriteLine($"Cleaned up {branchUserRoles.Count} BranchUserRole records for branch {branchId}");
        }
    }
} 