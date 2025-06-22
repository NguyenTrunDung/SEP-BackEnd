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

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Admin User
            await SeedAdminUserAsync(userManager);
            
            // Seed Hospital Staff Users (Doctors, Nurses, etc.)
            await SeedHospitalStaffAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            // Define minimal Identity roles - only for high-level user categorization
            // Actual permissions are handled by BranchRole system
            var roles = new Dictionary<string, string>
            {
                { "SystemAdmin", "System administrator - has access to all branches and system settings" },
                { "Manager", "Branch Manager (Admin for each branch) - management their own branch" },
                { "Staff", "Hospital/canteen staff member - access determined by branch roles" },
                { "Patient", "Hospital patient - limited access for ordering food" },
                { "Customer", "Guest user - wanna be a Customer will register account" }
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
                    await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
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
                new { Email = "manager@homms.com", FirstName = "Lê", LastName = "Quản Lý", Role = "Staff", Password = "Manager@123", BranchRole = "Quản lý" },
                
                // Cashier and front operations
                new { Email = "cashier@homms.com", FirstName = "Trần", LastName = "Thu Ngân", Role = "Staff", Password = "Cashier@123", BranchRole = "Thu Ngân" },
                new { Email = "cashier2@homms.com", FirstName = "Phạm", LastName = "Thanh Toán", Role = "Staff", Password = "Cashier@123", BranchRole = "Thu Ngân" },
                
                // General staff
                new { Email = "staff@homms.com", FirstName = "Võ", LastName = "Nhân Viên", Role = "Staff", Password = "Staff@123", BranchRole = "Nhân viên" },
                new { Email = "staff2@homms.com", FirstName = "Đặng", LastName = "Hỗ Trợ", Role = "Staff", Password = "Staff@123", BranchRole = "Nhân viên" },
                
                // Kitchen operations
                new { Email = "kitchen@homms.com", FirstName = "Bùi", LastName = "Đầu Bếp", Role = "Staff", Password = "Kitchen@123", BranchRole = "Nhà bếp" },
                new { Email = "kitchen2@homms.com", FirstName = "Lý", LastName = "Phụ Bếp", Role = "Staff", Password = "Kitchen@123", BranchRole = "Nhà bếp" },
                
                // Nursing staff for patient orders
                new { Email = "nurse@homms.com", FirstName = "Hoàng", LastName = "Y Tá", Role = "Staff", Password = "Nurse@123", BranchRole = "Y tá" },
                new { Email = "nurse2@homms.com", FirstName = "Cao", LastName = "Điều Dưỡng", Role = "Staff", Password = "Nurse@123", BranchRole = "Y tá" }
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


    }
} 