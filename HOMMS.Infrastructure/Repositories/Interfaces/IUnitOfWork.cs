using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IBranchRepository BranchRepository { get; }
<<<<<<< HEAD
        IOrderDetailsRepository OrderDetailsRepository { get; }  // Thêm
        IOrderRepository OrderRepository { get; }  // Thêm
=======
        IOrderRepository OrderRepository { get; }
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        // Add other repositories as needed, e.g.:
        // IMenuRepository MenuRepository { get; }
        // IFoodRepository FoodRepository { get; }

        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
} 