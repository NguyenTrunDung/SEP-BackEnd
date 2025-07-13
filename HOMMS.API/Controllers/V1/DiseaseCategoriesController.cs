using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class DiseaseCategoriesController : ControllerBase
    {
        private readonly IDiseaseCategoryService _diseaseCategoryService;
        private readonly IBranchContext _branchContext;

        public DiseaseCategoriesController(
            IDiseaseCategoryService diseaseCategoryService,
            IBranchContext branchContext)
        {
            _diseaseCategoryService = diseaseCategoryService;
            _branchContext = branchContext;
        }

        /// <summary>
        /// Gets all disease categories for the specified branch
        /// </summary>
        /// <returns>List of disease categories</returns>
        [HttpGet]
       // [Authorize(Policy = "Permission:diseasecategories:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<DiseaseCategoryDto>>>> GetDiseaseCategories([FromQuery] int branchId)
        {
            var categories = await _diseaseCategoryService.GetByBranchIdAsync(branchId);
            var totalCount = categories is ICollection<DiseaseCategoryDto> col ? col.Count : (categories?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<DiseaseCategoryDto>>(categories, "Disease categories retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets active disease categories for the specified branch
        /// </summary>
        /// <returns>List of active disease categories</returns>
        [HttpGet("active")]
        //[Authorize(Policy = "Permission:diseasecategories:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<DiseaseCategoryDto>>>> GetActiveDiseaseCategories([FromQuery] int branchId)
        {
            var categories = await _diseaseCategoryService.GetActiveByBranchIdAsync(branchId);
            var totalCount = categories is ICollection<DiseaseCategoryDto> col ? col.Count : (categories?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<DiseaseCategoryDto>>(categories, "Active disease categories retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets disease categories with patient statistics
        /// </summary>
        /// <returns>List of disease categories with patient counts</returns>
        [HttpGet("with-patient-counts")]
        //[Authorize(Policy = "Permission:diseasecategories:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<DiseaseCategoryDto>>>> GetDiseaseCategoriesWithPatientCounts([FromQuery] int branchId)
        {
            var categories = await _diseaseCategoryService.GetWithPatientCountsAsync(branchId);
            var totalCount = categories is ICollection<DiseaseCategoryDto> col ? col.Count : (categories?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<DiseaseCategoryDto>>(categories, "Disease categories with patient counts retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets disease categories with food restriction statistics
        /// </summary>
        /// <returns>List of disease categories with food restriction counts</returns>
        [HttpGet("with-food-restriction-counts")]
        //[Authorize(Policy = "Permission:diseasecategories:view")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<DiseaseCategoryDto>>>> GetDiseaseCategoriesWithFoodRestrictionCounts([FromQuery] int branchId)
        {
            var categories = await _diseaseCategoryService.GetWithFoodRestrictionCountsAsync(branchId);
            var totalCount = categories is ICollection<DiseaseCategoryDto> col ? col.Count : (categories?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<DiseaseCategoryDto>>(categories, "Disease categories with food restriction counts retrieved successfully", "success", totalCount));
        }

        /// <summary>
        /// Gets a disease category by ID
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <returns>Disease category details</returns>
        [HttpGet("{id}")]
        //[Authorize(Policy = "Permission:diseasecategories:view")]
        public async Task<ActionResult<ApiResponseBase<DiseaseCategoryDto>>> GetDiseaseCategory(int id)
        {
            var category = await _diseaseCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponseBase<DiseaseCategoryDto>(null, "Disease category not found", "error"));
            return Ok(new ApiResponseBase<DiseaseCategoryDto>(category, "Disease category retrieved successfully"));
        }

        /// <summary>
        /// Gets a disease category by code
        /// </summary>
        /// <param name="code">Disease category code</param>
        /// <returns>Disease category details</returns>
        [HttpGet("by-code/{code}")]
        //[Authorize(Policy = "Permission:diseasecategories:view")]
        public async Task<ActionResult<ApiResponseBase<DiseaseCategoryDto>>> GetDiseaseCategoryByCode(string code, [FromQuery] int branchId)
        {
            var category = await _diseaseCategoryService.GetByCodeAndBranchAsync(code, branchId);
            if (category == null)
                return NotFound(new ApiResponseBase<DiseaseCategoryDto>(null, "Disease category not found", "error"));
            return Ok(new ApiResponseBase<DiseaseCategoryDto>(category, "Disease category retrieved successfully"));
        }

        /// <summary>
        /// Creates a new disease category
        /// </summary>
        /// <param name="dto">Disease category creation data</param>
        /// <returns>Created disease category</returns>
        [HttpPost]
        //[Authorize(Policy = "Permission:diseasecategories:add")]
        public async Task<ActionResult<ApiResponseBase<DiseaseCategoryDto>>> CreateDiseaseCategory([FromBody] CreateDiseaseCategoryDto dto)
        {
            try
            {
                var branchId = _branchContext.GetCurrentBranchId();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";

                var created = await _diseaseCategoryService.CreateAsync(dto, branchId, userId);
                return CreatedAtAction(
                    nameof(GetDiseaseCategory), 
                    new { id = created.Id }, 
                    new ApiResponseBase<DiseaseCategoryDto>(created, "Disease category created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<DiseaseCategoryDto>(null, ex.Message, "error"));
            }
        }

        /// <summary>
        /// Updates an existing disease category
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <param name="dto">Disease category update data</param>
        /// <returns>Updated disease category</returns>
        [HttpPut("{id}")]
        //[Authorize(Policy = "Permission:diseasecategories:edit")]
        public async Task<ActionResult<ApiResponseBase<DiseaseCategoryDto>>> UpdateDiseaseCategory(int id, [FromBody] UpdateDiseaseCategoryDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                var updated = await _diseaseCategoryService.UpdateAsync(id, dto, userId);
                
                if (updated == null)
                    return NotFound(new ApiResponseBase<DiseaseCategoryDto>(null, "Disease category not found", "error"));
                
                return Ok(new ApiResponseBase<DiseaseCategoryDto>(updated, "Disease category updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<DiseaseCategoryDto>(null, ex.Message, "error"));
            }
        }

        /// <summary>
        /// Deletes a disease category
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        //[Authorize(Policy = "Permission:diseasecategories:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteDiseaseCategory(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                var deleted = await _diseaseCategoryService.DeleteAsync(id, userId);
                
                if (!deleted)
                    return NotFound(new ApiResponseBase<object>(null, "Disease category not found", "error"));
                
                return Ok(new ApiResponseBase<object>(null, "Disease category deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
            }
        }

        /// <summary>
        /// Checks if a disease category code is available
        /// </summary>
        /// <param name="code">Disease category code to check</param>
        /// <param name="excludeId">ID to exclude from check (optional, for updates)</param>
        /// <returns>True if code is available</returns>
        [HttpGet("check-code-availability")]
        //[Authorize(Policy = "Permission:diseasecategories:view")]
        public async Task<ActionResult<ApiResponseBase<object>>> CheckCodeAvailability(
            [FromQuery] string code, 
            [FromQuery] int branchId,
            [FromQuery] int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return BadRequest(new ApiResponseBase<object>(null, "Code parameter is required", "error"));
                }

                var isAvailable = await _diseaseCategoryService.IsCodeAvailableAsync(code, branchId, excludeId);
                return Ok(new ApiResponseBase<object>(isAvailable, isAvailable ? "Code is available" : "Code already exists"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
            }
        }
    }
}
