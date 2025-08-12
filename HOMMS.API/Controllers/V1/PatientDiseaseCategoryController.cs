using Asp.Versioning;
using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;

namespace HOMMS.API.Controllers.V1
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]


    public class PatientDiseaseCategoryController : ControllerBase
    {
        private readonly IPatientDiseaseCategoryService _patientDiseaseCategoryService;
        private readonly IPatientService _patientService;
        private readonly IDiseaseCategoryService _diseaseCategoryService;
      

        public PatientDiseaseCategoryController(IPatientDiseaseCategoryService patientDiseaseCategoryService,IPatientService patientService, IDiseaseCategoryService diseaseCategoryService)
        {
            _patientDiseaseCategoryService = patientDiseaseCategoryService;
           
            _diseaseCategoryService = diseaseCategoryService;
            _patientService = patientService;

        }

        [HttpGet]

        public async Task<ActionResult<ApiResponseBase<IEnumerable<PatientDiseaseCategoryDto>>>> GetAllAsync([FromQuery] int branchId)
        {
            var pa = await _patientDiseaseCategoryService.GetAllAsync(branchId);
            var totalCount = pa is ICollection<PatientDiseaseCategoryDto> col ? col.Count : (pa?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<PatientDiseaseCategoryDto>>(pa, "Patient-Disease-Category retrieved successfully", "success", totalCount));
        }



        [HttpGet("{id}")]

        public async Task<ActionResult<ApiResponseBase<PatientDiseaseCategoryDto> >>GetByIdAsync([FromQuery] int id)
        {

            var pa = await _patientDiseaseCategoryService.GetByIdAsync(id);
            if (pa == null)return NotFound(new ApiResponseBase<PatientDiseaseCategoryDto>(null, "Patient-Disease-Category not found","error"));
            return Ok(new ApiResponseBase<PatientDiseaseCategoryDto>(pa, "Patient-Disease-Category retrieved successfully", "success"));


        }



        [HttpPost]
        public async Task<ActionResult<ApiResponseBase<PatientDiseaseCategoryDto>>> CreateAsync([FromBody] CRUDPatientDiseaseCategoryDto dto)
        {
            try
            {

                var pat = await _patientService.GetByIdAsync(dto.PatientId);
                if (pat == null) return NotFound(new ApiResponseBase<PatientDto>(null, "Patient not found", "error"));
                if (pat.BranchId != dto.BranchId) return BadRequest(new { status = "error", message ="Patient are not in the same branch" });

                var category = await _diseaseCategoryService.GetByIdAsync(dto.DiseaseCategoryId);
                if (category == null) return NotFound(new ApiResponseBase<DiseaseCategoryDto>(null, "Disease category not found", "error"));
                if (category.BranchId != dto.BranchId) return BadRequest(new { status = "error", message = "DiseaseCategory are not in the same branch" });


                var existed = await _patientDiseaseCategoryService.ExistsAsync(dto.PatientId, dto.DiseaseCategoryId);
                if (existed)
                    return Conflict(new ApiResponseBase<object>(null, "Patient already has this disease category", "error"));



                var pa = await _patientDiseaseCategoryService.CreateAsync(dto);

                return Ok( new ApiResponseBase<PatientDiseaseCategoryDto>(pa, "Patient-Disease-Category created successfully", "success"));


            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });


            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponseBase<PatientDiseaseCategoryDto>>> UpdateAsync([FromQuery] int id, [FromBody] CRUDPatientDiseaseCategoryDto dto)
        {
            try
            {

                var pat = await _patientService.GetByIdAsync(dto.PatientId);
                if (pat == null) return NotFound(new ApiResponseBase<PatientDto>(null, "Patient not found", "error"));
                if (pat.BranchId != dto.BranchId) return BadRequest(new { status = "error", message = "Patient are not in the same branch" });

                var category = await _diseaseCategoryService.GetByIdAsync(dto.DiseaseCategoryId);
                if (category == null) return NotFound(new ApiResponseBase<DiseaseCategoryDto>(null, "Disease category not found", "error"));
                if (category.BranchId != dto.BranchId) return BadRequest(new { status = "error", message = "DiseaseCategory are not in the same branch" });


                var existed = await _patientDiseaseCategoryService.ExistsAsync(dto.PatientId, dto.DiseaseCategoryId);
                if (existed)
                    return Conflict(new ApiResponseBase<object>(null, "Patient already has this disease category", "error"));


                var pa = await _patientDiseaseCategoryService.UpdateAsync(id, dto);
                if (pa == null) return NotFound(new ApiResponseBase<PatientDiseaseCategoryDto>(null, "Patient-Disease-Category not found", "error"));
                return Ok(new ApiResponseBase<PatientDiseaseCategoryDto>(pa, "Patient-Disease-Category update successfully", "success"));



            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.InnerException?.Message });



            }
        }

            [HttpDelete]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteAsync([FromQuery] int id)
        {
            var pa = await _patientDiseaseCategoryService.DeleteAsync(id);
            if (!pa) return NotFound(new ApiResponseBase<object>(null, "Patient-Disease-Category not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "Patient-Disease-Category deleted successfully", "success"));
        }



    }
}
