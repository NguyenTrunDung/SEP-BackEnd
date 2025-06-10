using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            await SeedAdminUserAsync(userManager);
            
            // Seed Hospital Staff Users (Doctors, Nurses, etc.)
            await SeedHospitalStaffAsync(userManager);
            
            // Seed Branch User Relationships
            await SeedBranchUserRelationshipsAsync(userManager, branchRoleRepo, branchUserRoleRepo, dbContext, branchId);
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            // Define all roles including medical staff roles
            var roles = new Dictionary<string, string>
            {
                { "Admin", "System administrator with full access" },
                { "Manager", "Hospital manager with administrative access" },
                { "User", "Regular user account" },
                { "Doctor", "Medical doctor with patient care responsibilities" },
                { "Nurse", "Nursing staff with patient care responsibilities" },
                { "Nutritionist", "Dietary specialist managing nutritional care" }
            };

            foreach (var roleInfo in roles)
            {
                var roleName = roleInfo.Key;
                var description = roleInfo.Value;
                
                // Check if role exists
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    // Create role with description
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        Description = description,
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

        private static async Task SeedHospitalStaffAsync(UserManager<ApplicationUser> userManager)
        {
            // Define canteen staff accounts with branch roles
            var canteenStaffAccounts = new[]
            {
                // Branch management and operations
                new { Email = "branch.manager@homms.com", FirstName = "Nguyễn", LastName = "Chi Nhánh", Role = "Manager", Password = "BranchManager@123", BranchRole = "Quản lý chi nhánh" },
                new { Email = "manager@homms.com", FirstName = "Lê", LastName = "Quản Lý", Role = "Manager", Password = "Manager@123", BranchRole = "Quản lý" },
                
                // Cashier and front operations
                new { Email = "cashier@homms.com", FirstName = "Trần", LastName = "Thu Ngân", Role = "User", Password = "Cashier@123", BranchRole = "Thu Ngân" },
                new { Email = "cashier2@homms.com", FirstName = "Phạm", LastName = "Thanh Toán", Role = "User", Password = "Cashier@123", BranchRole = "Thu Ngân" },
                
                // General staff
                new { Email = "staff@homms.com", FirstName = "Võ", LastName = "Nhân Viên", Role = "User", Password = "Staff@123", BranchRole = "Nhân viên" },
                new { Email = "staff2@homms.com", FirstName = "Đặng", LastName = "Hỗ Trợ", Role = "User", Password = "Staff@123", BranchRole = "Nhân viên" },
                
                // Kitchen operations
                new { Email = "kitchen@homms.com", FirstName = "Bùi", LastName = "Đầu Bếp", Role = "User", Password = "Kitchen@123", BranchRole = "Nhà bếp" },
                new { Email = "kitchen2@homms.com", FirstName = "Lý", LastName = "Phụ Bếp", Role = "User", Password = "Kitchen@123", BranchRole = "Nhà bếp" },
                
                // Nursing staff for patient orders
                new { Email = "nurse@homms.com", FirstName = "Hoàng", LastName = "Y Tá", Role = "Nurse", Password = "Nurse@123", BranchRole = "Y tá" },
                new { Email = "nurse2@homms.com", FirstName = "Cao", LastName = "Điều Dưỡng", Role = "Nurse", Password = "Nurse@123", BranchRole = "Y tá" }
            };

            foreach (var staffInfo in canteenStaffAccounts)
            {
                var existingUser = await userManager.FindByEmailAsync(staffInfo.Email);
                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = staffInfo.Email,
                        Email = staffInfo.Email,
                        FirstName = staffInfo.FirstName,
                        LastName = staffInfo.LastName,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    var result = await userManager.CreateAsync(user, staffInfo.Password);
                    if (result.Succeeded)
                    {
                        // Assign appropriate identity role
                        await userManager.AddToRoleAsync(user, staffInfo.Role);
                        
                        // Note: Branch role assignment will be handled separately after branch roles are seeded
                    }
                }
            }
        }

        private static async Task SeedBranchUserRelationshipsAsync(
            UserManager<ApplicationUser> userManager,
            IRepository<BranchRole, int> branchRoleRepo,
            IRepository<BranchUserRole, int> branchUserRoleRepo,
            ApplicationDbContext dbContext,
            int branchId)
        {
            // Define user-branch role mappings
            var userBranchRoleMappings = new Dictionary<string, string>
            {
                { "branch.manager@homms.com", "Quản lý chi nhánh" },
                { "manager@homms.com", "Quản lý" },
                { "cashier@homms.com", "Thu Ngân" },
                { "cashier2@homms.com", "Thu Ngân" },
                { "staff@homms.com", "Nhân viên" },
                { "staff2@homms.com", "Nhân viên" },
                { "kitchen@homms.com", "Nhà bếp" },
                { "kitchen2@homms.com", "Nhà bếp" },
                { "nurse@homms.com", "Y tá" },
                { "nurse2@homms.com", "Y tá" }
            };

            foreach (var mapping in userBranchRoleMappings)
            {
                var userEmail = mapping.Key;
                var branchRoleName = mapping.Value;

                // Find user
                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null) continue;

                // Find branch role
                var branchRole = (await branchRoleRepo.GetByAsync(br => br.Name == branchRoleName && br.BranchId == branchId)).FirstOrDefault();
                if (branchRole == null) continue;

                // Check if BranchUser relationship exists
                var existingBranchUser = await dbContext.BranchUsers
                    .FirstOrDefaultAsync(bu => bu.UserId == user.Id && bu.BranchId == branchId);

                if (existingBranchUser == null)
                {
                    // Create BranchUser relationship
                    var branchUser = new BranchUser
                    {
                        BranchId = branchId,
                        UserId = user.Id,
                        IsDefault = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await dbContext.BranchUsers.AddAsync(branchUser);
                }

                // Check if BranchUserRole relationship exists
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
                        CreatedAt = DateTime.UtcNow
                    };
                    await branchUserRoleRepo.AddAsync(branchUserRole);
                }
            }

            // Save all changes
            await dbContext.SaveChangesAsync();
        }
    }
} 