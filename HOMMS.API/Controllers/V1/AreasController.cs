using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreasController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet("area")]
        public async Task<ActionResult<ApiResponseBase<List<AreaDto>>>> GetAllAreasAsync()
        {
            var areas = await _areaService.GetAllAreasAsync();
            var areaList = areas.ToList() ?? new List<AreaDto>();
            var totalCount = areaList.Count;
            return Ok(new ApiResponseBase<List<AreaDto>>(areaList, "Areas retrieved successfully", "success", totalCount));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAreaAsync([FromBody] AreaDto areaDto)
        {
            if (areaDto == null || string.IsNullOrWhiteSpace(areaDto.Name))
            {
                return BadRequest(new ApiResponseBase<object>(null, "Invalid area data: Area Name is required", "error"));
            }

            var success = await _areaService.CreateAreaAsync(areaDto);
            return success ? Ok(new ApiResponseBase<object>(null, "Area created successfully")) : BadRequest(new ApiResponseBase<object>(null, "Failed to create area", "error"));
        }

        [HttpPut("area/{id}")]
        public async Task<IActionResult> UpdateAreaAsync(int id, [FromBody] string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                return BadRequest(new ApiResponseBase<object>(null, "Area name is required", "error"));
            }

            var success = await _areaService.UpdateAreaAsync(id, newName);
            return success ? Ok(new ApiResponseBase<object>(null, "Area updated successfully")) : NotFound(new ApiResponseBase<object>(null, "Area not found", "error"));
        }

        [HttpDelete("area/{id}")]
        public async Task<IActionResult> DeleteAreaAsync(int id)
        {
            var success = await _areaService.DeleteAreaAsync(id);
            return success ? Ok(new ApiResponseBase<object>(null, "Area deleted successfully")) : NotFound(new ApiResponseBase<object>(null, "Area not found", "error"));
        }
    }
}