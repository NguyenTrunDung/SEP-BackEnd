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

        /// <summary>
        /// Retrieves a list of orders for a chef by a specific branch ID
        /// Orders with status "Preparing" or "Completed", and sorted by OrderDate in descending order
        /// </summary>
        /// <param name="branchId">The branch ID</param>
        /// <returns>A list of orders for a chef</returns>
        public async Task<IEnumerable<Order>> GetOrderListByChefAsync(int branchId)
        {
            return await DbSet.Where(o => o.BranchId == branchId && (o.Status == "Preparing" || o.Status == "Completed"))
                .Include(o => o.OrderDetails)
                .OrderBy(o => o.Status == "Completed")      //Ensures "Preparing" orders come first
                .ThenByDescending(o => o.OrderDate)         //Sorts by lastest date
                .ToListAsync();
        }

        /// <summary>
        /// Allows a chef to update the status of an order
        /// Ensures only "Preparing" orders can be updated to "Completed"
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <returns>True if the status update is successful, otherwise false</returns>
        public async Task<bool> UpdateOrderStatusByChefAsync(int orderId)
        {
            var order = await DbSet.FindAsync(orderId);
            if (order == null || order.Status != "Preparing") return false;     //Validate order exists and is in "Preparing" status

            order.Status = "Completed";
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Order>> GetOrdersByBranchIdAsync(int branchId)
        {
            return await _context.Orders
                                 .Where(o => o.BranchId == branchId)
                                 .ToListAsync();
        }

        public async Task<List<Order>> FilterOrdersAsync(
            DateTime? startOrderDate,
            DateTime? endOrderDate,
            DateTime? startReceiveDate,
            DateTime? endReceiveDate,
            string? receiveTime,
            string? status,
            string? customerName,
            string? customerPhone,
            int? minTotal,
            int? maxTotal,
            string? code)
        {
            var query = _context.Orders.AsQueryable();

            // Lọc theo OrderDate
            if (startOrderDate.HasValue)
                query = query.Where(o => o.OrderDate >= startOrderDate.Value);
            if (endOrderDate.HasValue)
                query = query.Where(o => o.OrderDate <= endOrderDate.Value);

            // Lọc theo ReceiveDate
            if (startReceiveDate.HasValue)
                query = query.Where(o => o.ReceiveDate.HasValue && o.ReceiveDate.Value >= startReceiveDate.Value);
            if (endReceiveDate.HasValue)
                query = query.Where(o => o.ReceiveDate.HasValue && o.ReceiveDate.Value <= endReceiveDate.Value);

            // Lọc theo ReceiveTime (exact match hoặc chứa)
            if (!string.IsNullOrWhiteSpace(receiveTime))
                query = query.Where(o => o.ReceiveTime != null && o.ReceiveTime.Contains(receiveTime));

            // Lọc theo Status (exact match)
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status == status);

            // Lọc theo CustomerName (tìm kiếm chứa)
            if (!string.IsNullOrWhiteSpace(customerName))
                query = query.Where(o => o.CustomerName != null && o.CustomerName.Contains(customerName));

            // Lọc theo CustomerPhone (tìm kiếm chứa)
            if (!string.IsNullOrWhiteSpace(customerPhone))
                query = query.Where(o => o.CustomerPhone != null && o.CustomerPhone.Contains(customerPhone));

            // Lọc theo Total (khoảng min-max)
            if (minTotal.HasValue)
                query = query.Where(o => o.Total.HasValue && o.Total.Value >= minTotal.Value);
            if (maxTotal.HasValue)
                query = query.Where(o => o.Total.HasValue && o.Total.Value <= maxTotal.Value);

            // Lọc theo Code (tìm kiếm chứa)
            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(o => o.Code != null && o.Code.Contains(code));

            return await query.ToListAsync();
        }

        public async Task<List<Order>> SearchOrdersAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await _context.Orders.ToListAsync();

            keyword = keyword.Trim();

            return await _context.Orders
                                 .Where(o =>
                                     (o.Code != null && o.Code.Contains(keyword)) ||
                                     (o.CustomerName != null && o.CustomerName.Contains(keyword)) ||
                                     (o.CustomerPhone != null && o.CustomerPhone.Contains(keyword)) ||
                                     (o.Status != null && o.Status.Contains(keyword)) ||
                                     (o.ReceiveTime != null && o.ReceiveTime.Contains(keyword))
                                 )
                                 .ToListAsync();
        }


    }
}
