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
    public interface IWalletRepository: IRepository<UserWallet, int>
    {
        Task<UserWallet?> GetWalletByIdAsync(string userId);
        Task<UserWallet> DepositAsync(string userId, long amount);
        Task<UserWallet> SetBalanceAsync(string userId, long newBalance);
        
        Task<(List<UserWalletTransactionDto> data, int totalCount)> GetWalletCreditHistoryAsync(string userId, int pageNumber, int pageSize);
        Task<List<UserWalletTransactionsDto>> GetWalletTransactionsByBranchAndUserAsync(int branchId);
        Task<List<WalletPurchaseHistoryDto>> GetPurchaseHistoryByUserIdAsync(string userId);

        Task<UserWalletTransaction> AddTransactionAsync(UserWalletTransaction transaction);
        Task<ApplicationUser?> GetUserByUsernameAsync(string username);
        Task<UserWalletTransaction?> GetLatestTransactionByUserIdAsync(string userId);
        Task<UserWalletTransaction?> GetByIdAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<bool> UpdateUserWalletTransactionAsync(UpdateUserWalletTransactionDto dto);
        Task<UserWallet?> GetByIdAsyncs(int id);
        Task<UserWallet?> GetByUserIdAsync(string userId);
        Task<ApplicationUser?> FindByNameAsync(string userName);
        Task AddAsync(UserWallet wallet);
        void Update(UserWallet wallet);
        void SoftDelete(UserWallet wallet);
        Task SaveChangesAsync();
        void UpdateUser(ApplicationUser user);
        Task<ApplicationUser?> FindUserByIdAsync(string userId);


    }
}
