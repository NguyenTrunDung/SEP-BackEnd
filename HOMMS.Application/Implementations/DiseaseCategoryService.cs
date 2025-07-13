using AutoMapper;
using HOMMS.Application.BaseServices;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace HOMMS.Application.Implementations
{
    /// <summary>
    /// Service implementation for Disease Category management
    /// </summary>
    public class DiseaseCategoryService : BaseService, IDiseaseCategoryService
    {
        private readonly IDiseaseCategoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<DiseaseCategoryService> _logger;

        public DiseaseCategoryService(
            IDiseaseCategoryRepository repository,
            IMapper mapper,
            IBranchContext branchContext,
            ILogger<DiseaseCategoryService> logger)
            : base(branchContext)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets all disease categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of disease categories</returns>
        public async Task<IEnumerable<DiseaseCategoryDto>> GetByBranchIdAsync(int branchId)
        {
            var diseaseCategories = await _repository.GetByBranchIdAsync(branchId);
            return _mapper.Map<IEnumerable<DiseaseCategoryDto>>(diseaseCategories);
        }

        /// <summary>
        /// Gets active disease categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of active disease categories</returns>
        public async Task<IEnumerable<DiseaseCategoryDto>> GetActiveByBranchIdAsync(int branchId)
        {
            var diseaseCategories = await _repository.GetActiveByBranchIdAsync(branchId);
            return _mapper.Map<IEnumerable<DiseaseCategoryDto>>(diseaseCategories);
        }

        /// <summary>
        /// Gets a disease category by ID
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <returns>Disease category or null if not found</returns>
        public async Task<DiseaseCategoryDto?> GetByIdAsync(int id)
        {
            var diseaseCategory = await _repository.GetByIdAsync(id);
            return _mapper.Map<DiseaseCategoryDto>(diseaseCategory);
        }

        /// <summary>
        /// Gets a disease category by code and branch
        /// </summary>
        /// <param name="code">Disease category code</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Disease category or null if not found</returns>
        public async Task<DiseaseCategoryDto?> GetByCodeAndBranchAsync(string code, int branchId)
        {
            var diseaseCategory = await _repository.GetByCodeAndBranchAsync(code, branchId);
            return _mapper.Map<DiseaseCategoryDto>(diseaseCategory);
        }

        /// <summary>
        /// Creates a new disease category
        /// </summary>
        /// <param name="dto">Create disease category DTO</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="userId">User ID creating the category</param>
        /// <returns>Created disease category</returns>
        public async Task<DiseaseCategoryDto> CreateAsync(CreateDiseaseCategoryDto dto, int branchId, string userId)
        {
            branchId = EnsureBranchId(branchId);
            
            // Auto-generate code if not provided
            var code = dto.Code;
            if (string.IsNullOrWhiteSpace(code))
            {
                code = await GenerateUniqueCodeAsync(dto.Name, branchId);
            }
            
            // Check if code already exists in this branch
            var codeExists = await _repository.CodeExistsAsync(code, branchId);
            if (codeExists)
            {
                throw new Exception($"Disease category code '{code}' already exists in this branch");
            }

            var diseaseCategory = _mapper.Map<DiseaseCategory>(dto);
            diseaseCategory.BranchId = branchId;
            diseaseCategory.Code = code;
            // Remove manual audit assignments - handled by ApplicationDbContext automatically

            var created = await _repository.AddAsync(diseaseCategory);
            return _mapper.Map<DiseaseCategoryDto>(created);
        }

        /// <summary>
        /// Updates an existing disease category
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <param name="dto">Update disease category DTO</param>
        /// <param name="userId">User ID updating the category</param>
        /// <returns>Updated disease category or null if not found</returns>
        public async Task<DiseaseCategoryDto?> UpdateAsync(int id, UpdateDiseaseCategoryDto dto, string userId)
        {
            var existingCategory = await _repository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return null;
            }

            // Update only the name field for simplified flow
            existingCategory.Name = dto.Name;
            
            // Optionally update other fields if provided (for future extensibility)
            if (!string.IsNullOrWhiteSpace(dto.Description))
                existingCategory.Description = dto.Description;
            
            if (!string.IsNullOrWhiteSpace(dto.DietaryRestrictions))
                existingCategory.DietaryRestrictions = dto.DietaryRestrictions;
            
            if (!string.IsNullOrWhiteSpace(dto.RecommendedFoods))
                existingCategory.RecommendedFoods = dto.RecommendedFoods;
            
            if (dto.ColorCode != null)
                existingCategory.ColorCode = dto.ColorCode;
            
            existingCategory.IsActive = dto.IsActive;
            existingCategory.SeverityLevel = dto.SeverityLevel;
            existingCategory.RequiresApproval = dto.RequiresApproval;
            existingCategory.SortOrder = dto.SortOrder;
            
            // Remove manual audit assignments - handled by ApplicationDbContext automatically

            await _repository.UpdateAsync(existingCategory);
            return _mapper.Map<DiseaseCategoryDto>(existingCategory);
        }

        /// <summary>
        /// Soft deletes a disease category
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <param name="userId">User ID deleting the category</param>
        /// <returns>True if deleted, false if not found</returns>
        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var existingCategory = await _repository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return false;
            }

            // Check if category is in use by patients
            var hasPatients = await _repository.GetWithPatientCountsAsync(existingCategory.BranchId);
            var categoryWithPatients = hasPatients.FirstOrDefault(dc => dc.Id == id);
            if (categoryWithPatients?.PatientDiseaseCategories.Any() == true)
            {
                throw new Exception("Cannot delete disease category that is assigned to patients");
            }

            // Soft delete using the repository's DeleteAsync method
            return await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Gets disease categories with patient statistics
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of disease categories with patient counts</returns>
        public async Task<IEnumerable<DiseaseCategoryDto>> GetWithPatientCountsAsync(int branchId)
        {
            var diseaseCategories = await _repository.GetWithPatientCountsAsync(branchId);
            var dtos = diseaseCategories.Select(dc =>
            {
                var dto = _mapper.Map<DiseaseCategoryDto>(dc);
                dto.TotalPatients = dc.PatientDiseaseCategories.Count(pdc => pdc.IsActive);
                return dto;
            }).ToList();

            return dtos;
        }

        /// <summary>
        /// Gets disease categories with food restriction statistics
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of disease categories with food restriction counts</returns>
        public async Task<IEnumerable<DiseaseCategoryDto>> GetWithFoodRestrictionCountsAsync(int branchId)
        {
            var diseaseCategories = await _repository.GetWithFoodRestrictionCountsAsync(branchId);
            var dtos = diseaseCategories.Select(dc =>
            {
                var dto = _mapper.Map<DiseaseCategoryDto>(dc);
                dto.TotalFoodRestrictions = dc.FoodRestrictions.Count(fr => fr.IsActive);
                return dto;
            }).ToList();

            return dtos;
        }

        /// <summary>
        /// Checks if a disease category code is available for use
        /// </summary>
        /// <param name="code">Disease category code</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if code is available</returns>
        public async Task<bool> IsCodeAvailableAsync(string code, int branchId, int? excludeId = null)
        {
            var codeExists = await _repository.CodeExistsAsync(code, branchId, excludeId);
            return !codeExists;
        }

        /// <summary>
        /// Generates a unique code for a disease category based on the name
        /// </summary>
        /// <param name="name">Disease category name</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Unique code</returns>
        private async Task<string> GenerateUniqueCodeAsync(string name, int branchId)
        {
            // Create base code from name (uppercase, no spaces, max 10 chars)
            var baseCode = new string(name.Where(char.IsLetterOrDigit).Take(10).ToArray()).ToUpperInvariant();
            
            if (string.IsNullOrEmpty(baseCode))
            {
                baseCode = "DC";
            }
            
            var code = baseCode;
            var counter = 1;
            
            // Keep trying until we find a unique code
            while (await _repository.CodeExistsAsync(code, branchId))
            {
                code = $"{baseCode}{counter:D2}";
                counter++;
                
                // Prevent infinite loop
                if (counter > 99)
                {
                    throw new Exception("Unable to generate unique code for disease category");
                }
            }
            
            return code;
        }
    }
}