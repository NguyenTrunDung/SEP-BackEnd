using HOMMS.Application.Interfaces;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IBranchContext _branchContext;
        private readonly IMapper _mapper;
        
        public BranchesController(
            IBranchRepository branchRepository,
            IBranchContext branchContext,
            IMapper mapper)
        {
            _branchRepository = branchRepository;
            _branchContext = branchContext;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Gets all branches for the current user if authenticated, or all active branches if not
        /// </summary>
        /// <returns>List of branches</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponseBase<List<BranchDto>>>> GetBranches()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var userBranches = await _branchRepository.GetUserBranchesAsync(userId);
                    var userBranchDtos = _mapper.Map<List<BranchDto>>(userBranches);
                    return Ok(new ApiResponseBase<List<BranchDto>>(userBranchDtos, "User branches retrieved successfully"));
                }
            }
            var activeBranches = await _branchRepository.GetActiveBranchesAsync();
            var activeBranchDtos = _mapper.Map<List<BranchDto>>(activeBranches);
            return Ok(new ApiResponseBase<List<BranchDto>>(activeBranchDtos, "Active branches retrieved successfully"));
        }
        
        /// <summary>
        /// Gets the default branch for the current user if authenticated, or the current branch if not
        /// </summary>
        /// <returns>Default/current branch</returns>
        [HttpGet("default")]
        public async Task<ActionResult<ApiResponseBase<BranchDto>>> GetDefaultBranch()
        {
            Branch branch = null;
            
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    branch = await _branchRepository.GetUserDefaultBranchAsync(userId);
                }
            }
            
            if (branch == null)
            {
                int branchId = _branchContext.GetCurrentBranchId();
                branch = await _branchRepository.GetByIdAsync(branchId);
            }
            
            if (branch == null)
            {
                var activeBranches = await _branchRepository.GetActiveBranchesAsync();
                branch = activeBranches.FirstOrDefault();
            }
            
            if (branch == null)
            {
                return NotFound(new ApiResponseBase<BranchDto>(null, "No available branches found", "error"));
            }
            
            var branchDto = _mapper.Map<BranchDto>(branch);
            return Ok(new ApiResponseBase<BranchDto>(branchDto, "Default branch retrieved successfully"));
        }
        
        /// <summary>
        /// Sets the current branch context for subsequent requests
        /// Works for both authenticated and guest users
        /// </summary>
        /// <param name="branchId">ID of the branch to set as current</param>
        /// <returns>Success or error result</returns>
        [HttpPost("set-current/{branchId}")]
        public async Task<IActionResult> SetCurrentBranch(int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null || !branch.IsActive)
            {
                return NotFound(new ApiResponseBase<BranchDto>(null, "Branch not found or inactive", "error"));
            }
            
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var userBranches = await _branchRepository.GetUserBranchesAsync(userId);
                    
                    if (userBranches.Any() && !userBranches.Any(b => b.Id == branchId))
                    {
                        return Forbid();
                    }
                    
                    await _branchRepository.SetUserDefaultBranchAsync(userId, branchId);
                }
            }
            
            _branchContext.SetCurrentBranchId(branchId);
            
            var branchDto = _mapper.Map<BranchDto>(branch);
            return Ok(new ApiResponseBase<BranchDto>(branchDto, "Current branch set successfully"));
        }
        
        /// <summary>
        /// Gets information about the current branch context
        /// Works for both authenticated and guest users
        /// </summary>
        /// <returns>Current branch</returns>
        [HttpGet("current")]
        public async Task<ActionResult<ApiResponseBase<BranchDto>>> GetCurrentBranch()
        {
            var branchId = _branchContext.GetCurrentBranchId();
            var branch = await _branchRepository.GetByIdAsync(branchId);
            
            if (branch == null || !branch.IsActive)
            {
                var activeBranches = await _branchRepository.GetActiveBranchesAsync();
                branch = activeBranches.FirstOrDefault();
                
                if (branch != null)
                {
                    _branchContext.SetCurrentBranchId(branch.Id);
                }
            }
            
            if (branch == null)
            {
                return NotFound(new ApiResponseBase<BranchDto>(null, "No available branches found", "error"));
            }
            
            var branchDto = _mapper.Map<BranchDto>(branch);
            return Ok(new ApiResponseBase<BranchDto>(branchDto, "Current branch retrieved successfully"));
        }
    }
} 