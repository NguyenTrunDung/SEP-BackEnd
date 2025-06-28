using Asp.Versioning;
using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MenuDetailController : ControllerBase
    {
        private readonly IMenuDetailService _menuDetailService;

        public MenuDetailController(IMenuDetailService menuDetailService)
        {
            _menuDetailService = menuDetailService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseBase<List<MenuDetailViewDto>>>> GetMenuList()
        {
            try
            {
                var menus = await _menuDetailService.GetAllMenusWithDetailsAsync();
                
                if (!menus.Any())
                {
                    return NotFound(new ApiResponseBase<List<MenuDetailViewDto>>(
                        null, 
                        "No menus found", 
                        "error", 
                        0
                    ));
                }

                return Ok(new ApiResponseBase<List<MenuDetailViewDto>>(
                    menus, 
                    "Menu list retrieved successfully", 
                    "success", 
                    menus.Count
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ApiResponseBase<List<MenuDetailViewDto>>(
                        null, 
                        $"An error occurred while retrieving menus: {ex.Message}", 
                        "error", 
                        0
                    ));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseBase<MenuDetailViewDto>>> GetMenuDetail(int id)
        {
            try
            {
                var result = await _menuDetailService.GetMenuWithDetailsAsync(id);
                
                if (result == null)
                {
                    return NotFound(new ApiResponseBase<MenuDetailViewDto>(
                        null, 
                        "Menu not found", 
                        "error"
                    ));
                }
                
                return Ok(new ApiResponseBase<MenuDetailViewDto>(
                    result, 
                    "Menu detail retrieved successfully", 
                    "success"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ApiResponseBase<MenuDetailViewDto>(
                        null, 
                        $"An error occurred while retrieving menu detail: {ex.Message}", 
                        "error"
                    ));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuDetail(int id, [FromBody] UpdateMenuDto dto)
        {
            if (id != dto.Id) return BadRequest("Mismatched ID");

            var success = await _menuDetailService.UpdateMenuWithDetailsAsync(dto);
            return success ? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddMenuDetail([FromBody] CreateMenuDto dto)
        {
            try
            {
                if (dto == null) return BadRequest();

                var success = await _menuDetailService.AddMenuWithDetailsAsync(dto);
                if (success)
                {
                    // Thường trả về 201 Created kèm URL resource mới tạo (nếu có Id trả về)
                    return CreatedAtAction(nameof(GetMenuDetail), new { id = dto.Id }, dto);
                }
                else
                {
                    return BadRequest("Unable to add menu.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
