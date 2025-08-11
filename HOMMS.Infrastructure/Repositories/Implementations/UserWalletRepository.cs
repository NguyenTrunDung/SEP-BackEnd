using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Domain.Enums;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class UserWalletRepository : Repository<UserWalletTransaction, int>, IUserWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserWalletRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<decimal> GetWalletBalanceAsync(string userId)
        {
            var userWallet = await _dbContext.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
            
            return userWallet?.Amount ?? 0;
        }

        public async Task<ApplicationUser?> GetUserWalletAsync(string userId)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<UserWallet?> GetUserWalletEntityAsync(string userId)
        {
            return await _dbContext.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<UserWallet> CreateUserWalletAsync(string userId, decimal initialAmount = 0)
        {
            var userWallet = new UserWallet
            {
                UserId = userId,
                Amount = initialAmount,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            _dbContext.UserWallets.Add(userWallet);
            await _dbContext.SaveChangesAsync();
            
            return userWallet;
        }

        public async Task<bool> UpdateWalletBalanceAsync(string userId, decimal newBalance)
        {
            var userWallet = await _dbContext.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (userWallet == null)
            {
                // Create wallet if it doesn't exist
                userWallet = new UserWallet
                {
                    UserId = userId,
                    Amount = newBalance,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow
                };
                _dbContext.UserWallets.Add(userWallet);
            }
            else
            {
                userWallet.Amount = newBalance;
                userWallet.LastModifiedAt = DateTime.UtcNow;
            }

            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetTransactionHistoryAsync(
            string userId, int pageNumber, int pageSize)
        {
            var query = _dbContext.UserWalletTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var transactions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (transactions, totalCount);
        }

        public async Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetDepositHistoryAsync(
            string userId, int pageNumber, int pageSize)
        {
            var query = _dbContext.UserWalletTransactions
                .Where(t => t.UserId == userId && t.TransactionType == WalletTransactionType.Credit)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var transactions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (transactions, totalCount);
        }

        public async Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetPurchaseHistoryAsync(
            string userId, int pageNumber, int pageSize)
        {
            var query = _dbContext.UserWalletTransactions
                .Where(t => t.UserId == userId && t.TransactionType == WalletTransactionType.OrderPayment)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var transactions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (transactions, totalCount);
        }

        public async Task<UserWalletTransaction> CreateTransactionAsync(UserWalletTransaction transaction)
        {
            _dbContext.UserWalletTransactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }

        public async Task<UserWalletInfoDto?> GetUserWalletInfoAsync(string userId)
        {
            var result = await (from w in _dbContext.UserWallets
                               join u in _dbContext.Users on w.UserId equals u.Id
                               where w.UserId == userId
                               select new UserWalletInfoDto
                               {
                                   UserId = u.Id,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   UserName = u.UserName,
                                   Email = u.Email,
                                   PhoneNumber = u.PhoneNumber ?? string.Empty,
                                   Balance = w.Amount,
                                   CustomerCode = u.CustomerCode,
                                   IsCustomerAccount = u.IsCustomerAccount,
                                   IsCustomerEnabled = u.IsCustomerEnabled
                               }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<List<UserWalletInfoDto>> GetUserWalletListByBranchAsync(int branchId)
        {
            var result = await (from w in _dbContext.UserWallets
                               join u in _dbContext.Users on w.UserId equals u.Id
                               join bu in _dbContext.BranchUsers on u.Id equals bu.UserId
                               where bu.BranchId == branchId
                               select new UserWalletInfoDto
                               {
                                   UserId = u.Id,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   UserName = u.UserName,
                                   Email = u.Email,
                                   PhoneNumber = u.PhoneNumber ?? string.Empty,
                                   Balance = w.Amount,
                                   CustomerCode = u.CustomerCode,
                                   IsCustomerAccount = u.IsCustomerAccount,
                                   IsCustomerEnabled = u.IsCustomerEnabled
                               }).ToListAsync();

            return result;
        }

        public async Task<List<UserWalletListItemDto>> GetUserWalletListAsync()
        {
            return await (from w in _dbContext.UserWallets
                         join u in _dbContext.Users on w.UserId equals u.Id
                         select new UserWalletListItemDto
                         {
                             UserId = u.Id,
                             Name = u.FullName,
                             Username = u.UserName,
                             Phone = u.PhoneNumber ?? string.Empty,
                             Balance = w.Amount
                         })
                         .ToListAsync();
        }
    }
} 