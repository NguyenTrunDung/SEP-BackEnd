using HOMMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace HOMMS.Infrastructure.Services
{
    /// <summary>
    /// Implementation of IBranchContext to manage branch context during request processing
    /// </summary>
    public class BranchContext : IBranchContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string BranchIdClaimType = "BranchId";
        private const string BranchCookieName = "HOMMS.BranchId";
        
        public BranchContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        /// <inheritdoc/>
        public int GetCurrentBranchId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            // First try to get from the user claims (for authenticated users)
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var branchClaim = httpContext.User.FindFirst(BranchIdClaimType);
                if (branchClaim != null && int.TryParse(branchClaim.Value, out int branchId))
                {
                    return branchId;
                }
            }
            
            // Then try to get from request items (set by middleware)
            if (httpContext?.Items.TryGetValue("CurrentBranchId", out var branchIdObj) == true)
            {
                if (branchIdObj is int branchId)
                {
                    return branchId;
                }
            }
            
            // Then try to get from cookies (for guests/unauthenticated users)
            if (httpContext?.Request.Cookies.TryGetValue(BranchCookieName, out var branchIdCookie) == true)
            {
                if (int.TryParse(branchIdCookie, out int branchId))
                {
                    // Store in current request context for future use
                    if (httpContext != null)
                    {
                        httpContext.Items["CurrentBranchId"] = branchId;
                    }
                    return branchId;
                }
            }
            
            // If no branch context is found, return default branch (typically main branch with ID 1)
            // This can be customized based on business requirements
            return 1; // Default branch ID instead of throwing an exception
        }
        
        /// <inheritdoc/>
        public void SetCurrentBranchId(int branchId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Store in request items for current request
                httpContext.Items["CurrentBranchId"] = branchId;
                
                // Store in cookies for guests/unauthenticated users and persistence across requests
                var cookieOptions = new CookieOptions
                {
                    // Set cookie to expire in 30 days
                    Expires = DateTime.UtcNow.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                };
                
                httpContext.Response.Cookies.Delete(BranchCookieName);
                httpContext.Response.Cookies.Append(BranchCookieName, branchId.ToString(), cookieOptions);
            }
        }
    }
} 