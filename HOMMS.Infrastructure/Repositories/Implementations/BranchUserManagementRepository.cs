using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class BranchUserManagementRepository : Repository<ApplicationUser, string>, IBranchUserManagementRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public BranchUserManagementRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
            : base(context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            // Kiểm tra xem user có email này có đang active trong bất kỳ branch nào không
            // Nếu tất cả BranchUser đều bị IsDeleted = true thì user có thể tái sử dụng
            var activeUserInAnyBranch = await (from u in _context.Users
                                             join bu in _context.BranchUsers on u.Id equals bu.UserId
                                             where u.Email == email && u.IsActive && !bu.IsDeleted
                                             select u).FirstOrDefaultAsync();
            
            if (activeUserInAnyBranch != null)
            {
                // User đang active trong ít nhất 1 branch -> không thể tái sử dụng
                return activeUserInAnyBranch;
            }
            
            // Tìm user với email này (có thể đã bị soft delete ở tất cả branch)
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<IdentityResult> CreateUserWithPasswordAsync(ApplicationUser user, string password)
            => await _userManager.CreateAsync(user, password);

        public async Task AddUserToBranchAsync(string userId, int branchId)
        {
            // IMPORTANT: Sử dụng IgnoreQueryFilters() để tìm cả record bị soft delete
            var existingBranchUser = await _context.BranchUsers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(bu => bu.UserId == userId && bu.BranchId == branchId);

            // DEBUG: Log information để debug
            Console.WriteLine($"DEBUG AddUserToBranchAsync - UserId: {userId}, BranchId: {branchId}");
            Console.WriteLine($"DEBUG ExistingBranchUser found: {existingBranchUser != null}");
            if (existingBranchUser != null)
            {
                Console.WriteLine($"DEBUG ExistingBranchUser IsDeleted: {existingBranchUser.IsDeleted}");
            }

            if (existingBranchUser != null)
            {
                // Nếu đã tồn tại nhưng bị soft delete, thì "undelete" nó
                if (existingBranchUser.IsDeleted)
                {
                    Console.WriteLine("DEBUG Undeleting existing BranchUser");
                    existingBranchUser.IsDeleted = false;
                    existingBranchUser.DeletedAt = null;
                    existingBranchUser.DeletedBy = null;
                    existingBranchUser.LastModifiedAt = DateTime.UtcNow;
                }
                else
                {
                    Console.WriteLine("DEBUG BranchUser already exists and not deleted");
                }
            }
            else
            {
                // Tạo mới nếu chưa có record nào
                Console.WriteLine("DEBUG Creating new BranchUser");
                _context.BranchUsers.Add(new BranchUser
                {
                    UserId = userId,
                    BranchId = branchId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }
            
            await _context.SaveChangesAsync();
            Console.WriteLine("DEBUG SaveChanges completed");
        }

        public async Task AddUserToBranchRoleAsync(string userId, int branchId, int branchRoleId)
        {
            // IMPORTANT: Sử dụng IgnoreQueryFilters() để tìm cả record bị soft delete
            var existingBranchUserRole = await _context.BranchUserRoles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(bur => bur.UserId == userId && bur.BranchId == branchId);

            if (existingBranchUserRole != null)
            {
                // Cập nhật role mới và undelete nếu cần
                existingBranchUserRole.BranchRoleId = branchRoleId;
                existingBranchUserRole.LastModifiedAt = DateTime.UtcNow;
                
                if (existingBranchUserRole.IsDeleted)
                {
                    existingBranchUserRole.IsDeleted = false;
                    existingBranchUserRole.DeletedAt = null;
                    existingBranchUserRole.DeletedBy = null;
                }
            }
            else
            {
                // Tạo mới nếu chưa có record nào
                _context.BranchUserRoles.Add(new BranchUserRole
                {
                    UserId = userId,
                    BranchId = branchId,
                    BranchRoleId = branchRoleId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationUser?> GetByIdInBranchAsync(string userId, int branchId)
        {
            var exists = await _context.BranchUsers
                .AnyAsync(x => x.UserId == userId && x.BranchId == branchId && !x.IsDeleted);
            return exists ? await _context.Users.FindAsync(userId) : null;
        }

        public async Task<List<UserDto>> GetAllInBranchAsync(int branchId)
        {
            return await (from u in _context.Users
                          join bu in _context.BranchUsers on u.Id equals bu.UserId
                          join bur in _context.BranchUserRoles
                            on new { UserId = u.Id, BranchId = (int?)bu.BranchId }
                            equals new {bur.UserId, bur.BranchId }
                          join br in _context.BranchRoles on bur.BranchRoleId equals br.Id
                          where !bu.IsDeleted && bu.BranchId == branchId && br.Name != "Admin System"
                          orderby bu.CreatedAt descending
                          select new UserDto
                          {
                              UserId = u.Id,
                              FullName = u.FirstName + " " + u.LastName,
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              Email = u.Email,
                              PhoneNumber = u.PhoneNumber,
                              UserName = u.UserName,
                              BranchId = bu.BranchId,
                              BranchRoleId = bur.BranchRoleId,
                              DepartmentId = u.DepartmentId,
                              BranchRoleName = br.Name,
                              IsActive = u.IsActive
                          }).ToListAsync();
        }

        public async Task<List<UserDto>> SearchInBranchAsync(string keyword, int branchId)
        {
            return await (from u in _context.Users
                          join bu in _context.BranchUsers on u.Id equals bu.UserId
                          join bur in _context.BranchUserRoles
                            on new { UserId = u.Id, BranchId = (int?)bu.BranchId }
                            equals new { bur.UserId, bur.BranchId }
                          where !bu.IsDeleted && bu.BranchId == branchId &&
                                (u.FirstName + " " + u.LastName).Contains(keyword)
                          select new UserDto
                          {
                              UserId = u.Id,
                              FullName = u.FirstName + " " + u.LastName,
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              Email = u.Email,
                              PhoneNumber = u.PhoneNumber,
                              UserName = u.UserName,
                              BranchId = bu.BranchId,
                              BranchRoleId = bur.BranchRoleId,
                              IsActive = u.IsActive
                          }).ToListAsync();
        }

        public async Task<List<UserDto>> GetByRoleInBranchAsync(int branchId, int branchRoleId)
        {
            return await (from u in _context.Users
                          join bu in _context.BranchUsers on u.Id equals bu.UserId
                          join bur in _context.BranchUserRoles
                            on new { UserId = u.Id, BranchId = (int?)bu.BranchId }
                            equals new { bur.UserId, bur.BranchId }
                          where !bu.IsDeleted && bu.BranchId == branchId && bur.BranchRoleId == branchRoleId
                          select new UserDto
                          {
                              UserId = u.Id,
                              FullName = u.FirstName + " " + u.LastName,
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              Email = u.Email,
                              PhoneNumber = u.PhoneNumber,
                              UserName = u.UserName,
                              BranchId = bu.BranchId,
                              BranchRoleId = bur.BranchRoleId,
                              IsActive = u.IsActive
                          }).ToListAsync();
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateUserBranchRoleAsync(string userId, int branchId, int newBranchRoleId)
        {
            var record = await _context.BranchUserRoles
        .FirstOrDefaultAsync(x => x.UserId == userId && x.BranchId == branchId && !x.IsDeleted);

            if (record != null)
            {
                // Cập nhật nếu đã tồn tại
                record.BranchRoleId = newBranchRoleId;
                record.LastModifiedAt = DateTime.UtcNow;
            }
            else
            {
                // Tạo mới nếu chưa có
                record = new BranchUserRole
                {
                    UserId = userId,
                    BranchId = branchId,
                    BranchRoleId = newBranchRoleId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                await _context.BranchUserRoles.AddAsync(record);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(string userId, int branchId)
        {
            var record = await _context.BranchUsers
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BranchId == branchId && !x.IsDeleted);
            if (record == null) return false;

            // Soft delete BranchUser record
            record.IsDeleted = true;
            record.DeletedAt = DateTime.UtcNow;

            // Soft delete BranchUserRole record tương ứng
            var roleRecord = await _context.BranchUserRoles
                .FirstOrDefaultAsync(bur => bur.UserId == userId && bur.BranchId == branchId && !bur.IsDeleted);
            if (roleRecord != null)
            {
                roleRecord.IsDeleted = true;
                roleRecord.DeletedAt = DateTime.UtcNow;
            }

            // Kiểm tra xem user còn active ở branch nào khác không
            var hasOtherActiveBranches = await _context.BranchUsers
                .AnyAsync(bu => bu.UserId == userId && bu.BranchId != branchId && !bu.IsDeleted);

            // Nếu user không còn active ở branch nào khác, set IsActive = false
            if (!hasOtherActiveBranches)
            {
                var recordUser = await _context.Users.FindAsync(userId);
                if (recordUser != null)
                {
                    recordUser.IsActive = false;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }

}
