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
        
        // New methods for controller support
        Task<List<BranchDto>> GetBranchesForUserAsync(string userId, bool isSystemAdmin);
        Task<BranchDto> GetCurrentBranchAsync();
        Task<BranchDto> ValidateAndSetCurrentBranchAsync(int branchId, string userId, bool isSystemAdmin);
        
        // CRUD operations
        Task<BranchDto> CreateBranchAsync(CreateBranchDto createDto, string createdBy);
        Task<BranchDto> UpdateBranchAsync(int id, UpdateBranchDto updateDto, string updatedBy);
        Task<bool> DeleteBranchAsync(int id, string deletedBy);
        Task<bool> RestoreBranchAsync(int id, string restoredBy);
    }
} 