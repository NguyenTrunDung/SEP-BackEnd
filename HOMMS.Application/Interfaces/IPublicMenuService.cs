using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IPublicMenuService
    {


        Task<IEnumerable<MenuDto>> GetMenusByBranchAndDateAsync(int branchId, DateTime date);

        Task<Dictionary<BranchDto, IEnumerable<MenuDto>>> GetMenusForAllBranchesAsync(DateTime date);


        Task<MenuDto> GetMenuWithDetailsAsync(int menuId);

        Task<IEnumerable<MenuDto>> GetMenusWithDetailsByBranchAndDateAsync(int branchId, DateTime date);


        Task<IEnumerable<MenuDto>> GetMenusByBranchDateAndTimeOfDayAsync(int branchId, DateTime date, string timeOfDay);


        Task<Dictionary<FoodCategoryDto, IEnumerable<MenuDetailDto>>> GetMenuDetailsByCategoryAsync(int menuId);



    }
}
