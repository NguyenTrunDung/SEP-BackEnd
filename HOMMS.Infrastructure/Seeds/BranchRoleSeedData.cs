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
                // Dashboard & Overview
                "overview:view",
                
                // Foods Management
                "foods:view",
                "foods:add",
                "foods:edit",
                "foods:delete",
                
                // Food Categories Management
                "foodcategories:view",
                "foodcategories:add", 
                "foodcategories:edit",
                "foodcategories:delete",
                
                // Orders Management
                "orders:view",
                "orders:add",
                "orders:edit",
                "orders:delete",
                "orders:approve",
                "orders:cancel",
                
                // Menu Management
                "menus:view",
                "menus:add",
                "menus:edit",
                "menus:delete",
                "menus:publish",
                
                // Kitchen Operations
                "kitchen:view",
                "kitchen:status",
                "kitchen:prepare",
                "kitchen:complete",
                
                // Delivery Management
                "delivery:view",
                "delivery:assign",
                "delivery:status",
                "delivery:complete",
                
                // User Management
                "users:view",
                "users:add",
                "users:edit",
                "users:delete",
                "users:roles",
                
                // Patient Management
                "patients:view",
                "patients:add",
                "patients:edit",
                "patients:delete",
                "patients:dietary",
                
                // Branch Management
                "branches:view",
                "branches:add",
                "branches:edit",
                "branches:delete",
                "branches:settings",
                
                // Reports & Analytics
                "reports:view",
                "reports:revenue",
                "reports:orders",
                "reports:patients",
                "reports:export",
                
                // Wallet & Financial
                "wallet:view",
                "wallet:transactions",
                "wallet:topup",
                "wallet:refund",
                
                // System Administration
                "system:settings",
                "system:backup",
                "system:logs",
                "system:maintenance",
                
                // Areas & Locations
                "areas:view",
                "areas:add",
                "areas:edit",
                "areas:delete",
                "locations:view",
                "locations:add",
                "locations:edit",
                "locations:delete"
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

            // Seed predefined roles with appropriate permissions
            var predefinedRoles = new List<BranchRole>
            {
                new BranchRole
                {
                    Name = "Quản lý",
                    BranchId = branchId,
                    IsDefault = true,
                    Permissions = string.Join(",", new[]
                    {
                        "overview:view",
                        "foods:view", "foods:add", "foods:edit", "foods:delete",
                        "foodcategories:view", "foodcategories:add", "foodcategories:edit", "foodcategories:delete",
                        "orders:view", "orders:add", "orders:edit", "orders:delete", "orders:approve",
                        "menus:view", "menus:add", "menus:edit", "menus:delete", "menus:publish",
                        "kitchen:view", "kitchen:status",
                        "delivery:view", "delivery:assign", "delivery:status",
                        "users:view", "users:add", "users:edit", "users:roles",
                        "patients:view", "patients:add", "patients:edit",
                        "reports:view", "reports:revenue", "reports:orders",
                        "wallet:view", "wallet:transactions"
                    }),
                    CreatedAt = DateTime.UtcNow
                },
                new BranchRole
                {
                    Name = "Nhân viên",
                    BranchId = branchId,
                    IsDefault = false,
                    Permissions = string.Join(",", new[]
                    {
                        "overview:view",
                        "foods:view", "foods:add", "foods:edit",
                        "foodcategories:view",
                        "orders:view", "orders:add", "orders:edit",
                        "menus:view", "menus:add", "menus:edit",
                        "kitchen:view", "kitchen:status",
                        "patients:view", "patients:add", "patients:edit",
                        "reports:view"
                    }),
                    CreatedAt = DateTime.UtcNow
                },
                new BranchRole
                {
                    Name = "Nhà bếp",
                    BranchId = branchId,
                    IsDefault = false,
                    Permissions = string.Join(",", new[]
                    {
                        "overview:view",
                        "orders:view",
                        "kitchen:view", "kitchen:status", "kitchen:prepare", "kitchen:complete",
                        "foods:view",
                        "menus:view"
                    }),
                    CreatedAt = DateTime.UtcNow
                },
                new BranchRole
                {
                    Name = "Giao hàng",
                    BranchId = branchId,
                    IsDefault = false,
                    Permissions = string.Join(",", new[]
                    {
                        "overview:view",
                        "orders:view",
                        "delivery:view", "delivery:status", "delivery:complete",
                        "patients:view"
                    }),
                    CreatedAt = DateTime.UtcNow
                },
                new BranchRole
                {
                    Name = "Y tá",
                    BranchId = branchId,
                    IsDefault = false,
                    Permissions = string.Join(",", new[]
                    {
                        "overview:view",
                        "orders:view", "orders:add",
                        "patients:view", "patients:add", "patients:edit", "patients:dietary",
                        "foods:view",
                        "menus:view"
                    }),
                    CreatedAt = DateTime.UtcNow
                },
                new BranchRole
                {
                    Name = "Kế toán",
                    BranchId = branchId,
                    IsDefault = false,
                    Permissions = string.Join(",", new[]
                    {
                        "overview:view",
                        "orders:view",
                        "reports:view", "reports:revenue", "reports:orders", "reports:export",
                        "wallet:view", "wallet:transactions", "wallet:topup", "wallet:refund"
                    }),
                    CreatedAt = DateTime.UtcNow
                }
            };
            foreach (var role in predefinedRoles)
            {
                var exists = (await branchRoleRepo.GetByAsync(r => r.Name == role.Name && r.BranchId == branchId)).Any();
                if (!exists)
                    await branchRoleRepo.AddAsync(role);
            }
        }
    }
} 