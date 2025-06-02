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

            var ven = new ChartOrderDto
            {
                Date = date.ToString("dd/MM"),
                OrderCount = rev.Count(),
                TotalAmount = rev.Sum(o => o.Total ?? 0),
                QuantityFood = rev.Sum(o => o.OrderDetails.Sum(od => od.Qty))
            };
                

            var nue = new RevenueDto
            {
                Order = rev.Count(),
                Total = rev.Sum(o => o.Total ?? 0),
                QuantityFood = rev.Sum(r => r.OrderDetails.Sum(re => re.Qty)),
                ChartOrders = new List<ChartOrderDto> { ven }
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
                .GroupBy(r => r.OrderDate.Month )
              .Select(e => new ChartOrderDto
              {
                  Date = new DateTime(date.Year, e.Key, 1).ToString("MM/yyyy"),
                  OrderCount = e.Count(),
                  TotalAmount = e.Sum(o => o.Total ?? 0),
                  QuantityFood = e.Sum(o => o.OrderDetails.Sum(od => od.Qty))
              })
                .OrderBy(c => DateTime.ParseExact(c.Date, "MM/yyyy", null))
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
                .GroupBy(r => r.OrderDate.Year)
               .Select(e => new ChartOrderDto
               {
                   Date = e.Key.ToString(),
                   OrderCount = e.Count(),
                   TotalAmount = e.Sum(o => o.Total ?? 0),
                   QuantityFood = e.Sum(o => o.OrderDetails.Sum(od => od.Qty))
               })
                .OrderBy(c => int.Parse(c.Date))
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
