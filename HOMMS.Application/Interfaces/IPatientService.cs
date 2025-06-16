using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IPatientService
    {

       

        /// <summary>
        /// Adds a new patient to the system.
        /// </summary>
        /// <param name="entity">Patient data for creation</param>
        /// <returns>The created patient with assigned ID</returns>
        Task<PatientDto> AddAsync(CreatePatientDto entity);

        /// <summary>
        /// Updates information for an existing patient.
        /// </summary>
        /// <param name="entity">Updated patient data</param>
        /// <returns>The updated patient record</returns>
        Task<PatientDto> UpdateAsync(string id, UpdatePatientDto entity);

        /// <summary>
        /// Deletes a patient by their ID.
        /// </summary>
        /// <param name="id">The ID of the patient to delete</param>
        /// <returns>True if deletion was successful, otherwise false</returns>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// Retrieves a patient by their ID.
        /// </summary>
        /// <param name="id">The ID of the patient</param>
        /// <returns>Patient information if found</returns>
        Task<PatientDto> GetByIdAsync(string id);

        /// <summary>
        /// Gets patients for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Patients for the branch</returns>
        Task<IEnumerable<PatientDto>> GetPatientsByBranchAsync(int branchId);

        /// <summary>
        /// Gets active patients for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Active patients for the branch</returns>
        Task<IEnumerable<PatientDto>> GetActivePatientsByBranchAsync(int branchId);

        /// <summary>
        /// Gets patient by medical record number
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="medicalRecordNumber">Medical record number</param>
        /// <returns>Patient with the medical record number</returns>
        Task<PatientDto?> GetPatientByMedicalRecordNumberAsync(int branchId, string medicalRecordNumber);

        /// <summary>
        /// Gets patient with their disease categories
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <returns>Patient with disease categories</returns>
        Task<PatientDto?> GetPatientWithDiseaseCategoriesAsync(string patientId);

        /// <summary>
        /// Gets patients with their disease categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Patients with disease categories</returns>
        Task<IEnumerable<PatientDto>> GetPatientsWithDiseaseCategoriesByBranchAsync(int branchId);

        /// <summary>
        /// Gets patients by room number
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="roomNumber">Room number</param>
        /// <returns>Patients in the room</returns>
        Task<IEnumerable<PatientDto>> GetPatientsByRoomAsync(int branchId, string roomNumber);

        /// <summary>
        /// Gets patients by attending physician
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="physicianName">Physician name</param>
        /// <returns>Patients under the physician's care</returns>
        Task<IEnumerable<PatientDto>> GetPatientsByPhysicianAsync(int branchId, string physicianName);

        /// <summary>
        /// Gets patients by external system ID
        /// </summary>
        /// <param name="externalSystemId">External system ID</param>
        /// <returns>Patient with the external system ID</returns>
        Task<PatientDto?> GetPatientByExternalIdAsync(string externalSystemId);

        /// <summary>
        /// Gets patients that need synchronization (not synced recently)
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="lastSyncThreshold">Threshold for last sync date</param>
        /// <returns>Patients needing sync</returns>
        Task<IEnumerable<PatientDto>> GetPatientsNeedingSyncAsync(int branchId, DateTime lastSyncThreshold);

        /// <summary>
        /// Gets patients requiring dietary supervision
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Patients requiring dietary supervision</returns>
        Task<IEnumerable<PatientDto>> GetPatientsRequiringDietarySupervisionAsync(int branchId);

        /// <summary>
        /// Gets recently discharged patients
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="fromDate">Start date for discharge</param>
        /// <returns>Recently discharged patients</returns>
        Task<IEnumerable<PatientDto>> GetRecentlyDischargedPatientsAsync(int branchId, DateTime fromDate);

        /// <summary>
        /// Searches patients by name or medical record number
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Matching patients</returns>
        Task<IEnumerable<PatientDto>> SearchPatientsAsync(int branchId, string searchTerm);

        /// <summary>
        /// Bulk update last sync time for patients
        /// </summary>
        /// <param name="patientIds">Patient IDs to update</param>
        /// <param name="syncTime">Sync timestamp</param>
        /// <returns>Number of updated records</returns>
        Task<int> BulkUpdateLastSyncAsync(IEnumerable<string> patientIds, DateTime syncTime);

         



    }
}
