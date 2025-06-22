using Asp.Versioning;
using AutoMapper;
using HOMMS.Application.Implementations;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOMMS.API.Controllers.V2
{

    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class SystemLogController : ControllerBase
    {

        private readonly ISystemLogService _systemLogService;
        private readonly IMapper _mapper;

        private readonly UserManager<ApplicationUser> _userManager;
        public SystemLogController(ISystemLogService systemLogService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {

            _systemLogService = systemLogService;
            _mapper = mapper;
            _userManager = userManager;
        }



        [HttpGet("Branch/{branchId}")]
        //[Authorize(Policy = "Permission:systemlog:view")]
        public async Task<ActionResult<ApiResponseBase<List<SystemLogDto>>> >GetSystemLogList([FromRoute] int branchId, [FromQuery] DateTime dateStart, [FromQuery] DateTime dateEnd)//nhập theo kiểu YY-MM-DD
        {
            try
            {
                var re = await _systemLogService.GetSystemLogListByBranchId(branchId, dateStart, dateEnd);
                if (!re.Any())
                {
                    return NotFound(new ApiResponseBase<List<SystemLogDto>>(null, "SystemLog not found", "error"));
                }

                var sy = _mapper.Map<List<SystemLogDto>>(re);
                var totalCount = sy.Count;

                return Ok(new ApiResponseBase<List<SystemLogDto>>(sy, "SystemLog retrieved successfully","success" ,totalCount));
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }


        [HttpPost]
        //[Authorize(Policy = "Permission:systemlog:view")]
        public async Task<IActionResult> AddSystemLog([FromForm] AddSystemLogDto dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            var user = await _userManager.FindByIdAsync(userId);
            await _systemLogService.LogAsync(dto.BranchId, user?.FullName ?? "Unknown", dto.Note, dto.Date);

            var sys = await _systemLogService.AddSystemLog(dto);
            return Ok( new ApiResponseBase<AddSystemLogDto>(sys, "Log created successfully"));
        }
    }
}
