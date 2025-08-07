using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class RevenueService : IRevenueService
    {


        private readonly IRevenueRepository _revenueRepository;
        private readonly IMapper _mapper;

        public RevenueService(IRevenueRepository revenueRepository, IMapper mapper)
        {
            _revenueRepository = revenueRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<RevenueDto>> GetRevenueByDayAsync(int branchId, DateTime date)
        {
            var rev = await _revenueRepository.GetRevenueByDayAsync(branchId, date);

            var ven = rev
                .GroupBy(r => r.OrderDate.Hour)
                .SelectMany(g => g.Select(e =>

                new ChartOrderDto
                {
                    Date = $"{date:dd/MM} {g.Key:D2}:00",
                    OrderCount = 1,
                    TotalAmount = e.Total ?? 0,
                    QuantityFood = e.OrderDetails.Sum(re => re.Qty),
                }))
                .OrderBy(c => DateTime.ParseExact(c.Date, "dd/MM HH:mm", null))
                .ToList(); // Explicitly convert to List<ChartOrderDto>

            var nue = new RevenueDto
            {
                Order = rev.Count(),
                Total = rev.Sum(o => o.Total ?? 0),
                QuantityFood = rev.Sum(r => r.OrderDetails.Sum(re => re.Qty)),
                ChartOrders = ven // No error now as ven is explicitly a List<ChartOrderDto>
            };

            return new List<RevenueDto> { nue };
        }

        public async Task<IEnumerable<RevenueDto>> GetRevenueByWeekAsync(int branchId, DateTime date)
        {
            var rev = await _revenueRepository.GetRevenueByWeekAsync(branchId, date);


            var ven = rev
                .GroupBy(r => r.OrderDate.Date)
                .Select(e => new ChartOrderDto
                {
                    Date = e.Key.ToString("dd/MM"),
                    OrderCount = e.Count(),
                    TotalAmount = e.Sum(o => o.Total ?? 0),
                    QuantityFood = e.Sum(o => o.OrderDetails.Sum(od => od.Qty))
                })
                .OrderBy(c => DateTime.ParseExact(c.Date, "dd/MM", null))
                .ToList();

            var nue = new RevenueDto
            {
                Order = rev.Count(),
                Total = rev.Sum(o => o.Total ?? 0),
                QuantityFood = rev.Sum(r => r.OrderDetails.Sum(re => re.Qty)),
                ChartOrders = ven
            };

            return new List<RevenueDto> { nue };
        }

        public async Task<IEnumerable<RevenueDto>> GetRevenueByMonthAsync(int branchId, DateTime date)
        {
            var rev = await _revenueRepository.GetRevenueByMonthAsync(branchId, date);

            var ven = rev
                .Where(r => r.OrderDate.Year == date.Year)
                .GroupBy(r => r.OrderDate.Date)
              .Select(e => new ChartOrderDto
              {
                  Date = e.Key.ToString("dd/MM"),
                  OrderCount = e.Count(),
                  TotalAmount = e.Sum(o => o.Total ?? 0),
                  QuantityFood = e.Sum(o => o.OrderDetails.Sum(od => od.Qty))
              })
                .OrderBy(c => DateTime.ParseExact(c.Date, "dd/MM", null))
                .ToList();

            var nue = new RevenueDto
            {
                Order = rev.Count(),
                Total = rev.Sum(o => o.Total ?? 0),
                QuantityFood = rev.Sum(r => r.OrderDetails.Sum(re => re.Qty)),
                ChartOrders = ven
            };

            return new List<RevenueDto> { nue };
        }



        public async Task<IEnumerable<RevenueDto>> GetRevenueByYearAsync(int branchId, DateTime date)
        {
            var rev = await _revenueRepository.GetRevenueByYearAsync(branchId, date);

            var ven = rev
                .GroupBy(r => r.OrderDate.Date)
               .Select(e => new ChartOrderDto
               {
                   Date = e.Key.ToString("dd/MM"),
                   OrderCount = e.Count(),
                   TotalAmount = e.Sum(o => o.Total ?? 0),
                   QuantityFood = e.Sum(o => o.OrderDetails.Sum(od => od.Qty))
               })
               .OrderBy(c => DateTime.ParseExact(c.Date, "dd/MM", null))
                .ToList();

            var nue = new RevenueDto
            {
                Order = rev.Count(),
                Total = rev.Sum(o => o.Total ?? 0),
                QuantityFood = rev.Sum(r => r.OrderDetails.Sum(re => re.Qty)),
                ChartOrders = ven
            };

            return new List<RevenueDto> { nue };
        }



    }


}
