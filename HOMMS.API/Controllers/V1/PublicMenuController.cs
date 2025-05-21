using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        
        public PublicMenuController(
            IMenuRepository menuRepository,
            IFoodRepository foodRepository,
            IFoodCategoryRepository foodCategoryRepository,
            IBranchContext branchContext)
        {
            _menuRepository = menuRepository;
            _foodRepository = foodRepository;
            _foodCategoryRepository = foodCategoryRepository;
            _branchContext = branchContext;
        }
        
        /// <summary>
        /// Gets menus for the currently selected branch
        /// </summary>
        /// <param name="date">Optional date to filter menus (defaults to today)</param>
        /// <returns>List of menus</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuDto>>> GetMenus([FromQuery] DateTime? date = null)
        {
            try
            {
                var dateToFilter = date ?? DateTime.Today;
                int branchId = _branchContext.GetCurrentBranchId();
                
                var menus = await _menuRepository.GetMenusByBranchAndDateAsync(branchId, dateToFilter);
                var menuDtos = menus.Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Date = m.Date,
                    TimeOfDay = m.TimeOfDay,
                    IsTime = m.IsTime,
                    TimeFrom = m.TimeFrom,
                    TimeTo = m.TimeTo,
                    BranchId = m.BranchId,
                    Branch = m.Branch != null ? new BranchDto
                    {
                        Id = m.Branch.Id,
                        Name = m.Branch.Name,
                        Code = m.Branch.Code,
                        Address = m.Branch.Address,
                        Phone = m.Branch.Phone,
                        Email = m.Branch.Email
                    } : null
                }).ToList();
                
                return Ok(menuDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Gets a specific menu by ID with its food details
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <returns>Menu details with foods</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuDto>> GetMenu(int id)
        {
            try
            {
                var menu = await _menuRepository.GetMenuWithDetailsAsync(id);
                
                if (menu == null)
                {
                    return NotFound("Menu not found");
                }
                
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
                
                // Organize menu details by food category
                var detailsByCategory = await _menuRepository.GetMenuDetailsByCategoryAsync(id);
                
                menuDto.FoodsByCategory = detailsByCategory.ToDictionary(
                    kvp => new FoodCategoryDto
                    {
                        Id = kvp.Key.Id,
                        Name = kvp.Key.Name,
                        ImageUrl = kvp.Key.Image,
                        Sort = kvp.Key.Sort ?? 0
                    },
                    kvp => kvp.Value.Select(md => new MenuDetailDto
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
                            Sort = md.Food.Sort
                        }
                    }).ToList()
                );
                
                // Add all details to the MenuDetails list for backward compatibility
                menuDto.MenuDetails = menuDto.FoodsByCategory.Values.SelectMany(details => details).ToList();
                
                return Ok(menuDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
        public async Task<ActionResult<IEnumerable<FoodCategoryDto>>> GetFoodCategories()
        {
            try
            {
                int branchId = _branchContext.GetCurrentBranchId();
                
                var categories = await _foodCategoryRepository.GetActiveCategoriesByBranchAsync(branchId);
                var categoryDtos = categories.Select(c => new FoodCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.Image,
                    Sort = c.Sort ?? 0
                }).OrderBy(c => c.Sort).ToList();
                
                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Gets all foods for a specific category in the current branch
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of foods in the category</returns>
        [HttpGet("categories/{categoryId}/foods")]
        public async Task<ActionResult<IEnumerable<FoodDto>>> GetFoodsByCategory(int categoryId)
        {
            try
            {
                int branchId = _branchContext.GetCurrentBranchId();
                
                var foods = await _foodRepository.GetFoodsByBranchAndCategoryAsync(branchId, categoryId);
                var foodDtos = foods.Select(f => new FoodDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    CategoryId = f.CategoryId,
                    ImageUrl = f.Image,
                    IsSetDish = f.IsSetDish,
                    IsAddOn = f.IsAddOn,
                    PriceForGuest = f.PriceForGuest,
                    PriceForPatient = f.PriceForPatient,
                    PriceForStaff = f.PriceForStaff,
                    Sort = f.Sort
                }).OrderBy(f => f.Sort).ToList();
                
                return Ok(foodDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 