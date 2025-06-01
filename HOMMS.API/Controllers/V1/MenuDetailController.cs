using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuDetailController : ControllerBase
    {
        private readonly IMenuDetailService _menuDetailService;

        public MenuDetailController(IMenuDetailService menuDetailService)
        {
            _menuDetailService = menuDetailService;
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
            return success ? NoContent() : NotFound();
        }
    }
}
