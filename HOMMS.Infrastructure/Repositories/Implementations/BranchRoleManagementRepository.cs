using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class BranchRoleManagementRepository : Repository<BranchRole, int>, IBranchRoleManagementRepository
    {
        private readonly ApplicationDbContext _context;

        public BranchRoleManagementRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
        public async Task<List<BranchRole>> GetRolesByBranchIdAsync(int? branchId)
        {
            // Get distinct roles that are associated with the specified branch through BranchUserRole
            //return await _context.BranchRoles
            //    .Where(r => !r.IsDeleted &&
            //               _context.BranchUserRoles.Any(bur => 
            //                   bur.BranchRoleId == r.Id && 
            //                   bur.BranchId == branchId && 
            //                   !bur.IsDeleted))
            //    .OrderBy(r => r.CreatedAt)
            //    .ToListAsync();
            return await _context.BranchRoles
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

        }

        public async Task<List<BranchRole>> SearchRolesByBranchIdAsync(int? branchId, string keyword)
        {
            // Get distinct roles that are associated with the specified branch through BranchUserRole and match keyword
            return await _context.BranchRoles
                .Where(r => !r.IsDeleted &&
                           EF.Functions.Like(r.Name, $"%{keyword}%") &&
                           _context.BranchUserRoles.Any(bur => 
                               bur.BranchRoleId == r.Id && 
                               bur.BranchId == branchId && 
                               !bur.IsDeleted))
                .ToListAsync();
        }


        public override async Task<BranchRole?> GetByIdAsync(int id)
        {
            var role = await _context.BranchRoles.FindAsync(id);
            return role != null && !role.IsDeleted ? role : null;
        }

        public async Task<BranchRole> CreateAsync(BranchRole entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;

            _context.BranchRoles.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<BranchRole?> UpdateAsync(int id, BranchRole updated)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = updated.Name;
            existing.Permissions = updated.Permissions;
            existing.IsDefault = updated.IsDefault;
            existing.LastModifiedAt = DateTime.UtcNow;
            existing.LastModifiedBy = updated.LastModifiedBy;

            _context.BranchRoles.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;

            _context.BranchRoles.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
