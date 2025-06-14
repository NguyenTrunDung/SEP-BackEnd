using HOMMS.Domain.Entities;
using HOMMS.Domain.Enums;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class WalletRepository : Repository<UserWalletTransaction, int>, IWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WalletRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserWalletTransaction?> GetWalletAsync(string userId)
        {
            return await _dbContext.UserWalletTransactions
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        /// <summary>
        /// Add Amount to current BalanceAfter
        /// </summary>
        public async Task<UserWalletTransaction> DepositAsync(string userId, long amount, string description)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0.");

            var wallet = await GetWalletAsync(userId)
                ?? throw new InvalidOperationException("User wallet not initialized.");

            wallet.Amount = amount;
            wallet.BalanceAfter += amount;
            wallet.TransactionType = WalletTransactionType.Credit;
            wallet.Description = description;
            wallet.BranchId = wallet.BranchId;
            wallet.LastModifiedAt = DateTime.UtcNow;

            _dbContext.UserWalletTransactions.Update(wallet);
            await _dbContext.SaveChangesAsync();

            return wallet;
        }

        /// <summary>
        /// Update wallet balance directly
        /// </summary>
        public async Task<UserWalletTransaction> SetBalanceAsync(string userId, long newBalance)
        {
            if (newBalance < 0)
                throw new ArgumentException("The balance cannot be negative.");

            var wallet = await GetWalletAsync(userId)
                ?? throw new InvalidOperationException("User wallet not initialized.");

            long delta = newBalance - wallet.BalanceAfter;

            wallet.Amount = delta;
            wallet.BalanceAfter = newBalance;
            wallet.TransactionType = WalletTransactionType.Adjustment;
            wallet.BranchId = wallet.BranchId;
            wallet.LastModifiedAt = DateTime.UtcNow;

            _dbContext.UserWalletTransactions.Update(wallet);
            await _dbContext.SaveChangesAsync();

            return wallet;
        }
    }
}
