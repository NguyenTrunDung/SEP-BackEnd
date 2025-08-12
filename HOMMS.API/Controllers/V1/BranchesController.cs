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
using System;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;
        
        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
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
                    var isSystemAdmin = User.IsInRole("SystemAdmin");
                    var branchDtos = await _branchService.GetBranchesForUserAsync(userId, isSystemAdmin);
                    var totalCount = branchDtos.Count;
                    
                    var message = isSystemAdmin 
                        ? "All branches retrieved successfully" 
                        : "User branches retrieved successfully";
                    
                    return Ok(new ApiResponseBase<List<BranchDto>>(branchDtos, message, "success", totalCount));
                }
            }
            
            // For unauthenticated users, return all active branches
            var activeBranchDtos = await _branchService.GetActiveBranchesAsync();
            var totalCountActive = activeBranchDtos.Count;
            return Ok(new ApiResponseBase<List<BranchDto>>(activeBranchDtos, "Active branches retrieved successfully", "success", totalCountActive));
        }

        /// <summary>
        /// Gets a specific branch by ID
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <returns>Branch details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseBase<BranchDto>>> GetBranchById(int id)
        {
            var branchDto = await _branchService.GetByIdAsync(id);
            
            if (branchDto == null)
            {
                return NotFound(new ApiResponseBase<BranchDto>(null, "Branch not found", "error"));
            }
            
            return Ok(new ApiResponseBase<BranchDto>(branchDto, "Branch retrieved successfully"));
        }
        
        /// <summary>
        /// Gets the default branch for the current user if authenticated, or the current branch if not
        /// </summary>
        /// <returns>Default/current branch</returns>
        [HttpGet("default")]
        public async Task<ActionResult<ApiResponseBase<BranchDto>>> GetDefaultBranch()
        {
            BranchDto branchDto = null;
            
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    branchDto = await _branchService.GetDefaultBranchAsync(userId);
                }
            }
            
            if (branchDto == null)
            {
                branchDto = await _branchService.GetCurrentBranchAsync();
            }
            
            if (branchDto == null)
            {
                return NotFound(new ApiResponseBase<BranchDto>(null, "No available branches found", "error"));
            }
            
            return Ok(new ApiResponseBase<BranchDto>(branchDto, "Default branch retrieved successfully"));
        }
        
        /// <summary>
        /// Creates a new branch
        /// </summary>
        /// <param name="createDto">Branch creation data</param>
        /// <returns>Created branch</returns>
        [HttpPost]
       // [Authorize(Roles = "SystemAdmin")]
        public async Task<ActionResult<ApiResponseBase<BranchDto>>> CreateBranch([FromBody] CreateBranchDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponseBase<BranchDto>(null, "Invalid input data", "error"));
                }

                var createdBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var branchDto = await _branchService.CreateBranchAsync(createDto, createdBy);
                
                return CreatedAtAction(
                    nameof(GetBranchById),
                    new { id = branchDto.Id },
                    new ApiResponseBase<BranchDto>(branchDto, "Branch created successfully")
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseBase<BranchDto>(null, ex.Message, "error"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseBase<BranchDto>(null, "An error occurred while creating the branch", "error"));
            }
        }

        /// <summary>
        /// Updates an existing branch
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <param name="updateDto">Branch update data</param>
        /// <returns>Updated branch</returns>
        [HttpPut("{id}")]
       // [Authorize(Roles = "SystemAdmin")]
        public async Task<ActionResult<ApiResponseBase<BranchDto>>> UpdateBranch(int id, [FromBody] UpdateBranchDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponseBase<BranchDto>(null, "Invalid input data", "error"));
                }

                var updatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var branchDto = await _branchService.UpdateBranchAsync(id, updateDto, updatedBy);
                
                return Ok(new ApiResponseBase<BranchDto>(branchDto, "Branch updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseBase<BranchDto>(null, ex.Message, "error"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseBase<BranchDto>(null, "An error occurred while updating the branch", "error"));
            }
        }

        /// <summary>
        /// Soft deletes a branch
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <returns>Success result</returns>
        [HttpDelete("{id}")]
       // [Authorize(Roles = "SystemAdmin")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteBranch(int id)
        {
            try
            {
                var deletedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var success = await _branchService.DeleteBranchAsync(id, deletedBy);
                
                if (!success)
                {
                    return NotFound(new ApiResponseBase<object>(null, "Branch not found", "error"));
                }
                
                return Ok(new ApiResponseBase<object>(null, "Branch deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseBase<object>(null, "An error occurred while deleting the branch", "error"));
            }
        }

        /// <summary>
        /// Restores a soft deleted branch
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <returns>Success result</returns>
        [HttpPost("{id}/restore")]
        //[Authorize(Roles = "SystemAdmin")]
        public async Task<ActionResult<ApiResponseBase<object>>> RestoreBranch(int id)
        {
            try
            {
                var restoredBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var success = await _branchService.RestoreBranchAsync(id, restoredBy);
                
                if (!success)
                {
                    return NotFound(new ApiResponseBase<object>(null, "Branch not found or not deleted", "error"));
                }
                
                return Ok(new ApiResponseBase<object>(null, "Branch restored successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseBase<object>(null, ex.Message, "error"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseBase<object>(null, "An error occurred while restoring the branch", "error"));
            }
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
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var isSystemAdmin = User.IsInRole("SystemAdmin");
                    var branchDto = await _branchService.ValidateAndSetCurrentBranchAsync(branchId, userId, isSystemAdmin);
                    
                    if (branchDto == null)
                    {
                        if (isSystemAdmin)
                        {
                            return NotFound(new ApiResponseBase<BranchDto>(null, "Branch not found or inactive", "error"));
                        }
                        else
                        {
                            return Forbid();
                        }
                    }
                    
                    var message = isSystemAdmin 
                        ? "Admin System should use the dashboard dropdown to switch branches. Branch context set for this request."
                        : "Current branch set successfully";
                    
                    return Ok(new ApiResponseBase<BranchDto>(branchDto, message));
                }
            }
            
            // For unauthenticated users, just validate and set the branch context
            var branch = await _branchService.GetByIdAsync(branchId);
            if (branch == null)
            {
                return NotFound(new ApiResponseBase<BranchDto>(null, "Branch not found or inactive", "error"));
            }
            
            _branchService.SetCurrentBranchId(branchId);
            return Ok(new ApiResponseBase<BranchDto>(branch, "Current branch set successfully"));
        }
        
        /// <summary>
        /// Gets information about the current branch context
        /// Works for both authenticated and guest users
        /// </summary>
        /// <returns>Current branch</returns>
        [HttpGet("current")]
        public async Task<ActionResult<ApiResponseBase<BranchDto>>> GetCurrentBranch()
        {
            var branchDto = await _branchService.GetCurrentBranchAsync();
            
            if (branchDto == null)
            {
                return NotFound(new ApiResponseBase<BranchDto>(null, "No available branches found", "error"));
            }
            
            return Ok(new ApiResponseBase<BranchDto>(branchDto, "Current branch retrieved successfully"));
        }

        /// <summary>
        /// Checks if a branch name is available for use
        /// </summary>
        /// <param name="name">Branch name to check</param>
        /// <param name="excludeId">Branch ID to exclude from check (for updates)</param>
        /// <returns>True if name is available</returns>
        [HttpGet("check-name-availability")]
        public async Task<ActionResult<ApiResponseBase<object>>> CheckNameAvailability(
            [FromQuery] string name, 
            [FromQuery] int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new ApiResponseBase<object>(null, "Name parameter is required", "error"));
                }

                var isAvailable = await _branchService.IsBranchNameAvailableAsync(name, excludeId);
                var message = isAvailable ? "Branch name is available" : "Branch name already exists";
                
                return Ok(new ApiResponseBase<object>(isAvailable, message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseBase<object>(null, $"Error checking name availability: {ex.Message}", "error"));
            }
        }
    }
} 
