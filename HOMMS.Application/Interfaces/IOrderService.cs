using HOMMS.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrdersByBranchIdAsync(int branchId);

        Task<List<OrderDto>> SearchOrdersAsync(string keyword);

        /// <summary>
        /// Lọc đơn hàng theo các tiêu chí cụ thể: ngày đơn hàng, ngày nhận, thời gian nhận, trạng thái, tên KH, SĐT KH, tổng tiền, mã đơn.
        /// </summary>
        Task<List<OrderDto>> FilterOrdersAsync(
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
            string? code
        );

        Task<IEnumerable<OrderDto>> GetOrderListByChefAsync(int branchId);
        Task<bool> UpdateOrderStatusByChefAsync(int orderId);

        Task<OrderDto> GetByIdAsync(int id);
        Task<OrderDto> AddPatientOrderAsync(CreatePatientOrderDto entity);
        Task<OrderDto> AddAsync(OrderDto entity);

        Task<OrderDto> UpdateAsync(int id, UpdateOrderDto entity);
        Task<bool> DeleteAsync(int id);

        //add order with location
        Task<OrderDto> AddOrderV2Async(OrderDtoV2 dto);

        /// <summary>
        /// Gets orders by branch ID with optional filtering and search capabilities
        /// </summary>
        Task<List<OrderDto>> GetOrdersByBranchWithFiltersAsync(
            int branchId,
            DateTime? startOrderDate = null,
            DateTime? endOrderDate = null,
            DateTime? startReceiveDate = null,
            DateTime? endReceiveDate = null,
            string? receiveTime = null,
            string? status = null,
            string? customerName = null,
            string? customerPhone = null,
            int? minTotal = null,
            int? maxTotal = null,
            string? code = null,
            string? keyword = null,
            bool? isPaid = null
        );

        /// <summary>
        /// Gets orders with status "Preparing" for kitchen view with detailed food information
        /// </summary>
        Task<List<OrderDto>> GetOrdersByStatusPreparingAsync(int branchId);


    }
}
