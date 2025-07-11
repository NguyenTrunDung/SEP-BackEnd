using HOMMS.Application.Interfaces;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class BranchRoleManagementService: IBranchRoleManagementService
    {
        private readonly IBranchRoleManagementRepository _repository;

        public BranchRoleManagementService(IBranchRoleManagementRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BranchRole>> GetByBranchAsync(int branchId, string? keyword = null)
        {
            return await _repository.GetByAsync(r =>
                r.BranchId == branchId &&
                !r.IsDeleted &&
                (string.IsNullOrEmpty(keyword) || r.Name.Contains(keyword)));
        }

        public async Task<BranchRole?> GetByIdAsync(int id)
        {
            var role = await _repository.GetByIdAsync(id);
            return role?.IsDeleted == true ? null : role;
        }

        public async Task<BranchRole> CreateAsync(BranchRole role)
        {
            role.CreatedAt = DateTime.UtcNow;
            role.IsDeleted = false;
            return await _repository.AddAsync(role);
        }

        public async Task<BranchRole?> UpdateAsync(int id, BranchRole updated)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null || existing.IsDeleted) return null;

            existing.Name = updated.Name;
            existing.Permissions = updated.Permissions;
            existing.IsDefault = updated.IsDefault;
            existing.LastModifiedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null || existing.IsDeleted) return false;

            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return true;
        }
    }
}
