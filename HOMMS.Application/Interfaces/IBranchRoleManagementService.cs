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
        Task<IEnumerable<BranchRole>> GetByBranchAsync(int branchId, string? keyword = null);
        Task<BranchRole?> GetByIdAsync(int id);
        Task<BranchRole> CreateAsync(BranchRole role);
        Task<BranchRole?> UpdateAsync(int id, BranchRole updated);
        Task<bool> DeleteAsync(int id);
    }
}
