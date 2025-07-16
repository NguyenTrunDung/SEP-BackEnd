using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BranchUserManagementController : ControllerBase
    {
        private readonly IBranchUserManagementService _service;

        public BranchUserManagementController(IBranchUserManagementService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateBranchUserRequest request)
        {
            var result = await _service.CreateUserAsync(request);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok("User created successfully");
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] int branchId, [FromQuery] string? keyword = null)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var result = await _service.SearchUsersAsync(keyword, branchId);
                return Ok(result);
            }

            var all = await _service.GetAllUsersAsync(branchId);
            return Ok(all);
        }


        [HttpGet("by-role")]
        public async Task<IActionResult> GetByRole([FromQuery] int branchId, [FromQuery] int branchRoleId)
        {
            var users = await _service.GetUsersByRoleAsync(branchId, branchRoleId);
            return Ok(users);
        }

        [HttpGet("{userId}/branch/{branchId}")]
        public async Task<IActionResult> GetById(string userId, int branchId)
        {
            var user = await _service.GetUserByIdAsync(userId, branchId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            var success = await _service.UpdateUserAsync(request);
            if (!success) return BadRequest("User not found or email already exists");
            return Ok("User updated successfully");
        }


        [HttpDelete("{userId}/branch/{branchId}")]
        public async Task<IActionResult> Delete(string userId, int branchId)
        {
            var success = await _service.DeleteUserAsync(userId, branchId);
            if (!success) return NotFound("User not found or already deleted");
            return Ok("User deleted successfully");
        }
    }
}
