using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.Graph.Models.Security;
using System.Threading.Tasks;
using static HOMMS.Infrastructure.Repositories.Implementations.WalletRepository;

namespace HOMMS.Application.Interfaces
{
    public interface IWalletService
    {
        Task<UserWalletTransaction> DepositAsync(string userId, long amount, int branchId, string description,string createdBy);
        Task<UserWallet> SetBalanceAsync(string userId, long newBalance);
        Task<UserWallet?> GetWalletByIdAsync(string userId);
        Task<List<UserWalletTransactionDto>> GetWalletCreditHistoryAsync(string userId);
        Task<List<UserWalletTransactionsDto>> GetWalletTransactionsByBranchAsync(int branchId);
        Task<List<WalletPurchaseHistoryDto>> GetPurchaseHistoryByUserIdAsync(string userId);
        Task<UserWalletTransaction> AddWalletTransactionAsync(CreateUserWalletTransactionDto dto);
        Task<bool> DeactivateTransactionAsync(int transactionId);
        Task<bool> UpdateWalletTransactionAsync(UpdateUserWalletTransactionDto dto);

        Task<WalletResponseDto> CreateWalletAsync(CreateWalletRequestDto dto);
        Task<WalletResponseDto> UpdateWalletAsync(UpdateWalletRequestDto dto);
        Task<bool> DeactivateWalletAsync(int id);
    }
}
