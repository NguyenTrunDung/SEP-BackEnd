using Asp.Versioning;
using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HOMMS.API.Controllers.V1
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentService _feedbackService;
        private readonly IMapper _mapper;

        public CommentController(ICommentService feedbackService, IMapper mapper)
        {
            _feedbackService = feedbackService;
            _mapper = mapper;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseBase<CommentDto>>> GetFeedbackById(int id)
        {
            var feed = await _feedbackService.GetFeedbackByIdAsync(id);
            if (feed == null) return NotFound(new ApiResponseBase<CommentDto>(null, "Feedback not found", "error"));
            return Ok(new ApiResponseBase<CommentDto>(feed, "Feedback retrieved successfully"));
        }

        [HttpGet("All")]
        public async Task<ActionResult<ApiResponseBase<List<CommentDto>>>> GetAllFeedback()
        {
            var feed = await _feedbackService.GetAllFeedbackAsync();
            if (feed == null) return NotFound(new ApiResponseBase<List<CommentDto>>(null, "Feedback not found", "error", 0));
            var feedsDto = _mapper.Map<List<CommentDto>>(feed);
            var count = feedsDto.Count;
            return Ok(new ApiResponseBase<List<CommentDto>>(feedsDto, "Feedback retrieved successfully", "success", count));
        }


        [HttpGet("AllByBranch")]
        public async Task<ActionResult<ApiResponseBase<List<CommentDto>>>> GetAllFeedbackByBranch([FromQuery] int branchId)
        {
            var feed = await _feedbackService.GetAllFeedbackByBranchAsync(branchId);
            if (feed == null) return NotFound(new ApiResponseBase<List<CommentDto>>(null, "Feedback not found", "error", 0));
            var feedsDto = _mapper.Map<List<CommentDto>>(feed);
            var count = feedsDto.Count;
            return Ok(new ApiResponseBase<List<CommentDto>>(feedsDto, "Feedback retrieved successfully", "success", count));
        }


        [HttpGet("By-Order")]
        public async Task<ActionResult<ApiResponseBase<List<CommentDto>>>> GetAllFeedbackByBranchAndOrderId([FromQuery] int OrderId, [FromQuery] int branchId)
        {
            var feed = await _feedbackService.GetFeedbackByOrderIdAndBranchAsync(OrderId, branchId);
            if (feed == null) return NotFound(new ApiResponseBase<List<CommentDto>>(null, "Feedback not found", "error", 0));
            var feedsDto = _mapper.Map<List<CommentDto>>(feed);
            var count = feedsDto.Count;
            return Ok(new ApiResponseBase<List<CommentDto>>(feedsDto, "Feedback retrieved successfully", "success", count));
        }



        [HttpGet("By-User")]
        public async Task<ActionResult<ApiResponseBase<List<CommentDto>>>> GetAllFeedbackByBranchAndUserId([FromQuery] string UserId)
        {
            var feed = await _feedbackService.GetFeedbackByUserIdAndBranchAsync(UserId);
            if (feed == null) return NotFound(new ApiResponseBase<List<CommentDto>>(null, "Feedback not found", "error", 0));
            var feedsDto = _mapper.Map<List<CommentDto>>(feed);
            var count = feedsDto.Count;
            return Ok(new ApiResponseBase<List<CommentDto>>(feedsDto, "Feedback retrieved successfully", "success", count));
        }

        [HttpGet("By-User-Branch")]
        public async Task<ActionResult<ApiResponseBase<List<CommentDto>>>> GetFeedbackByUserIdAndActruallBranchAsync([FromQuery] string UserId, [FromQuery] int branchId)
        {
            var feed = await _feedbackService.GetFeedbackByUserIdAndActruallBranchAsync(UserId, branchId);
            if (feed == null) return NotFound(new ApiResponseBase<List<CommentDto>>(null, "Feedback not found", "error", 0));
            var feedsDto = _mapper.Map<List<CommentDto>>(feed);
            var count = feedsDto.Count;
            return Ok(new ApiResponseBase<List<CommentDto>>(feedsDto, "Feedback retrieved successfully", "success", count));
        }


        [HttpGet("By-Star")]
        public async Task<ActionResult<ApiResponseBase<List<CommentDto>>>> GetAllFeedbackByBranchAndStar([FromQuery] int star, [FromQuery] int OrderId, [FromQuery] int branchId)
        {
            var feed = await _feedbackService.GetRatingByStarAndOrderAndBranchAsync(star, OrderId, branchId);
            if (feed == null) return NotFound(new ApiResponseBase<List<CommentDto>>(null, "Feedback not found", "error", 0));
            var feedsDto = _mapper.Map<List<CommentDto>>(feed);
            var count = feedsDto.Count;
            return Ok(new ApiResponseBase<List<CommentDto>>(feedsDto, "Feedback retrieved successfully", "success", count));
        }



        // POST: FeedbackController/Create
        [HttpPost]

        public async Task<ActionResult<ApiResponseBase<CommentDto>>> CreateFeedback([FromBody] CRUDCommentDto dto)
        {

            try
            {

                if (dto.Star is < 0 or > 5) return BadRequest(new ApiResponseBase<CommentDto>(null, "Star rating must be greater than or equal to 0 or less than or equal to 5", "error"));

                if (dto.OrderId == 0) dto.OrderId = null;
                if (dto.FoodId == 0) dto.FoodId = null;

                if (dto.OrderId == null && dto.FoodId == null) return BadRequest(new ApiResponseBase<CommentDto>(null, "Comment must be linked to Order or Food", "error"));


                var check = await _feedbackService.CheckDuplicateAsync(dto.UserId, dto.OrderId, dto.FoodId, dto.BranchId);
                if (check is false) return BadRequest(new ApiResponseBase<CommentDto>(null, "User already rated this order", "error"));

                var created = await _feedbackService.CreateFeedbackAsync(dto);
                return CreatedAtAction(nameof(GetFeedbackById), new { id = created.Id }, new ApiResponseBase<CommentDto>(created, "feedback created successfully"));

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }


        }



        // POST: FeedbackController/Edit/5
        [HttpPut("{id}")]

        public async Task<ActionResult<ApiResponseBase<CommentDto>>> EditFeedback(int id, [FromBody] CRUDCommentDto dto)
        {
            if (dto.Star is < 0 or > 5) return BadRequest(new ApiResponseBase<CommentDto>(null, "Star rating must be greater than or equal to 0 or less than or equal to 5", "error"));
            var update = await _feedbackService.UpdateFeedbackAsync(id, dto);
            if (update == null) return NotFound(new ApiResponseBase<CommentDto>(null, "Feedback not found", "error"));
            return Ok(new ApiResponseBase<CommentDto>(update, "feedback update successfully"));
        }

        // POST: FeedbackController/Delete/5
        [HttpDelete("{id}")]

        public async Task<ActionResult<ApiResponseBase<object>>> DeleteFeedback(int id)
        {
            var delete = await _feedbackService.DeleteFeedbackAsync(id);
            if (!delete) return NotFound(new ApiResponseBase<object>(null, "Feedback not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "feedback delete successfully"));
        }
    }
}
