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
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Order>> GetOrderListByChefAsync(int branchId)
        {
            return await DbSet.Where(o => o.BranchId == branchId && (o.Status == "Pending" || o.Status == "Preparing"))
                .Include(o => o.OrderDetails)
                .OrderBy(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusByChefAsync(int orderId, string status)
        {
            var order = await DbSet.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
}
