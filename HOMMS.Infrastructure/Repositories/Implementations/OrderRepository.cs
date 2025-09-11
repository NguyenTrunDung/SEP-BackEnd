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
            return await DbSet.Where(o => o.BranchId == branchId && (o.Status == "Confirmed"))
                .Include(o => o.OrderDetails)
                   .ThenInclude(o => o.Food)
                .OrderBy(o => o.Status == "Confirmed")      //Ensures "Confirmed" orders come first
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
            if (order == null || order.Status != "Confirmed") return false;     //Validate order exists and is in "Confirmed" status

            order.Status = "Delivered";
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

        //add order with location
        public async Task<Order> AddOrderV2Async(Order order)
        {
            // Log the incoming order details
            Console.WriteLine($"[OrderRepository.AddOrderV2Async] Incoming Order OrderDetails Count: {order?.OrderDetails?.Count ?? 0}");
            if (order?.OrderDetails != null)
            {
                Console.WriteLine("[OrderRepository.AddOrderV2Async] Incoming Order OrderDetails Details:");
                foreach (var detail in order.OrderDetails)
                {
                    Console.WriteLine($"  - FoodId: {detail.FoodId}, Qty: {detail.Qty}, Note: {detail.Note}, Price: {detail.Price}");
                }
            }

            if (order.LocationId.HasValue)
            {
                var location = await _context.Locations.FindAsync(order.LocationId.Value);
                if (location == null)
                    throw new Exception("Location không tồn tại.");
            }

            await DbSet.AddAsync(order);
            await _context.SaveChangesAsync();
            
            // Log the saved order details
            Console.WriteLine($"[OrderRepository.AddOrderV2Async] Saved Order OrderDetails Count: {order?.OrderDetails?.Count ?? 0}");
            if (order?.OrderDetails != null)
            {
                Console.WriteLine("[OrderRepository.AddOrderV2Async] Saved Order OrderDetails Details:");
                foreach (var detail in order.OrderDetails)
                {
                    Console.WriteLine($"  - FoodId: {detail.FoodId}, Qty: {detail.Qty}, Note: {detail.Note}, Price: {detail.Price}");
                }
            }
            
            return order;
        }

        /// <summary>
        /// Gets orders by branch ID with optional filtering and search capabilities
        /// Combines the functionality of GetOrdersByBranchIdAsync, FilterOrdersAsync, and SearchOrdersAsync
        /// </summary>
        public async Task<List<Order>> GetOrdersByBranchWithFiltersAsync(
            int branchId,
            DateTime? startOrderDate = null,
            DateTime? endOrderDate = null,
            DateTime? startReceiveDate = null,
            DateTime? endReceiveDate = null,
            string? receiveTime = null,

            bool? IsPatientOrder = null,
            string? customerName = null,
            string? customerPhone = null,
            int? minTotal = null,
            int? maxTotal = null,
            string? code = null,
            string? keyword = null)
        {
            var query = _context.Orders.Where(o => o.BranchId == branchId);



            // Apply keyword search if provided
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(o =>
                    (o.Code != null && o.Code.Contains(keyword)) ||
                    (o.CustomerName != null && o.CustomerName.Contains(keyword)) ||
                    (o.CustomerPhone != null && o.CustomerPhone.Contains(keyword)) ||

                    (o.ReceiveTime != null && o.ReceiveTime.Contains(keyword))
                );
            }


            if (IsPatientOrder == true) {
                query = query.Where(o => o.IsPatientOrder == true);
            }
            else
            {
                query = query.Where(o => o.IsPatientOrder == false);
            }

                query = query.Where(o => o.Status == "Completed" && o.IsPaid == true);
           

            // Apply date filters
            if (startOrderDate.HasValue)
                query = query.Where(o => o.OrderDate.Date >= startOrderDate.Value.Date);
            if (endOrderDate.HasValue)
                query = query.Where(o => o.OrderDate.Date <= endOrderDate.Value.Date);

            // Apply receive date filters
            if (startReceiveDate.HasValue)
                query = query.Where(o => o.ReceiveDate.HasValue && o.ReceiveDate.Value >= startReceiveDate.Value);
            if (endReceiveDate.HasValue)
                query = query.Where(o => o.ReceiveDate.HasValue && o.ReceiveDate.Value <= endReceiveDate.Value);

            // Apply other filters
            if (!string.IsNullOrWhiteSpace(receiveTime))
                query = query.Where(o => o.ReceiveTime != null && o.ReceiveTime.Contains(receiveTime));

             
               

            if (!string.IsNullOrWhiteSpace(customerName))
                query = query.Where(o => o.CustomerName != null && o.CustomerName.Contains(customerName));

            if (!string.IsNullOrWhiteSpace(customerPhone))
                query = query.Where(o => o.CustomerPhone != null && o.CustomerPhone.Contains(customerPhone));

            if (minTotal.HasValue)
                query = query.Where(o => o.Total.HasValue && o.Total.Value >= minTotal.Value);
            if (maxTotal.HasValue)
                query = query.Where(o => o.Total.HasValue && o.Total.Value <= maxTotal.Value);

            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(o => o.Code != null && o.Code.Contains(code));


               

            // Apply Include statements at the end to load related entities
            return await query
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                        .ThenInclude(f => f.Category)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Menu)
                .Include(o => o.Patient)
                .Include(o => o.Branch)
                .Include(o => o.Location)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets orders with status "Preparing" for kitchen view with detailed food information
        /// </summary>
        public async Task<List<Order>> GetOrdersByStatusPreparingAsync(int branchId)
        {
            return await _context.Orders
                .Where(o => o.BranchId == branchId && o.Status == "Confirmed")
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                        .ThenInclude(f => f.Category)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Menu)
                .Include(o => o.Patient)
                .Include(o => o.Branch)
                .Include(o => o.Location)
                .OrderBy(o => o.OrderDate)
                .ToListAsync();
        }



    }
}