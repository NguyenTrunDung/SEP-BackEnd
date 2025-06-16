using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface ISystemLogRepository:IRepository<SystemLog, int>
    {

        Task<IEnumerable<SystemLog>> GetSystemLogAll(int branchId, DateTime dateStart, DateTime dateEnd);


   


    }
}
