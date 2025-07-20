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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBranchUserManagementRepository _branchUserManagement;
        public WalletRepository(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IBranchUserManagementRepository branchUserManagement)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _branchUserManagement = branchUserManagement;
        }

        public async Task<UserWallet?> GetWalletByIdAsync(string userId)
        {
            return await _dbContext.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        /// <summary>
        /// Add Amount to current BalanceAfter
        /// </summary>
        public async Task<UserWalletTransaction> DepositAsync(string userId, long amount, int branchId, string description, string createdBy)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0.");

            var wallet = await GetWalletByIdAsync(userId)
                ?? throw new InvalidOperationException("User wallet not initialized.");

            decimal newBalance = wallet.Amount + amount;
            var transaction = new UserWalletTransaction
            {
                UserId = userId,
                BranchId = branchId,
                TransactionType = WalletTransactionType.Credit,
                Amount = amount,
                BalanceAfter = (long)newBalance,
                Description = description,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.UserWalletTransactions.Add(transaction);
            wallet.Amount = newBalance;
            wallet.LastModifiedAt = DateTime.UtcNow;
            _dbContext.UserWallets.Update(wallet);
            await _dbContext.SaveChangesAsync();
            return transaction;
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
            public string CreatedBy { get; set; }
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
            public string CreatedBy { get; set; }

        }

        public class WalletPurchaseHistoryDto
        {
            public string TransactionId { get; set; } = null!;
            public string OrderId { get; set; } = null!;
            public string? Description { get; set; }
            public WalletTransactionType TransactionType { get; set; }
            public DateTime CreatedAt { get; set; }
            public decimal Amount { get; set; }
            public List<string> FoodNames { get; set; } = new();
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
        public async Task<List<UserWalletTransactionDto>> GetWalletCreditHistoryAsync(string userId)
        {
            var data = await _dbContext.UserWalletTransactions
                .Where(x => x.UserId == userId && x.TransactionType == WalletTransactionType.Credit && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new UserWalletTransactionDto
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    BalanceAfter = x.BalanceAfter,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy
                })
                .ToListAsync();

            return data;
        }

        public async Task<List<UserWalletTransactionsDto>> GetWalletTransactionsByBranchAndUserAsync(int branchId)
        {
            return await _dbContext.UserWalletTransactions
                .Where(t => t.BranchId == branchId)
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
                    CreatedAt = t.CreatedAt,
                    CreatedBy = t.CreatedBy
                })
                .ToListAsync();
        }
        public async Task<(List<WalletPurchaseHistoryDto> Items, int TotalCount)> GetPurchaseHistoryByUserIdAsync(string userId)
        {
            var query = _dbContext.UserWalletTransactions
        .Where(t => t.UserId == userId && t.TransactionType == WalletTransactionType.OrderPayment && !t.IsDeleted)
        .OrderByDescending(t => t.CreatedAt);

            var transactions = await query.ToListAsync();

            var orderIds = transactions
                .Where(t => t.OrderId.HasValue)
                .Select(t => t.OrderId!.Value)
                .Distinct()
                .ToList();

            var foodMap = await _dbContext.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId) && !od.IsDeleted)
                .Join(_dbContext.Foods, od => od.FoodId, f => f.Id, (od, f) => new { od.OrderId, f.Name })
                .GroupBy(x => x.OrderId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.Name).ToList());

            var items = transactions.Select(t => new WalletPurchaseHistoryDto
            {
                TransactionId = t.Id.ToString(),
                OrderId = t.OrderId?.ToString() ?? "",
                Description = t.Description,
                TransactionType = WalletTransactionType.OrderPayment,
                CreatedAt = t.CreatedAt,
                Amount = t.Amount,
                FoodNames = t.OrderId != null && foodMap.ContainsKey(t.OrderId.Value)
                    ? foodMap[t.OrderId.Value]
                    : new List<string>()
            }).ToList();

            return (items, items.Count);
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


        public class CreateWalletRequestDto
        {
            public string UserName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public string Password { get; set; } = null!;
            public long Amount { get; set; }
            public string Description { get; set; } = null!;
            public int BranchId { get; set; }
            public string? CreatedBy { get; set; }
        }

        public class UpdateWalletRequestDto
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public long Amount { get; set; }
            public string Description { get; set; } = null!;
        }

        public class WalletResponseDto
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string FullName { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public string Email { get; set; } = null!;
            public long Amount { get; set; }
            public string Description { get; set; } = null!;
            public int BranchId { get; set; }
        }
        public async Task<UserWallet?> GetByIdAsyncs(int id)
        {
            return await _dbContext.UserWallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);
        }

        public async Task<UserWallet?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.UserWallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId && !w.IsDeleted);
        }

        public async Task AddAsync(UserWallet wallet)
        {
            await _dbContext.UserWallets.AddAsync(wallet);
        }

        public void Update(UserWallet wallet)
        {
            _dbContext.UserWallets.Update(wallet);
        }

        public void SoftDelete(UserWallet wallet)
        {
            wallet.IsDeleted = true;
            wallet.DeletedAt = DateTime.UtcNow;
            wallet.DeletedBy = "system"; // hoặc lấy từ HttpContext.User
            _dbContext.UserWallets.Update(wallet);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public async Task<ApplicationUser?> FindByNameAsync(string userName)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
        public void UpdateUser(ApplicationUser user)
        {
            _dbContext.Users.Update(user);
        }
        public async Task<ApplicationUser?> FindUserByIdAsync(string userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<WalletResponseDto> CreateWalletAsync(CreateWalletRequestDto dto)
        {
            // Kiểm tra email
            if (await _dbContext.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email đã tồn tại.");

            // Tạo user
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception($"Không tạo được user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            // Gán vào chi nhánh
            var branchUser = new BranchUser
            {
                UserId = user.Id,
                BranchId = dto.BranchId,
                CreatedAt = DateTime.UtcNow
            };
            await _dbContext.BranchUsers.AddAsync(branchUser);

            // Tạo ví
            var wallet = new UserWallet
            {
                UserId = user.Id,
                Amount = dto.Amount,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = string.IsNullOrWhiteSpace(dto.CreatedBy) ? "admin" : dto.CreatedBy!
            };
            await _dbContext.UserWallets.AddAsync(wallet);
            await _dbContext.SaveChangesAsync();

            return new WalletResponseDto
            {
                Id = wallet.Id,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Amount = (long)wallet.Amount,
            };
        }
        public async Task<bool> DeleteUserWallet(string userId, int branchId)
        {
            var branchUser = await _dbContext.BranchUsers
                .FirstOrDefaultAsync(bu => bu.UserId == userId && bu.BranchId == branchId && !bu.IsDeleted);

            if (branchUser == null)
                return false;

            branchUser.IsDeleted = true;
            branchUser.DeletedAt = DateTime.UtcNow;

            var userWallet = await _dbContext.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (userWallet != null)
            {
                _dbContext.UserWallets.Remove(userWallet);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
