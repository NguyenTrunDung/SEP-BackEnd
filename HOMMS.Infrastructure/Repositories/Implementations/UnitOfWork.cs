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
        public IMenuDetailRepository MenuDetailRepository { get; }

        public IOrderDetailsRepository OrderDetailsRepository { get; }//new ordertail

        public IPatientRepository PatientRepository { get; }
        public IWalletRepository WalletRepository { get; }
        public IBranchRoleManagementRepository BranchRoleManagementRepository { get; }
        public IBranchUserManagementRepository BranchUserManagementRepository { get; }
        public IUserWalletRepository UserWalletRepository { get; }

        // Add other repositories as needed

        public UnitOfWork(ApplicationDbContext context, IBranchRepository branchRepository, IOrderDetailsRepository orderDetailsRepository, 
            IOrderRepository orderRepository, IMenuDetailRepository menuDetailRepository, IPatientRepository patientRepository, 
            IWalletRepository walletRepository, IBranchRoleManagementRepository branchRoleManagementRepository, IBranchUserManagementRepository branchUserManagementRepository,
            IUserWalletRepository userWalletRepository)//add order, orderdetail
        {
            _context = context;
            BranchRepository = branchRepository;
            OrderRepository = orderRepository;//moi them 
            OrderDetailsRepository = orderDetailsRepository;//moi them 
            MenuDetailRepository = menuDetailRepository;
            PatientRepository = patientRepository;
            WalletRepository = walletRepository;    
            BranchRoleManagementRepository = branchRoleManagementRepository;
            BranchUserManagementRepository = branchUserManagementRepository;
            UserWalletRepository = userWalletRepository;
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