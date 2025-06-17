using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HOMMS.Common.Helpers;
using Asp.Versioning;
using System.Linq;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AreasController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreasController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        /// <summary>
        /// Gets all areas for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>List of areas</returns>
        [HttpGet]
        [Authorize(Policy = "Permission:areas:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<AreaDto>>>> GetAreas([FromQuery] int branchId)
        {
            var areas = await _areaService.GetAreasByBranchAsync(branchId);
            var totalCount = areas is ICollection<AreaDto> col ? col.Count : (areas?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<AreaDto>>(areas, "Areas retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets active areas for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>List of active areas</returns>
        [HttpGet("active")]
        [Authorize(Policy = "Permission:areas:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<AreaDto>>>> GetActiveAreas([FromQuery] int branchId)
        {
            var areas = await _areaService.GetActiveAreasByBranchAsync(branchId);
            var totalCount = areas is ICollection<AreaDto> col ? col.Count : (areas?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<AreaDto>>(areas, "Active areas retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets areas with their locations for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>List of areas with locations</returns>
        [HttpGet("with-locations")]
        [Authorize(Policy = "Permission:areas:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<AreaDto>>>> GetAreasWithLocations([FromQuery] int branchId)
        {
            var areas = await _areaService.GetAreasWithLocationsByBranchAsync(branchId);
            var totalCount = areas is ICollection<AreaDto> col ? col.Count : (areas?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<AreaDto>>(areas, "Areas with locations retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets area by ID
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Area details</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:areas:view")]
        public async Task<ActionResult<ApiResponseBase<AreaDto>>> GetArea(int id)
        {
            var area = await _areaService.GetByIdAsync(id);
            if (area == null)
                return NotFound(new ApiResponseBase<AreaDto>(null, "Area not found", "error"));
            return Ok(new ApiResponseBase<AreaDto>(area, "Area retrieved successfully"));
        }

        /// <summary>
        /// Gets area with locations by ID
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Area with locations</returns>
        [HttpGet("{id}/with-locations")]
        [Authorize(Policy = "Permission:areas:view")]
        public async Task<ActionResult<ApiResponseBase<AreaDto>>> GetAreaWithLocations(int id)
        {
            var area = await _areaService.GetWithLocationsAsync(id);
            if (area == null)
                return NotFound(new ApiResponseBase<AreaDto>(null, "Area not found", "error"));
            return Ok(new ApiResponseBase<AreaDto>(area, "Area with locations retrieved successfully"));
        }

        /// <summary>
        /// Creates a new area
        /// </summary>
        /// <param name="dto">Area creation data</param>
        /// <returns>Created area</returns>
        [HttpPost]
        [Authorize(Policy = "Permission:areas:add")]
        public async Task<ActionResult<ApiResponseBase<AreaDto>>> CreateArea([FromBody] CreateAreaDto dto)
        {
            try
            {
                var created = await _areaService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetArea), new { id = created.Id }, new ApiResponseBase<AreaDto>(created, "Area created successfully"));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseBase<AreaDto>(null, ex.Message, "error"));
            }
        }

        /// <summary>
        /// Updates an existing area
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <param name="dto">Area update data</param>
        /// <returns>Updated area</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:areas:edit")]
        public async Task<ActionResult<ApiResponseBase<AreaDto>>> UpdateArea(int id, [FromBody] UpdateAreaDto dto)
        {
            try
            {
                var updated = await _areaService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new ApiResponseBase<AreaDto>(null, "Area not found", "error"));
                return Ok(new ApiResponseBase<AreaDto>(updated, "Area updated successfully"));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseBase<AreaDto>(null, ex.Message, "error"));
            }
        }

        /// <summary>
        /// Deletes an area (soft delete)
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission:areas:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteArea(int id)
        {
            var deleted = await _areaService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponseBase<object>(null, "Area not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "Area deleted successfully"));
        }

        /// <summary>
        /// Validates if area name is unique within a branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="name">Area name</param>
        /// <param name="excludeId">Area ID to exclude from check</param>
        /// <returns>Validation result</returns>
        [HttpGet("validate-name")]
        [Authorize(Policy = "Permission:areas:view")]
        public async Task<ActionResult<ApiResponseBase<object>>> ValidateAreaName(
            [FromQuery] int branchId,
            [FromQuery] string name,
            [FromQuery] int? excludeId = null)
        {
            var isUnique = await _areaService.IsNameUniqueAsync(branchId, name, excludeId);
            return Ok(new ApiResponseBase<object>(isUnique, isUnique ? "Area name is available" : "Area name already exists"));
        }
    }
} 