using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.Graph.Models.Security;
using System.Threading.Tasks;
using static HOMMS.Infrastructure.Repositories.Implementations.WalletRepository;

namespace HOMMS.Application.Interfaces
{
    public interface IWalletService
    {
        Task<UserWallet> DepositAsync(string userId, long amount, int branchId, string description,string createdBy);
        Task<UserWallet> SetBalanceAsync(string userId, long newBalance);
        Task<UserWallet?> GetWalletByIdAsync(string userId);
        Task<object> GetWalletCreditHistoryAsync(string userId, int pageNumber, int pageSize);
        Task<List<UserWalletTransactionsDto>> GetWalletTransactionsByBranchAsync(int branchId);
        Task<(List<WalletPurchaseHistoryDto> Items, int TotalCount)> GetPurchaseHistoryByUserIdAsync(string userId, int pageNumber, int pageSize);
        Task<UserWalletTransaction> AddWalletTransactionAsync(CreateUserWalletTransactionDto dto);
        Task<bool> DeactivateTransactionAsync(int transactionId);
        Task<bool> UpdateWalletTransactionAsync(UpdateUserWalletTransactionDto dto);

        Task<WalletResponseDto> CreateWalletAsync(CreateWalletRequestDto dto);
        Task<WalletResponseDto> UpdateWalletAsync(UpdateWalletRequestDto dto);
        Task<bool> DeactivateWalletAsync(int id);
    }
}
