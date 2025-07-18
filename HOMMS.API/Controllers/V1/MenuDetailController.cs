using Asp.Versioning;
using AutoMapper;
using HOMMS.Application.Implementations;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("templates")]
        public async Task<ActionResult<ApiResponseBase<List<MenuTemplateDto>>>> GetMenuTemplates()
        {
            try
            {
                var templates = await _menuDetailService.GetMenuTemplatesAsync();
                
                return Ok(new ApiResponseBase<List<MenuTemplateDto>>(
                    templates, 
                    "Menu templates retrieved successfully", 
                    "success", 
                    templates.Count
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ApiResponseBase<List<MenuTemplateDto>>(
                        null, 
                        $"An error occurred while retrieving menu templates: {ex.Message}", 
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

        [HttpGet("recent-for-template")]
        public async Task<ActionResult<ApiResponseBase<List<MenuTemplateDto>>>> GetRecentMenusForTemplate([FromQuery] int days = 14)
        {
            try
            {
                // Limit days to prevent excessive data retrieval
                if (days > 30) days = 30;
                if (days < 1) days = 14;

                var cutoffDate = DateTime.Today.AddDays(-days);
                var allMenus = await _menuDetailService.GetAllMenusWithDetailsAsync();
                
                // Filter menus from the specified number of days ago
                var recentMenus = allMenus
                    .Where(menu => menu.Date >= cutoffDate && menu.Date < DateTime.Today)
                    .Select(menu => new MenuTemplateDto
                    {
                        Id = menu.Id,
                        Date = menu.Date,
                        Name = menu.Name,
                        TimeOfDay = menu.TimeOfDay,
                        TotalDishes = menu.Details?.Count ?? 0,
                        IsTime = menu.IsTime,
                        TimeFrom = menu.TimeFrom,
                        TimeTo = menu.TimeTo,
                        BranchId = menu.BranchId,
                        CreatedAt = menu.CreatedAt,
                        CreatedBy = menu.CreatedBy
                    })
                    .OrderByDescending(menu => menu.Date)
                    .ThenByDescending(menu => menu.CreatedAt)
                    .ToList();

                return Ok(new ApiResponseBase<List<MenuTemplateDto>>(
                    recentMenus, 
                    $"Recent menus for template retrieved successfully (past {days} days)", 
                    "success", 
                    recentMenus.Count
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ApiResponseBase<List<MenuTemplateDto>>(
                        null, 
                        $"An error occurred while retrieving recent menus: {ex.Message}", 
                        "error", 
                        0
                    ));
            }
        }

        [HttpPost("{id}/copy")]
        public async Task<ActionResult<ApiResponseBase<MenuDetailViewDto>>> CopyMenuAsTemplate(int id, [FromBody] CopyMenuRequestDto request)
        {
            try
            {
                var result = await _menuDetailService.CopyMenuAsTemplateAsync(id, request.NewDate, request.NewName);
                
                if (result == null)
                {
                    return NotFound(new ApiResponseBase<MenuDetailViewDto>(
                        null, 
                        "Source menu not found", 
                        "error"
                    ));
                }
                
                return Ok(new ApiResponseBase<MenuDetailViewDto>(
                    result, 
                    "Menu copied successfully", 
                    "success"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ApiResponseBase<MenuDetailViewDto>(
                        null, 
                        $"An error occurred while copying menu: {ex.Message}", 
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

        [HttpDelete("{id}")]      
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteFood(int id)
        {
            var deleted = await _menuDetailService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponseBase<object>(null, "MenuDetail not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "MenuDetail deleted successfully"));
        }
    }
}
