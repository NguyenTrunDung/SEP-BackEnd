using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class WalletRepository : Repository<UserWalletTransaction, int>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
