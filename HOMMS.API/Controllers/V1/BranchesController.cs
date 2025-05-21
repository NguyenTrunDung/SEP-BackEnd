using HOMMS.Application.Interfaces;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IBranchContext _branchContext;
        
        public BranchesController(
            IBranchRepository branchRepository,
            IBranchContext branchContext)
        {
            _branchRepository = branchRepository;
            _branchContext = branchContext;
        }
        
        /// <summary>
        /// Gets all branches for the current user if authenticated, or all active branches if not
        /// </summary>
        /// <returns>List of branches</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
        {
            // For authenticated users, get their associated branches
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var userBranches = await _branchRepository.GetUserBranchesAsync(userId);
                    return Ok(userBranches);
                }
            }
            
            // For guests or if user has no branches, return all active branches
            var activeBranches = await _branchRepository.GetActiveBranchesAsync();
            return Ok(activeBranches);
        }
        
        /// <summary>
        /// Gets the default branch for the current user if authenticated, or the current branch if not
        /// </summary>
        /// <returns>Default/current branch</returns>
        [HttpGet("default")]
        public async Task<ActionResult<Branch>> GetDefaultBranch()
        {
            Branch branch = null;
            
            // For authenticated users, get their default branch
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    branch = await _branchRepository.GetUserDefaultBranchAsync(userId);
                }
            }
            
            // If no default branch found, try to get current branch from context
            if (branch == null)
            {
                int branchId = _branchContext.GetCurrentBranchId();
                branch = await _branchRepository.GetByIdAsync(branchId);
            }
            
            // If still no branch found, return the first active branch
            if (branch == null)
            {
                var activeBranches = await _branchRepository.GetActiveBranchesAsync();
                branch = activeBranches.FirstOrDefault();
            }
            
            if (branch == null)
            {
                return NotFound("No available branches found");
            }
            
            return Ok(branch);
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
            // Verify the branch exists and is active
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null || !branch.IsActive)
            {
                return NotFound("Branch not found or inactive");
            }
            
            // For authenticated users, check if they have access to this branch
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var userBranches = await _branchRepository.GetUserBranchesAsync(userId);
                    
                    // If user has specific branches and this one isn't in the list, forbid access
                    if (userBranches.Any() && !userBranches.Any(b => b.Id == branchId))
                    {
                        return Forbid();
                    }
                    
                    // Set as default for the user
                    await _branchRepository.SetUserDefaultBranchAsync(userId, branchId);
                }
            }
            
            // For both authenticated and guest users, set the branch in context
            _branchContext.SetCurrentBranchId(branchId);
            
            return Ok(new { message = "Current branch set successfully", branch = branch });
        }
        
        /// <summary>
        /// Gets information about the current branch context
        /// Works for both authenticated and guest users
        /// </summary>
        /// <returns>Current branch</returns>
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentBranch()
        {
            try
            {
                var branchId = _branchContext.GetCurrentBranchId();
                var branch = await _branchRepository.GetByIdAsync(branchId);
                
                if (branch == null || !branch.IsActive)
                {
                    // If branch doesn't exist or is inactive, return first active branch
                    var activeBranches = await _branchRepository.GetActiveBranchesAsync();
                    branch = activeBranches.FirstOrDefault();
                    
                    if (branch != null)
                    {
                        // Update context with valid branch
                        _branchContext.SetCurrentBranchId(branch.Id);
                    }
                }
                
                if (branch == null)
                {
                    return NotFound("No available branches found");
                }
                
                return Ok(branch);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 