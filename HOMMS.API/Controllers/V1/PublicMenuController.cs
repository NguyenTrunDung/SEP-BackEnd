using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HOMMS.Common.Helpers;

namespace HOMMS.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/public/menus")]
    public class PublicMenuController : ControllerBase
    {
        private readonly IPublicMenuService _publicMenuService;
        private readonly IFoodService _foodService;
        private readonly IFoodCategoryService _foodCategoryService;
        private readonly IBranchContext _branchContext;
        private readonly IMapper _mapper;

        public PublicMenuController(
            IPublicMenuService publicMenuService,
            IFoodService foodService,
            IFoodCategoryService foodCategoryService,
            IBranchContext branchContext,
            IMapper mapper)
        {
            _publicMenuService = publicMenuService;
            _foodService = foodService;
            _foodCategoryService = foodCategoryService;
            _branchContext = branchContext;
            _mapper = mapper;
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
            return Ok(new ApiResponseBase<List<MenuDto>>(menuDtos, "Menus retrieved successfully"));
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
            // TODO: Implement these service methods for real data
            var foods = await _foodService.GetFoodsByBranchAndDateAsync(branchId, dateToFilter); // You need to implement this
            var categories = await _foodService.GetCategoriesByBranchAndDateAsync(branchId, dateToFilter); // You need to implement this
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            var categoryDtos = _mapper.Map<List<FoodCategoryDto>>(categories);
            var result = new {
                foods = foodDtos,
                categories = categoryDtos
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
            // TODO: Implement this service method for real data
            var categories = await _foodService.GetCategoriesByBranchAndDateAsync(branchId, dateToFilter); // You need to implement this
            var categoryDtos = _mapper.Map<List<FoodCategoryDto>>(categories);
            return Ok(new ApiResponseBase<List<FoodCategoryDto>>(categoryDtos, "Categories for the day retrieved successfully"));
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
            // TODO: Implement this service method for real data
            var foods = await _foodService.GetFoodsByBranchCategoryAndDateAsync(branchId, categoryId, dateToFilter); // You need to implement this
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            return Ok(new ApiResponseBase<List<FoodDto>>(foodDtos, "Foods in category for the day retrieved successfully"));
        }

        /// <summary>
        /// Gets all foods for a branch and date (menu/guest context)
        /// </summary>
        [HttpGet("foods/by-branch-date")]
        public async Task<ActionResult<ApiResponseBase<List<FoodDto>>>> GetFoodsByBranchAndDate([FromQuery] int branchId, [FromQuery] DateTime date)
        {
            var foods = await _foodService.GetFoodsByBranchAndDateAsync(branchId, date);
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            return Ok(new ApiResponseBase<List<FoodDto>>(foodDtos, "Foods for branch and date retrieved successfully"));
        }

        /// <summary>
        /// Gets all categories for a branch and date (menu/guest context)
        /// </summary>
        [HttpGet("categories/by-branch-date")]
        public async Task<ActionResult<ApiResponseBase<List<FoodCategoryDto>>>> GetCategoriesByBranchAndDate([FromQuery] int branchId, [FromQuery] DateTime date)
        {
            var categories = await _foodService.GetCategoriesByBranchAndDateAsync(branchId, date);
            var categoryDtos = _mapper.Map<List<FoodCategoryDto>>(categories);
            return Ok(new ApiResponseBase<List<FoodCategoryDto>>(categoryDtos, "Categories for branch and date retrieved successfully"));
        }

        /// <summary>
        /// Gets all foods for a branch, category, and date (menu/guest context)
        /// </summary>
        [HttpGet("foods/by-branch-category-date")]
        public async Task<ActionResult<ApiResponseBase<List<FoodDto>>>> GetFoodsByBranchCategoryAndDate([FromQuery] int branchId, [FromQuery] int categoryId, [FromQuery] DateTime date)
        {
            var foods = await _foodService.GetFoodsByBranchCategoryAndDateAsync(branchId, categoryId, date);
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            return Ok(new ApiResponseBase<List<FoodDto>>(foodDtos, "Foods for branch, category, and date retrieved successfully"));
        }
    }
}