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


        public async Task<IEnumerable<CommentDto>> GetAllFeedbackAsync()
        {
            return await DbSet
                
                   .Include(c => c.Food)
                     .OrderByDescending(f => f.CreatedAt)
                     .Select(c => new CommentDto
                     {
                         Id = c.Id,
                         Star = c.Star,
                         CommentLines = c.CommentLines,
                         FoodId = c.FoodId,
                         Image = c.Food.Image,
                         OrderId = c.OrderId,
                         UserId = c.UserId,
                         BranchId = c.BranchId,
                         CreatedAt = c.CreatedAt
                     })

                  .ToListAsync();
        }



        public async Task<IEnumerable<CommentDto>> GetAllFeedbackByBranchAsync(int BranchId)
        {
            return await DbSet
                  .Where(c => c.BranchId == BranchId)
                   .Include(c => c.Food)
                     .OrderByDescending(f => f.CreatedAt)
                     .Select(c => new CommentDto
                     {
                         Id = c.Id,
                         Star = c.Star,
                         CommentLines = c.CommentLines,
                         FoodId = c.FoodId,
                         Image = c.Food.Image,
                         OrderId = c.OrderId,
                         UserId = c.UserId,
                         BranchId = c.BranchId,
                          CreatedAt = c.CreatedAt
                     })
                
                  .ToListAsync();
        }

        public async Task<IEnumerable<CommentDto>> GetFeedbackByOrderIdAndBranchAsync(int OrderId, int BranchId)
        {
            return await DbSet
                 .Where(c => c.BranchId == BranchId && c.OrderId == OrderId)
                  .Include(c => c.Food)
                  .OrderByDescending(f => f.CreatedAt)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Star = c.Star,
                        CommentLines = c.CommentLines,
                        FoodId = c.FoodId,
                        Image = c.Food.Image,
                        OrderId = c.OrderId,
                        UserId = c.UserId,
                        BranchId = c.BranchId,
                         CreatedAt = c.CreatedAt
                    })
                 
                 .ToListAsync();
        }

        public async Task<IEnumerable<CommentDto>> GetFeedbackByUserIdAndBranchAsync(string UserId)
        {
            return await DbSet
                 .Where(c => c.UserId == UserId)
                  .Include(c => c.Food)
                  .OrderByDescending(f => f.CreatedAt)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Star = c.Star,
                        CommentLines = c.CommentLines,
                        FoodId = c.FoodId,
                        Image = c.Food.Image,
                        OrderId = c.OrderId,
                        UserId = c.UserId,
                        BranchId = c.BranchId,
                        CreatedAt = c.CreatedAt

                    })
                 
                 .ToListAsync();
        }

        public async Task<IEnumerable<CommentDto>> GetRatingByStarAndOrderAndBranchAsync(int star, int OrderId, int BranchId)
        {
            return await DbSet
                  .Where(c => c.BranchId == BranchId && c.Star == star && c.OrderId == OrderId)
                   .Include(c => c.Food)
                   .Select(c => new CommentDto
                   {
                       Id = c.Id,
                       Star = c.Star,
                       CommentLines = c.CommentLines,
                       FoodId = c.FoodId,
                       Image = c.Food.Image,
                       OrderId = c.OrderId,
                       UserId = c.UserId,
                       BranchId = c.BranchId,
                        CreatedAt = c.CreatedAt
                   })
                  .OrderByDescending(f => f.CreatedAt)
                  .ToListAsync();
        }

        public async Task<Comment> CheckDuplicateAsync(string UserId, int? OrderId, int? FoodId, int BranchId)
        {
            return await DbSet
                  .AsNoTracking()
                  .FirstOrDefaultAsync(c => c.BranchId == BranchId && c.UserId == UserId && ((OrderId != null && c.OrderId == OrderId) || (FoodId != null && c.FoodId == FoodId)));

        }

        public async Task<IEnumerable<CommentDto>> GetFeedbackByUserIdAndActruallBranchAsync(string UserId, int BranchId)
        {
            return await DbSet
                 .Where(c => c.UserId == UserId && c.BranchId == BranchId)
                  .Include(c => c.Food)
                 .Select(c => new CommentDto
                 {
                     Id = c.Id,
                     Star = c.Star,
                     CommentLines = c.CommentLines,
                     FoodId = c.FoodId,
                     Image = c.Food.Image,
                     OrderId = c.OrderId,
                     UserId = c.UserId,
                     BranchId = c.BranchId,
                      CreatedAt = c.CreatedAt
                 })
                 .OrderByDescending(f => f.CreatedAt)
                 .ToListAsync();
        }



        public async Task<CommentDto> GetFeedbackByIdAsync(int id)
        {
            return await DbSet
                .Where(c => c.Id == id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Star = c.Star,
                    CommentLines = c.CommentLines,
                    FoodId = c.FoodId,
                    Image = c.Food.Image,
                    OrderId = c.OrderId,
                    UserId = c.UserId,
                    BranchId = c.BranchId,
                     CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();
        }


    }
}
