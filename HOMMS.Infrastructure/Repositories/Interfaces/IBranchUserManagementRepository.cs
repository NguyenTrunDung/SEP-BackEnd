using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IBranchUserManagementRepository: IRepository<ApplicationUser, string>
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IdentityResult> CreateUserWithPasswordAsync(ApplicationUser user, string password);
        Task AddUserToBranchAsync(string userId, int branchId);
        Task AddUserToBranchRoleAsync(string userId, int branchId, int branchRoleId);
        Task<ApplicationUser?> GetByIdInBranchAsync(string userId, int branchId);
        Task<List<UserDto>> GetAllInBranchAsync(int branchId);
        Task<List<UserDto>> SearchInBranchAsync(string keyword, int branchId);
        Task<List<UserDto>> GetByRoleInBranchAsync(int branchId, int branchRoleId);
        Task UpdateUserAsync(ApplicationUser user);
        Task<bool> UpdateUserBranchRoleAsync(string userId, int branchId, int newBranchRoleId);

        Task<bool> SoftDeleteAsync(string userId, int branchId);
    }
}
