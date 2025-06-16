using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BranchUserRoleController : ControllerBase
    {
        private readonly IBranchUserRoleService _branchUserRoleService;

        public BranchUserRoleController(IBranchUserRoleService branchUserRoleService)
        {
            _branchUserRoleService = branchUserRoleService;
        }

        [HttpGet("by-role")]
        public async Task<ActionResult<List<UserByRoleDto>>> GetUsersByRole([FromQuery] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name is required.");
            }

            var users = await _branchUserRoleService.GetUsersByRoleNameAsync(roleName);

            if (users == null || !users.Any())
            {
                return NotFound($"No users found with role '{roleName}'.");
            }

            return Ok(users);
        }
    }
}
