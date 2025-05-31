using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class MenuDetailRepository : Repository<MenuDetail, int>, IMenuDetailRepository
    {
        public MenuDetailRepository(ApplicationDbContext dbContext)
         : base(dbContext)
        {
        }
    }
}
