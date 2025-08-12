using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
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
        private BranchRoleDto MapToDto(BranchRole entity)
        {
            return new BranchRoleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                BranchId = entity.BranchId,
                IsDefault = entity.IsDefault,
                Permissions = string.IsNullOrEmpty(entity.Permissions)
                    ? new List<string>()
                    : entity.Permissions.Split(',').ToList()
            };
        }
        private BranchRole MapToEntity(BranchRoleCreateUpdateDto dto)
        {
            return new BranchRole
            {
                Name = dto.Name,
                BranchId = dto.BranchId,
                IsDefault = dto.IsDefault,
                Permissions = dto.Permissions != null
                    ? string.Join(",", dto.Permissions)
                    : string.Empty
            };
        }

        private void UpdateEntityFromDto(BranchRole entity, BranchRoleCreateUpdateDto dto)
        {
            entity.Name = dto.Name;
            entity.BranchId = dto.BranchId;
            entity.IsDefault = dto.IsDefault;
            entity.Permissions = dto.Permissions != null
                ? string.Join(",", dto.Permissions)
                : string.Empty;
        }

        public async Task<IEnumerable<BranchRoleDto>> GetByBranchAsync(int branchId, string? keyword = null)
        {
            var roles = await _repository.GetByAsync(r =>
                r.BranchId == branchId &&
                !r.IsDeleted &&
                r.Name != "Admin System" &&
                (string.IsNullOrEmpty(keyword) || r.Name.Contains(keyword)));

            return roles.Select(MapToDto);
        }

        public async Task<BranchRoleDto?> GetByIdAsync(int id)
        {
            var role = await _repository.GetByIdAsync(id);
            return role?.IsDeleted == true ? null : MapToDto(role);
        }

        public async Task<BranchRoleDto> CreateAsync(BranchRoleCreateUpdateDto dto)
        {
            var entity = MapToEntity(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;

            var created = await _repository.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task<BranchRoleDto?> UpdateAsync(int id, BranchRoleCreateUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null || existing.IsDeleted) return null;

            UpdateEntityFromDto(existing, dto);
            existing.LastModifiedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing);
            return MapToDto(updated);
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
