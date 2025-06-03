using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBranchRepository BranchRepository { get; }
<<<<<<< HEAD
        public IOrderRepository OrderRepository { get; }//new order
        public IOrderDetailsRepository OrderDetailsRepository { get; }//new ordertail
        // Add other repositories as needed

        public UnitOfWork(ApplicationDbContext context, IBranchRepository branchRepository, IOrderDetailsRepository orderDetailsRepository, IOrderRepository orderRepository)//add order, orderdetail
        {
            _context = context;
            BranchRepository = branchRepository;
            OrderRepository = orderRepository;//moi them 
            OrderDetailsRepository = orderDetailsRepository;//moi them 
=======
        public IOrderRepository OrderRepository { get; }
        // Add other repositories as needed

        public UnitOfWork(ApplicationDbContext context, IBranchRepository branchRepository, IOrderRepository orderRepository)
        {
            _context = context;
            BranchRepository = branchRepository;
            OrderRepository = orderRepository;
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
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