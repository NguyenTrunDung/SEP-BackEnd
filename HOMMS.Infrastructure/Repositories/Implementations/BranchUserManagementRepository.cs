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
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<IdentityResult> CreateUserWithPasswordAsync(ApplicationUser user, string password)
            => await _userManager.CreateAsync(user, password);

        public async Task AddUserToBranchAsync(string userId, int branchId)
        {
            _context.BranchUsers.Add(new BranchUser
            {
                UserId = userId,
                BranchId = branchId,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task AddUserToBranchRoleAsync(string userId, int branchId, int branchRoleId)
        {
            _context.BranchUserRoles.Add(new BranchUserRole
            {
                UserId = userId,
                BranchId = branchId,
                BranchRoleId = branchRoleId,
                CreatedAt = DateTime.UtcNow
            });
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
                          where !bu.IsDeleted && bu.BranchId == branchId
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

            record.IsDeleted = true;
            record.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
