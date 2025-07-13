using HOMMS.Domain.Entities;
using HOMMS.Domain.Enums;
using HOMMS.Domain.Dtos;
using System.Collections.Generic;

namespace HOMMS.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing user wallet operations
    /// All amounts are in VND (Vietnamese Dong) - whole numbers only
    /// </summary>
    public interface IUserWalletService
    {
        /// <summary>
        /// Gets the current wallet balance for a user in VND
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The current wallet balance in VND</returns>
        Task<long> GetWalletBalanceAsync(string userId);
        
        /// <summary>
        /// Deposits money to a user's wallet
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="amount">The amount to deposit in VND</param>
        /// <param name="description">Description of the deposit</param>
        /// <param name="depositedByUserId">ID of the user making the deposit</param>
        /// <param name="branchId">The branch ID where transaction occurs</param>
        /// <returns>The transaction record</returns>
        Task<UserWalletTransaction> DepositAsync(string userId, long amount, string description, 
            string depositedByUserId, int branchId);
        
        /// <summary>
        /// Deducts money from a user's wallet for order payment
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="amount">The amount to deduct in VND</param>
        /// <param name="orderId">The order ID</param>
        /// <param name="branchId">The branch ID</param>
        /// <returns>True if successful, false if insufficient balance</returns>
        Task<bool> DeductForOrderAsync(string userId, long amount, int orderId, int branchId);
        
        /// <summary>
        /// Checks if a user has sufficient wallet balance for an amount
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="amount">The amount to check in VND</param>
        /// <returns>True if sufficient balance, false otherwise</returns>
        Task<bool> HasSufficientBalanceAsync(string userId, long amount);
        
        /// <summary>
        /// Gets deposit history for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>List of deposit transactions</returns>
        Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetDepositHistoryAsync(
            string userId, int pageNumber = 1, int pageSize = 10);
        
        /// <summary>
        /// Gets purchase history for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>List of purchase transactions</returns>
        Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetPurchaseHistoryAsync(
            string userId, int pageNumber = 1, int pageSize = 10);
        
        /// <summary>
        /// Gets all transaction history for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>List of all transactions</returns>
        Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetTransactionHistoryAsync(
            string userId, int pageNumber = 1, int pageSize = 10);
        // Thêm method mới:
        Task<UserWalletInfoDto?> GetUserWalletInfoAsync(string userId);
        Task<List<UserWalletListItemDto>> GetUserWalletListAsync();
    }
} 