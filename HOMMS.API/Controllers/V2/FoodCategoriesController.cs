using Asp.Versioning;
using HOMMS.Application.Implementations;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HOMMS.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    public class FoodCategoriesController : ControllerBase
    {
        private readonly IFoodCategoryService _foodCategoryService;


        private readonly ISystemLogService _systemLogService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FoodCategoriesController(IFoodCategoryService foodCategoryService, ISystemLogService systemLogService, UserManager<ApplicationUser> userManager)
        {
            _foodCategoryService = foodCategoryService;
            _systemLogService = systemLogService;
            _userManager = userManager;
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

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var user = await _userManager.FindByIdAsync(userId);
            await _systemLogService.LogAsync(dto.BranchId, user.FullName, $"đã tạo {dto.Name}", DateTime.UtcNow);

            var created = await _foodCategoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, new ApiResponseBase<FoodCategoryDto>(created, "Category created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:foodcategories:edit")]
        public async Task<ActionResult<ApiResponseBase<FoodCategoryDto>>> UpdateCategory(int id, [FromBody] FoodCategoryDto dto)
        {

            var food = await _foodCategoryService.GetByIdAsync(id);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var user = await _userManager.FindByIdAsync(userId);
            await _systemLogService.LogAsync(food.BranchId, user.FullName, $"đã cập nhật {food.Name}", DateTime.UtcNow);


            var updated = await _foodCategoryService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new ApiResponseBase<FoodCategoryDto>(null, "Category not found", "error"));
            return Ok(new ApiResponseBase<FoodCategoryDto>(updated, "Category updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission:foodcategories:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteCategory(int id)
        {
            var food = await _foodCategoryService.GetByIdAsync(id);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var user = await _userManager.FindByIdAsync(userId);
            await _systemLogService.LogAsync(food.BranchId, user.FullName, $"đã xóa {food.Name}", DateTime.UtcNow);


            var deleted = await _foodCategoryService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponseBase<object>(null, "Category not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "Category deleted successfully"));
        }
    }
} 