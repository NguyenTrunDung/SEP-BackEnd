using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IBranchRoleManagementService
    {
        Task<IEnumerable<BranchRoleDto>> GetByBranchAsync(int? branchId, string? keyword = null);
        Task<BranchRoleDto?> GetByIdAsync(int id);
        Task<BranchRoleDto> CreateAsync(BranchRoleCreateUpdateDto role);
        Task<BranchRoleDto?> UpdateAsync(int id, BranchRoleCreateUpdateDto updated);
        Task<bool> DeleteAsync(int id);
    }
}
