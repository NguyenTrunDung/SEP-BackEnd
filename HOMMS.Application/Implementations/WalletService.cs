using HOMMS.Application.Interfaces;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public Task<UserWalletTransaction?> GetWalletAsync(string userId)
        {
            return _walletRepository.GetWalletAsync(userId);
        }

        public Task<UserWalletTransaction> DepositAsync(string userId, long amount, string description)
        {
            return _walletRepository.DepositAsync(userId, amount, description);
        }

        public Task<UserWalletTransaction> SetBalanceAsync(string userId, long newBalance)
        {
            return _walletRepository.SetBalanceAsync(userId, newBalance);
        }
    }
}
