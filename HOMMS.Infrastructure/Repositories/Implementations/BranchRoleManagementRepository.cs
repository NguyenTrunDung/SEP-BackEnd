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
        public async Task<List<BranchRole>> GetByBranchIdAsync(int branchId)
        {
            return await _context.BranchRoles
                .Where(r => r.BranchId == branchId && !r.IsDeleted)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<BranchRole>> SearchAsync(int branchId, string keyword)
        {
            return await _context.BranchRoles
                .Where(r =>
                    r.BranchId == branchId &&
                    !r.IsDeleted &&
                    EF.Functions.Like(r.Name, $"%{keyword}%"))
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
