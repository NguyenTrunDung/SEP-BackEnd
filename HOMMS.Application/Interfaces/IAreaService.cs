using HOMMS.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IAreaService
    {
        Task<IEnumerable<AreaDto>> GetAllAreasAsync();
        Task<bool> CreateAreaAsync(AreaDto areaDto);
        Task<bool> UpdateAreaAsync(int id, string newName);
        Task<bool> DeleteAreaAsync(int id);
    }
}
