using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class BranchUserRoleService: IBranchUserRoleService
    {
        private readonly IBranchUserRoleRepository _branchUserRoleRepository;

        public BranchUserRoleService(IBranchUserRoleRepository branchUserRoleRepository)
        {
            _branchUserRoleRepository = branchUserRoleRepository;
        }

        public async Task<List<UserByRoleDto>> GetUsersByRoleNameAsync(string roleName)
        {
            return await _branchUserRoleRepository.GetUsersByRoleNameAsync(roleName);
        }
    }
}
