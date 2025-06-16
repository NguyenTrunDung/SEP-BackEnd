using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Domain.Enums;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOMMS.Infrastructure.Repositories.Implementations.WalletRepository;

namespace HOMMS.Application.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public WalletService(IWalletRepository walletRepository, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _walletRepository = walletRepository;
            _passwordHasher = passwordHasher;

        }

        public Task<UserWalletTransaction?> GetWalletAsync(string userId)
        {
            return _walletRepository.GetWalletAsync(userId);
        }

        public Task<UserWalletTransaction> DepositAsync(string userId, long amount, string description)
        {
            return _walletRepository.DepositAsync(userId, amount, description);
        }

        public Task<UserWalletTransaction> SetBalanceAsync(string userId, long newBalance)
        {
            return _walletRepository.SetBalanceAsync(userId, newBalance);
        }
        public async Task<object> GetWalletCreditHistoryAsync(string userId, int pageNumber, int pageSize)
        {
            var (data, totalCount) = await _walletRepository.GetWalletCreditHistoryAsync(userId, pageNumber, pageSize);

            return new
            {
                status = "success",
                data,
                totalCount
            };
        }
        public async Task<List<UserWalletTransactionsDto>> GetWalletTransactionsByBranchAsync(int branchId)
        {
            return await _walletRepository.GetWalletTransactionsByBranchAndUserAsync(branchId);
        }
        public async Task<List<WalletPurchaseHistoryDto>> GetPurchaseHistoryByUserIdAsync(string userId)
        {
            return await _walletRepository.GetPurchaseHistoryByUserIdAsync(userId);
        }
        public async Task<UserWalletTransaction> AddWalletTransactionAsync(CreateUserWalletTransactionDto dto)
        {
            var user = await _walletRepository.GetUserByUsernameAsync(dto.UserName);
            if (user == null)
                throw new Exception("User không tồn tại.");

            var lastTransaction = await _walletRepository.GetLatestTransactionByUserIdAsync(user.Id);
            long currentBalance = lastTransaction?.BalanceAfter ?? 0;
            long newBalance = currentBalance + dto.Amount;

            var newTransaction = new UserWalletTransaction
            {
                UserId = user.Id,
                BranchId = dto.BranchId,
                TransactionType = WalletTransactionType.Credit,
                Amount = dto.Amount,
                BalanceAfter = newBalance,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            return await _walletRepository.AddTransactionAsync(newTransaction);
        }
        public async Task<bool> DeactivateTransactionAsync(int transactionId)
        {
            return await _walletRepository.DeactivateAsync(transactionId);
        }
        public async Task<bool> UpdateWalletTransactionAsync(UpdateUserWalletTransactionDto dto)
        {
            return await _walletRepository.UpdateUserWalletTransactionAsync(dto);
        }
    }
}
