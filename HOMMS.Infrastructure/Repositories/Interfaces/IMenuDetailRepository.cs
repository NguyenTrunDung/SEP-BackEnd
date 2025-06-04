using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IMenuDetailRepository : IRepository<Menu, int>
    {
        Task<Menu?> GetMenuWithDetailsAsync(int id);
        Task<bool> UpdateMenuWithDetailsAsync(Menu menu);
        Task<bool> AddMenuWithDetailsAsync(Menu menu);

    }
}
