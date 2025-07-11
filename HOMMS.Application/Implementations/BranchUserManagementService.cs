using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class BranchUserManagementService: IBranchUserManagementService
    {
        private readonly IBranchUserManagementRepository _repository;

        public BranchUserManagementService(IBranchUserManagementRepository repository)
        {
            _repository = repository;
        }

        public async Task<IdentityResult> CreateUserAsync(CreateBranchUserRequest request)
        {
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email
            };

            var existingUser = await _repository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already exists." });
            }

            var result = await _repository.CreateUserWithPasswordAsync(user, request.Password);
            if (!result.Succeeded)
                return result;

            await _repository.AddUserToBranchAsync(user.Id, request.BranchId);
            await _repository.AddUserToBranchRoleAsync(user.Id, request.BranchId, request.BranchRoleId);

            return IdentityResult.Success;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(int branchId)
        {
            return await _repository.GetAllInBranchAsync(branchId);
        }

        public async Task<List<UserDto>> SearchUsersAsync(string keyword, int branchId)
        {
            return await _repository.SearchInBranchAsync(keyword, branchId);
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(int branchId, int branchRoleId)
        {
            return await _repository.GetByRoleInBranchAsync(branchId, branchRoleId);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId, int branchId)
        {
            return await _repository.GetByIdInBranchAsync(userId, branchId);
        }

        public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
        {
            var user = await _repository.GetByIdInBranchAsync(request.UserId, request.BranchId);
            if (user == null) return false;

            // Nếu đổi email, thì check xem email mới có trùng ai khác không
            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _repository.GetByEmailAsync(request.Email);
                if (emailExists != null && emailExists.Id != request.UserId)
                {
                    return false; 
                }

                user.Email = request.Email;
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await _repository.UpdateUserAsync(user);
            return true;

        }

        public async Task<bool> DeleteUserAsync(string userId, int branchId)
        {
            return await _repository.SoftDeleteAsync(userId, branchId);
        }
    }
}
