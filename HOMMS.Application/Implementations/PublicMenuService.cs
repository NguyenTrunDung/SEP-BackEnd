using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class PublicMenuService : IPublicMenuService
    {

        private readonly IMapper _mapper;
        private readonly IMenuRepository _menuRepository;

        public PublicMenuService(IMapper mapper, IMenuRepository menuRepository)
        {
            _mapper = mapper;
            _menuRepository = menuRepository;
        }

        public async Task<Dictionary<FoodCategoryDto, IEnumerable<MenuDetailDto>>> GetMenuDetailsByCategoryAsync(int menuId)
        {
            var menus = await _menuRepository.GetMenuDetailsByCategoryAsync(menuId);
            return _mapper.Map<Dictionary<FoodCategoryDto, IEnumerable<MenuDetailDto>>>(menus);
        }

        public async Task<IEnumerable<MenuDto>> GetMenusByBranchAndDateAsync(int branchId, DateTime date)
        {
            var menus = await _menuRepository.GetMenusByBranchAndDateAsync(branchId, date);
            return _mapper.Map<IEnumerable<MenuDto>>(menus);
        }

        public async Task<IEnumerable<MenuDto>> GetMenusByBranchDateAndTimeOfDayAsync(int branchId, DateTime date, string timeOfDay)
        {
            var menus = await _menuRepository.GetMenusByBranchDateAndTimeOfDayAsync(branchId, date, timeOfDay);
            return _mapper.Map<IEnumerable<MenuDto>>(menus);
        }

        public async Task<Dictionary<BranchDto, IEnumerable<MenuDto>>> GetMenusForAllBranchesAsync(DateTime date)
        {
            var menus = await _menuRepository.GetMenusForAllBranchesAsync(date);
            return _mapper.Map<Dictionary<BranchDto, IEnumerable<MenuDto>>>(menus);
        }

        public async Task<IEnumerable<MenuDto>> GetMenusWithDetailsByBranchAndDateAsync(int branchId, DateTime date)
        {
            var menus = await _menuRepository.GetMenusWithDetailsByBranchAndDateAsync(branchId, date);
            return _mapper.Map<IEnumerable<MenuDto>>(menus);
        }

        public async Task<MenuDto> GetMenuWithDetailsAsync(int menuId)
        {
            var menus = await _menuRepository.GetMenuWithDetailsAsync(menuId);
            return _mapper.Map<MenuDto>(menus);
        }
    }
}
