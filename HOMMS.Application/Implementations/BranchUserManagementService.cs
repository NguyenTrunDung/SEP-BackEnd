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
        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly IWalletRepository _walletRepository;
        private readonly IBranchRoleManagementRepository _branchRoleRepository;
        private readonly ApplicationDbContext _context;
        
        public BranchUserManagementService(IBranchUserManagementRepository repository
            ,UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IWalletRepository walletRepository,
            IBranchRoleManagementRepository branchRoleRepository, ApplicationDbContext context)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
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
                user.DepartmentId = request.DepartmentId;

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
                    EmailConfirmed = true,
                    DepartmentId = request.DepartmentId,
                };

                var result = await _repository.CreateUserWithPasswordAsync(user, request.Password);
                if (!result.Succeeded)
                    return result;
            }

            await _repository.AddUserToBranchAsync(user.Id, request.BranchId);
            await _repository.AddUserToBranchRoleAsync(user.Id, request.BranchId, request.BranchRoleId);

            // Lấy thông tin branch role để kiểm tra permissions
            var branchRole = await _branchRoleRepository.GetByIdAsync(request.BranchRoleId);
            //var userIdentityRoles = await _roleManager.FindByIdAsync(user.Id);
            //Console.WriteLine($"User Name: {user.UserName} , User Id: {user.Id} RoleName: {userIdentityRoles.Name}");
            // Fix for CS8602: Dereference of a possibly null reference.
            //var userIdentityRoles = await _roleManager.GetRoleNameAsync("Guest");
            //if (userIdentityRoles != null)
            //    // Replace the following line:
            //    var userIdentityRoles = await _roleManager.GetRoleNameAsync("Guest");

            // With this corrected line:
            var guestRole = await _roleManager.FindByNameAsync("Guest");
            if (guestRole != null)
            {
                var userIdentityRoles = await _roleManager.GetRoleNameAsync(guestRole);
                Console.WriteLine($"User Name: {user.UserName} , User Id: {user.Id} RoleName: {userIdentityRoles}");
            }
            else
            {
                Console.WriteLine($"User Name: {user.UserName} , User Id: {user.Id} RoleName: <Role not found>");
            }
        
            // Kiểm tra field permission và assign role tương ứng
            if (branchRole != null)
            {
                if (string.IsNullOrEmpty(branchRole.Permissions) || string.IsNullOrWhiteSpace(branchRole.Permissions))
                {
                    // Nếu permission rỗng thì assign role "Guest"
                    await _userManager.AddToRoleAsync(user, "Guest");
                }
                else
                {
                    // Nếu có permission thì assign role "Staff"
                    await _userManager.AddToRoleAsync(user, "Staff");
                }
            }
            else
            {
                // Fallback nếu không tìm thấy branchRole
                await _userManager.AddToRoleAsync(user, "Guest");
            }

            // Chỉ tạo ví cho những user có role là "Bác sĩ" hoặc "Y tá" và nếu là "Guest"
            //if (branchRole != null || guestRole != null && (branchRole.Name == "Bác sĩ" || branchRole.Name == "Điều dưỡng trưởng" || guestRole.Name == "Guest"))
            //{
            //    // Ensure branchRole and guestRole are not null before accessing their properties
            //    if ((branchRole != null && (branchRole.Name == "Bác sĩ" || branchRole.Name == "Điều dưỡng trưởng")) ||
            //        (guestRole != null && guestRole.Name == "Guest"))
            //    {
            //        await _walletRepository.CreateNewWallet(user.Id, request.CreatedBy);
            //    }
            //    await _walletRepository.CreateNewWallet(user.Id, request.CreatedBy);
            //}

            // Ensure branchRole and guestRole are not null before accessing their properties
            if ((branchRole != null && (branchRole.Name == "Bác sĩ" || branchRole.Name == "Điều dưỡng trưởng")) ||
                (guestRole != null && guestRole.Name == "Guest"))
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

            // Add departmentId update
            if (request.DepartmentId.HasValue)
            {
                user.DepartmentId = request.DepartmentId.Value;
            }

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
