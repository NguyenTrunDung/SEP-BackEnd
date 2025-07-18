using HOMMS.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IMenuDetailService
    {
        Task<MenuDetailViewDto?> GetMenuWithDetailsAsync(int menuId);
        Task<bool> UpdateMenuWithDetailsAsync(UpdateMenuDto dto);
        Task<bool> AddMenuWithDetailsAsync(CreateMenuDto dto);
        Task<List<MenuDetailViewDto>> GetAllMenusWithDetailsAsync();
        Task<UpdateMenuDto> GetByIdAsync(int id);
        
        // New methods for menu template functionality
        Task<List<MenuTemplateDto>> GetMenuTemplatesAsync();
        Task<MenuDetailViewDto?> CopyMenuAsTemplateAsync(int sourceMenuId, DateTime newDate, string? newName = null);

        //Delete
        Task<bool> DeleteAsync(int id);

    }
}
