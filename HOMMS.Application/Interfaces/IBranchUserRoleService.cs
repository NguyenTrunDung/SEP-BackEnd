using HOMMS.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IBranchUserRoleService
    {
        Task<List<UserByRoleDto>> GetUsersByRoleNameAsync(string roleName);
    }
}
