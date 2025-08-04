using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface ICommentService
    {


        Task<CommentDto> GetFeedbackByIdAsync(int id);
        Task<IEnumerable<CommentDto>> GetAllFeedbackAsync();
        Task<IEnumerable<CommentDto>> GetAllFeedbackByBranchAsync(int BranchId);
        Task<IEnumerable<CommentDto>> GetFeedbackByOrderIdAndBranchAsync(int OrderId, int BranchId);
        Task<IEnumerable<CommentDto>> GetFeedbackByUserIdAndBranchAsync(string UserId);
        Task<IEnumerable<CommentDto>> GetFeedbackByUserIdAndActruallBranchAsync(string UserId, int BranchId);
        Task<IEnumerable<CommentDto>> GetRatingByStarAndOrderAndBranchAsync(int star, int OrderId, int BranchId);

        Task<CommentDto> CreateFeedbackAsync(CRUDCommentDto dto);
        Task<CommentDto> UpdateFeedbackAsync(int id, CRUDCommentDto dto);
        Task<bool> DeleteFeedbackAsync(int id);

        Task<bool> CheckDuplicateAsync(string UserId, int? OrderId, int? FoodId, int BranchId);





    }
}
