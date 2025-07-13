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
        private readonly ApplicationDbContext _context;

        public UserWalletRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<long> GetWalletBalanceAsync(string userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.WalletBalance)
                .FirstOrDefaultAsync();
            
            return user;
        }

        public async Task<ApplicationUser?> GetUserWalletAsync(string userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<UserWalletTransaction> Transactions, int TotalCount)> GetTransactionHistoryAsync(
            string userId, int pageNumber, int pageSize)
        {
            var query = _context.UserWalletTransactions
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
            var query = _context.UserWalletTransactions
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
            var query = _context.UserWalletTransactions
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
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.WalletBalance = newBalance;
            user.LastModifiedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserWalletTransaction> CreateTransactionAsync(UserWalletTransaction transaction)
        {
            _context.UserWalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
    }
} 