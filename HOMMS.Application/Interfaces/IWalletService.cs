using HOMMS.Domain.Entities;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IWalletService
    {
        Task<UserWalletTransaction> DepositAsync(string userId, long amount, string description);
        Task<UserWalletTransaction> SetBalanceAsync(string userId, long newBalance);
        Task<UserWalletTransaction?> GetWalletAsync(string userId);
    }
}
