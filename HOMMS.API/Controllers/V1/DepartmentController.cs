using Asp.Versioning;
using HOMMS.Application.Implementations;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController (IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }


        
        [HttpGet]
        [Authorize(Policy = "Permission:Department:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<DepartmentDto>>>> GetDepartments([FromQuery] int branchId)
        {
            var dep = await _departmentService.GetDepartmentByBranchAsync(branchId);
            var totalCount = dep is ICollection<DepartmentDto> col ? col.Count : (dep?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<DepartmentDto>>(dep, "Departments retrieved successfully", "success", totalCount));
        }

        
        [HttpGet("active")]
        [Authorize(Policy = "Permission:Department:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<DepartmentDto>>>> GetActiveDepartments([FromQuery] int branchId)
        {
            var dep = await _departmentService.GetActiveDepartmentByBranchAsync(branchId);
            var totalCount = dep is ICollection<DepartmentDto> col ? col.Count : (dep?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<DepartmentDto>>(dep, "Active Departments retrieved successfully", "success", totalCount));
        }

         
         [HttpGet("{id}")]
        [Authorize(Policy = "Permission:Department:view")]
        public async Task<ActionResult<ApiResponseBase<DepartmentDto>>> GetDepartment(int id)
        {
            var dep = await _departmentService.GetByIdAsync(id);
            if (dep == null)
                return NotFound(new ApiResponseBase<DepartmentDto>(null, "Department not found", "error"));
            return Ok(new ApiResponseBase<DepartmentDto>(dep, "Department retrieved successfully"));
        }

       
 
        [HttpPost]
        [Authorize(Policy = "Permission:Department:add")]
        public async Task<ActionResult<ApiResponseBase<DepartmentDto>>> CreateDepartment([FromBody] CreateDepartmentDto dto)
        {
            try
            {
                var created = await _departmentService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetDepartment), new { id = created.Id }, new ApiResponseBase<DepartmentDto>(created, "Department created successfully"));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseBase<DepartmentDto>(null, ex.Message, "error"));
            }
        }

       
        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:Department:edit")]
        public async Task<ActionResult<ApiResponseBase<DepartmentDto>>> UpdateDepartment(int id, [FromBody] CreateDepartmentDto dto)
        {
            try
            {
                var updated = await _departmentService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new ApiResponseBase<DepartmentDto>(null, "Department not found", "error"));
                return Ok(new ApiResponseBase<DepartmentDto>(updated, "Department updated successfully"));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseBase<DepartmentDto>(null, ex.Message, "error"));
            }
        }

       
        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission:Department:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteDepartment(int id)
        {
            var deleted = await _departmentService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponseBase<object>(null, "Department not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "Department deleted successfully"));
        }

      

    }
}
