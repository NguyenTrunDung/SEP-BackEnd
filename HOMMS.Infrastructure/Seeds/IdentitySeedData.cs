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
            await SeedAdminUserAsync(userManager);
            
            // Seed Hospital Staff Users (Doctors, Nurses, etc.)
            await SeedHospitalStaffAsync(userManager);
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
            // Define hospital staff accounts
            var staffAccounts = new[]
            {
                // Nurses
                new { Email = "nurse.tran@hospital.com", FirstName = "Trần", LastName = "Thị Hoa", Role = "Nurse", Password = "Nurse@123456" },
                new { Email = "nurse.duc@hospital.com", FirstName = "Nguyễn", LastName = "Minh Đức", Role = "Nurse", Password = "Nurse@123456" },
                new { Email = "nurse.thanh@hospital.com", FirstName = "Lê", LastName = "Văn Thành", Role = "Nurse", Password = "Nurse@123456" },
                new { Email = "nurse.mai@hospital.com", FirstName = "Phạm", LastName = "Thị Mai", Role = "Nurse", Password = "Nurse@123456" },
                
                // Doctors
                // new { Email = "doctor.khai@hospital.com", FirstName = "Lương", LastName = "Văn Khải", Role = "Doctor", Password = "Doctor@123456" },
                // new { Email = "doctor.linh@hospital.com", FirstName = "Nguyễn", LastName = "Thị Linh", Role = "Doctor", Password = "Doctor@123456" },
                // new { Email = "doctor.hung@hospital.com", FirstName = "Trần", LastName = "Văn Hùng", Role = "Doctor", Password = "Doctor@123456" },
                
                // Nutritionists
                // new { Email = "nutritionist.anh@hospital.com", FirstName = "Hoàng", LastName = "Thị Anh", Role = "Nutritionist", Password = "Nutritionist@123456" },
                // new { Email = "nutritionist.minh@hospital.com", FirstName = "Võ", LastName = "Minh Tâm", Role = "Nutritionist", Password = "Nutritionist@123456" }
            };

            foreach (var staffInfo in staffAccounts)
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
                        EmailConfirmed = true, // Auto-confirm email for staff
                        PhoneNumberConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    var result = await userManager.CreateAsync(user, staffInfo.Password);
                    if (result.Succeeded)
                    {
                        // Assign appropriate role
                        await userManager.AddToRoleAsync(user, staffInfo.Role);
                    }
                }
            }
        }
    }
} 