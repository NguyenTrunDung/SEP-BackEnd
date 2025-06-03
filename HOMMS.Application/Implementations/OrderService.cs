<<<<<<< HEAD
﻿using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Repositories.Implementations;
=======
﻿using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
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
<<<<<<< HEAD
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
=======
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        }

        public async Task<IEnumerable<OrderDto>> GetOrderListByChefAsync(int branchId)
        {
<<<<<<< HEAD
            var orders = await _unitOfWork.OrderRepository.GetOrderListByChefAsync(branchId);
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
=======
            var orders = await _orderRepository.GetOrderListByChefAsync(branchId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        }

        public async Task<bool> UpdateOrderStatusByChefAsync(int orderId, string status)
        {
<<<<<<< HEAD
            return await _unitOfWork.OrderRepository.UpdateOrderStatusByChefAsync(orderId, status);
=======
            return await _orderRepository.UpdateOrderStatusByChefAsync(orderId, status);
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        }
    }
}
