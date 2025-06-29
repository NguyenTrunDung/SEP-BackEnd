using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HOMMS.Common.Helpers;
using Asp.Versioning;
using System;
using System.Linq;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    public class FoodCategoriesController : ControllerBase
    {
        private readonly IFoodCategoryService _foodCategoryService;
        private readonly IWebHostEnvironment _env;

        public FoodCategoriesController(IFoodCategoryService foodCategoryService, IWebHostEnvironment env)
        {
            _foodCategoryService = foodCategoryService;
            _env = env;
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
            try
            {
                var created = await _foodCategoryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, new ApiResponseBase<FoodCategoryDto>(created, "Category created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }

        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:foodcategories:edit")]
        public async Task<ActionResult<ApiResponseBase<FoodCategoryDto>>> UpdateCategory(int id, [FromBody] FoodCategoryDto dto)
        {
            try
            {
                var updated = await _foodCategoryService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new ApiResponseBase<FoodCategoryDto>(null, "Category not found", "error"));
                return Ok(new ApiResponseBase<FoodCategoryDto>(updated, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
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

        // Image upload endpoints following the same pattern as FoodsController
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "Permission:foodcategories:add")]
        public async Task<IActionResult> CreateCategoryWithImage([FromForm] FoodCategoryCreateRequest request)
        {
            try
            {
                var categoryDto = new FoodCategoryDto
                {
                    Name = request.Name,
                    ImageUrl = request.ImageUrl,
                   // Sort = request.Sort ?? 0, // Will be overridden by auto-sort if not provided
                    BranchId = request.BranchId
                };

                FoodCategoryDto result;
                
                // Use auto-sort if no sort value provided, otherwise use the provided value
                if (request.Sort.HasValue)
                {
                    result = await _foodCategoryService.CreateCategoryAsync(categoryDto, request.Image, _env.WebRootPath);
                }
                else
                {
                    result = await _foodCategoryService.CreateCategoryWithAutoSortAsync(categoryDto, request.Image, _env.WebRootPath);
                }

                return Ok(new ApiResponseBase<FoodCategoryDto>(result, "Category created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<FoodCategoryDto>(null, ex.Message, "error"));
            }
        }

        [HttpPut("update/{id}")]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "Permission:foodcategories:edit")]
        public async Task<IActionResult> UpdateCategoryWithImage(int id, [FromForm] FoodCategoryCreateRequest request)
        {
            try
            {
                var dto = new FoodCategoryDto
                {
                    Name = request.Name,
                    ImageUrl = request.ImageUrl,
                    Sort = request.Sort ?? 0, 
                    BranchId = request.BranchId
                };

                var result = await _foodCategoryService.UpdateCategoryAsync(id, dto, request.Image, _env.WebRootPath);
                return Ok(new ApiResponseBase<FoodCategoryDto>(result, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<FoodCategoryDto>(null, ex.Message, "error"));
            }
        }

     

        // Reordering endpoints for drag-and-drop functionality
        [HttpPost("reorder")]
        [Authorize(Policy = "Permission:foodcategories:edit")]
        public async Task<IActionResult> ReorderCategories([FromBody] ReorderFoodCategoriesRequest request)
        {
            try
            {
                var categoryOrders = request.CategoryOrders.Select(co => (co.CategoryId, co.Sort));
                var success = await _foodCategoryService.ReorderCategoriesAsync(categoryOrders, request.BranchId);
                
                if (success)
                {
                    return Ok(new ApiResponseBase<object>(null, "Categories reordered successfully"));
                }
                else
                {
                    return BadRequest(new ApiResponseBase<object>(null, "Failed to reorder categories", "error"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
            }
        }

        [HttpPost("move")]
        [Authorize(Policy = "Permission:foodcategories:edit")]
        public async Task<IActionResult> MoveCategory([FromBody] MoveFoodCategoryRequest request)
        {
            try
            {
                var success = await _foodCategoryService.MoveCategoryAsync(request.CategoryId, request.NewPosition, request.BranchId);
                
                if (success)
                {
                    return Ok(new ApiResponseBase<object>(null, "Category moved successfully"));
                }
                else
                {
                    return BadRequest(new ApiResponseBase<object>(null, "Failed to move category", "error"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
            }
        }
    }
}