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

        /// <summary>
        /// Checks if a branch with the given code exists (excluding soft deleted branches)
        /// Note: Code is now optional, so this method may not be needed in the future
        /// </summary>
        /// <param name="code">Branch code to check</param>
        /// <param name="excludeId">Branch ID to exclude from check (for updates)</param>
        /// <returns>True if branch code exists, false otherwise</returns>
        [Obsolete("Code is now optional and not used for uniqueness validation. Use ExistsByNameAsync instead.")]
        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);

        /// <summary>
        /// Checks if a branch with the given name exists (excluding soft deleted branches)
        /// </summary>
        /// <param name="name">Branch name to check</param>
        /// <param name="excludeId">Branch ID to exclude from check (for updates)</param>
        /// <returns>True if branch name exists, false otherwise</returns>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        /// <summary>
        /// Gets a branch by ID including soft deleted branches (for restoration)
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <returns>Branch if found, null otherwise</returns>
        Task<Branch> GetByIdIncludingDeletedAsync(int id);

        /// <summary>
        /// Restores a soft deleted branch
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <param name="restoredBy">User who restored the branch</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> RestoreAsync(int id, string restoredBy);
    }
} 