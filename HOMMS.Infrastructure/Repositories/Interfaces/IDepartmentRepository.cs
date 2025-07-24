using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department, int>
    {

 
        Task<IEnumerable<Department>> GetDepartmentByBranchAsync(int branchId);

         
        Task<IEnumerable<Department>> GetActiveDepartmentByBranchAsync(int branchId);

     
 
        Task<bool> IsDepartmentNameUniqueAsync(int branchId, string name, int? excludeId = null);



    }
}
