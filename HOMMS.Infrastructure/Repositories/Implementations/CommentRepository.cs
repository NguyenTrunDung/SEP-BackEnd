using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    public class CommentRepository : Repository<Comment, int>, ICommentRepository
    {

        public CommentRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<Comment>> GetAllFeedbackByBranchAsync(int BranchId)
        {
            return await DbSet
                  .Where(c => c.BranchId == BranchId)
                  .OrderByDescending(f => f.CreatedAt)
                  .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetFeedbackByOrderIdAndBranchAsync(int OrderId, int BranchId)
        {
            return await DbSet
                 .Where(c => c.BranchId == BranchId&& c.OrderId == OrderId)
                 .OrderByDescending(f => f.CreatedAt)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetFeedbackByUserIdAndBranchAsync(string UserId)
        {
            return await DbSet
                 .Where(c => c.UserId == UserId)
                 .OrderByDescending(f => f.CreatedAt)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetRatingByStarAndOrderAndBranchAsync(int star, int OrderId, int BranchId)
        {
            return await DbSet
                  .Where(c => c.BranchId == BranchId && c.Star == star && c.OrderId == OrderId)
                  .OrderByDescending(f => f.CreatedAt)
                  .ToListAsync();
        }

        public async Task<Comment> CheckDuplicateAsync(string UserId, int OrderId, int BranchId)
        {
            return await DbSet
                  .AsNoTracking()
                  .FirstOrDefaultAsync(c => c.BranchId == BranchId && c.UserId == UserId && c.OrderId == OrderId);
                
        }

        public async Task<IEnumerable<Comment>> GetFeedbackByUserIdAndActruallBranchAsync(string UserId, int BranchId)
        {
            return await DbSet
                 .Where(c => c.UserId == UserId && c.BranchId == BranchId)
                 .OrderByDescending(f => f.CreatedAt)
                 .ToListAsync();
        }
    }
}
