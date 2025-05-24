using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;

namespace HOMMS.Infrastructure.Seeds
{
    public static class IdentitySeedData
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, int branchId)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var branchRoleRepo = scope.ServiceProvider.GetRequiredService<IRepository<BranchRole, int>>();
            var branchUserRoleRepo = scope.ServiceProvider.GetRequiredService<IRepository<BranchUserRole, int>>();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Admin User
            var adminUser = await SeedAdminUserAsync(userManager);

            // Assign Admin System branch role ONLY to the seeded admin user, and ensure only one exists
            if (adminUser != null)
            {
                var adminSystemRole = (await branchRoleRepo.GetByAsync(r => r.Name == "Admin System" && r.BranchId == branchId)).FirstOrDefault();
                if (adminSystemRole != null)
                {
                    // Remove any other BranchUserRole assignments for Admin System (if exist)
                    var allAdminSystemAssignments = await branchUserRoleRepo.GetByAsync(bur => bur.BranchRoleId == adminSystemRole.Id && bur.BranchId == branchId);
                    foreach (var assignment in allAdminSystemAssignments)
                    {
                        if (assignment.UserId != adminUser.Id)
                        {
                            await branchUserRoleRepo.DeleteAsync(assignment);
                        }
                    }
                    // Ensure only the seeded admin user has Admin System
                    var alreadyAssigned = (await branchUserRoleRepo.GetByAsync(bur => bur.UserId == adminUser.Id && bur.BranchRoleId == adminSystemRole.Id && bur.BranchId == branchId)).Any();
                    if (!alreadyAssigned)
                    {
                        await branchUserRoleRepo.AddAsync(new BranchUserRole
                        {
                            UserId = adminUser.Id,
                            BranchId = branchId,
                            BranchRoleId = adminSystemRole.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            // Define roles
            string[] roleNames = { "Admin", "Manager", "User" };

            foreach (var roleName in roleNames)
            {
                // Check if role exists
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    // Create role with description
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"Built-in {roleName} role",
                        CreatedAt = DateTime.UtcNow
                    };

                    await roleManager.CreateAsync(role);
                }
            }
        }

        private static async Task<ApplicationUser> SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            // Check if admin user exists
            var adminUser = await userManager.FindByEmailAsync("admin@homms.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@homms.com",
                    Email = "admin@homms.com",
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true, // Auto-confirm email for admin
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123456");
                if (result.Succeeded)
                {
                    // Assign admin role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            return adminUser;
        }
    }
} 