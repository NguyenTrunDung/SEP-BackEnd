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
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Gets all locations for a specific area
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <returns>List of locations</returns>
        [HttpGet("by-area")]
        [Authorize(Policy = "Permission:locations:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<LocationDto>>>> GetLocationsByArea([FromQuery] int areaId)
        {
            var locations = await _locationService.GetLocationsByAreaAsync(areaId);
            var totalCount = locations is ICollection<LocationDto> col ? col.Count : (locations?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<LocationDto>>(locations, "Locations retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets active locations for a specific area
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <returns>List of active locations</returns>
        [HttpGet("by-area/active")]
        [Authorize(Policy = "Permission:locations:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<LocationDto>>>> GetActiveLocationsByArea([FromQuery] int areaId)
        {
            var locations = await _locationService.GetActiveLocationsByAreaAsync(areaId);
            var totalCount = locations is ICollection<LocationDto> col ? col.Count : (locations?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<LocationDto>>(locations, "Active locations retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets all locations for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>List of locations</returns>
        [HttpGet("by-branch")]
        [Authorize(Policy = "Permission:locations:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<LocationDto>>>> GetLocationsByBranch([FromQuery] int branchId)
        {
            var locations = await _locationService.GetLocationsByBranchAsync(branchId);
            var totalCount = locations is ICollection<LocationDto> col ? col.Count : (locations?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<LocationDto>>(locations, "Locations retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets active locations for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>List of active locations</returns>
        [HttpGet("by-branch/active")]
        [Authorize(Policy = "Permission:locations:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<LocationDto>>>> GetActiveLocationsByBranch([FromQuery] int branchId)
        {
            var locations = await _locationService.GetActiveLocationsByBranchAsync(branchId);
            var totalCount = locations is ICollection<LocationDto> col ? col.Count : (locations?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<LocationDto>>(locations, "Active locations retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets location by ID
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <returns>Location details</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:locations:view")]
        public async Task<ActionResult<ApiResponseBase<LocationDto>>> GetLocation(int id)
        {
            var location = await _locationService.GetByIdAsync(id);
            if (location == null)
                return NotFound(new ApiResponseBase<LocationDto>(null, "Location not found", "error"));
            return Ok(new ApiResponseBase<LocationDto>(location, "Location retrieved successfully"));
        }

        /// <summary>
        /// Gets location with area information by ID
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <returns>Location with area details</returns>
        [HttpGet("{id}/with-area")]
        [Authorize(Policy = "Permission:locations:view")]
        public async Task<ActionResult<ApiResponseBase<LocationDto>>> GetLocationWithArea(int id)
        {
            var location = await _locationService.GetWithAreaAsync(id);
            if (location == null)
                return NotFound(new ApiResponseBase<LocationDto>(null, "Location not found", "error"));
            return Ok(new ApiResponseBase<LocationDto>(location, "Location with area retrieved successfully"));
        }

        /// <summary>
        /// Creates a new location
        /// </summary>
        /// <param name="dto">Location creation data</param>
        /// <returns>Created location</returns>
        [HttpPost]
        [Authorize(Policy = "Permission:locations:add")]
        public async Task<ActionResult<ApiResponseBase<LocationDto>>> CreateLocation([FromBody] CreateLocationDto dto)
        {
            try
            {
                var created = await _locationService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetLocation), new { id = created.Id }, new ApiResponseBase<LocationDto>(created, "Location created successfully"));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseBase<LocationDto>(null, ex.Message, "error"));
            }
        }

        /// <summary>
        /// Updates an existing location
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <param name="dto">Location update data</param>
        /// <returns>Updated location</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:locations:edit")]
        public async Task<ActionResult<ApiResponseBase<LocationDto>>> UpdateLocation(int id, [FromBody] UpdateLocationDto dto)
        {
            try
            {
                var updated = await _locationService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new ApiResponseBase<LocationDto>(null, "Location not found", "error"));
                return Ok(new ApiResponseBase<LocationDto>(updated, "Location updated successfully"));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseBase<LocationDto>(null, ex.Message, "error"));
            }
        }

        /// <summary>
        /// Deletes a location (soft delete)
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission:locations:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteLocation(int id)
        {
            var deleted = await _locationService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponseBase<object>(null, "Location not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "Location deleted successfully"));
        }

        /// <summary>
        /// Validates if location name is unique within an area
        /// </summary>
        /// <param name="areaId">Area ID</param>
        /// <param name="name">Location name</param>
        /// <param name="excludeId">Location ID to exclude from check</param>
        /// <returns>Validation result</returns>
        /// the ApiResponseBase<object> should be bool ?????
        [HttpGet("validate-name")]
        [Authorize(Policy = "Permission:locations:view")]
        public async Task<ActionResult<ApiResponseBase<object>>> ValidateLocationName(
            [FromQuery] int areaId,
            [FromQuery] string name,
            [FromQuery] int? excludeId = null)
        {
            var isUnique = await _locationService.IsNameUniqueAsync(areaId, name, excludeId);
            return Ok(new ApiResponseBase<object>(isUnique, isUnique ? "Location name is available" : "Location name already exists"));
        }

        /// <summary>
        /// Validates if room number is unique within a branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="roomNumber">Room number</param>
        /// <param name="excludeId">Location ID to exclude from check</param>
        /// <returns>Validation result</returns>
        [HttpGet("validate-room-number")]
        [Authorize(Policy = "Permission:locations:view")]
        public async Task<ActionResult<ApiResponseBase<object>>> ValidateRoomNumber(
            [FromQuery] int branchId,
            [FromQuery] string roomNumber,
            [FromQuery] int? excludeId = null)
        {
            var isUnique = await _locationService.IsRoomNumberUniqueAsync(branchId, roomNumber, excludeId);
            return Ok(new ApiResponseBase<object>(isUnique, isUnique ? "Room number is available" : "Room number already exists"));
        }
    }
} 