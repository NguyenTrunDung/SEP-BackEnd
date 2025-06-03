using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/public/revenues")]
    public class RevenueController : ControllerBase
    {

        private readonly IRevenueService _revenueService;
        private readonly IMapper _mapper;

        public RevenueController(IRevenueService revenueService, IMapper mapper)
        {

            _revenueService = revenueService;
            _mapper = mapper;
        }

        [HttpGet("Branch/{branchId}/Day/{date}")]
        [Authorize(Policy = "Permission:orders:view")]
        public async Task<IActionResult> GetRevenueByDayAsync(int branchId, DateTime date)//nhập theo kiểu YY-MM-DD
        {
            var re = await _revenueService.GetRevenueByDayAsync(branchId, date);
            if (!re.Any())
            {
                return NotFound(new ApiResponseBase<IEnumerable<RevenueDto>>(null, "Revenues not found", "error"));
            }

            return Ok(new ApiResponseBase<IEnumerable<RevenueDto>>(re, "Revenues by day retrieved successfully"));
        }


        [HttpGet("Branch/{branchId}/Week/{date}")]
        [Authorize(Policy = "Permission:orders:view")]
        public async Task<IActionResult> GetRevenueByWeekAsync(int branchId, DateTime date)
        {
            var re = await _revenueService.GetRevenueByWeekAsync(branchId, date);
            if (!re.Any())
            {
                return NotFound(new ApiResponseBase<IEnumerable<RevenueDto>>(null, "Revenues not found", "error"));
            }

            return Ok(new ApiResponseBase<IEnumerable<RevenueDto>>(re, "Revenues by week retrieved successfully"));
        }


        [HttpGet("Branch/{branchId}/Month/{date}")]
        [Authorize(Policy = "Permission:orders:view")]
        public async Task<IActionResult> GetRevenueByMonthAsync(int branchId, DateTime date)
        {
            var re = await _revenueService.GetRevenueByMonthAsync(branchId, date);
            if (!re.Any())
            {
                return NotFound(new ApiResponseBase<IEnumerable<RevenueDto>>(null, "Revenues not found", "error"));
            }

            return Ok(new ApiResponseBase<IEnumerable<RevenueDto>>(re, "Revenues by month retrieved successfully"));
        }


        [HttpGet("Branch/{branchId}/Year/{date}")]
        [Authorize(Policy = "Permission:orders:view")]
        public async Task<IActionResult> GetRevenueByYearAsync(int branchId, DateTime date)
        {
            var re = await _revenueService.GetRevenueByYearAsync(branchId, date);
            if (!re.Any())
            {
                return NotFound(new ApiResponseBase<IEnumerable<RevenueDto>>(null, "Revenues not found", "error"));
            }

            return Ok(new ApiResponseBase<IEnumerable<RevenueDto>>(re, "Revenues by year retrieved successfully"));
        }


    }
}
