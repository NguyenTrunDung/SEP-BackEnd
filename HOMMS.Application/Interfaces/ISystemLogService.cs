using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface ISystemLogService
    {

        Task<IEnumerable<SystemLogDto>> GetSystemLogAll(int branchId, DateTime dateStart, DateTime dateEnd);
        Task<AddSystemLogDto> AddSystemLog(AddSystemLogDto systemLogDto);

    }
}
