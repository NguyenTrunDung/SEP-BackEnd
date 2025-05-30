using HOMMS.Domain.Entities;
using HOMMS.Domain.Enums;

namespace HOMMS.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing user wallet operations (simplified for POC)
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
        /// Adds money to a user's wallet (by admin/manager)
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="amount">The amount to add in VND</param>
        /// <param name="description">Description of the transaction (supports Vietnamese)</param>
        /// <param name="addedByUserId">ID of the user adding the money</param>
        /// <param name="branchId">The branch ID where transaction occurs</param>
        /// <returns>The transaction record</returns>
        Task<UserWalletTransaction> AddMoneyAsync(string userId, long amount, string description, 
            string addedByUserId, int branchId);
        
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
        /// Gets wallet transaction history for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>List of wallet transactions</returns>
        Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetTransactionHistoryAsync(
            string userId, int pageNumber = 1, int pageSize = 10);
        
        /// <summary>
        /// Creates a customer account with specified wallet balance
        /// </summary>
        /// <param name="customerData">Customer account information</param>
        /// <param name="initialBalance">Initial wallet balance in VND</param>
        /// <param name="createdByUserId">ID of the user creating the account</param>
        /// <param name="branchId">The branch ID</param>
        /// <returns>The created customer user</returns>
        Task<ApplicationUser> CreateCustomerAccountAsync(CreateCustomerRequest customerData, 
            long initialBalance, string createdByUserId, int branchId);
    }
    
    /// <summary>
    /// Request model for creating customer accounts (simplified for POC)
    /// Supports Vietnamese customer information
    /// </summary>
    public class CreateCustomerRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CustomerCode { get; set; }
        public string? Address { get; set; }
        /// <summary>
        /// Customer notes in Vietnamese (allergies, dietary restrictions, etc.)
        /// </summary>
        public string? CustomerNotes { get; set; }
    }
} 