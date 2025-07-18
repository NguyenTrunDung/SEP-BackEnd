using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IBranchRepository BranchRepository { get; }
        IOrderDetailsRepository OrderDetailsRepository { get; }  // Th�m
        IOrderRepository OrderRepository { get; }  // Th�m
        IMenuDetailRepository MenuDetailRepository { get; }
        IPatientRepository PatientRepository { get; }
        // Add other repositories as needed, e.g.:
        // IMenuRepository MenuRepository { get; }
        // IFoodRepository FoodRepository { get; }

        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
} 