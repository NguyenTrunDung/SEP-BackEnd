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
using System.Threading.Tasks;
using System.Xml;

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

            
            foreach (var order in rev)
            {
               
            }

            // Nếu không có dữ liệu, trả về dữ liệu mặc định
            if (!rev.Any())
            {
                var defaultChartOrders = Enumerable.Range(0, 24)
                    .Select(i => new ChartOrderDto
                    {
                        Date = $"{i:D2}:00",
                        OrderCount = 0,
                        TotalAmount = 0,
                        QuantityFood = 0
                    })
                    .ToList();

                var defaultRevenue = new RevenueDto
                {
                    Order = 0,
                    Total = 0,
                    QuantityFood = 0,
                    ChartOrders = defaultChartOrders
                };

               
                return new List<RevenueDto> { defaultRevenue };
            }

            // Nhóm dữ liệu theo giờ
            var ven = rev
                .GroupBy(r => r.OrderDate.Hour)
                .Select(g => new ChartOrderDto
                {
                    Date = $"{g.Key:D2}:00",
                    OrderCount = g.Count(),
                    TotalAmount = g.Sum(o => o.Total ?? 0),
                    QuantityFood = g.Sum(o => o.OrderDetails?.Sum(od => od.Qty) ?? 0)
                })
                .OrderBy(c => int.Parse(c.Date.Split(':')[0]))
                .ToList();

            // Bổ sung các giờ thiếu
            var fullChartOrders = Enumerable.Range(0, 24)
                .Select(i => ven.FirstOrDefault(v => v.Date == $"{i:D2}:00") ?? new ChartOrderDto
                {
                    Date = $"{i:D2}:00",
                    OrderCount = 0,
                    TotalAmount = 0,
                    QuantityFood = 0
                })
                .ToList();

            var nue = new RevenueDto
            {
                Order = rev.Count(),
                Total = rev.Sum(o => o.Total ?? 0),
                QuantityFood = rev.Sum(r => r.OrderDetails?.Sum(od => od.Qty) ?? 0),
                ChartOrders = fullChartOrders
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
                .OrderBy(c => DateTime.ParseExact(c.Date, "dd/MM", CultureInfo.InvariantCulture))
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
                .GroupBy(r => r.OrderDate.Date)
                .Select(e => new ChartOrderDto
                {
                    Date = e.Key.ToString("dd/MM"),
                    OrderCount = e.Count(),
                    TotalAmount = e.Sum(o => o.Total ?? 0),
                    QuantityFood = e.Sum(o => o.OrderDetails.Sum(od => od.Qty))
                })
                .OrderBy(c => DateTime.ParseExact(c.Date, "dd/MM", CultureInfo.InvariantCulture))
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
                .GroupBy(r => r.OrderDate.Month)
                .Select(e => new ChartOrderDto
                {
                    Date = new DateTime(date.Year, e.Key, 1).ToString("MMM", CultureInfo.InvariantCulture),
                    OrderCount = e.Count(),
                    TotalAmount = e.Sum(o => o.Total ?? 0),
                    QuantityFood = e.Sum(o => o.OrderDetails.Sum(od => od.Qty))
                })
                .OrderBy(c => DateTime.ParseExact(c.Date, "MMM", CultureInfo.InvariantCulture))
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