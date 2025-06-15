using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class PatientService : IPatientService
    {

        private readonly IPatientRepository _repository;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PatientDto> AddAsync(CreatePatientDto entity)
        {
            var pa = _mapper.Map<Patient>(entity);
            var ti = await _repository.AddAsync(pa);
            return _mapper.Map<PatientDto>(ti);
        }

        public async Task<int> BulkUpdateLastSyncAsync(IEnumerable<string> patientIds, DateTime syncTime)
        {
            var pa = await _repository.BulkUpdateLastSyncAsync(patientIds, syncTime);
            return pa;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var pa = await _repository.GetByIdAsync(id);
            if (pa == null) return false;
            await _repository.DeleteAsync(pa);
            return true;
        }

        public async Task<IEnumerable<PatientDto>> GetActivePatientsByBranchAsync(int branchId)
        {
            var pa = await _repository.GetActivePatientsByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<PatientDto>>(pa);
        }



        public async Task<PatientDto> GetByIdAsync(string id)
        {
            var pa = await _repository.GetByIdAsync(id);
            return _mapper.Map<PatientDto>(pa);
        }

        public async Task<PatientDto?> GetPatientByExternalIdAsync(string externalSystemId)
        {
            var pa = await _repository.GetPatientByExternalIdAsync(externalSystemId);
            return _mapper.Map<PatientDto>(pa);
        }

        public async Task<PatientDto?> GetPatientByMedicalRecordNumberAsync(int branchId, string medicalRecordNumber)
        {
            var pa = await _repository.GetPatientByMedicalRecordNumberAsync(branchId, medicalRecordNumber);
            return _mapper.Map<PatientDto>(pa);
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsByBranchAsync(int branchId)
        {
            var pa = await _repository.GetPatientsByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<PatientDto>>(pa);
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsByPhysicianAsync(int branchId, string physicianName)
        {
            var pa = await _repository.GetPatientsByPhysicianAsync(branchId, physicianName);
            return _mapper.Map<IEnumerable<PatientDto>>(pa);
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsByRoomAsync(int branchId, string roomNumber)
        {
            var pa = await _repository.GetPatientsByRoomAsync(branchId, roomNumber);
            return _mapper.Map<IEnumerable<PatientDto>>(pa);
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsNeedingSyncAsync(int branchId, DateTime lastSyncThreshold)
        {
            var pa = await _repository.GetPatientsNeedingSyncAsync(branchId, lastSyncThreshold);
            return _mapper.Map<IEnumerable<PatientDto>>(pa);
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsRequiringDietarySupervisionAsync(int branchId)
        {
            var pa = await _repository.GetPatientsRequiringDietarySupervisionAsync(branchId);
            return _mapper.Map<IEnumerable<PatientDto>>(pa);
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsWithDiseaseCategoriesByBranchAsync(int branchId)
        {
            var pa = await _repository.GetPatientsWithDiseaseCategoriesByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<PatientDto>>(pa);
        }

        public async Task<PatientDto?> GetPatientWithDiseaseCategoriesAsync(string patientId)
        {
            var pa = await _repository.GetPatientWithDiseaseCategoriesAsync(patientId);
            return _mapper.Map<PatientDto>(pa);
        }

        public async Task<IEnumerable<PatientDto>> GetRecentlyDischargedPatientsAsync(int branchId, DateTime fromDate)
        {
            var pa = await _repository.GetRecentlyDischargedPatientsAsync(branchId, fromDate);
            return _mapper.Map<IEnumerable<PatientDto>>(pa);
        }

        public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(int branchId, string searchTerm)
        {
            var pa = await _repository.SearchPatientsAsync(branchId, searchTerm);
            return _mapper.Map<IEnumerable<PatientDto>>(pa);
        }

        public async Task<PatientDto> UpdateAsync(string id, UpdatePatientDto entity)
        {
            var pa = await _repository.GetByIdAsync(id);
            if (pa == null) return null;
            if (!string.IsNullOrEmpty(entity.MedicalRecordNumber) && entity.MedicalRecordNumber != pa.MedicalRecordNumber)
            {
                var exists = await _repository.AnyAsync(p =>
                    p.BranchId == pa.BranchId &&
                    p.MedicalRecordNumber == entity.MedicalRecordNumber &&
                    p.Id != id);

                if (exists) throw new DuplicateRecordException("Mã hồ sơ bệnh án đã tồn tại trong chi nhánh.");
            }
            _mapper.Map(entity, pa);
            await _repository.UpdateAsync(pa);
            return _mapper.Map<PatientDto>(pa);
        }
        public class DuplicateRecordException : Exception
        {
            public DuplicateRecordException(string message) : base(message) { }
        }
    }
}

