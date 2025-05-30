using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Kiota.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class RevenueRepository : Repository<Order, int>, IRevenueRepository
    {
        public RevenueRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Order>> GetRevenueByDayAsync(int branchId, DateTime date)
        {
            return await DbSet
                 .Where(r => r.BranchId == branchId && r.Total != null && r.OrderDate.Date == date.Date)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetRevenueByWeekAsync(int branchId, DateTime date)
        {
          
            var startOfWeek = date.Date.AddDays(-(int)date.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            return await DbSet
                 .Where(r => r.BranchId == branchId && r.Total != null && r.OrderDate.Date >= startOfWeek &&
                             r.OrderDate.Date <= endOfWeek)
                 .ToListAsync();
        }
        
        public async Task<IEnumerable<Order>> GetRevenueByMonthAsync(int branchId, DateTime date)
        {
            return await DbSet
                   .Where(r => r.BranchId == branchId && r.Total != null && r.OrderDate.Year == date.Year && r.OrderDate.Month == date.Month)
                   .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetRevenueByYearAsync(int branchId, DateTime date)
        {
            return await DbSet
                 .Where(r => r.BranchId == branchId && r.Total != null && r.OrderDate.Year == date.Year)
                 .ToListAsync();
        }
    }


      


}
