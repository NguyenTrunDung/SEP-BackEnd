using Asp.Versioning;
using AutoMapper;
using HOMMS.Application.Implementations;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOMMS.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MenuDetailController : ControllerBase
    {
        private readonly IMenuDetailService _menuDetailService;

        private readonly ISystemLogService _systemLogService;
        private readonly UserManager<ApplicationUser> _userManager;

        public MenuDetailController(IMenuDetailService menuDetailService, ISystemLogService systemLogService, UserManager<ApplicationUser> userManager)
        {
            _menuDetailService = menuDetailService;

            _systemLogService = systemLogService;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetMenuList()
        {
            return Ok();

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuDetail(int id)
        {
            var result = await _menuDetailService.GetMenuWithDetailsAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuDetail(int id, [FromBody] UpdateMenuDto dto)
        {

            if (id != dto.Id) return BadRequest("Mismatched ID");
            var success = await _menuDetailService.UpdateMenuWithDetailsAsync(dto);

            var food = await _menuDetailService.GetByIdAsync(id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            var user = await _userManager.FindByIdAsync(userId);
            await _systemLogService.LogAsync(food.BranchId, user?.FullName ?? "Unknown", $"đã cập nhật chi tiết menu {food.Name}", DateTime.UtcNow);

            
            return success ? NoContent() : NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddMenuDetail([FromBody] CreateMenuDto dto)
        {
            
            
            if (dto == null) return BadRequest();

                      


            var success = await _menuDetailService.AddMenuWithDetailsAsync(dto);
            if (success)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                var user = await _userManager.FindByIdAsync(userId);
                await _systemLogService.LogAsync(dto.BranchId, user?.FullName ?? "Unknown", $"đã thêm chi tiết menu {dto.Name}", DateTime.UtcNow);

                // Thường trả về 201 Created kèm URL resource mới tạo (nếu có Id trả về)
                return CreatedAtAction(nameof(GetMenuDetail), new { id = dto.Id }, dto);
            }
            else
            {
                return BadRequest("Unable to add menu.");
            }
        }

    }
}
