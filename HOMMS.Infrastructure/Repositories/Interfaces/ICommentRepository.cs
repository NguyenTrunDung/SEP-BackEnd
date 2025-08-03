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

        Task<IEnumerable<Comment>> GetRatingByStarAndOrderAndBranchAsync(int star, int OrderId, int BranchId);
        Task<Comment> CheckDuplicateAsync(string UserId, int OrderId, int BranchId);

        Task<IEnumerable<Comment>> GetAllFeedbackByBranchAsync(int BranchId);

        Task<IEnumerable<Comment>> GetFeedbackByOrderIdAndBranchAsync(int OrderId, int BranchId);
        Task<IEnumerable<Comment>> GetFeedbackByUserIdAndBranchAsync(string UserId);
        Task<IEnumerable<Comment>> GetFeedbackByUserIdAndActruallBranchAsync(string UserId, int BranchId);
    }
}

