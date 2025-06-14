using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IWalletRepository: IRepository<UserWalletTransaction, int>
    {
        Task<UserWalletTransaction?> GetWalletAsync(string userId);
        Task<UserWalletTransaction> DepositAsync(string userId, long amount, string description);
        Task<UserWalletTransaction> SetBalanceAsync(string userId, long newBalance);
    }
}
