using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        public Task<Patient> AddAsync(Patient entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<Patient, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> BulkUpdateLastSyncAsync(IEnumerable<string> patientIds, DateTime syncTime)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Patient entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetActivePatientsByBranchAsync(int branchId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetByAsync(Expression<Func<Patient, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Patient> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Patient?> GetPatientByExternalIdAsync(string externalSystemId)
        {
            throw new NotImplementedException();
        }

        public Task<Patient?> GetPatientByMedicalRecordNumberAsync(int branchId, string medicalRecordNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetPatientsByBranchAsync(int branchId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetPatientsByPhysicianAsync(int branchId, string physicianName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetPatientsByRoomAsync(int branchId, string roomNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetPatientsNeedingSyncAsync(int branchId, DateTime lastSyncThreshold)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetPatientsRequiringDietarySupervisionAsync(int branchId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetPatientsWithDiseaseCategoriesByBranchAsync(int branchId)
        {
            throw new NotImplementedException();
        }

        public Task<Patient?> GetPatientWithDiseaseCategoriesAsync(string patientId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetRecentlyDischargedPatientsAsync(int branchId, DateTime fromDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> SearchPatientsAsync(int branchId, string searchTerm)
        {
            throw new NotImplementedException();
        }

        public Task<Patient> UpdateAsync(Patient entity)
        {
            throw new NotImplementedException();
        }
    }
}
