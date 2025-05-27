using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IOrderDetailsRepository: IRepository<OrderDetails, int>
    {
        IQueryable<OrderDetails> GetAll();
    }
}
