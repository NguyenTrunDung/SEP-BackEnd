using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IBranchUserRoleRepository: IRepository<BranchUserRole, int>
    {
        Task<List<UserByRoleDto>> GetUsersByRoleNameAsync(string roleName);
    }
}
