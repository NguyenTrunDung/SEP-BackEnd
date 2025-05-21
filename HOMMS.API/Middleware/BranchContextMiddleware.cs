using HOMMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.API.Middleware
{
    /// <summary>
    /// Middleware to set branch context for each request
    /// </summary>
    public class BranchContextMiddleware
    {
        private readonly RequestDelegate _next;
        
        public BranchContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context, IBranchContext branchContext)
        {
            bool branchSet = false;
            
            // Option 1: Extract from route data
            if (context.Request.RouteValues.TryGetValue("branchId", out var branchIdFromRoute) && 
                int.TryParse(branchIdFromRoute?.ToString(), out int branchId))
            {
                branchContext.SetCurrentBranchId(branchId);
                branchSet = true;
            }
            // Option 2: Extract from header
            else if (context.Request.Headers.TryGetValue("X-Branch-Id", out var branchIdHeader) && 
                    int.TryParse(branchIdHeader.FirstOrDefault(), out branchId))
            {
                branchContext.SetCurrentBranchId(branchId);
                branchSet = true;
            }
            // Option 3: Extract from query string (for guest users or direct links)
            else if (context.Request.Query.TryGetValue("branchId", out var branchIdQuery) &&
                    int.TryParse(branchIdQuery.FirstOrDefault(), out branchId))
            {
                branchContext.SetCurrentBranchId(branchId);
                branchSet = true;
            }
            
            // If branch wasn't set from request, try to get existing branch context
            // This will use cookies for guests or claims for authenticated users
            if (!branchSet)
            {
                try
                {
                    // This will use the implementation in BranchContext that checks
                    // claims, cookies, etc. and doesn't throw an exception
                    var currentBranchId = branchContext.GetCurrentBranchId();
                }
                catch
                {
                    // Ignore errors, as branch context might not be available
                }
            }
            
            await _next(context);
        }
    }
} 