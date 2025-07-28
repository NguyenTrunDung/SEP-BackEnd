using AutoMapper;
using HOMMS.Application.BaseServices;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class CommentService : BaseService, ICommentService
    {

        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository feedbackRepository, IMapper mapper, IBranchContext branchContext) : base(branchContext)
        {
            _repository = feedbackRepository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<CommentDto>> GetRatingByStarAndOrderAndBranchAsync(int star,int OrderId, int BranchId)
        {
            var feed = await _repository.GetRatingByStarAndOrderAndBranchAsync(star, OrderId, BranchId);
            return _mapper.Map<IEnumerable<CommentDto>>(feed);

        }


        public async Task<bool> CheckDuplicateAsync(string UserId, int OrderId, int BranchId)
        {
            var feed = await _repository.CheckDuplicateAsync(UserId, OrderId, BranchId);
            if (feed is null) return true;
            return false;

        }


        public async Task<IEnumerable<CommentDto>> GetAllFeedbackByBranchAsync(int BranchId)
        {

            var feed = await _repository.GetAllFeedbackByBranchAsync(BranchId);
            return _mapper.Map<IEnumerable<CommentDto>>(feed);
        }


        public async Task<CommentDto> CreateFeedbackAsync(CommentDto dto)
        {
            dto.BranchId = EnsureBranchId(dto.BranchId);
            var feed = _mapper.Map<Comment>(dto);
            var created = await _repository.AddAsync(feed);
            return _mapper.Map<CommentDto>(created);
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {


            var feed = await _repository.GetByIdAsync(id);
            if (feed == null) return false;
            await _repository.DeleteAsync(feed);
            return true;

        }

        public async Task<IEnumerable<CommentDto>> GetAllFeedbackAsync()
        {
            var feed = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CommentDto>>(feed);
        }



        public async Task<CommentDto> GetFeedbackByIdAsync(int id)
        {
            var feed = await _repository.GetByIdAsync(id);
            return _mapper.Map<CommentDto>(feed);
        }


        public async Task<CommentDto> UpdateFeedbackAsync(int id, CommentDto dto)
        {

            dto.BranchId = EnsureBranchId(dto.BranchId);
            var feed = await _repository.GetByIdAsync(id);
            if (feed == null) return null;

            // Manually update non-key properties to avoid modifying Id
            feed.Star = dto.Star;
            feed.CommentLines = dto.CommentLines;
            feed.OrderId = dto.OrderId;
            feed.BranchId = dto.BranchId;
            feed.UserId = dto.UserId;

            await _repository.UpdateAsync(feed);
            return _mapper.Map<CommentDto>(feed);

        }



        public async Task<IEnumerable<CommentDto>> GetFeedbackByOrderIdAndBranchAsync(int OrderId, int BranchId)
        {
            var feed = await _repository.GetFeedbackByOrderIdAndBranchAsync(OrderId, BranchId);
            return _mapper.Map<IEnumerable<CommentDto>>(feed);
        }

        public async Task<IEnumerable<CommentDto>> GetFeedbackByUserIdAndBranchAsync(string UserId)
        {
            var feed = await _repository.GetFeedbackByUserIdAndBranchAsync(UserId);
            return _mapper.Map<IEnumerable<CommentDto>>(feed);
        }
    }
}
