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
    public class OrderRepository : Repository<Order, int>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersByBranchIdAsync(int branchId)
        {
            return await _context.Orders
                                 .Where(o => o.BranchId == branchId)
                                 .ToListAsync();
        }
        public async Task<List<Order>> SearchOrdersAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await _context.Orders.ToListAsync();

            return await _context.Orders
                                 .Where(o =>
                                     o.Code!.Contains(keyword) ||
                                     o.CustomerName!.Contains(keyword) ||
                                     o.CustomerPhone!.Contains(keyword) ||
                                     o.CustomerAddress!.Contains(keyword) ||
                                     o.VatName!.Contains(keyword) ||
                                     o.VatTaxCode!.Contains(keyword) ||
                                     o.Note!.Contains(keyword)
                                 )
                                 .ToListAsync();
        }

        public async Task<List<Order>> FilterOrdersAsync(
            int? branchId,
            DateTime? startDate,
            DateTime? endDate,
            string? status,
            string? type,
            bool? hasVat,
            bool? printed)
        {
            var query = _context.Orders.AsQueryable();

            if (branchId.HasValue)
                query = query.Where(o => o.BranchId == branchId.Value);

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(o => o.Type == type);

            if (hasVat.HasValue)
                query = query.Where(o => o.HasVat == hasVat.Value);

            if (printed.HasValue)
                query = query.Where(o => o.Printed == printed.Value);

            return await query.ToListAsync();
        }

    }
}
