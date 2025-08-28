using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HOMMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HOMMS.Domain.Entities;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class DiseaseCategoryFoodRestrictionsController : ControllerBase
    {
        private readonly IDiseaseCategoryFoodRestrictionService _foodRestrictionService;
        private readonly IBranchContext _branchContext;
        private readonly ApplicationDbContext _context;

        public DiseaseCategoryFoodRestrictionsController(
            IDiseaseCategoryFoodRestrictionService foodRestrictionService,
            IBranchContext branchContext,
            ApplicationDbContext context)
        {
            _foodRestrictionService = foodRestrictionService;
            _branchContext = branchContext;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<DiseaseCategoryFoodRestrictionDto>>>> GetFoodRestrictions([FromQuery] int branchId)
        {
            var restrictions = await _foodRestrictionService.GetByBranchIdAsync(branchId);
            var totalCount = restrictions is ICollection<DiseaseCategoryFoodRestrictionDto> col ? col.Count : (restrictions?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<DiseaseCategoryFoodRestrictionDto>>(restrictions, "Food restrictions retrieved successfully", "success", totalCount));
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<DiseaseCategoryFoodRestrictionDto>>>> GetActiveFoodRestrictions([FromQuery] int branchId)
        {
            var restrictions = await _foodRestrictionService.GetActiveByBranchIdAsync(branchId);
            var totalCount = restrictions is ICollection<DiseaseCategoryFoodRestrictionDto> col ? col.Count : (restrictions?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<DiseaseCategoryFoodRestrictionDto>>(restrictions, "Active food restrictions retrieved successfully", "success", totalCount));
        }

        [HttpGet("patient/{patientId}/allowed-foods")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<FoodDto>>>> GetAllowedFoodsForPatient(string patientId, [FromQuery] int branchId, [FromQuery] int? diseaseCategoryId = null)
        {
            var allowedFoods = await _foodRestrictionService.GetAllowedFoodsForPatientAsync(patientId, branchId, diseaseCategoryId);
            var totalCount = allowedFoods is ICollection<FoodDto> col ? col.Count : (allowedFoods?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<FoodDto>>(allowedFoods, "Allowed foods for patient retrieved successfully", "success", totalCount));
        }

        //[HttpGet("debug/restrictions")]
        //public async Task<ActionResult<ApiResponseBase<object>>> GetDebugRestrictions([FromQuery] int branchId)
        //{
        //    try
        //    {
        //        var allRestrictions = await _foodRestrictionService.GetAllAsync();
        //        var branchRestrictions = allRestrictions.Where(r => r.BranchId == branchId).ToList();
                
        //        var debugInfo = new
        //        {
        //            TotalRestrictions = allRestrictions.Count(),
        //            BranchRestrictions = branchRestrictions.Count(),
        //            Restrictions = branchRestrictions.Select(r => new
        //            {
        //                Id = r.Id,
        //                DiseaseCategoryId = r.DiseaseCategoryId,
        //                DiseaseCategoryName = r.DiseaseCategory?.Name,
        //                FoodId = r.FoodId,
        //                FoodName = r.Food?.Name,
        //                RestrictionLevel = r.RestrictionLevel,
        //                IsActive = r.IsActive,
        //                IsDeleted = r.IsDeleted
        //            }).ToList()
        //        };

        //        return Ok(new ApiResponseBase<object>(debugInfo, "Debug restrictions retrieved successfully", "success"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
        //    }
        //}

        // [HttpGet("debug/patient/{patientId}/disease-categories")]
        // public async Task<ActionResult<ApiResponseBase<object>>> GetDebugPatientDiseaseCategories(string patientId)
        // {
        //     try
        //     {
        //         // Get patient disease categories directly from context
        //         var patientDiseaseCategories = await _context.PatientDiseaseCategories
        //             .Where(pdc => pdc.PatientId == patientId)
        //             .Include(pdc => pdc.DiseaseCategory)
        //             .ToListAsync();

        //         var debugInfo = new
        //         {
        //             PatientId = patientId,
        //             TotalCategories = patientDiseaseCategories.Count,
        //             Categories = patientDiseaseCategories.Select(pdc => new
        //             {
        //                 Id = pdc.Id,
        //                 DiseaseCategoryId = pdc.DiseaseCategoryId,
        //                 DiseaseCategoryName = pdc.DiseaseCategory?.Name,
        //                 DiseaseCategoryCode = pdc.DiseaseCategory?.Code,
        //                 PatientSeverityLevel = pdc.PatientSeverityLevel,
        //                 IsActive = pdc.IsActive,
        //                 IsDeleted = pdc.IsDeleted,
        //                 DiagnosedDate = pdc.DiagnosedDate,
        //                 PatientSpecificNotes = pdc.PatientSpecificNotes
        //             }).ToList()
        //         };

        //         return Ok(new ApiResponseBase<object>(debugInfo, "Debug patient disease categories retrieved successfully", "success"));
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
        //     }
        // }

        [HttpGet("patient/{patientId}/validate-food/{foodId}")]
        public async Task<ActionResult<ApiResponseBase<FoodValidationResultDto>>> ValidateFoodForPatient(string patientId, int foodId, [FromQuery] int branchId)
        {
            var validationResult = await _foodRestrictionService.ValidateFoodForPatientAsync(patientId, foodId, branchId);
            return Ok(new ApiResponseBase<FoodValidationResultDto>(validationResult, "Food validation completed successfully"));
        }

        [HttpPost("patient/{patientId}/validate-foods")]
        public async Task<ActionResult<ApiResponseBase<IEnumerable<FoodValidationResultDto>>>> ValidateFoodsForPatient(string patientId, [FromBody] List<int> foodIds, [FromQuery] int branchId)
        {
            var validationResults = await _foodRestrictionService.ValidateFoodsForPatientAsync(patientId, foodIds, branchId);
            var totalCount = validationResults is ICollection<FoodValidationResultDto> col ? col.Count : (validationResults?.Count() ?? 0);
            return Ok(new ApiResponseBase<IEnumerable<FoodValidationResultDto>>(validationResults, "Food validation completed successfully", "success", totalCount));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseBase<DiseaseCategoryFoodRestrictionDto>>> CreateFoodRestriction([FromBody] CreateDiseaseCategoryFoodRestrictionDto dto)
        {
            try
            {
                var branchId = _branchContext.GetCurrentBranchId();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";

                var created = await _foodRestrictionService.CreateAsync(dto, branchId, userId);
                return CreatedAtAction(
                    nameof(GetFoodRestriction),
                    new { id = created.Id },
                    new ApiResponseBase<DiseaseCategoryFoodRestrictionDto>(created, "Food restriction created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<DiseaseCategoryFoodRestrictionDto>(null, ex.Message, "error"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseBase<DiseaseCategoryFoodRestrictionDto>>> GetFoodRestriction(int id)
        {
            var restriction = await _foodRestrictionService.GetByIdAsync(id);
            if (restriction == null)
                return NotFound(new ApiResponseBase<DiseaseCategoryFoodRestrictionDto>(null, "Food restriction not found", "error"));
            return Ok(new ApiResponseBase<DiseaseCategoryFoodRestrictionDto>(restriction, "Food restriction retrieved successfully"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseBase<DiseaseCategoryFoodRestrictionDto>>> UpdateFoodRestriction(int id, [FromBody] UpdateDiseaseCategoryFoodRestrictionDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                var updated = await _foodRestrictionService.UpdateAsync(id, dto, userId);

                if (updated == null)
                    return NotFound(new ApiResponseBase<DiseaseCategoryFoodRestrictionDto>(null, "Food restriction not found", "error"));

                return Ok(new ApiResponseBase<DiseaseCategoryFoodRestrictionDto>(updated, "Food restriction updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<DiseaseCategoryFoodRestrictionDto>(null, ex.Message, "error"));
            }
        }

        [HttpPost("seed")]
        public async Task<ActionResult<ApiResponseBase<object>>> SeedFoodRestrictions()
        {
            try
            {
                // Clear existing restrictions
                var existingRestrictions = await _context.DiseaseCategoryFoodRestrictions.ToListAsync();
                _context.DiseaseCategoryFoodRestrictions.RemoveRange(existingRestrictions);
                await _context.SaveChangesAsync();

                // Seed new restrictions
                await HOMMS.Infrastructure.Seeds.DiseaseCategoryFoodRestrictionSeedData.SeedDiseaseCategoryFoodRestrictionsAsync(HttpContext.RequestServices);

                return Ok(new ApiResponseBase<object>(null, "Food restrictions seeded successfully", "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
            }
        }

        // [HttpPost("seed-debug")]
        // public async Task<ActionResult<ApiResponseBase<object>>> SeedFoodRestrictionsDebug()
        // {
        //     try
        //     {
        //         // Get all foods and disease categories for debugging
        //         var foods = await _context.Foods.ToListAsync();
        //         var diseaseCategories = await _context.DiseaseCategories.ToListAsync();
                
        //         var debugInfo = new
        //         {
        //             Foods = foods.Select(f => new { f.Id, f.Name }).ToList(),
        //             DiseaseCategories = diseaseCategories.Select(dc => new { dc.Id, dc.Name, dc.Code }).ToList(),
        //             FoodCount = foods.Count,
        //             DiseaseCategoryCount = diseaseCategories.Count
        //         };

        //         return Ok(new ApiResponseBase<object>(debugInfo, "Debug info retrieved successfully", "success"));
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
        //     }
        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteFoodRestriction(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                var deleted = await _foodRestrictionService.DeleteAsync(id, userId);

                if (!deleted)
                    return NotFound(new ApiResponseBase<object>(null, "Food restriction not found", "error"));

                return Ok(new ApiResponseBase<object>(null, "Food restriction deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
            }
        }

        [HttpPost("create-nutritional-meal")]
        public async Task<ActionResult<ApiResponseBase<object>>> CreateNutritionalMeal([FromBody] CreateNutritionalMealDto dto)
        {
            try
            {
                var branchId = _branchContext.GetCurrentBranchId();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";

                // Create a new disease category food restriction directly
                var restriction = new DiseaseCategoryFoodRestriction
                {
                    BranchId = branchId,
                    DiseaseCategoryId = dto.DiseaseCategoryId,
                    FoodId = dto.FoodId, // No food entity needed, this is a nutritional meal restriction
                    RestrictionLevel = 3, // Default to "Prohibited" level
                    Reason = dto.Reason,
                    AlternativeRecommendations = dto.AlternativeRecommendations,
                    IsActive = dto.IsActive,
                    RequiresPhysicianOverride = dto.RequiresPhysicianOverride,
                    MealTime = dto.MealTime, // Comma-separated meal times
                    Name = dto.Name, // Store the nutritional meal name directly
                    Price = dto.Price, // Store the nutritional meal price directly
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = userId
                };

                _context.DiseaseCategoryFoodRestrictions.Add(restriction);
                await _context.SaveChangesAsync();

                // Return the created restriction with nutritional meal info
                var result = new
                {
                    code = restriction.Id.ToString(), // Use restriction ID as code
                    id = restriction.Id,
                    name = dto.Name,
                    price = dto.Price,
                    diseaseCategoryId = dto.DiseaseCategoryId,
                    mealTime = dto.MealTime,
                    reason = dto.Reason,
                    message = "Nutritional meal restriction created successfully"
                };

                return Ok(new ApiResponseBase<object>(result, "Nutritional meal restriction created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
            }
        }
    }
} 