using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HOMMS.Common.Helpers;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/public/menus")]
    [ApiController]
    public class PublicMenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IFoodCategoryRepository _foodCategoryRepository;
        private readonly IBranchContext _branchContext;
        private readonly IMapper _mapper;
        
        public PublicMenuController(
            IMenuRepository menuRepository,
            IFoodRepository foodRepository,
            IFoodCategoryRepository foodCategoryRepository,
            IBranchContext branchContext,
            IMapper mapper)
        {
            _menuRepository = menuRepository;
            _foodRepository = foodRepository;
            _foodCategoryRepository = foodCategoryRepository;
            _branchContext = branchContext;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Gets menus for the currently selected branch
        /// </summary>
        /// <param name="date">Optional date to filter menus (defaults to today)</param>
        /// <returns>List of menus</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponseBase<List<MenuDto>>>> GetMenus([FromQuery] DateTime? date = null)
        {
            var dateToFilter = date ?? DateTime.Today;
            int branchId = _branchContext.GetCurrentBranchId();
            var menus = await _menuRepository.GetMenusByBranchAndDateAsync(branchId, dateToFilter);
            var menuDtos = _mapper.Map<List<MenuDto>>(menus);
            return Ok(new ApiResponseBase<List<MenuDto>>(menuDtos, "Menus retrieved successfully"));
        }
        
        /// <summary>
        /// Gets a specific menu by ID with its food details
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Menu details with foods</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseBase<MenuDto>>> GetMenu(int id)
        {
            var menu = await _menuRepository.GetMenuWithDetailsAsync(id);
            if (menu == null)
                return NotFound(new ApiResponseBase<MenuDto>(null, "Menu not found", "error"));
            var menuDto = _mapper.Map<MenuDto>(menu);
            // Optionally map details by category if needed
            return Ok(new ApiResponseBase<MenuDto>(menuDto, "Menu retrieved successfully"));
        }
        
        /// <summary>
        /// Gets all menus with their food details for the current branch on a specific date
        /// </summary>
        /// <param name="date">Optional date to filter menus (defaults to today)</param>
        /// <returns>List of menus with their food details</returns>
        [HttpGet("with-details")]
        public async Task<ActionResult<IEnumerable<MenuDto>>> GetMenusWithDetails([FromQuery] DateTime? date = null)
        {
            try
            {
                var dateToFilter = date ?? DateTime.Today;
                int branchId = _branchContext.GetCurrentBranchId();
                
                var menus = await _menuRepository.GetMenusWithDetailsByBranchAndDateAsync(branchId, dateToFilter);
                var menuDtos = new List<MenuDto>();
                
                foreach (var menu in menus)
                {
                    var menuDto = new MenuDto
                    {
                        Id = menu.Id,
                        Name = menu.Name,
                        Date = menu.Date,
                        TimeOfDay = menu.TimeOfDay,
                        IsTime = menu.IsTime,
                        TimeFrom = menu.TimeFrom,
                        TimeTo = menu.TimeTo,
                        BranchId = menu.BranchId,
                        Branch = menu.Branch != null ? new BranchDto
                        {
                            Id = menu.Branch.Id,
                            Name = menu.Branch.Name,
                            Code = menu.Branch.Code,
                            Address = menu.Branch.Address,
                            Phone = menu.Branch.Phone,
                            Email = menu.Branch.Email
                        } : null
                    };
                    
                    // Add menu details
                    var menuDetails = menu.MenuDetails
                        .Where(md => md.Status == true && md.Food?.Category != null)
                        .Select(md => new MenuDetailDto
                        {
                            Id = md.Id,
                            MenuId = md.MenuId,
                            FoodId = md.FoodId,
                            Quantity = md.Qty,
                            Sold = md.Sold,
                            PriceForGuest = md.PriceForGuest ?? md.Food?.PriceForGuest,
                            PriceForPatient = md.PriceForPatient ?? md.Food?.PriceForPatient,
                            PriceForStaff = md.PriceForStaff ?? md.Food?.PriceForStaff,
                            DiscountPrice = md.DiscountPrice,
                            Status = md.Status,
                            Food = new FoodDto
                            {
                                Id = md.Food.Id,
                                Name = md.Food.Name,
                                Description = md.Food.Description,
                                CategoryId = md.Food.CategoryId,
                                ImageUrl = md.Food.Image,
                                IsSetDish = md.Food.IsSetDish,
                                IsAddOn = md.Food.IsAddOn,
                                PriceForGuest = md.Food.PriceForGuest,
                                PriceForPatient = md.Food.PriceForPatient,
                                PriceForStaff = md.Food.PriceForStaff,
                                Sort = md.Food.Sort,
                                Category = md.Food.Category != null ? new FoodCategoryDto
                                {
                                    Id = md.Food.Category.Id,
                                    Name = md.Food.Category.Name,
                                    ImageUrl = md.Food.Category.Image,
                                    Sort = md.Food.Category.Sort ?? 0
                                } : null
                            }
                        }).ToList();
                    
                    menuDto.MenuDetails = menuDetails;
                    
                    // Organize by category
                    menuDto.FoodsByCategory = menuDetails
                        .Where(md => md.Food?.Category != null)
                        .GroupBy(md => md.Food.Category!)
                        .OrderBy(g => g.Key.Sort)
                        .ToDictionary(
                            g => g.Key, 
                            g => g.ToList()
                        );
                    
                    menuDtos.Add(menuDto);
                }
                
                return Ok(menuDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Gets all menus for all active branches on a specific date
        /// </summary>
        /// <param name="date">Optional date to filter menus (defaults to today)</param>
        /// <returns>List of branches with their menus</returns>
        [HttpGet("all-branches")]
        public async Task<ActionResult<AllBranchesMenusDto>> GetMenusForAllBranches([FromQuery] DateTime? date = null)
        {
            try
            {
                var dateToFilter = date ?? DateTime.Today;
                var branchesWithMenus = await _menuRepository.GetMenusForAllBranchesAsync(dateToFilter);
                
                var result = new AllBranchesMenusDto
                {
                    Date = dateToFilter,
                    Branches = branchesWithMenus.Select(kvp => new BranchMenusDto
                    {
                        Branch = new BranchDto
                        {
                            Id = kvp.Key.Id,
                            Name = kvp.Key.Name,
                            Code = kvp.Key.Code,
                            Address = kvp.Key.Address,
                            Phone = kvp.Key.Phone,
                            Email = kvp.Key.Email
                        },
                        Menus = kvp.Value.Select(m => new MenuDto
                        {
                            Id = m.Id,
                            Name = m.Name,
                            Date = m.Date,
                            TimeOfDay = m.TimeOfDay,
                            IsTime = m.IsTime,
                            TimeFrom = m.TimeFrom,
                            TimeTo = m.TimeTo,
                            BranchId = m.BranchId
                        }).ToList()
                    }).ToList()
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Gets all active food categories for the current branch
        /// </summary>
        /// <returns>List of food categories</returns>
        [HttpGet("categories")]
        public async Task<ActionResult<ApiResponseBase<List<FoodCategoryDto>>>> GetFoodCategories()
        {
            int branchId = _branchContext.GetCurrentBranchId();
            var categories = await _foodCategoryRepository.GetActiveCategoriesByBranchAsync(branchId);
            var categoryDtos = _mapper.Map<List<FoodCategoryDto>>(categories);
            return Ok(new ApiResponseBase<List<FoodCategoryDto>>(categoryDtos, "Categories retrieved successfully"));
        }
        
        /// <summary>
        /// Gets all foods for a specific category in the current branch
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of foods in the category</returns>
        [HttpGet("categories/{categoryId}/foods")]
        public async Task<ActionResult<ApiResponseBase<List<FoodDto>>>> GetFoodsByCategory(int categoryId)
        {
            int branchId = _branchContext.GetCurrentBranchId();
            var foods = await _foodRepository.GetFoodsByBranchAndCategoryAsync(branchId, categoryId);
            var foodDtos = _mapper.Map<List<FoodDto>>(foods);
            return Ok(new ApiResponseBase<List<FoodDto>>(foodDtos, "Foods retrieved successfully"));
        }
    }
} 