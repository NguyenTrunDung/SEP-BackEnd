using AutoMapper;
using HOMMS.Application.Implementations;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/public/SystemLog")]
    public class SystemLogController : ControllerBase
    {

        private readonly ISystemLogService _systemLogService;
        private readonly IMapper _mapper;

        public SystemLogController(ISystemLogService systemLogService, IMapper mapper)
        {

            _systemLogService = systemLogService;
            _mapper = mapper;
        }



        [HttpGet("Branch/{branchId}")]
        //[Authorize(Policy = "Permission:systemlog:view")]
        public async Task<IActionResult> GetSystemLog(int branchId, DateTime dateStart, DateTime dateEnd)//nhập theo kiểu YY-MM-DD
        {
            try
            {
                var re = await _systemLogService.GetSystemLogAll(branchId, dateStart, dateEnd);
                if (!re.Any())
                {
                    return NotFound(new ApiResponseBase<IEnumerable<SystemLogDto>>(null, "SystemLog not found", "error"));
                }

                return Ok(new ApiResponseBase<IEnumerable<SystemLogDto>>(re, "SystemLog retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }


        [HttpPost]
        //[Authorize(Policy = "Permission:systemlog:view")]
        public async Task<IActionResult> AddSystemLog([FromBody] AddSystemLogDto dto)
        {
           var sys = await _systemLogService.AddSystemLog(dto);
            return Ok( new ApiResponseBase<AddSystemLogDto>(sys, "Log created successfully"));
        }
    }
}
