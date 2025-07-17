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

        public Task<UserWallet?> GetWalletByIdAsync(string userId)
        {
            return _walletRepository.GetWalletByIdAsync(userId);
        }

        public Task<UserWallet> DepositAsync(string userId, long amount)
        {
            return _walletRepository.DepositAsync(userId, amount);
        }

        public Task<UserWallet> SetBalanceAsync(string userId, long newBalance)
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
        public async Task<WalletResponseDto> CreateWalletAsync(CreateWalletRequestDto dto)
        {
            // Truy vấn user từ UserName
            var user = await _walletRepository.FindByNameAsync(dto.UserName);
            if (user == null)
                throw new Exception($"Không tìm thấy user với UserName: {dto.UserName}");

            // Cập nhật thông tin user nếu có thay đổi
            bool isModified = false;

            if (!string.Equals(user.FirstName, dto.FirstName, StringComparison.OrdinalIgnoreCase))
            {
                user.FirstName = dto.FirstName;
                isModified = true;
            }

            if (!string.Equals(user.LastName, dto.LastName, StringComparison.OrdinalIgnoreCase))
            {
                user.LastName = dto.LastName;
                isModified = true;
            }

            if (!string.Equals(user.PhoneNumber, dto.PhoneNumber, StringComparison.OrdinalIgnoreCase))
            {
                user.PhoneNumber = dto.PhoneNumber;
                isModified = true;
            }

            if (isModified)
            {
                _walletRepository.UpdateUser(user);
            }

            var userWallet = new UserWallet
            {
                UserId = user.Id,
                Amount = dto.Amount,
                CreatedBy = "system"
            };

            await _walletRepository.AddAsync(userWallet);
            await _walletRepository.SaveChangesAsync();

            return new WalletResponseDto
            {
                Id = userWallet.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Amount = (long)userWallet.Amount,
                Description = dto.Description,
                BranchId = dto.BranchId
            };
        }

        public async Task<WalletResponseDto> UpdateWalletAsync(UpdateWalletRequestDto dto)
        {
            var userWallet = await _walletRepository.GetByIdAsyncs(dto.Id);
            if (userWallet == null)
                throw new Exception("Wallet not found");

            // 1. Tìm user liên kết
            var user = await _walletRepository.FindUserByIdAsync(userWallet.UserId);
            if (user == null)
                throw new Exception("User not found");

            // 2. Cập nhật thông tin người dùng
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;
            user.LastModifiedAt = DateTime.UtcNow;
            user.LastModifiedBy = "system";

            _walletRepository.UpdateUser(user); // <-- bạn cần định nghĩa hàm này

            // 3. Cập nhật ví
            userWallet.Amount = dto.Amount;
            userWallet.LastModifiedAt = DateTime.UtcNow;
            userWallet.LastModifiedBy = "system";

            _walletRepository.Update(userWallet);
            await _walletRepository.SaveChangesAsync();

            return new WalletResponseDto
            {
                Id = userWallet.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Amount = (long)userWallet.Amount,
                Description = dto.Description,
                BranchId = dto.BranchId
            };
        }

        public async Task<bool> DeactivateWalletAsync(int id)
        {
            var userWallet = await _walletRepository.GetByIdAsyncs(id);
            if (userWallet == null)
                return false;

            userWallet.IsDeleted = true;
            userWallet.DeletedAt = DateTime.UtcNow;
            userWallet.DeletedBy = "system";

            _walletRepository.Update(userWallet);
            await _walletRepository.SaveChangesAsync();
                return true; // <-- thiếu dòng này

        }
        }
    }
