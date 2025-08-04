using Asp.Versioning;
using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static HOMMS.Application.Implementations.PatientService;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {

        private readonly IPatientService _patientService;
        private readonly IMapper _mapper;


        public PatientController(IPatientService patientService, IMapper mapper)
        {
            _patientService = patientService;
            _mapper = mapper;
        }

        [HttpGet("by-branch")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<List<PatientDto>>>> GetPatientsByBranch([FromQuery] int branchId)
        {
            try
            {
                var pa = await _patientService.GetPatientsByBranchAsync(branchId);
                if (!pa.Any()) return NotFound(new ApiResponseBase<List<PatientDto>>(null, "Patient not found", "error", 0));
                var ti = _mapper.Map<List<PatientDto>>(pa);
                var count = ti.Count;
                return Ok(new ApiResponseBase<List<PatientDto>>(ti, "Patient retrieved successfully", "success", count));
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }

        [HttpGet("active-by-branch")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<List<PatientDto>>>> GetActivePatientsByBranch([FromQuery] int branchId)
        {

            var pa = await _patientService.GetActivePatientsByBranchAsync(branchId);
            if (!pa.Any()) return NotFound(new ApiResponseBase<List<PatientDto>>(null, "Patient not found", "error", 0));
            var ti = _mapper.Map<List<PatientDto>>(pa);
            var count = ti.Count;
            return Ok(new ApiResponseBase<List<PatientDto>>(ti, "Patient retrieved successfully", "success", count));

        }

        [HttpGet]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<PatientDto>>> GetById([FromQuery] string id)
        {

            var pa = await _patientService.GetByIdAsync(id);
            if (pa == null) return NotFound(new ApiResponseBase<PatientDto>(null, "Patient not found", "error"));

            return Ok(new ApiResponseBase<PatientDto>(pa, "Patient retrieved successfully", "success"));

        }



        [HttpGet("by-medical-record-number")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<PatientDto>>> GetPatientByMedicalRecordNumber([FromQuery] int branchId, [FromQuery] string medicalRecordNumber)
        {

            var pa = await _patientService.GetPatientByMedicalRecordNumberAsync(branchId, medicalRecordNumber);
            if (pa == null) return NotFound(new ApiResponseBase<PatientDto>(null, "Patient not found", "error"));

            return Ok(new ApiResponseBase<PatientDto>(pa, "Patient retrieved successfully", "success"));

        }


        [HttpGet("with-disease-categories")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<PatientDto>>> GetPatientWithDiseaseCategories([FromQuery] string patientId)
        {

            var pa = await _patientService.GetPatientWithDiseaseCategoriesAsync(patientId);
            if (pa == null) return NotFound(new ApiResponseBase<PatientDto>(null, "Patient not found", "error"));

            return Ok(new ApiResponseBase<PatientDto>(pa, "Patient retrieved successfully", "success"));

        }


        [HttpGet("by-physician")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<List<PatientDto>>>> GetPatientsByPhysician([FromQuery] int branchId, [FromQuery] string physicianName)
        {
            var pa = await _patientService.GetPatientsByPhysicianAsync(branchId, physicianName);
            if (!pa.Any()) return NotFound(new ApiResponseBase<List<PatientDto>>(null, "Patient not found", "error", 0));
            var ti = _mapper.Map<List<PatientDto>>(pa);
            var count = ti.Count;
            return Ok(new ApiResponseBase<List<PatientDto>>(ti, "Patient retrieved successfully", "success", count));


        }

        [HttpGet("by-external-id")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<PatientDto>>> GetPatientByExternalId([FromQuery] string externalSystemId)
        {
            var pa = await _patientService.GetPatientByExternalIdAsync(externalSystemId);

            if (pa == null) return NotFound(new ApiResponseBase<PatientDto>(null, "Patient not found", "error"));

            return Ok(new ApiResponseBase<PatientDto>(pa, "Patient retrieved successfully", "success"));

        }



        [HttpGet("by-room")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<List<PatientDto>>>> GetPatientsByRoom([FromQuery] int branchId, [FromQuery] string roomNumber)
        {
            var pa = await _patientService.GetPatientsByRoomAsync(branchId, roomNumber);
            if (!pa.Any()) return NotFound(new ApiResponseBase<List<PatientDto>>(null, "Patient not found", "error", 0));
            var ti = _mapper.Map<List<PatientDto>>(pa);
            var count = ti.Count;
            return Ok(new ApiResponseBase<List<PatientDto>>(ti, "Patient retrieved successfully", "success", count));


        }



        [HttpGet("needing-sync")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<List<PatientDto>>>> GetPatientsNeedingSync([FromQuery] int branchId, [FromQuery] DateTime lastSyncThreshold)
        {
            var pa = await _patientService.GetPatientsNeedingSyncAsync(branchId, lastSyncThreshold);
            if (!pa.Any()) return NotFound(new ApiResponseBase<List<PatientDto>>(null, "Patient not found", "error", 0));
            var ti = _mapper.Map<List<PatientDto>>(pa);
            var count = ti.Count;
            return Ok(new ApiResponseBase<List<PatientDto>>(ti, "Patient retrieved successfully", "success", count));


        }


        [HttpGet("dietary-supervision")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<List<PatientDto>>>> GetPatientsRequiringDietarySupervision([FromQuery] int branchId)
        {
            var pa = await _patientService.GetPatientsRequiringDietarySupervisionAsync(branchId);
            if (!pa.Any()) return NotFound(new ApiResponseBase<List<PatientDto>>(null, "Patient not found", "error", 0));
            var ti = _mapper.Map<List<PatientDto>>(pa);
            var count = ti.Count;
            return Ok(new ApiResponseBase<List<PatientDto>>(ti, "Patient retrieved successfully", "success", count));


        }





        [HttpGet("with-disease-categories-by-branch")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<List<PatientDto>>>> GetPatientsWithDiseaseCategoriesByBranch([FromQuery] int branchId)
        {
            var pa = await _patientService.GetPatientsWithDiseaseCategoriesByBranchAsync(branchId);
            if (!pa.Any()) return NotFound(new ApiResponseBase<List<PatientDto>>(null, "Patient not found", "error", 0));
            var ti = _mapper.Map<List<PatientDto>>(pa);
            var count = ti.Count;
            return Ok(new ApiResponseBase<List<PatientDto>>(ti, "Patient retrieved successfully", "success", count));


        }



        [HttpGet("recently-discharged")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<List<PatientDto>>>> GetRecentlyDischargedPatients([FromQuery] int branchId, [FromQuery] DateTime fromDate)
        {
            var pa = await _patientService.GetRecentlyDischargedPatientsAsync(branchId, fromDate);
            if (!pa.Any()) return NotFound(new ApiResponseBase<List<PatientDto>>(null, "Patient not found", "error", 0));
            var ti = _mapper.Map<List<PatientDto>>(pa);
            var count = ti.Count;
            return Ok(new ApiResponseBase<List<PatientDto>>(ti, "Patient retrieved successfully", "success", count));


        }


        [HttpGet("search")]
        //[Authorize(Policy = "Permission:Patient:view")]
        public async Task<ActionResult<ApiResponseBase<List<PatientDto>>>> SearchPatients([FromQuery] int branchId, [FromQuery] string searchTerm)
        {
            var pa = await _patientService.SearchPatientsAsync(branchId, searchTerm);
            if (!pa.Any()) return NotFound(new ApiResponseBase<List<PatientDto>>(null, "Patient not found", "error", 0));
            var ti = _mapper.Map<List<PatientDto>>(pa);
            var count = ti.Count;
            return Ok(new ApiResponseBase<List<PatientDto>>(ti, "Patient retrieved successfully", "success", count));


        }


        [HttpGet("bulk-update")]
        //[Authorize(Policy = "Permission:Patient:edit")]
        public async Task<ActionResult<int>> BulkUpdateLastSync([FromQuery] string patientIds, [FromQuery] DateTime syncTime)
        {
            try
            {
                var idList = patientIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => id.Trim());

                var pa = await _patientService.BulkUpdateLastSyncAsync(idList, syncTime);

                if (pa == 0) return NotFound("Patient not found");

                return Ok(new ApiResponseBase<PatientDto>(null, "Patient update Last-Sync successfully", "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }



        [HttpPost]
        //[Authorize(Policy = "Permission:Patient:add")]
        public async Task<ActionResult<ApiResponseBase<PatientDto>>> CreatePatient([FromBody] CreatePatientDto entity)
        {
            try
            {
                var pa = await _patientService.AddAsync(entity);

                return CreatedAtAction(nameof(GetById), new { id = pa.Id }, new ApiResponseBase<PatientDto>(pa, "Patient created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.InnerException?.Message });
            }
        }


        [HttpPut("{id}")]
        //[Authorize(Policy = "Permission:Patient:edit")]
        public async Task<ActionResult<ApiResponseBase<PatientDto>>> UpdatePatient(string id, [FromBody] UpdatePatientDto entity)
        {
            try
            {
                var pa = await _patientService.UpdateAsync(id, entity);

                if (pa == null) return NotFound(new ApiResponseBase<PatientDto>(null, "Patient not found", "error"));
                return Ok(new ApiResponseBase<PatientDto>(pa, "Patient updated successfully"));
            }
            catch (DuplicateRecordException ex)
            {
                return BadRequest(new { status = "error", message = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }


        [HttpDelete("{id}")]
        //[Authorize(Policy = "Permission:Patient:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeletePatient(string id)
        {
            try
            {
                var pa = await _patientService.DeleteAsync(id);

                if (pa == null) return NotFound(new ApiResponseBase<PatientDto>(null, "Patient not found", "error"));
                return Ok(new ApiResponseBase<object>(null, "Patient deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.InnerException?.Message });
            }
        }




    }
}
