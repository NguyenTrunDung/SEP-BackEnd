using Asp.Versioning;
using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/public/revenues")]

    [ApiController]
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
        //[Authorize(Policy = "Permission:orders:view")]
        public async Task<ActionResult<ApiResponseBase<List<RevenueDto>>> >GetRevenueByDayAsync(int branchId, DateTime date)//nhập theo kiểu YY-MM-DD
        {
            var re = await _revenueService.GetRevenueByDayAsync(branchId, date);
            if (!re.Any())
            {
                return NotFound(new ApiResponseBase<List<RevenueDto>>(null, "Revenues not found", "error"));
            }
            var ve = _mapper.Map<List<RevenueDto>>(re);
            var count = ve.Count;

            return Ok(new  ApiResponseBase<List<RevenueDto>>(ve, "Revenues by day retrieved successfully","success",count));
        }


        [HttpGet("Branch/{branchId}/Week/{date}")]
        //[Authorize(Policy = "Permission:orders:view")]
        public async Task<ActionResult<ApiResponseBase<List<RevenueDto>>>> GetRevenueByWeekAsync(int branchId, DateTime date)
        {
            var re = await _revenueService.GetRevenueByWeekAsync(branchId, date);
            if (!re.Any())
            {
                return NotFound(new ApiResponseBase<List<RevenueDto>>(null, "Revenues not found", "error"));
            }
            var ve = _mapper.Map<List<RevenueDto>>(re);
            var count = ve.Count;

            return Ok(new ApiResponseBase<List<RevenueDto>>(ve, "Revenues by week retrieved successfully","success",count));
        }


        [HttpGet("Branch/{branchId}/Month/{date}")]
        //[Authorize(Policy = "Permission:orders:view")]
        public async Task<ActionResult<ApiResponseBase<List<RevenueDto>>>> GetRevenueByMonthAsync(int branchId, DateTime date)
        {
            var re = await _revenueService.GetRevenueByMonthAsync(branchId, date);
            if (!re.Any())
            {
                return NotFound(new ApiResponseBase<List<RevenueDto>>(null, "Revenues not found", "error"));
            }
            var ve = _mapper.Map<List<RevenueDto>>(re);
            var count = ve.Count;
            return Ok(new ApiResponseBase<List<RevenueDto>>(ve, "Revenues by month retrieved successfully","success",count));
        }


        [HttpGet("Branch/{branchId}/Year/{date}")]
        ////[Authorize(Policy = "Permission:orders:view")]
        public async Task<ActionResult<ApiResponseBase<List<RevenueDto>>>> GetRevenueByYearAsync(int branchId, DateTime date)
        {
            var re = await _revenueService.GetRevenueByYearAsync(branchId, date);
            if (!re.Any())
            {
                return NotFound(new ApiResponseBase<List<RevenueDto>>(null, "Revenues not found", "error"));
            }
            var ve = _mapper.Map<List<RevenueDto>>(re);
            var count = ve.Count;
            return Ok(new ApiResponseBase<List<RevenueDto>>(ve, "Revenues by year retrieved successfully", "success", count));
        }


    }
}
