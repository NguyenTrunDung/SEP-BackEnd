using HOMMS.Domain.Entities;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for user wallet operations
    /// </summary>
    public interface IUserWalletRepository : IRepository<UserWalletTransaction, int>
    {
        /// <summary>
        /// Gets wallet balance for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The current wallet balance</returns>
        Task<long> GetWalletBalanceAsync(string userId);
        
        /// <summary>
        /// Gets user wallet information
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>User wallet information</returns>
        Task<ApplicationUser?> GetUserWalletAsync(string userId);
        
        /// <summary>
        /// Gets wallet transaction history for a user with pagination
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Tuple of transactions and total count</returns>
        Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetTransactionHistoryAsync(
            string userId, int pageNumber, int pageSize);
        
        /// <summary>
        /// Gets deposit history for a user with pagination
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Tuple of deposit transactions and total count</returns>
        Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetDepositHistoryAsync(
            string userId, int pageNumber, int pageSize);
        
        /// <summary>
        /// Gets purchase history for a user with pagination
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Tuple of purchase transactions and total count</returns>
        Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetPurchaseHistoryAsync(
            string userId, int pageNumber, int pageSize);
        
        /// <summary>
        /// Updates user wallet balance
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="newBalance">The new balance</param>
        /// <returns>True if successful</returns>
        Task<bool> UpdateWalletBalanceAsync(string userId, long newBalance);
        
        /// <summary>
        /// Creates a new wallet transaction
        /// </summary>
        /// <param name="transaction">The transaction to create</param>
        /// <returns>The created transaction</returns>
        Task<UserWalletTransaction> CreateTransactionAsync(UserWalletTransaction transaction);
    
        Task<UserWalletInfoDto?> GetUserWalletInfoAsync(string userId);
        Task<List<UserWalletListItemDto>> GetUserWalletListAsync();
    }
} 