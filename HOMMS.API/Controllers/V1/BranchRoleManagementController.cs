using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BranchRoleManagementController : ControllerBase
    {
        private readonly IBranchRoleManagementService _service;

        public BranchRoleManagementController(IBranchRoleManagementService service)
        {
            _service = service;
        }
        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetAllOrSearch(int branchId, [FromQuery] string? keyword)
        {
            var result = await _service.GetByBranchAsync(branchId, keyword);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _service.GetByIdAsync(id);
            return role == null ? NotFound() : Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BranchRoleCreateUpdateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BranchRoleCreateUpdateDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
