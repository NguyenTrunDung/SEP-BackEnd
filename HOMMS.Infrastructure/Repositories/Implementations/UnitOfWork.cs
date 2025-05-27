using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBranchRepository BranchRepository { get; }
        public IOrderRepository OrderRepository { get; }//new order
        public IOrderDetailsRepository OrderDetailsRepository { get; }//new ordertail
        // Add other repositories as needed

        public UnitOfWork(ApplicationDbContext context, IBranchRepository branchRepository, IOrderDetailsRepository orderDetailsRepository, IOrderRepository orderRepository)//add order, orderdetail
        {
            _context = context;
            BranchRepository = branchRepository;
            OrderRepository = orderRepository;//moi them 
            OrderDetailsRepository = orderDetailsRepository;//moi them 
            // Initialize other repositories here
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
} 