using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class BranchUserRoleRepository : Repository<BranchUserRole, int>, IBranchUserRoleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BranchUserRoleRepository(ApplicationDbContext dbContext) : base(dbContext) {
            _dbContext = dbContext;
        }
        public async Task<List<UserByRoleDto>> GetUsersByRoleNameAsync(string roleName)
        {
            var query = from bur in _dbContext.BranchUserRoles
                        join br in _dbContext.BranchRoles on bur.BranchRoleId equals br.Id
                        join bu in _dbContext.BranchUsers
                            on new { UserId = bur.UserId, BranchId = (int)bur.BranchId }
                            equals new { UserId = bu.UserId, BranchId = bu.BranchId }
                        join b in _dbContext.Branches on bu.BranchId equals b.Id
                        join u in _dbContext.Users on bur.UserId equals u.Id
                        where br.Name == roleName
                        select new UserByRoleDto
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            RoleName = br.Name,
                            BranchName = b.Name
                        };

            return await query.Distinct().ToListAsync();
        }


    }
}
