using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for Disease Category operations
    /// </summary>
    public class DiseaseCategoryRepository : Repository<DiseaseCategory, int>, IDiseaseCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public DiseaseCategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all disease categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of disease categories</returns>
        public async Task<IEnumerable<DiseaseCategory>> GetByBranchIdAsync(int branchId)
        {
            return await _context.DiseaseCategories
                .Where(dc => dc.BranchId == branchId && !dc.IsDeleted)
                .Include(dc => dc.Branch)
                .OrderBy(dc => dc.SortOrder)
                .ThenBy(dc => dc.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a disease category by code and branch
        /// </summary>
        /// <param name="code">Disease category code</param>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Disease category if found</returns>
        public async Task<DiseaseCategory?> GetByCodeAndBranchAsync(string code, int branchId)
        {
            return await _context.DiseaseCategories
                .Include(dc => dc.Branch)
                .FirstOrDefaultAsync(dc => dc.Code == code && dc.BranchId == branchId && !dc.IsDeleted);
        }

        /// <summary>
        /// Gets active disease categories for a specific branch
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Collection of active disease categories</returns>
        public async Task<IEnumerable<DiseaseCategory>> GetActiveByBranchIdAsync(int branchId)
        {
            return await _context.DiseaseCategories
                .Where(dc => dc.BranchId == branchId && dc.IsActive && !dc.IsDeleted)
                .Include(dc => dc.Branch)
                .OrderBy(dc => dc.SortOrder)
                .ThenBy(dc => dc.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Checks if a disease category code exists in a branch
        /// </summary>
        /// <param name="code">Disease category code</param>
        /// <param name="branchId">Branch ID</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if code exists</returns>
        public async Task<bool> CodeExistsAsync(string code, int branchId, int? excludeId = null)
        {
            var query = _context.DiseaseCategories
                .Where(dc => dc.Code == code && dc.BranchId == branchId && !dc.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(dc => dc.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Gets disease categories with patient counts
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Disease categories with patient statistics</returns>
        public async Task<IEnumerable<DiseaseCategory>> GetWithPatientCountsAsync(int branchId)
        {
            return await _context.DiseaseCategories
                .Where(dc => dc.BranchId == branchId && !dc.IsDeleted)
                .Include(dc => dc.Branch)
                .Include(dc => dc.PatientDiseaseCategories.Where(pdc => pdc.IsActive && !pdc.IsDeleted))
                .OrderBy(dc => dc.SortOrder)
                .ThenBy(dc => dc.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets disease categories with food restriction counts
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>Disease categories with food restriction statistics</returns>
        public async Task<IEnumerable<DiseaseCategory>> GetWithFoodRestrictionCountsAsync(int branchId)
        {
            return await _context.DiseaseCategories
                .Where(dc => dc.BranchId == branchId && !dc.IsDeleted)
                .Include(dc => dc.Branch)
                .Include(dc => dc.FoodRestrictions.Where(fr => fr.IsActive && !fr.IsDeleted))
                .OrderBy(dc => dc.SortOrder)
                .ThenBy(dc => dc.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Override GetByIdAsync to include related entities
        /// </summary>
        /// <param name="id">Disease category ID</param>
        /// <returns>Disease category with related data</returns>
        public override async Task<DiseaseCategory?> GetByIdAsync(int id)
        {
            return await _context.DiseaseCategories
                .Include(dc => dc.Branch)
                .FirstOrDefaultAsync(dc => dc.Id == id && !dc.IsDeleted);
        }

        /// <summary>
        /// Override GetAllAsync to include related entities and apply branch filtering
        /// </summary>
        /// <returns>All disease categories with related data</returns>
        public override async Task<IEnumerable<DiseaseCategory>> GetAllAsync()
        {
            return await _context.DiseaseCategories
                .Where(dc => !dc.IsDeleted)
                .Include(dc => dc.Branch)
                .OrderBy(dc => dc.BranchId)
                .ThenBy(dc => dc.SortOrder)
                .ThenBy(dc => dc.Name)
                .ToListAsync();
        }
    }
}