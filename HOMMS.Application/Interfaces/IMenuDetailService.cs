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

    }
}
