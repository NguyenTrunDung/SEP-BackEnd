using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Seeds
{
    public static class BranchRoleSeedData
    {
        public static async Task SeedBranchRolesAsync(IServiceProvider serviceProvider, int branchId)
        {
            using var scope = serviceProvider.CreateScope();
            var branchRoleRepo = scope.ServiceProvider.GetRequiredService<IRepository<BranchRole, int>>();

            var allPermissions = new List<string>
            {
                "overview:views",
                "orders:add",
                "orders:views",
                "orders:edit",
                "orders:delete",
                "menu:views",
                "menu:add",
                "menu:edit",
                "menu:delete",
                "kitchen:views",
                "kitchen:status",
                "delivery:views",
                "delivery:status",
                "users:add",
                "users:edit",
                "users:delete",
                "users:views",
                "userGroups:add",
                "userGroups:edit",
                "userGroups:delete",
                "userGroups:views",
                // Add all other permissions in your system here
            };

            // Ensure only one Admin System BranchRole exists
            var adminSystemRoleExists = (await branchRoleRepo.GetByAsync(r => r.Name == "Admin System" && r.BranchId == branchId)).Any();
            if (!adminSystemRoleExists)
            {
                var adminSystemRole = new BranchRole
                {
                    Name = "Admin System",
                    BranchId = branchId,
                    IsDefault = false,
                    Permissions = string.Join(",", allPermissions),
                    CreatedAt = DateTime.UtcNow
                };
                await branchRoleRepo.AddAsync(adminSystemRole);
            }

            // Seed other roles as usual
            var otherRoles = new List<BranchRole>
            {
                new BranchRole
                {
                    Name = "Quản lý",
                    BranchId = branchId,
                    IsDefault = true,
                    Permissions = "overview:views,orders:add,orders:views,orders:edit,menu:views,menu:add,menu:edit",
                    CreatedAt = DateTime.UtcNow
                },
                new BranchRole
                {
                    Name = "Nhân viên",
                    BranchId = branchId,
                    IsDefault = false,
                    Permissions = "overview:views,orders:views,orders:add,orders:edit,menu:views,menu:edit,menu:add,kitchen:views",
                    CreatedAt = DateTime.UtcNow
                }
            };
            foreach (var role in otherRoles)
            {
                var exists = (await branchRoleRepo.GetByAsync(r => r.Name == role.Name && r.BranchId == branchId)).Any();
                if (!exists)
                    await branchRoleRepo.AddAsync(role);
            }
        }
    }
} 