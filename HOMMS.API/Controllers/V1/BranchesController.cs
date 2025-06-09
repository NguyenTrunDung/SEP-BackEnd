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
using Asp.Versioning;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository; //sai
        private readonly IBranchService _branchService;
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
                    var totalCount = userBranchDtos.Count;
                    return Ok(new ApiResponseBase<List<BranchDto>>(userBranchDtos, "User branches retrieved successfully", "success", totalCount));
                }
            }
            var activeBranches = await _branchRepository.GetActiveBranchesAsync();
            var activeBranchDtos = _mapper.Map<List<BranchDto>>(activeBranches);
            var totalCountActive = activeBranchDtos.Count;
            return Ok(new ApiResponseBase<List<BranchDto>>(activeBranchDtos, "Active branches retrieved successfully", "success", totalCountActive));
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
                    // Check if user is Admin System
                    if (User.IsInRole("Admin"))
                    {
                        // Optionally, return a message for Admin System
                        _branchContext.SetCurrentBranchId(branchId);
                        var branchDto = _mapper.Map<BranchDto>(branch);
                        return Ok(new ApiResponseBase<BranchDto>(branchDto, "Admin System should use the dashboard dropdown to switch branches. Branch context set for this request."));
                    }
                    var userBranches = await _branchRepository.GetUserBranchesAsync(userId);
                    if (userBranches.Any() && !userBranches.Any(b => b.Id == branchId))
                    {
                        return Forbid();
                    }
                    await _branchRepository.SetUserDefaultBranchAsync(userId, branchId);
                }
            }
            _branchContext.SetCurrentBranchId(branchId);
            var branchDto2 = _mapper.Map<BranchDto>(branch);
            return Ok(new ApiResponseBase<BranchDto>(branchDto2, "Current branch set successfully"));
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

        /// <summary>
        /// Example: Only users with 'orders:add' permission can access this endpoint
        /// </summary>
        [Authorize(Policy = "Permission:orders:add")]
        [HttpGet("secure-action")]
        public IActionResult SecureAction()
        {
            return Ok(new ApiResponseBase<string>("You have 'orders:add' permission!", "Permission check successful"));
        }

        /// <summary>
        /// Get the current Admin System user for a specific branch
        /// </summary>
        [Authorize(Policy = "Permission:users:views")]
        [HttpGet("admin-system-user")]
        public async Task<ActionResult<ApiResponseBase<string>>> GetAdminSystemUser(
            [FromQuery] string branchCode,
            [FromServices] IRepository<BranchUserRole, int> branchUserRoleRepo,
            [FromServices] IRepository<BranchRole, int> branchRoleRepo,
            [FromServices] IRepository<ApplicationUser, string> userRepo,
            [FromServices] IRepository<Branch, int> branchRepo)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
                return BadRequest(new ApiResponseBase<string>(null, "Missing branchCode parameter", "error"));
            var branch = (await branchRepo.GetByAsync(b => b.Code == branchCode)).FirstOrDefault();
            if (branch == null)
                return NotFound(new ApiResponseBase<string>(null, $"Không tìm thấy branch với code {branchCode}", "error"));
            var adminSystemRole = (await branchRoleRepo.GetByAsync(r => r.Name == "Admin System" && r.BranchId == branch.Id)).FirstOrDefault();
            if (adminSystemRole == null)
                return NotFound(new ApiResponseBase<string>(null, "Không tìm thấy role Admin System", "error"));
            var adminAssignment = (await branchUserRoleRepo.GetByAsync(bur => bur.BranchRoleId == adminSystemRole.Id && bur.BranchId == branch.Id)).FirstOrDefault();
            if (adminAssignment == null)
                return NotFound(new ApiResponseBase<string>(null, "Không có user nào được gán Admin System", "error"));
            var user = await userRepo.GetByIdAsync(adminAssignment.UserId);
            return Ok(new ApiResponseBase<string>(user?.Email, "Admin System user hiện tại"));
        }

        /// <summary>
        /// Attempt to assign Admin System to another user (should be forbidden)
        /// </summary>
        [Authorize(Policy = "Permission:users:edit")]
        [HttpPost("assign-admin-system/{userId}")]
        public async Task<ActionResult<ApiResponseBase<string>>> AssignAdminSystem(
            string userId,
            [FromQuery] string branchCode,
            [FromServices] IRepository<BranchUserRole, int> branchUserRoleRepo,
            [FromServices] IRepository<BranchRole, int> branchRoleRepo,
            [FromServices] IRepository<Branch, int> branchRepo)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
                return BadRequest(new ApiResponseBase<string>(null, "Missing branchCode parameter", "error"));
            var branch = (await branchRepo.GetByAsync(b => b.Code == branchCode)).FirstOrDefault();
            if (branch == null)
                return NotFound(new ApiResponseBase<string>(null, $"Không tìm thấy branch với code {branchCode}", "error"));
            var adminSystemRole = (await branchRoleRepo.GetByAsync(r => r.Name == "Admin System" && r.BranchId == branch.Id)).FirstOrDefault();
            if (adminSystemRole == null)
                return NotFound(new ApiResponseBase<string>(null, "Không tìm thấy role Admin System", "error"));
            // Check if this user is already the admin system
            var adminAssignment = (await branchUserRoleRepo.GetByAsync(bur => bur.BranchRoleId == adminSystemRole.Id && bur.BranchId == branch.Id)).FirstOrDefault();
            if (adminAssignment != null && adminAssignment.UserId != userId)
            {
                return Forbid();
            }
            if (adminAssignment != null && adminAssignment.UserId == userId)
            {
                return Ok(new ApiResponseBase<string>(userId, "User này đã là Admin System"));
            }
            // If no assignment exists, allow (for initial setup only)
            await branchUserRoleRepo.AddAsync(new BranchUserRole
            {
                UserId = userId,
                BranchId = branch.Id,
                BranchRoleId = adminSystemRole.Id,
                CreatedAt = System.DateTime.UtcNow
            });
            return Ok(new ApiResponseBase<string>(userId, "Đã gán Admin System cho user (chỉ khi chưa có ai)", "success"));
        }

        /// <summary>
        /// List all users with Admin System role for a specific branch
        /// </summary>
        [Authorize(Policy = "Permission:users:views")]
        [HttpGet("admin-system-users")]
        public async Task<ActionResult<ApiResponseBase<List<string>>>> ListAdminSystemUsers(
            [FromQuery] string branchCode,
            [FromServices] IRepository<BranchUserRole, int> branchUserRoleRepo,
            [FromServices] IRepository<BranchRole, int> branchRoleRepo,
            [FromServices] IRepository<ApplicationUser, string> userRepo,
            [FromServices] IRepository<Branch, int> branchRepo)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
                return BadRequest(new ApiResponseBase<List<string>>(null, "Missing branchCode parameter", "error"));
            var branch = (await branchRepo.GetByAsync(b => b.Code == branchCode)).FirstOrDefault();
            if (branch == null)
                return NotFound(new ApiResponseBase<List<string>>(null, $"Không tìm thấy branch với code {branchCode}", "error"));
            var adminSystemRole = (await branchRoleRepo.GetByAsync(r => r.Name == "Admin System" && r.BranchId == branch.Id)).FirstOrDefault();
            if (adminSystemRole == null)
                return NotFound(new ApiResponseBase<List<string>>(null, "Không tìm thấy role Admin System", "error"));
            var adminAssignments = await branchUserRoleRepo.GetByAsync(bur => bur.BranchRoleId == adminSystemRole.Id && bur.BranchId == branch.Id);
            var emails = new List<string>();
            foreach (var assignment in adminAssignments)
            {
                var user = await userRepo.GetByIdAsync(assignment.UserId);
                if (user != null)
                    emails.Add(user.Email);
            }
            return Ok(new ApiResponseBase<List<string>>(emails, "Danh sách user có quyền Admin System"));
        }
    }
} 
