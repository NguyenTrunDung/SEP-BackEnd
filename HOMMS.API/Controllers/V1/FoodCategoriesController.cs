using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HOMMS.Common.Helpers;
using Asp.Versioning;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
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
        public async Task<ActionResult<ApiResponseBase<IEnumerable<FoodCategoryDto>>>> GetCategories([FromQuery] int branchId)
        {
            var categories = await _foodCategoryService.GetCategoriesByBranchAsync(branchId);
            var totalCount = categories is ICollection<FoodCategoryDto> col ? col.Count : (categories?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<FoodCategoryDto>>(categories, "Categories retrieved successfully", "success", totalCount));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:foodcategories:view")]
        public async Task<ActionResult<ApiResponseBase<FoodCategoryDto>>> GetCategory(int id)
        {
            var category = await _foodCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponseBase<FoodCategoryDto>(null, "Category not found", "error"));
            return Ok(new ApiResponseBase<FoodCategoryDto>(category, "Category retrieved successfully"));
        }

        [HttpPost]
        [Authorize(Policy = "Permission:foodcategories:add")]
        public async Task<ActionResult<ApiResponseBase<FoodCategoryDto>>> CreateCategory([FromBody] FoodCategoryDto dto)
        {
            var created = await _foodCategoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, new ApiResponseBase<FoodCategoryDto>(created, "Category created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:foodcategories:edit")]
        public async Task<ActionResult<ApiResponseBase<FoodCategoryDto>>> UpdateCategory(int id, [FromBody] FoodCategoryDto dto)
        {
            var updated = await _foodCategoryService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new ApiResponseBase<FoodCategoryDto>(null, "Category not found", "error"));
            return Ok(new ApiResponseBase<FoodCategoryDto>(updated, "Category updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission:foodcategories:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteCategory(int id)
        {
            var deleted = await _foodCategoryService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponseBase<object>(null, "Category not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "Category deleted successfully"));
        }
    }
} 