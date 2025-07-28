using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IDepartmentService
    {

        Task<IEnumerable<DepartmentDto>> GetDepartmentByBranchAsync(int branchId);


        Task<IEnumerable<DepartmentDto>> GetActiveDepartmentByBranchAsync(int branchId);



        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);

        
        Task<DepartmentDto?> UpdateAsync(int id, CreateDepartmentDto dto);

       
        Task<bool> DeleteAsync(int id);

        
        Task<bool> IsNameUniqueAsync(int branchId, string name, int? excludeId = null);

        Task<DepartmentDto?> GetByIdAsync(int id);
    }
}
