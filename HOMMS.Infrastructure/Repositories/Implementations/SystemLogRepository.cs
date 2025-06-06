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
    public class SystemLogRepository : Repository<SystemLog, int>, ISystemLogRepository
    {

        private readonly ApplicationDbContext _dbContext;
        public SystemLogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

      

        public async Task<IEnumerable<SystemLog>> GetSystemLogAll(int branchId, DateTime dateStart, DateTime dateEnd)
        {
            return await DbSet
                .Where(s => s.BranchId == branchId &&
                ((s.CreatedAt.Date >= dateStart && s.CreatedAt.Date <= dateEnd)
              || (s.LastModifiedAt.HasValue && s.LastModifiedAt.Value.Date >= dateStart && s.LastModifiedAt.Value.Date <= dateEnd)))
                .ToListAsync();
        }
    }
}
