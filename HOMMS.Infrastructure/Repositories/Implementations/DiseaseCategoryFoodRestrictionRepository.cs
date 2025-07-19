using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for Disease Category Food Restriction operations
    /// </summary>
    public class DiseaseCategoryFoodRestrictionRepository : Repository<DiseaseCategoryFoodRestriction, int>, IDiseaseCategoryFoodRestrictionRepository
    {
        private readonly ApplicationDbContext _context;

        public DiseaseCategoryFoodRestrictionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetByDiseaseCategoryIdAsync(int diseaseCategoryId)
        {
            return await _context.DiseaseCategoryFoodRestrictions
                .Where(dcfr => dcfr.DiseaseCategoryId == diseaseCategoryId && !dcfr.IsDeleted)
                .Include(dcfr => dcfr.DiseaseCategory)
                .Include(dcfr => dcfr.Food)
                .Include(dcfr => dcfr.Branch)
                .OrderBy(dcfr => dcfr.RestrictionLevel)
                .ThenBy(dcfr => dcfr.Food!.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetByFoodIdAsync(int foodId)
        {
            return await _context.DiseaseCategoryFoodRestrictions
                .Where(dcfr => dcfr.FoodId == foodId && !dcfr.IsDeleted)
                .Include(dcfr => dcfr.DiseaseCategory)
                .Include(dcfr => dcfr.Food)
                .Include(dcfr => dcfr.Branch)
                .OrderBy(dcfr => dcfr.RestrictionLevel)
                .ThenBy(dcfr => dcfr.DiseaseCategory!.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetByBranchIdAsync(int branchId)
        {
            return await _context.DiseaseCategoryFoodRestrictions
                .Where(dcfr => dcfr.BranchId == branchId && !dcfr.IsDeleted)
                .Include(dcfr => dcfr.DiseaseCategory)
                .Include(dcfr => dcfr.Food)
                .Include(dcfr => dcfr.Branch)
                .OrderBy(dcfr => dcfr.DiseaseCategory!.Name)
                .ThenBy(dcfr => dcfr.Food!.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Food>> GetAllowedFoodsForPatientAsync(string patientId, int branchId, int? diseaseCategoryId = null)
        {
            // If a specific disease category ID is provided, use it directly
            if (diseaseCategoryId.HasValue)
            {
                Console.WriteLine($"🔍 GetAllowedFoodsForPatientAsync - Patient: {patientId}, Branch: {branchId}, DiseaseCategory: {diseaseCategoryId}");
                
                // Get foods that are restricted for the specific disease category
                var specificRestrictedFoods = await _context.DiseaseCategoryFoodRestrictions
                    .Where(fo => fo.DiseaseCategoryId == diseaseCategoryId.Value && 
                                fo.IsActive && 
                                fo.BranchId == branchId)
                    .Select(r => r.Food)
                    .Where(f => !f.IsDeleted)
                    .Distinct()
                    .ToListAsync();

                Console.WriteLine($"🚫 Restricted Foods Count: {specificRestrictedFoods.Count}");
                Console.WriteLine($"🍽️ Restricted Food Names: [{string.Join(", ", specificRestrictedFoods.Select(f => f.Name))}]");

                return specificRestrictedFoods;
            }

            // Original logic for when no specific disease category is provided
            // Get patient's disease categories
            var patientDiseaseCategories = await _context.PatientDiseaseCategories
                .Where(pdc => pdc.PatientId == patientId && pdc.IsActive && !pdc.IsDeleted)
                .Select(pdc => pdc.DiseaseCategoryId)
                .ToListAsync();

            // Debug logging
            Console.WriteLine($"🔍 GetAllowedFoodsForPatientAsync - Patient: {patientId}, Branch: {branchId}");
            Console.WriteLine($"📋 Patient Disease Categories: [{string.Join(", ", patientDiseaseCategories)}]");

            if (!patientDiseaseCategories.Any())
            {
                Console.WriteLine($"⚠️ No disease categories found for patient {patientId}, returning ALL foods");
                // If patient has no disease categories, return all foods
                return await _context.Foods
                    .Where(f => f.BranchId == branchId && !f.IsDeleted)
                    .OrderBy(f => f.Name)
                    .ToListAsync();
            }

            // Get foods that are restricted for any of the patient's disease categories
            var patientRestrictedFoods = await _context.DiseaseCategoryFoodRestrictions
                .Where(fo => patientDiseaseCategories.Contains(fo.DiseaseCategoryId) && 
                            fo.IsActive && 
                            fo.BranchId == branchId)
                .Select(r => r.Food)
                .Where(f => !f.IsDeleted)
                .Distinct()
                .ToListAsync();

            Console.WriteLine($"🚫 Restricted Foods Count: {patientRestrictedFoods.Count}");
            Console.WriteLine($"🍽️ Restricted Food Names: [{string.Join(", ", patientRestrictedFoods.Select(f => f.Name))}]");

            return patientRestrictedFoods;
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetRestrictedFoodsForPatientAsync(string patientId, int branchId)
        {
            // Get patient's disease categories
            var patientDiseaseCategories = await _context.PatientDiseaseCategories
                .Where(pdc => pdc.PatientId == patientId && pdc.IsActive && !pdc.IsDeleted)
                .Select(pdc => pdc.DiseaseCategoryId)
                .ToListAsync();

            if (!patientDiseaseCategories.Any())
            {
                return new List<DiseaseCategoryFoodRestriction>();
            }

            return await _context.DiseaseCategoryFoodRestrictions
                .Where(dcfr => dcfr.BranchId == branchId && 
                              patientDiseaseCategories.Contains(dcfr.DiseaseCategoryId) &&
                              dcfr.IsActive && !dcfr.IsDeleted)
                .Include(dcfr => dcfr.DiseaseCategory)
                .Include(dcfr => dcfr.Food)
                .Include(dcfr => dcfr.Branch)
                .OrderBy(dcfr => dcfr.RestrictionLevel)
                .ThenBy(dcfr => dcfr.Food!.Name)
                .ToListAsync();
        }

        public async Task<(bool IsAllowed, IEnumerable<DiseaseCategoryFoodRestriction> Restrictions)> ValidateFoodForPatientAsync(string patientId, int foodId, int branchId)
        {
            // Get patient's disease categories
            var patientDiseaseCategories = await _context.PatientDiseaseCategories
                .Where(pdc => pdc.PatientId == patientId && pdc.IsActive && !pdc.IsDeleted)
                .Select(pdc => pdc.DiseaseCategoryId)
                .ToListAsync();

            if (!patientDiseaseCategories.Any())
            {
                // If patient has no disease categories, food is allowed
                return (true, new List<DiseaseCategoryFoodRestriction>());
            }

            // Get restrictions for this food and patient's disease categories
            var restrictions = await _context.DiseaseCategoryFoodRestrictions
                .Where(dcfr => dcfr.BranchId == branchId && 
                              dcfr.FoodId == foodId &&
                              patientDiseaseCategories.Contains(dcfr.DiseaseCategoryId) &&
                              dcfr.IsActive && !dcfr.IsDeleted)
                .Include(dcfr => dcfr.DiseaseCategory)
                .Include(dcfr => dcfr.Food)
                .OrderBy(dcfr => dcfr.RestrictionLevel)
                .ToListAsync();

            // Check if any restriction is at warning level or higher
            var hasHighRestriction = restrictions.Any(r => r.RestrictionLevel >= 2);
            var isAllowed = !hasHighRestriction;

            return (isAllowed, restrictions);
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetByRestrictionLevelAsync(int branchId, int restrictionLevel)
        {
            return await _context.DiseaseCategoryFoodRestrictions
                .Where(dcfr => dcfr.BranchId == branchId && 
                              dcfr.RestrictionLevel == restrictionLevel &&
                              dcfr.IsActive && !dcfr.IsDeleted)
                .Include(dcfr => dcfr.DiseaseCategory)
                .Include(dcfr => dcfr.Food)
                .Include(dcfr => dcfr.Branch)
                .OrderBy(dcfr => dcfr.DiseaseCategory!.Name)
                .ThenBy(dcfr => dcfr.Food!.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetActiveByBranchIdAsync(int branchId)
        {
            return await _context.DiseaseCategoryFoodRestrictions
                .Where(dcfr => dcfr.BranchId == branchId && 
                              dcfr.IsActive && !dcfr.IsDeleted)
                .Include(dcfr => dcfr.DiseaseCategory)
                .Include(dcfr => dcfr.Food)
                .Include(dcfr => dcfr.Branch)
                .OrderBy(dcfr => dcfr.DiseaseCategory!.Name)
                .ThenBy(dcfr => dcfr.Food!.Name)
                .ToListAsync();
        }

        public async Task<bool> RestrictionExistsAsync(int diseaseCategoryId, int foodId, int branchId, int? excludeId = null)
        {
            var query = _context.DiseaseCategoryFoodRestrictions
                .Where(dcfr => dcfr.DiseaseCategoryId == diseaseCategoryId &&
                              dcfr.FoodId == foodId &&
                              dcfr.BranchId == branchId &&
                              !dcfr.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(dcfr => dcfr.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public override async Task<DiseaseCategoryFoodRestriction?> GetByIdAsync(int id)
        {
            return await _context.DiseaseCategoryFoodRestrictions
                .Include(dcfr => dcfr.DiseaseCategory)
                .Include(dcfr => dcfr.Food)
                .Include(dcfr => dcfr.Branch)
                .FirstOrDefaultAsync(dcfr => dcfr.Id == id && !dcfr.IsDeleted);
        }

        public override async Task<IEnumerable<DiseaseCategoryFoodRestriction>> GetAllAsync()
        {
            return await _context.DiseaseCategoryFoodRestrictions
                .Where(dcfr => !dcfr.IsDeleted)
                .Include(dcfr => dcfr.DiseaseCategory)
                .Include(dcfr => dcfr.Food)
                .Include(dcfr => dcfr.Branch)
                .OrderBy(dcfr => dcfr.BranchId)
                .ThenBy(dcfr => dcfr.DiseaseCategory!.Name)
                .ThenBy(dcfr => dcfr.Food!.Name)
                .ToListAsync();
        }
    }
} 