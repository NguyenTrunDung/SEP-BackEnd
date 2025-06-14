using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.Graph.Models.Security;
using System.Threading.Tasks;
using static HOMMS.Infrastructure.Repositories.Implementations.WalletRepository;

namespace HOMMS.Application.Interfaces
{
    public interface IWalletService
    {
        Task<UserWalletTransaction> DepositAsync(string userId, long amount, string description);
        Task<UserWalletTransaction> SetBalanceAsync(string userId, long newBalance);
        Task<UserWalletTransaction?> GetWalletAsync(string userId);
        Task<object> GetWalletCreditHistoryAsync(string userId, int pageNumber, int pageSize);
        Task<List<UserWalletTransactionsDto>> GetWalletTransactionsByBranchAsync(int branchId);
        Task<List<WalletPurchaseHistoryDto>> GetPurchaseHistoryByUserIdAsync(string userId);
        Task<UserWalletTransaction> AddWalletTransactionAsync(CreateUserWalletTransactionDto dto);
        Task<bool> DeactivateTransactionAsync(int transactionId);
        Task<bool> UpdateWalletTransactionAsync(UpdateUserWalletTransactionDto dto);


    }
}
