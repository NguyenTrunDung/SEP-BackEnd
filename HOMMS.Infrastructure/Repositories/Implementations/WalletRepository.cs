using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Domain.Enums;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class WalletRepository : Repository<UserWallet, int>, IWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WalletRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserWallet?> GetWalletByIdAsync(string userId)
        {
            return await _dbContext.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        /// <summary>
        /// Add Amount to current BalanceAfter
        /// </summary>
        public async Task<UserWallet> DepositAsync(string userId, long amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0.");

            var wallet = await GetWalletByIdAsync(userId)
                ?? throw new InvalidOperationException("User wallet not initialized.");

            wallet.Amount += amount;
            wallet.LastModifiedAt = DateTime.UtcNow;

            _dbContext.UserWallets.Update(wallet);
            await _dbContext.SaveChangesAsync();

            return wallet;
        }

        /// <summary>
        /// Update wallet balance directly
        /// </summary>
        public async Task<UserWallet> SetBalanceAsync(string userId, long newBalance)
        {
            if (newBalance < 0)
                throw new ArgumentException("The balance cannot be negative.");

            var wallet = await GetWalletByIdAsync(userId)
                ?? throw new InvalidOperationException("User wallet not initialized.");

            long newAmount = newBalance;

            wallet.Amount = newAmount;
            wallet.LastModifiedAt = DateTime.UtcNow;

            _dbContext.UserWallets.Update(wallet);
            await _dbContext.SaveChangesAsync();

            return wallet;
        }
        public class UserWalletTransactionDto
        {
            public long Id { get; set; }
            public long Amount { get; set; }
            public long BalanceAfter { get; set; }
            public string Description { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }
        public class UserWalletTransactionsDto
        {
            public int Id { get; set; }
            public string UserId { get; set; } = string.Empty;
            public string? UserName { get; set; }
            public long Amount { get; set; }
            public long BalanceAfter { get; set; }
            public string Description { get; set; } = string.Empty;
            public WalletTransactionType TransactionType { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class WalletPurchaseHistoryDto
        {
            public int TransactionId { get; set; }
            public DateTime CreatedAt { get; set; }
            public long Amount { get; set; }
            public string Description { get; set; }
            public int OrderId { get; set; }
            public List<OrderDetailDto> OrderDetails { get; set; } = new();
        }

        public class OrderDetailDto
        {
            public int FoodId { get; set; }
            public string FoodName { get; set; } = string.Empty;
            public int Qty { get; set; }
            public int Price { get; set; }
            public int Total { get; set; }
        }
        public class CreateUserWalletTransactionDto
        {
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty; // used for user validation if needed
            public long Amount { get; set; }
            public string Description { get; set; } = string.Empty;
            public int BranchId { get; set; }
        }
        public class UpdateUserWalletTransactionDto
        {
            public int Id { get; set; } // ID của transaction để tìm
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public long Amount { get; set; }
            public string Description { get; set; } = string.Empty;
            public int BranchId { get; set; }
        }





        public async Task<(List<UserWalletTransactionDto> data, int totalCount)> GetWalletCreditHistoryAsync(string userId, int pageNumber, int pageSize)
        {
            var query = _dbContext.UserWalletTransactions
                .Where(x => x.UserId == userId && x.TransactionType == WalletTransactionType.Credit)
                .OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new UserWalletTransactionDto
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    BalanceAfter = x.BalanceAfter,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return (data, totalCount);
        }
        public async Task<List<UserWalletTransactionsDto>> GetWalletTransactionsByBranchAndUserAsync(int branchId)
        {
            return await _dbContext.UserWalletTransactions
                .Where(t => t.BranchId == branchId )
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new UserWalletTransactionsDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    UserName = t.User.UserName,
                    Amount = t.Amount,
                    BalanceAfter = t.BalanceAfter,
                    Description = t.Description,
                    TransactionType = t.TransactionType,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }
        public async Task<List<WalletPurchaseHistoryDto>> GetPurchaseHistoryByUserIdAsync(string userId)
        {
            return await _dbContext.UserWalletTransactions
                .Where(t => t.UserId == userId && t.TransactionType == WalletTransactionType.OrderPayment && t.OrderId != null)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new WalletPurchaseHistoryDto
                {
                    TransactionId = t.Id,
                    CreatedAt = t.CreatedAt,
                    Amount = t.Amount,
                    Description = t.Description,
                    OrderId = t.OrderId.Value,
                    OrderDetails = t.Order!.OrderDetails.Select(od => new OrderDetailDto
                    {
                        FoodId = od.FoodId ?? 0,
                        FoodName = od.Food != null ? od.Food.Name : (od.Menu != null ? od.Menu.Name : ""),
                        Qty = od.Qty ?? 0,
                        Price = od.Price ?? 0,
                        Total = od.Total ?? 0
                    }).ToList()
                })
                .ToListAsync();
        }
        public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<UserWalletTransaction?> GetLatestTransactionByUserIdAsync(string userId)
        {
            return await _dbContext.UserWalletTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<UserWalletTransaction> AddTransactionAsync(UserWalletTransaction transaction)
        {
            _dbContext.UserWalletTransactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }

        public async Task<UserWalletTransaction?> GetByIdAsync(int id)
        {
            return await _dbContext.UserWalletTransactions
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<bool> DeactivateAsync(int id)
        {
            var transaction = await _dbContext.UserWalletTransactions
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return false;

            transaction.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateUserWalletTransactionAsync(UpdateUserWalletTransactionDto dto)
        {
            var transaction = await _dbContext.UserWalletTransactions
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == dto.Id && !t.IsDeleted);

            if (transaction == null)
                return false;

            // Cập nhật transaction
            transaction.Amount = dto.Amount;
            transaction.Description = dto.Description;
            transaction.BranchId = dto.BranchId;
            transaction.BalanceAfter = dto.Amount; // Giả sử logic đơn giản

            // Cập nhật user
            transaction.User.FirstName = dto.FirstName;
            transaction.User.LastName = dto.LastName;
            transaction.User.PhoneNumber = dto.PhoneNumber;

            await _dbContext.SaveChangesAsync();
            return true;
        }


    }
}
