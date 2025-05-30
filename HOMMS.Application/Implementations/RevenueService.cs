using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
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

            var group = rev
                .GroupBy(r => r.OrderDate.Date)
                .Select(e => new RevenueDto
                {
                    RDate = e.Key,
                    RTotal = e.Sum(r => r.Total ?? 0)
                });

            return group;
        }

        public async Task<IEnumerable<RevenueDto>> GetRevenueByWeekAsync(int branchId, DateTime date)
        {
            var rev = await _revenueRepository.GetRevenueByWeekAsync(branchId, date);


            var group = rev
                .GroupBy(r => r.OrderDate)
                .Select(e => new RevenueDto
                {
                    RDate = e.Key,
                    RTotal = e.Sum(r => r.Total ?? 0)
                })
                    .OrderBy(r => r.RDate);

            return group;
        }

        public async Task<IEnumerable<RevenueDto>> GetRevenueByMonthAsync(int branchId, DateTime date)
        {
            var rev = await _revenueRepository.GetRevenueByMonthAsync(branchId, date);

            var group = rev
                .GroupBy(r => new { r.OrderDate.Year, r.OrderDate.Month })
                .Select(e => new RevenueDto
                {
                    RDate = new DateTime(e.Key.Year, e.Key.Month, 1),
                    RTotal = e.Sum(r => r.Total ?? 0)
                });
            return group;
        }



        public async Task<IEnumerable<RevenueDto>> GetRevenueByYearAsync(int branchId, DateTime date)
        {
            var rev = await _revenueRepository.GetRevenueByYearAsync(branchId, date);

            var group = rev
                .GroupBy(r => r.OrderDate.Year)
                .Select(e => new RevenueDto
                {
                    RDate = new DateTime(e.Key, 1, 1),
                    RTotal = e.Sum(r => r.Total ?? 0)
                });
            return group;
        }



    }


}
