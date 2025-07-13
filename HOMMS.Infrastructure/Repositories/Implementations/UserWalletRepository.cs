using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Domain.Enums;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for user wallet operations
    /// </summary>
public class UserWalletRepository : Repository<UserWalletTransaction, int>, IUserWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserWalletRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserWalletInfoDto?> GetUserWalletInfoAsync(string userId)
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserWalletInfoDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    Balance = u.WalletBalance,
                    CustomerCode = u.CustomerCode,
                    IsCustomerAccount = u.IsCustomerAccount,
                    IsCustomerEnabled = u.IsCustomerEnabled
                })
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<UserWalletInfoDto>> GetUserWalletListByBranchAsync(int branchId)
        {
            var users = await (from bu in _dbContext.BranchUsers
                               join u in _dbContext.Users on bu.UserId equals u.Id
                               join w in _dbContext.UserWallets on u.Id equals w.UserId into uw
                               from w in uw.DefaultIfEmpty() 
                               where bu.BranchId == branchId
                               select new UserWalletInfoDto
                               {
                                   UserId = u.Id,
                                   FullName = u.FullName,
                                   Email = u.Email ?? string.Empty,
                                   Balance =  w.Amount,
                                   PhoneNumber = u.PhoneNumber,
                                   CustomerCode = u.CustomerCode,
                                   IsCustomerAccount = u.IsCustomerAccount,
                                   IsCustomerEnabled = u.IsCustomerEnabled
                               }).ToListAsync();

            return users;
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

        public async Task<long> GetWalletBalanceAsync(string userId)
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.WalletBalance)
                .FirstOrDefaultAsync();
            
            return user;
        }

        public async Task<ApplicationUser?> GetUserWalletAsync(string userId)
        {
            return await _dbContext.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetTransactionHistoryAsync(
            string userId, int pageNumber, int pageSize)
        {
            var query = _dbContext.UserWalletTransactions
                .Include(t => t.User)
                .Include(t => t.Branch)
                .Where(t => t.UserId == userId && !t.IsDeleted)
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
                .Include(t => t.User)
                .Include(t => t.Branch)
                .Where(t => t.UserId == userId && 
                           t.TransactionType == WalletTransactionType.Credit && 
                           !t.IsDeleted)
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
                .Include(t => t.User)
                .Include(t => t.Branch)
                .Include(t => t.Order)
                .Where(t => t.UserId == userId && 
                           t.TransactionType == WalletTransactionType.OrderPayment && 
                           !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            
            var transactions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (transactions, totalCount);
        }

        public async Task<bool> UpdateWalletBalanceAsync(string userId, long newBalance)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.WalletBalance = newBalance;
            user.LastModifiedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<UserWalletTransaction> CreateTransactionAsync(UserWalletTransaction transaction)
        {
            _dbContext.UserWalletTransactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }
    }
} 