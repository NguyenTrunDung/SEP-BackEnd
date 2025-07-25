using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class DepartmentRepository : Repository<Department, int>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<Department>> GetActiveDepartmentByBranchAsync(int branchId)
        {
            return await DbSet
                .Where(d => d.BranchId == branchId && d.IsActive && !d.IsDeleted)
                .OrderBy(d => d.Sort)
                .ThenBy(d => d.Name)
                .ToListAsync();

        }

        public async Task<IEnumerable<Department>> GetDepartmentByBranchAsync(int branchId)
        {
            return await DbSet
               .Where(d => d.BranchId == branchId && !d.IsDeleted)
               .OrderBy(d => d.Sort)
               .ThenBy(d => d.Name)
               .ToListAsync();
        }

        public async Task<bool> IsDepartmentNameUniqueAsync(int branchId, string name, int? excludeId = null)
        {
            var query = DbSet.Where(a => a.BranchId == branchId && !a.IsDeleted && a.Name.ToLower() == name.ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(a => a.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }
    }
}
