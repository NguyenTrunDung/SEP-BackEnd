using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Repositories.Implementations;
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
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetOrderListByChefAsync(int branchId)
        {
            var orders = await _orderRepository.GetOrderListByChefAsync(branchId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<bool> UpdateOrderStatusByChefAsync(int orderId)
        {
            return await _orderRepository.UpdateOrderStatusByChefAsync(orderId);
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
            var orders = await _unitOfWork.OrderRepository.FilterOrdersAsync(
                startOrderDate,
                endOrderDate,
                startReceiveDate,
                endReceiveDate,
                receiveTime,
                status,
                customerName,
                customerPhone,
                minTotal,
                maxTotal,
                code
            );

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
