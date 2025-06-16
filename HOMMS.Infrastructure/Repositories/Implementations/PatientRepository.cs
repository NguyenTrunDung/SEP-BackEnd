using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class PatientRepository : Repository<Patient, string>, IPatientRepository
    {

        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context) : base(context)
        {

            _context = context;
        }


        public async Task<IEnumerable<Patient>> GetPatientsByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(p => p.BranchId == branchId)
                .OrderByDescending(p => p.AdmissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetActivePatientsByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(p => p.BranchId == branchId && p.IsActive)
                .OrderByDescending(p => p.AdmissionDate)
                .ToListAsync();

        }


        public async Task<Patient?> GetPatientByExternalIdAsync(string externalSystemId)
        {
            return await DbSet
              .Where(p => p.ExternalSystemId == externalSystemId)
              .FirstOrDefaultAsync();
        }

        public async Task<Patient?> GetPatientByMedicalRecordNumberAsync(int branchId, string medicalRecordNumber)
        {
            return await DbSet
                .Where(p => p.BranchId == branchId && p.MedicalRecordNumber == medicalRecordNumber)
                .FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Patient>> GetPatientsByPhysicianAsync(int branchId, string physicianName)
        {
            return await DbSet
               .Where(p => p.BranchId == branchId && p.AttendingPhysician == physicianName)
               .OrderByDescending(p => p.IsActive)
               .ThenByDescending(p => p.AdmissionDate)
               .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetPatientsByRoomAsync(int branchId, string roomNumber)
        {
            return await DbSet
                .Where(p => p.BranchId == branchId && p.RoomNumber == roomNumber)
                .OrderByDescending(p => p.IsActive)
                .ThenByDescending(p => p.RoomNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetPatientsNeedingSyncAsync(int branchId, DateTime lastSyncThreshold)
        {
            return await DbSet
                .Where(p => p.BranchId == branchId && p.LastSyncAt.Value.Date == lastSyncThreshold.Date)
                .OrderByDescending(p => p.IsActive)
                .ThenByDescending(p => p.LastSyncAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetPatientsRequiringDietarySupervisionAsync(int branchId)
        {
            return await DbSet
               .Where(p => p.BranchId == branchId && p.RequiresDietarySupervision)
               .OrderByDescending(p => p.IsActive)
               .ThenByDescending(p => p.AdmissionDate)
               .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetPatientsWithDiseaseCategoriesByBranchAsync(int branchId)
        {
            return await DbSet
               .Include(c => c.PatientDiseaseCategories)
               .ThenInclude(c => c.DiseaseCategory)
               .Where(p => p.BranchId == branchId)
               .OrderByDescending(p => p.IsActive)
               .ThenByDescending(p => p.AdmissionDate)
               .ToListAsync();
        }

        public async Task<Patient?> GetPatientWithDiseaseCategoriesAsync(string patientId)
        {
            return await DbSet
               .Include(c => c.PatientDiseaseCategories)
               .ThenInclude(c => c.DiseaseCategory)
               .Where(c => c.Id == patientId)
               .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Patient>> GetRecentlyDischargedPatientsAsync(int branchId, DateTime fromDate)
        {
            return await DbSet
                .Where(p => p.BranchId == branchId && p.DischargeDate.Value.Date == fromDate.Date)
                .OrderByDescending(p => p.DischargeDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(int branchId, string searchTerm)
        {
            return await DbSet
                .Where(p => p.BranchId == branchId && (p.FullName.Contains(searchTerm) || p.Id.Contains(searchTerm) || p.MedicalRecordNumber.Contains(searchTerm)))
                .ToListAsync();
        }


        public async Task<int> BulkUpdateLastSyncAsync(IEnumerable<string> patientIds, DateTime syncTime)
        {
            var patients = await DbSet
                        .Where(p => patientIds.Contains(p.Id))
                        .ToListAsync();

            foreach (var patient in patients)
            {
                patient.LastSyncAt = syncTime;
            }

            return await _context.SaveChangesAsync();
        }

         

    }
}
