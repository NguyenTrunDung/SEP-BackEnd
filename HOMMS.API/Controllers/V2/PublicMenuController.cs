using Asp.Versioning;
using AutoMapper;
using HOMMS.Application.Implementations;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HOMMS.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/public/menus")]

    [ApiController]
    public class PublicMenuController : ControllerBase
    {
        private readonly IPublicMenuService _publicMenuService;
        private readonly IFoodService _foodService;
        private readonly IFoodCategoryService _foodCategoryService;
        private readonly IBranchContext _branchContext;
        private readonly IMapper _mapper;

        private readonly ISystemLogService _systemLogService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PublicMenuController(
            IPublicMenuService publicMenuService,
            IFoodService foodService,
            IFoodCategoryService foodCategoryService,
            IBranchContext branchContext,
            IMapper mapper, ISystemLogService systemLogService, UserManager<ApplicationUser> userManager)
        {
            _publicMenuService = publicMenuService;
            _foodService = foodService;
            _foodCategoryService = foodCategoryService;
            _branchContext = branchContext;
            _mapper = mapper;

            _systemLogService = systemLogService;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets all menus for the current branch on a specific date
        /// </summary>
        /// <param name="date">Optional date to filter menus (defaults to today)</param>
        /// <returns>List of menus</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponseBase<List<MenuDto>>>> GetMenus([FromQuery] DateTime? date = null)
        {
            var dateToFilter = date ?? DateTime.Today;
            int branchId = _branchContext.GetCurrentBranchId();
            var menus = await _publicMenuService.GetMenusByBranchAndDateAsync(branchId, dateToFilter);
            var menuDtos = _mapper.Map<List<MenuDto>>(menus);
            var totalCount = menuDtos.Count;
            return Ok(new ApiResponseBase<List<MenuDto>>(menuDtos, "Menus retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets all menus for the current branch on a specific date
        /// </summary>
        /// <param name="date">Optional date to filter menus (defaults to today)</param>
        /// <returns>List of menus</returns>
        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<ApiResponseBase<List<MenuDto>>>> GetMenusByBranch(int branchId)
        {
            //int branchId = _branchContext.GetCurrentBranchId();
            var menus = await _publicMenuService.GetMenusByBranch(branchId);
            var menuDtos = _mapper.Map<List<MenuDto>>(menus);
            return Ok(new ApiResponseBase<List<MenuDto>>(menuDtos, "Menus retrieved successfully"));
        }

        /// <summary>
        /// Deletes a menu by its ID
        /// </summary>
        /// <param name="menuId">The ID of the menu to delete</param>
        /// <returns>
        /// 200 OK if the menu was deleted successfully,  
        /// 404 Not Found if the menu does not exist
        /// </returns>
        [HttpDelete("{menuId}")]
        public async Task<IActionResult> DeleteMenu(int menuId)
        {

            var food = await _publicMenuService.GetMenuWithDetailsAsync(menuId);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var user = await _userManager.FindByIdAsync(userId);
            await _systemLogService.LogAsync(food.BranchId, user.FullName, $"đã xóa menu {food.Name}", DateTime.UtcNow);


            var result = await _publicMenuService.DeleteMenu(menuId);
            if (!result)
            {
                return NotFound(new { message = "Menu not found" });
            }

            return Ok(new { message = "Menu deleted successfully" });
        }

        /// <summary>
        /// Gets details for a specific menu
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Menu details with foods and categories</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseBase<MenuDto>>> GetMenu(int id)
        {
            var menu = await _publicMenuService.GetMenuWithDetailsAsync(id);
            if (menu == null)
                return NotFound(new ApiResponseBase<MenuDto>(null, "Menu not found", "error"));
            var menuDto = _mapper.Map<MenuDto>(menu);
            return Ok(new ApiResponseBase<MenuDto>(menuDto, "Menu retrieved successfully"));
        }

        /// <summary>
        /// Gets all foods and categories for the current branch by date
        /// </summary>
        /// <param name="date">Optional date to filter (defaults to today)</param>
        /// <returns>Foods and categories for the day</returns>
        [HttpGet("menu-by-date")]
        public async Task<ActionResult<ApiResponseBase<object>>> GetMenuByDate([FromQuery] DateTime? date = null)
        {
            var dateToFilter = date ?? DateTime.Today;
            int branchId = _branchContext.GetCurrentBranchId();
            var foods = await _foodService.GetFoodsByBranchAndDateAsync(branchId, dateToFilter);
            var categories = await _foodService.GetCategoriesByBranchAndDateAsync(branchId, dateToFilter);
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            var categoryDtos = _mapper.Map<List<FoodCategoryDto>>(categories);
            var result = new
            {
                foods = foodDtos,
                categories = categoryDtos,
                foodsTotalCount = foodDtos.Count,
                categoriesTotalCount = categoryDtos.Count
            };
            return Ok(new ApiResponseBase<object>(result, "Foods and categories for the day retrieved successfully"));
        }

        /// <summary>
        /// Gets all categories for the current branch by date
        /// </summary>
        /// <param name="date">Optional date to filter (defaults to today)</param>
        /// <returns>List of food categories for the day</returns>
        [HttpGet("categories/by-date")]
        public async Task<ActionResult<ApiResponseBase<List<FoodCategoryDto>>>> GetCategoriesByDate([FromQuery] DateTime? date = null)
        {
            var dateToFilter = date ?? DateTime.Today;
            int branchId = _branchContext.GetCurrentBranchId();
            var categories = await _foodService.GetCategoriesByBranchAndDateAsync(branchId, dateToFilter);
            var categoryDtos = _mapper.Map<List<FoodCategoryDto>>(categories);
            var totalCount = categoryDtos.Count;
            return Ok(new ApiResponseBase<List<FoodCategoryDto>>(categoryDtos, "Categories for the day retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets all foods for a specific category and date
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <param name="date">Optional date to filter (defaults to today)</param>
        /// <returns>List of foods in the category for the day</returns>
        [HttpGet("categories/{categoryId}/foods")]
        public async Task<ActionResult<ApiResponseBase<List<FoodDto>>>> GetFoodsByCategoryAndDate(int categoryId, [FromQuery] DateTime? date = null)
        {
            var dateToFilter = date ?? DateTime.Today;
            int branchId = _branchContext.GetCurrentBranchId();
            var foods = await _foodService.GetFoodsByBranchCategoryAndDateAsync(branchId, categoryId, dateToFilter);
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            var totalCount = foodDtos.Count;
            return Ok(new ApiResponseBase<List<FoodDto>>(foodDtos, "Foods in category for the day retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets all foods for a branch and date (menu/guest context)
        /// </summary>
        [HttpGet("foods/by-branch-date")]
        public async Task<ActionResult<ApiResponseBase<List<FoodDto>>>> GetFoodsByBranchAndDate([FromQuery] int branchId, [FromQuery] DateTime date)
        {
            var foods = await _foodService.GetFoodsByBranchAndDateAsync(branchId, date);
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            var totalCount = foodDtos.Count;
            return Ok(new ApiResponseBase<List<FoodDto>>(foodDtos, "Foods for branch and date retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets all categories for a branch and date (menu/guest context)
        /// </summary>
        [HttpGet("categories/by-branch-date")]
        public async Task<ActionResult<ApiResponseBase<List<FoodCategoryDto>>>> GetCategoriesByBranchAndDate([FromQuery] int branchId, [FromQuery] DateTime date)
        {
            var categories = await _foodService.GetCategoriesByBranchAndDateAsync(branchId, date);
            var categoryDtos = _mapper.Map<List<FoodCategoryDto>>(categories);
            var totalCount = categoryDtos.Count;
            return Ok(new ApiResponseBase<List<FoodCategoryDto>>(categoryDtos, "Categories for branch and date retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets all foods for a branch, category, and date (menu/guest context)
        /// </summary>
        [HttpGet("foods/by-branch-category-date")]
        public async Task<ActionResult<ApiResponseBase<List<FoodDto>>>> GetFoodsByBranchCategoryAndDate([FromQuery] int branchId, [FromQuery] int categoryId, [FromQuery] DateTime date)
        {
            var foods = await _foodService.GetFoodsByBranchCategoryAndDateAsync(branchId, categoryId, date);
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            var totalCount = foodDtos.Count;
            return Ok(new ApiResponseBase<List<FoodDto>>(foodDtos, "Foods for branch, category, and date retrieved successfully", "success", totalCount));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponseBase<List<MenuDto>>>> SearchMenusByDate(
         [FromQuery] string date,
         [FromQuery] int? branchId = null)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return BadRequest(new ApiResponseBase<List<MenuDto>>(null, "Invalid date format. Use yyyy-MM-dd", "error"));
            }

            int effectiveBranchId = branchId ?? _branchContext.GetCurrentBranchId();
            var menus = await _publicMenuService.SearchMenusByDateAsync(parsedDate, effectiveBranchId);
            var menuDtos = _mapper.Map<List<MenuDto>>(menus);

            return Ok(new ApiResponseBase<List<MenuDto>>(menuDtos, "Menus retrieved successfully"));
        }



        [HttpGet("foods/by-disease-categories")]
        public async Task<ActionResult<ApiResponseBase<List<FoodDto>>>> GetFoodWithDiseaseCategoryFoodRestriction([FromQuery] int branchId, [FromQuery] int categoryId)
        {
            var foods = await _foodService.GetFoodWithDiseaseCategoryFoodRestrictionAsync(branchId, categoryId);
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            var totalCount = foodDtos.Count;
            return Ok(new ApiResponseBase<List<FoodDto>>(foodDtos, "Foods for disease categories retrieved successfully", "success", totalCount));
        }

    }
}