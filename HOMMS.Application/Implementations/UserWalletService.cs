using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Domain.Enums;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    /// <summary>
    /// Service implementation for user wallet operations
    /// </summary>
    public class UserWalletService : IUserWalletService
    {
        private readonly IUserWalletRepository _userWalletRepository;

        public UserWalletService(IUserWalletRepository userWalletRepository)
        {
            _userWalletRepository = userWalletRepository;
        }

        public async Task<decimal> GetWalletBalanceAsync(string userId)
        {
            return await _userWalletRepository.GetWalletBalanceAsync(userId);
        }

        public async Task<UserWalletTransaction> DepositAsync(string userId, decimal amount, string description, 
            string depositedByUserId, int branchId)
        {
            // Get current balance
            var currentBalance = await _userWalletRepository.GetWalletBalanceAsync(userId);
            var newBalance = currentBalance + amount;

            // Update wallet balance
            await _userWalletRepository.UpdateWalletBalanceAsync(userId, newBalance);

            // Create transaction record
            var transaction = new UserWalletTransaction
            {
                UserId = userId,
                Amount = (long)amount, // Convert decimal to long
                TransactionType = WalletTransactionType.Credit,
                Description = description,
                BranchId = branchId,
                BalanceAfter = (long)newBalance, // Convert decimal to long
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            return await _userWalletRepository.CreateTransactionAsync(transaction);
        }

        public async Task<bool> DeductForOrderAsync(string userId, decimal amount, int orderId, int branchId)
        {
            // Check if user has sufficient balance
            var hasSufficientBalance = await HasSufficientBalanceAsync(userId, amount);
            if (!hasSufficientBalance)
            {
                return false;
            }

            // Get current balance
            var currentBalance = await _userWalletRepository.GetWalletBalanceAsync(userId);
            var newBalance = currentBalance - amount;

            // Update wallet balance
            var updateSuccess = await _userWalletRepository.UpdateWalletBalanceAsync(userId, newBalance);
            if (!updateSuccess)
            {
                return false;
            }

            // Create transaction record
            var transaction = new UserWalletTransaction
            {
                UserId = userId,
                Amount = (long)amount, // Convert decimal to long
                TransactionType = WalletTransactionType.OrderPayment,
                Description = $"Payment for order #{orderId}",
                BranchId = branchId,
                OrderId = orderId,
                BalanceAfter = (long)newBalance, // Convert decimal to long
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _userWalletRepository.CreateTransactionAsync(transaction);
            return true;
        }

        public async Task<bool> HasSufficientBalanceAsync(string userId, decimal amount)
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

        public async Task<UserWalletInfoDto?> GetUserWalletInfoAsync(string userId)
        {
            return await _userWalletRepository.GetUserWalletInfoAsync(userId);
        }

        public async Task<List<UserWalletInfoDto>> GetUserWalletListByBranchAsync(int branchId)
        {
            return await _userWalletRepository.GetUserWalletListByBranchAsync(branchId);
        }

        public async Task<List<UserWalletListItemDto>> GetUserWalletListAsync()
        {
            return await _userWalletRepository.GetUserWalletListAsync();
        }
    }
} 