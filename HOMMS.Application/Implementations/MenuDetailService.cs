using AutoMapper;
using HOMMS.Application.BaseServices;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using HOMMS.Infrastructure.Services;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class MenuDetailService : BaseService, IMenuDetailService
    {
        private readonly IMenuDetailRepository _menuRepository;
        private readonly IMapper _mapper;

        public MenuDetailService(IMenuDetailRepository menuRepository, IBranchContext branchContext, IMapper mapper)
            : base(branchContext)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public async Task<UpdateMenuDto> GetByIdAsync(int id)
        {
            var detail = await _menuRepository.GetByIdAsync(id);
            return _mapper.Map<UpdateMenuDto>(detail);
        }
        public async Task<MenuDetailViewDto?> GetMenuWithDetailsAsync(int menuId)
        {
            var menu = await _menuRepository.GetMenuWithDetailsAsync(menuId);
            if (menu == null) return null;

            // Use AutoMapper for consistent mapping
            // return _mapper.Map<MenuDetailViewDto>(menu);

            // Manual mapping (reverted for testing)
            return new MenuDetailViewDto
            {
                Id = menu.Id,
                IsTime = menu.IsTime,
                TimeFrom = menu.TimeFrom,
                TimeTo = menu.TimeTo,
                Date = menu.Date,
                BranchId = menu.BranchId,
                CreatedAt = menu.CreatedAt,
                UpdatedAt = menu.LastModifiedAt,
                CreatedBy = menu.CreatedBy,
                UpdatedBy = menu.LastModifiedBy,
                TimeOfDay = menu.TimeOfDay,
                Name = menu.Name,
                Details = menu.MenuDetails.Select(md => new MenuDetailsDto
                {
                    Id = md.Id,
                    MenuId = md.MenuId,
                    FoodId = md.FoodId,
                    FoodName = md.Food?.Name,
                    IsQty = md.IsQty,
                    Qty = md.Qty,
                    Sold = md.Sold,
                    PriceForGuest = md.PriceForGuest,
                    PriceForPatient = md.PriceForPatient,
                    PriceForStaff = md.PriceForStaff,
                    DiscountPrice = md.DiscountPrice,
                    Status = md.Status,
                    DiscountFrom = md.DiscountFrom,
                    DiscountTo = md.DiscountTo,
                    CreatedBy = md.CreatedBy,
                    UpdatedBy = md.LastModifiedBy,
                    CreatedAt = md.CreatedAt,
                    UpdatedAt = md.LastModifiedAt,
                    Food = md.Food == null ? null : new FoodDto
                    {
                        Id = md.Food.Id,
                        Name = md.Food.Name,
                        BranchId = md.Food.BranchId,
                        CategoryId = md.Food.CategoryId,
                        Description = md.Food.Description,
                        IsSetDish = md.Food.IsSetDish,
                        IsAddOn = md.Food.IsAddOn,
                        ForPatient = md.Food.ForPatient,
                        PriceForGuest = md.Food.PriceForGuest,
                        PriceForPatient = md.Food.PriceForPatient,
                        PriceForStaff = md.Food.PriceForStaff,
                        DiseaseCategoryId = md.Food.DiseaseCategoryId,
                        Sort = md.Food.Sort,
                        CreatedAt = md.Food.CreatedAt,
                        UpdatedAt = md.Food.LastModifiedAt,
                        CreatedBy = md.Food.CreatedBy,
                        UpdatedBy = md.Food.LastModifiedBy,
                        Image = md.Food.Image,
                        SetDishDetails = null
                    }
                }).ToList()
            };
        }

        public async Task<bool> UpdateMenuWithDetailsAsync(UpdateMenuDto dto)
        {

            var menu = await _menuRepository.GetMenuWithDetailsAsync(dto.Id);
            if (menu == null) return false;
            dto.BranchId = EnsureBranchId(dto.BranchId);

            // Update menu
            menu.Date = dto.Date;
            menu.TimeOfDay = dto.TimeOfDay;
            menu.IsTime = dto.IsTime;
            menu.TimeFrom = dto.TimeFrom;
            menu.TimeTo = dto.TimeTo;
            menu.Name = dto.Name;

            // Update details
            var detailIds = dto.Details.Where(d => d.Id.HasValue).Select(d => d.Id.Value).ToList();
            var toRemove = menu.MenuDetails.Where(md => !detailIds.Contains(md.Id)).ToList();
            foreach (var rem in toRemove)
                menu.MenuDetails.Remove(rem);

            foreach (var d in dto.Details)
            {
                var existing = d.Id.HasValue
                    ? menu.MenuDetails.FirstOrDefault(x => x.Id == d.Id.Value)
                    : null;

                if (existing != null)
                {
                    existing.FoodId = d.FoodId;
                    existing.Qty = d.Qty;
                    existing.PriceForGuest = d.PriceForGuest;
                    existing.PriceForPatient = d.PriceForPatient;
                    existing.PriceForStaff = d.PriceForStaff;
                    existing.DiscountPrice = d.DiscountPrice;
                    existing.Status = d.Status;
                    existing.DiscountFrom = d.DiscountFrom;
                    existing.DiscountTo = d.DiscountTo;
                    existing.IsQty = d.IsQty;
                }
                else
                {
                    menu.MenuDetails.Add(new MenuDetail
                    {
                        FoodId = d.FoodId,
                        Qty = d.Qty,
                        PriceForGuest = d.PriceForGuest,
                        PriceForPatient = d.PriceForPatient,
                        PriceForStaff = d.PriceForStaff,
                        DiscountPrice = d.DiscountPrice,
                        Status = d.Status,
                        DiscountFrom = d.DiscountFrom,
                        DiscountTo = d.DiscountTo,
                        IsQty = d.IsQty,
                    });
                }
            }

            return await _menuRepository.UpdateMenuWithDetailsAsync(menu);
        }
        public async Task<bool> AddMenuWithDetailsAsync(CreateMenuDto dto)
        {
            dto.BranchId = EnsureBranchId(dto.BranchId);

            var menu = new Menu
            {

                Date = dto.Date,
                TimeOfDay = dto.TimeOfDay,
                IsTime = dto.IsTime,
                TimeFrom = dto.TimeFrom,
                TimeTo = dto.TimeTo,
                Name = dto.Name,
               // BranchId = dto.BranchId , // Assuming BranchId is part of CreateMenuDto
                MenuDetails = dto.Details.Select(d => new MenuDetail
                {
                    FoodId = d.FoodId,
                    Qty = d.Qty,
                    PriceForGuest = d.PriceForGuest,
                    PriceForPatient = d.PriceForPatient,
                    PriceForStaff = d.PriceForStaff,
                    DiscountPrice = d.DiscountPrice,
                    Status = d.Status,
                    DiscountFrom = d.DiscountFrom,
                    DiscountTo = d.DiscountTo,
                    IsQty = d.IsQty
                }).ToList()
            };

            return await _menuRepository.AddMenuWithDetailsAsync(menu);
        }

        public async Task<List<MenuDetailViewDto>> GetAllMenusWithDetailsAsync()
        {
            var branchId = EnsureBranchId(0);
            var menus = await _menuRepository.GetAllMenusWithDetailsAsync(branchId);

            // Manual mapping (reverted for testing)
            return menus.Select(menu => new MenuDetailViewDto
            {
                Id = menu.Id,
                IsTime = menu.IsTime,
                TimeFrom = menu.TimeFrom,
                TimeTo = menu.TimeTo,
                Date = menu.Date,
                BranchId = menu.BranchId,
                CreatedAt = menu.CreatedAt,
                UpdatedAt = menu.LastModifiedAt,
                CreatedBy = menu.CreatedBy,
                UpdatedBy = menu.LastModifiedBy,
                TimeOfDay = menu.TimeOfDay,
                Name = menu.Name,
                Details = menu.MenuDetails.Select(md => new MenuDetailsDto
                {
                    Id = md.Id,
                    MenuId = md.MenuId,
                    FoodId = md.FoodId,
                    FoodName = md.Food?.Name,
                    IsQty = md.IsQty,
                    Qty = md.Qty,
                    Sold = md.Sold,
                    PriceForGuest = md.PriceForGuest,
                    PriceForPatient = md.PriceForPatient,
                    PriceForStaff = md.PriceForStaff,
                    DiscountPrice = md.DiscountPrice,
                    Status = md.Status,
                    DiscountFrom = md.DiscountFrom,
                    DiscountTo = md.DiscountTo,
                    CreatedBy = md.CreatedBy,
                    UpdatedBy = md.LastModifiedBy,
                    CreatedAt = md.CreatedAt,
                    UpdatedAt = md.LastModifiedAt,
                    Food = md.Food == null ? null : new FoodDto
                    {
                        Id = md.Food.Id,
                        Name = md.Food.Name,
                        BranchId = md.Food.BranchId,
                        CategoryId = md.Food.CategoryId,
                        Description = md.Food.Description,
                        IsSetDish = md.Food.IsSetDish,
                        IsAddOn = md.Food.IsAddOn,
                        ForPatient = md.Food.ForPatient,
                        PriceForGuest = md.Food.PriceForGuest,
                        PriceForPatient = md.Food.PriceForPatient,
                        PriceForStaff = md.Food.PriceForStaff,
                        DiseaseCategoryId = md.Food.DiseaseCategoryId,
                        Sort = md.Food.Sort,
                        CreatedAt = md.Food.CreatedAt,
                        UpdatedAt = md.Food.LastModifiedAt,
                        CreatedBy = md.Food.CreatedBy,
                        UpdatedBy = md.Food.LastModifiedBy,
                        Image = md.Food.Image,
                        SetDishDetails = null
                    }
                }).ToList()
            }).ToList();
        }
    }
}
