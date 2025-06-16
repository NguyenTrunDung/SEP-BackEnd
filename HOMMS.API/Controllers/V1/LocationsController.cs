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
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("location")]
        public async Task<ActionResult<ApiResponseBase<List<LocationDto>>>> GetAllLocationsAsync()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            var locationList = locations.ToList() ?? new List<LocationDto>();
            var totalCount = locationList.Count;
            return Ok(new ApiResponseBase<List<LocationDto>>(locationList, "Locations retrieved successfully", "success", totalCount));
        }

        [HttpPost]
        public async Task<IActionResult> CreateLocationAsync([FromBody] LocationDto locationDto)
        {
            if (locationDto == null || string.IsNullOrWhiteSpace(locationDto.Name) || locationDto.AreaId <= 0)
            {
                return BadRequest(new ApiResponseBase<object>(null, "Invalid location data: Area and Location Name are required", "error"));
            }

            var success = await _locationService.CreateLocationAsync(locationDto);
            return success ? Ok(new ApiResponseBase<object>(null, "Location created successfully")) : BadRequest(new ApiResponseBase<object>(null, "Failed to create location", "error"));
        }

        [HttpPut("location/{id}")]
        public async Task<IActionResult> UpdateLocationAsync(int id, [FromBody] LocationDto locationDto)
        {
            if (locationDto == null || string.IsNullOrWhiteSpace(locationDto.Name) || locationDto.AreaId <= 0)
            {
                return BadRequest(new ApiResponseBase<object>(null, "Invalid location data: Area and Location Name are required", "error"));
            }
            var success = await _locationService.UpdateLocationAsync(id, locationDto);
            return success ? Ok(new ApiResponseBase<object>(null, "Location updated successfully")) : NotFound(new ApiResponseBase<object>(null, "Location not found", "error"));
        }

        [HttpDelete("location/{id}")]
        public async Task<IActionResult> DeleteLocationAsync(int id)
        {
            var success = await _locationService.DeleteLocationAsync(id);
            return success ? Ok(new ApiResponseBase<object>(null, "Location deleted successfully")) : NotFound(new ApiResponseBase<object>(null, "Location not found", "error"));
        }
    }
}
