using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IBranchUserManagementService
    {
        Task<IdentityResult> CreateUserAsync(CreateBranchUserRequest request);
        Task<List<UserDto>> GetAllUsersAsync(int branchId);
        Task<List<UserDto>> SearchUsersAsync(string keyword, int branchId);
        Task<List<UserDto>> GetUsersByRoleAsync(int branchId, int branchRoleId);
        Task<ApplicationUser?> GetUserByIdAsync(string userId, int branchId);
        Task<bool> UpdateUserAsync(UpdateUserRequest request);
        Task<bool> DeleteUserAsync(string userId, int branchId);
    }
}
