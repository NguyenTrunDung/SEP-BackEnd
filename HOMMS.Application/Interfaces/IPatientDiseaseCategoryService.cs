using HOMMS.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IPatientDiseaseCategoryService
    {

        Task<IEnumerable<PatientDiseaseCategoryDto>> GetAllAsync(int branchId);
        Task<PatientDiseaseCategoryDto> GetByIdAsync(int id);
        Task<PatientDiseaseCategoryDto> CreateAsync(CRUDPatientDiseaseCategoryDto dto);
        Task<PatientDiseaseCategoryDto> UpdateAsync(int id, CRUDPatientDiseaseCategoryDto dto);
        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(string Patientid,int DiseaseCategoryId);
    }
}
