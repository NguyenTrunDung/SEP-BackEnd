using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOMMS.Infrastructure.Repositories.Implementations.WalletRepository;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IWalletRepository: IRepository<UserWalletTransaction, int>
    {
        Task<UserWalletTransaction?> GetWalletAsync(string userId);
        Task<UserWalletTransaction> DepositAsync(string userId, long amount, string description);
        Task<UserWalletTransaction> SetBalanceAsync(string userId, long newBalance);
        
        Task<(List<UserWalletTransactionDto> data, int totalCount)> GetWalletCreditHistoryAsync(string userId, int pageNumber, int pageSize);
        Task<List<UserWalletTransactionsDto>> GetWalletTransactionsByBranchAndUserAsync(int branchId);
        Task<List<WalletPurchaseHistoryDto>> GetPurchaseHistoryByUserIdAsync(string userId);

        Task<UserWalletTransaction> AddTransactionAsync(UserWalletTransaction transaction);
        Task<ApplicationUser?> GetUserByUsernameAsync(string username);
        Task<UserWalletTransaction?> GetLatestTransactionByUserIdAsync(string userId);
        Task<UserWalletTransaction?> GetByIdAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<bool> UpdateUserWalletTransactionAsync(UpdateUserWalletTransactionDto dto);


    }
}
