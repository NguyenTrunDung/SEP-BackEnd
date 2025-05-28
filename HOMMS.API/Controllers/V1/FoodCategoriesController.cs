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
    public class FoodCategoriesController : ControllerBase
    {
        private readonly IFoodCategoryService _foodCategoryService;

        public FoodCategoriesController(IFoodCategoryService foodCategoryService)
        {
            _foodCategoryService = foodCategoryService;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:foodcategories:view")]
        public async Task<ActionResult<IEnumerable<FoodCategoryDto>>> GetCategories([FromQuery] int branchId)
        {
            var categories = await _foodCategoryService.GetCategoriesByBranchAsync(branchId);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:foodcategories:view")]
        public async Task<ActionResult<FoodCategoryDto>> GetCategory(int id)
        {
            var category = await _foodCategoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Policy = "Permission:foodcategories:add")]
        public async Task<ActionResult<FoodCategoryDto>> CreateCategory([FromBody] FoodCategoryDto dto)
        {
            var created = await _foodCategoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:foodcategories:edit")]
        public async Task<ActionResult<FoodCategoryDto>> UpdateCategory(int id, [FromBody] FoodCategoryDto dto)
        {
            var updated = await _foodCategoryService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission:foodcategories:delete")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _foodCategoryService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 