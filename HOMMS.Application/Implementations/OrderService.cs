using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OrderDto>> GetOrdersByBranchIdAsync(int branchId)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersByBranchIdAsync(branchId);

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                ReceiveDate = o.ReceiveDate,
                ReceiveTime = o.ReceiveTime,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                Total = o.Total,
                Code = o.Code
            }).ToList();
        }
        public async Task<List<OrderDto>> SearchOrdersAsync(string keyword)
        {
            var orders = await _unitOfWork.OrderRepository.SearchOrdersAsync(keyword);

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                ReceiveDate = o.ReceiveDate,
                ReceiveTime = o.ReceiveTime,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                Total = o.Total,
                Code = o.Code
            }).ToList();
        }

        public async Task<List<OrderDto>> FilterOrdersAsync(
            int? branchId,
            DateTime? startDate,
            DateTime? endDate,
            string? status,
            string? type,
            bool? hasVat,
            bool? printed)
        {
            var orders = await _unitOfWork.OrderRepository.FilterOrdersAsync(
                branchId, startDate, endDate, status, type, hasVat, printed);

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                ReceiveDate = o.ReceiveDate,
                ReceiveTime = o.ReceiveTime,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                Total = o.Total,
                Code = o.Code
            }).ToList();
        }

    }
}
