using HOMMS.Domain.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IBranchService
    {
        Task<List<BranchDto>> GetBranchesAsync(string userId);
        Task<List<BranchDto>> GetActiveBranchesAsync();
        Task<BranchDto> GetDefaultBranchAsync(string userId);
        Task<BranchDto> GetByIdAsync(int branchId);
        Task SetUserDefaultBranchAsync(string userId, int branchId);
        int GetCurrentBranchId();
        void SetCurrentBranchId(int branchId);
        // Add other branch-related service methods as needed
    }
} 