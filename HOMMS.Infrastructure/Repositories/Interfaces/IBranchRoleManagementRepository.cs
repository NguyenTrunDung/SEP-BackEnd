using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IBranchRoleManagementRepository: IRepository<BranchRole, int>
    {
        Task<List<BranchRole>> GetRolesByBranchIdAsync(int? branchId);
        Task<List<BranchRole>> SearchRolesByBranchIdAsync(int? branchId, string keyword);
        Task<BranchRole?> GetByIdAsync(int id);
        Task<BranchRole> CreateAsync(BranchRole entity);
        Task<BranchRole?> UpdateAsync(int id, BranchRole entity);
        Task<bool> SoftDeleteAsync(int id);
    }
}
