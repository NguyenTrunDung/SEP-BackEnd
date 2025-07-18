using HOMMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for branch (tenant) operations
    /// </summary>
    public interface IBranchRepository : IRepository<Branch, int>
    {
        /// <summary>
        /// Gets all active branches
        /// </summary>
        /// <returns>Collection of active branches</returns>
        Task<IEnumerable<Branch>> GetActiveBranchesAsync();
        
        /// <summary>
        /// Gets branches for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Collection of branches the user belongs to</returns>
        Task<IEnumerable<Branch>> GetUserBranchesAsync(string userId);
        
        /// <summary>
        /// Gets the default branch for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Default branch for the user, or null if none is set</returns>
        Task<Branch> GetUserDefaultBranchAsync(string userId);
        
        /// <summary>
        /// Adds a user to a branch
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="branchId">The branch ID</param>
        /// <param name="isDefault">Whether this should be the user's default branch</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> AddUserToBranchAsync(string userId, int branchId, bool isDefault = false);
        
        /// <summary>
        /// Removes a user from a branch
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="branchId">The branch ID</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> RemoveUserFromBranchAsync(string userId, int branchId);
        
        /// <summary>
        /// Sets a branch as the default for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="branchId">The branch ID</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> SetUserDefaultBranchAsync(string userId, int branchId);
    }
} 