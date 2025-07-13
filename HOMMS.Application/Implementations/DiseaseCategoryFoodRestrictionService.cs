using AutoMapper;
using HOMMS.Application.BaseServices;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using HOMMS.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace HOMMS.Application.Implementations
{
    public class DiseaseCategoryFoodRestrictionService : BaseService, IDiseaseCategoryFoodRestrictionService
    {
        private readonly IDiseaseCategoryFoodRestrictionRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<DiseaseCategoryFoodRestrictionService> _logger;

        public DiseaseCategoryFoodRestrictionService(
            IDiseaseCategoryFoodRestrictionRepository repository,
            IMapper mapper,
            IBranchContext branchContext,
            ILogger<DiseaseCategoryFoodRestrictionService> logger)
            : base(branchContext)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetByDiseaseCategoryIdAsync(int diseaseCategoryId)
        {
            var restrictions = await _repository.GetByDiseaseCategoryIdAsync(diseaseCategoryId);
            return _mapper.Map<IEnumerable<DiseaseCategoryFoodRestrictionDto>>(restrictions);
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetByFoodIdAsync(int foodId)
        {
            var restrictions = await _repository.GetByFoodIdAsync(foodId);
            return _mapper.Map<IEnumerable<DiseaseCategoryFoodRestrictionDto>>(restrictions);
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetByBranchIdAsync(int branchId)
        {
            var restrictions = await _repository.GetByBranchIdAsync(branchId);
            return _mapper.Map<IEnumerable<DiseaseCategoryFoodRestrictionDto>>(restrictions);
        }

        public async Task<DiseaseCategoryFoodRestrictionDto?> GetByIdAsync(int id)
        {
            var restriction = await _repository.GetByIdAsync(id);
            return _mapper.Map<DiseaseCategoryFoodRestrictionDto>(restriction);
        }

        public async Task<IEnumerable<FoodDto>> GetAllowedFoodsForPatientAsync(string patientId, int branchId, int? diseaseCategoryId = null)
        {
            branchId = EnsureBranchId(branchId);
            
            var allowedFoods = await _repository.GetAllowedFoodsForPatientAsync(patientId, branchId, diseaseCategoryId);
            return _mapper.Map<IEnumerable<FoodDto>>(allowedFoods);
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetRestrictedFoodsForPatientAsync(string patientId, int branchId)
        {
            branchId = EnsureBranchId(branchId);
            
            var restrictedFoods = await _repository.GetRestrictedFoodsForPatientAsync(patientId, branchId);
            return _mapper.Map<IEnumerable<DiseaseCategoryFoodRestrictionDto>>(restrictedFoods);
        }

        public async Task<FoodValidationResultDto> ValidateFoodForPatientAsync(string patientId, int foodId, int branchId)
        {
            branchId = EnsureBranchId(branchId);
            
            var (isAllowed, restrictions) = await _repository.ValidateFoodForPatientAsync(patientId, foodId, branchId);
            var restrictionDtos = _mapper.Map<List<DiseaseCategoryFoodRestrictionDto>>(restrictions);
            
            var result = new FoodValidationResultDto
            {
                FoodId = foodId,
                FoodName = restrictions.FirstOrDefault()?.Food?.Name ?? "Unknown Food",
                IsAllowed = isAllowed,
                Restrictions = restrictionDtos,
                HighestRestrictionLevel = restrictions.Any() ? restrictions.Max(r => r.RestrictionLevel) : 0
            };
            
            // Generate validation message
            if (!isAllowed && restrictions.Any())
            {
                var highestRestriction = restrictions.OrderByDescending(r => r.RestrictionLevel).First();
                result.ValidationMessage = $"Food is {GetRestrictionLevelName(highestRestriction.RestrictionLevel)} for {highestRestriction.DiseaseCategory?.Name}: {highestRestriction.Reason}";
            }
            else if (isAllowed && restrictions.Any(r => r.RestrictionLevel == 1))
            {
                var advisoryRestriction = restrictions.First(r => r.RestrictionLevel == 1);
                result.ValidationMessage = $"Advisory for {advisoryRestriction.DiseaseCategory?.Name}: {advisoryRestriction.Reason}";
            }
            else
            {
                result.ValidationMessage = "Food is allowed for this patient.";
            }
            
            return result;
        }

        public async Task<IEnumerable<FoodValidationResultDto>> ValidateFoodsForPatientAsync(string patientId, IEnumerable<int> foodIds, int branchId)
        {
            branchId = EnsureBranchId(branchId);
            
            var validationResults = new List<FoodValidationResultDto>();
            
            foreach (var foodId in foodIds)
            {
                var result = await ValidateFoodForPatientAsync(patientId, foodId, branchId);
                validationResults.Add(result);
            }
            
            return validationResults;
        }

        public async Task<DiseaseCategoryFoodRestrictionDto> CreateAsync(CreateDiseaseCategoryFoodRestrictionDto dto, int branchId, string userId)
        {
            branchId = EnsureBranchId(branchId);
            
            // Check if restriction already exists
            var restrictionExists = await _repository.RestrictionExistsAsync(dto.DiseaseCategoryId, dto.FoodId, branchId);
            if (restrictionExists)
            {
                throw new Exception($"Food restriction already exists for this disease category and food combination");
            }
            
            var restriction = _mapper.Map<DiseaseCategoryFoodRestriction>(dto);
            restriction.BranchId = branchId;
            
            var created = await _repository.AddAsync(restriction);
            return _mapper.Map<DiseaseCategoryFoodRestrictionDto>(created);
        }

        public async Task<DiseaseCategoryFoodRestrictionDto?> UpdateAsync(int id, UpdateDiseaseCategoryFoodRestrictionDto dto, string userId)
        {
            var existingRestriction = await _repository.GetByIdAsync(id);
            if (existingRestriction == null)
            {
                return null;
            }
            
            _mapper.Map(dto, existingRestriction);
            
            await _repository.UpdateAsync(existingRestriction);
            return _mapper.Map<DiseaseCategoryFoodRestrictionDto>(existingRestriction);
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var existingRestriction = await _repository.GetByIdAsync(id);
            if (existingRestriction == null)
            {
                return false;
            }
            
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetByRestrictionLevelAsync(int branchId, int restrictionLevel)
        {
            branchId = EnsureBranchId(branchId);
            
            var restrictions = await _repository.GetByRestrictionLevelAsync(branchId, restrictionLevel);
            return _mapper.Map<IEnumerable<DiseaseCategoryFoodRestrictionDto>>(restrictions);
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestrictionDto>> GetActiveByBranchIdAsync(int branchId)
        {
            branchId = EnsureBranchId(branchId);
            
            var restrictions = await _repository.GetActiveByBranchIdAsync(branchId);
            return _mapper.Map<IEnumerable<DiseaseCategoryFoodRestrictionDto>>(restrictions);
        }

        public async Task<bool> RestrictionExistsAsync(int diseaseCategoryId, int foodId, int branchId, int? excludeId = null)
        {
            branchId = EnsureBranchId(branchId);
            
            return await _repository.RestrictionExistsAsync(diseaseCategoryId, foodId, branchId, excludeId);
        }

        private static string GetRestrictionLevelName(int restrictionLevel)
        {
            return restrictionLevel switch
            {
                1 => "Advisory",
                2 => "Warning",
                3 => "Prohibited",
                4 => "Dangerous",
                _ => "Unknown"
            };
        }
    }
} 