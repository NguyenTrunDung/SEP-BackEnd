using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface ICommentRepository : IRepository<Comment, int>
    {
        Task<IEnumerable<CommentDto>> GetAllFeedbackAsync();
        Task<IEnumerable<CommentDto>> GetRatingByStarAndOrderAndBranchAsync(int star, int OrderId, int BranchId);
        Task<Comment> CheckDuplicateAsync(string UserId, int? OrderId, int? FoodId, int BranchId);

        Task<IEnumerable<CommentDto>> GetAllFeedbackByBranchAsync(int BranchId);

        Task<IEnumerable<CommentDto>> GetFeedbackByOrderIdAndBranchAsync(int OrderId, int BranchId);
        Task<IEnumerable<CommentDto>> GetFeedbackByUserIdAndBranchAsync(string UserId);
        Task<IEnumerable<CommentDto>> GetFeedbackByUserIdAndActruallBranchAsync(string UserId, int BranchId);


        Task<CommentDto> GetFeedbackByIdAsync(int id);
    }
}

