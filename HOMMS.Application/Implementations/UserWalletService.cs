using HOMMS.Application.Interfaces;
using HOMMS.Domain.Entities;
using HOMMS.Domain.Enums;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HOMMS.Application.Implementations
{
    /// <summary>
    /// Service implementation for user wallet operations
    /// </summary>
    public class UserWalletService : IUserWalletService
    {
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly DbContext _dbContext;

        public UserWalletService(IUserWalletRepository userWalletRepository, DbContext dbContext)
        {
            _userWalletRepository = userWalletRepository;
            _dbContext = dbContext;
        }

        public async Task<UserWalletInfoDto?> GetUserWalletInfoAsync(string userId)
        {
            return await _userWalletRepository.GetUserWalletInfoAsync(userId);
        }

        public async Task<List<UserWalletListItemDto>> GetUserWalletListAsync()
        {
            return await _userWalletRepository.GetUserWalletListAsync();
        }

        public async Task<long> GetWalletBalanceAsync(string userId)
        {
            return await _userWalletRepository.GetWalletBalanceAsync(userId);
        }

        public async Task<UserWalletTransaction> DepositAsync(string userId, long amount, string description, 
            string depositedByUserId, int branchId)
        {
            // Get current balance
            var currentBalance = await _userWalletRepository.GetWalletBalanceAsync(userId);
            var newBalance = currentBalance + amount;

            // Update user wallet balance
            await _userWalletRepository.UpdateWalletBalanceAsync(userId, newBalance);

            // Create transaction record
            var transaction = new UserWalletTransaction
            {
                UserId = userId,
                BranchId = branchId,
                TransactionType = WalletTransactionType.Credit,
                Amount = amount,
                BalanceAfter = newBalance,
                Description = description,
                CreatedBy = depositedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            return await _userWalletRepository.CreateTransactionAsync(transaction);
        }

        public async Task<bool> DeductForOrderAsync(string userId, long amount, int orderId, int branchId)
        {
            // Check if user has sufficient balance
            var currentBalance = await _userWalletRepository.GetWalletBalanceAsync(userId);
            if (currentBalance < amount)
                return false;

            var newBalance = currentBalance - amount;

            // Update user wallet balance
            await _userWalletRepository.UpdateWalletBalanceAsync(userId, newBalance);

            // Create transaction record
            var transaction = new UserWalletTransaction
            {
                UserId = userId,
                BranchId = branchId,
                TransactionType = WalletTransactionType.OrderPayment,
                Amount = -amount, // Negative for deduction
                BalanceAfter = newBalance,
                Description = $"Thanh toán đơn hàng #{orderId}",
                OrderId = orderId,
                CreatedAt = DateTime.UtcNow
            };

            await _userWalletRepository.CreateTransactionAsync(transaction);
            return true;
        }

        public async Task<bool> HasSufficientBalanceAsync(string userId, long amount)
        {
            var currentBalance = await _userWalletRepository.GetWalletBalanceAsync(userId);
            return currentBalance >= amount;
        }

        public async Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetDepositHistoryAsync(
            string userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _userWalletRepository.GetDepositHistoryAsync(userId, pageNumber, pageSize);
        }

        public async Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetPurchaseHistoryAsync(
            string userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _userWalletRepository.GetPurchaseHistoryAsync(userId, pageNumber, pageSize);
        }

        public async Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetTransactionHistoryAsync(
            string userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _userWalletRepository.GetTransactionHistoryAsync(userId, pageNumber, pageSize);
        }

        }
} 