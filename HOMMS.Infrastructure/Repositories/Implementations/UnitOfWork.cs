using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBranchRepository BranchRepository { get; }
        // Add other repositories as needed

        public UnitOfWork(ApplicationDbContext context, IBranchRepository branchRepository)
        {
            _context = context;
            BranchRepository = branchRepository;
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