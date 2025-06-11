using HOMMS.Application.BaseServices;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using HOMMS.Infrastructure.Services;
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

        public MenuDetailService(IMenuDetailRepository menuRepository, IBranchContext branchContext)
            : base(branchContext)
        {
            _menuRepository = menuRepository;
        }

        public async Task<MenuDetailViewDto?> GetMenuWithDetailsAsync(int menuId)
        {
            var menu = await _menuRepository.GetMenuWithDetailsAsync(menuId);
            if (menu == null) return null;

            return new MenuDetailViewDto
            {
                Id = menu.Id,
                Date = menu.Date,
                TimeOfDay = menu.TimeOfDay,
                IsTime = menu.IsTime,
                TimeFrom = menu.TimeFrom,
                TimeTo = menu.TimeTo,
                Name = menu.Name,
                Details = menu.MenuDetails.Select(md => new MenuDetailsDto
                {
                    Id = md.Id,
                    FoodId = md.FoodId,
                    FoodName = md.Food?.Name,
                    Qty = md.Qty,
                    PriceForGuest = md.PriceForGuest,
                    PriceForPatient = md.PriceForPatient,
                    PriceForStaff = md.PriceForStaff,
                    DiscountPrice = md.DiscountPrice,
                    Status = md.Status,
                    DiscountFrom = md.DiscountFrom,
                    DiscountTo = md.DiscountTo,
                    IsQty = md.IsQty
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

    }
}
