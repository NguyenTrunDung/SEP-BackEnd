using AutoMapper;
using HOMMS.Application.BaseServices;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class PatientDiseaseCategoryService : BaseService, IPatientDiseaseCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<PatientDiseaseCategory, int> _repository; 

        public PatientDiseaseCategoryService(IBranchContext branchContext, IMapper mapper, IRepository<PatientDiseaseCategory, int> repository) : base(branchContext)
        {
            _mapper = mapper;
            _repository = repository; 
        }

        public async Task<PatientDiseaseCategoryDto> CreateAsync(CRUDPatientDiseaseCategoryDto dto)
        {
            var entity = _mapper.Map<PatientDiseaseCategory>(dto);
            var createdEntity = await _repository.AddAsync(entity);
            return _mapper.Map<PatientDiseaseCategoryDto>(createdEntity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Entity not found");
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(string Patientid, int DiseaseCategoryId)
        {
            return await _repository.AnyAsync(p => p.PatientId == Patientid&& p.DiseaseCategoryId ==DiseaseCategoryId);
          
        }

        public async Task<IEnumerable<PatientDiseaseCategoryDto>> GetAllAsync(int branchId)
        {
            var entities = await _repository.GetByAsync(e => e.BranchId == branchId);
            return _mapper.Map<IEnumerable<PatientDiseaseCategoryDto>>(entities);
           
        }

        public async Task<PatientDiseaseCategoryDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<PatientDiseaseCategoryDto>(entity);
        }

        public async Task<PatientDiseaseCategoryDto> UpdateAsync(int id, CRUDPatientDiseaseCategoryDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Entity not found");

            _mapper.Map(dto, entity);
            var updatedEntity = await _repository.UpdateAsync(entity);
            return _mapper.Map<PatientDiseaseCategoryDto>(updatedEntity);
        }
    }
}
