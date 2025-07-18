namespace HOMMS.Application.Interfaces
{
    /// <summary>
    /// Interface for accessing branch (tenant) context information
    /// </summary>
    public interface IBranchContext
    {
        /// <summary>
        /// Gets the current branch ID from the execution context
        /// </summary>
        /// <returns>The ID of the current branch</returns>
        int GetCurrentBranchId();
        
        /// <summary>
        /// Sets the current branch ID for the execution context
        /// </summary>
        /// <param name="branchId">The branch ID to set</param>
        void SetCurrentBranchId(int branchId);
    }
} 