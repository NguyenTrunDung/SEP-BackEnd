using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using HOMMS.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWalletRepository _walletRepository;
        private readonly IBranchRoleManagementRepository _branchRoleRepository;
        private readonly ApplicationDbContext _context;
        
        public BranchUserManagementService(IBranchUserManagementRepository repository
            ,UserManager<ApplicationUser> userManager, IWalletRepository walletRepository,
            IBranchRoleManagementRepository branchRoleRepository, ApplicationDbContext context)
        {
            _repository = repository;
            _userManager = userManager;
            _walletRepository = walletRepository;
            _branchRoleRepository = branchRoleRepository;
            _context = context;
        }

        public async Task<IdentityResult> CreateUserAsync(CreateBranchUserRequest request)
        {
            var existingUser = await _repository.GetByEmailAsync(request.Email);
            ApplicationUser user;

            if (existingUser != null)
            {
                // Kiểm tra xem user có đang active trong branch hiện tại không
                var existingUserInCurrentBranch = await _context.BranchUsers
                    .FirstOrDefaultAsync(bu => bu.UserId == existingUser.Id && bu.BranchId == request.BranchId && !bu.IsDeleted);

                if (existingUserInCurrentBranch != null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User already exists in this branch." });
                }

                // Kiểm tra xem user có đang active trong bất kỳ branch nào khác không
                var hasActiveBranchElsewhere = await _context.BranchUsers
                    .AnyAsync(bu => bu.UserId == existingUser.Id && bu.BranchId != request.BranchId && !bu.IsDeleted);

                if (hasActiveBranchElsewhere)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Email already exists in another branch." });
                }

                // Tái sử dụng user đã bị soft delete ở tất cả branch
                user = existingUser;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.UserName = request.UserName;
                user.PhoneNumber = request.PhoneNumber;
                user.IsActive = true;
                user.EmailConfirmed = true;

                // Cập nhật password mới
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (!passwordResult.Succeeded)
                {
                    return passwordResult;
                }

                await _repository.UpdateUserAsync(user);
            }
            else
            {
                // Tạo user mới
                user = new ApplicationUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _repository.CreateUserWithPasswordAsync(user, request.Password);
                if (!result.Succeeded)
                    return result;
            }

            await _repository.AddUserToBranchAsync(user.Id, request.BranchId);
            await _repository.AddUserToBranchRoleAsync(user.Id, request.BranchId, request.BranchRoleId);
            await _userManager.AddToRoleAsync(user, "Staff");

            // Chỉ tạo ví cho những user có role là "Bác sĩ" hoặc "Y tá"
            var branchRole = await _branchRoleRepository.GetByIdAsync(request.BranchRoleId);
            if (branchRole != null && (branchRole.Name == "Bác sĩ" || branchRole.Name == "Y tá"))
            {
                await _walletRepository.CreateNewWallet(user.Id, request.CreatedBy);
            }
            
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
            user.IsActive = request.IsActive;
            user.PhoneNumber = request.PhoneNumber;
            await _repository.UpdateUserAsync(user);
            await _repository.UpdateUserBranchRoleAsync(request.UserId, request.BranchId, request.BranchRoleId);
            return true;
        }
        public async Task<bool> UpdateUserWalletAsync(UpdateUserRequest2 request)
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
            user.UserName = request.UserName;
            user.IsActive = request.IsActive;
            user.PhoneNumber = request.PhoneNumber;
            await _repository.UpdateUserAsync(user);
            return true;
        }
        public async Task<bool> DeleteUserAsync(string userId, int branchId)
        {
            return await _repository.SoftDeleteAsync(userId, branchId);
        }
    }
}
