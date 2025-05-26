using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodsController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:foods:view")]
        public async Task<ActionResult<IEnumerable<FoodDto>>> GetFoods([FromQuery] int branchId)
        {
            var foods = await _foodService.GetFoodsByBranchAsync(branchId);
            return Ok(foods);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:foods:view")]
        public async Task<ActionResult<FoodDto>> GetFood(int id)
        {
            var food = await _foodService.GetByIdAsync(id);
            if (food == null) return NotFound();
            return Ok(food);
        }

        [HttpPost]
        [Authorize(Policy = "Permission:foods:add")]
        public async Task<ActionResult<FoodDto>> CreateFood([FromBody] FoodDto dto)
        {
            var created = await _foodService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetFood), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:foods:edit")]
        public async Task<ActionResult<FoodDto>> UpdateFood(int id, [FromBody] FoodDto dto)
        {
            var updated = await _foodService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission:foods:delete")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var deleted = await _foodService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 