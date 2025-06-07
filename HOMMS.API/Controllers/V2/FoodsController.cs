using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodService _foodService;
        private readonly IMapper _mapper;
        private readonly ISystemLogService _systemLogService;

        public FoodsController(IFoodService foodService, IMapper mapper, ISystemLogService systemLogService)
        {
            _foodService = foodService;
            _mapper = mapper;
            _systemLogService = systemLogService;
        }



        [MapToApiVersion("2.0")]
        [HttpGet]
        [Authorize(Policy = "Permission:foods:view")]
        public async Task<ActionResult<ApiResponseBase<List<FoodDto>>>> GetFoods([FromQuery] int branchId)
        {
            var foods = await _foodService.GetFoodsByBranchAsync(branchId);
            if (foods == null)
                return NotFound(new ApiResponseBase<List<FoodDto>>(null, "Foods not found", "error", 0));
            var foodsDto = _mapper.Map<List<FoodDto>>(foods);
            var totalCount = foodsDto.Count;


            return Ok(new ApiResponseBase<List<FoodDto>>(foodsDto, "Foods retrieved successfully", "success", totalCount));
        }




        [MapToApiVersion("2.0")]
        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:foods:view")]
        public async Task<ActionResult<ApiResponseBase<FoodDto>>> GetFood(int id)
        {
            var food = await _foodService.GetByIdAsync(id);
            if (food == null)
                return NotFound(new ApiResponseBase<FoodDto>(null, "Food not found", "error"));
            return Ok(new ApiResponseBase<FoodDto>(food, "Food retrieved successfully"));
        }


        [MapToApiVersion("2.0")]
        [HttpPost]
        [Authorize(Policy = "Permission:foods:add")]

        public async Task<ActionResult<ApiResponseBase<FoodDtoV2>>> CreateFoodv2([FromBody] FoodDtoV2 dto)
        {

            await _systemLogService.AddSystemLog(new AddSystemLogDto
            {
                BranchId = dto.BranchId,
                UserId = 1,
                Note = "Đã thêm món ăn " + dto.Name,
                Date = dto.Date
            });


            var created = await _foodService.CreateAsyncV2(dto);

            return CreatedAtAction(nameof(GetFood), new { id = created.Id }, new ApiResponseBase<FoodDtoV2>(created, "Food created successfully"));
        }

        [MapToApiVersion("2.0")]
        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:foods:edit")]
        public async Task<ActionResult<ApiResponseBase<FoodDtoV2>>> UpdateFood(int id, [FromBody] FoodDtoV2_3 dto)
        {

            try
            {
                var food = await _foodService.GetByIdAsyncV2(id);
                await _systemLogService.AddSystemLog(new AddSystemLogDto
                {
                    BranchId = food.BranchId,
                    UserId = 1,
                    Note = "đã cập nhật món ăn " + food.Name,
                    Date = food.Date
                });


                var updated = await _foodService.UpdateAsyncV2(id, dto);
                if (updated == null)
                    return NotFound(new ApiResponseBase<FoodDtoV2_3>(null, "Food not found", "error"));
                return Ok(new ApiResponseBase<FoodDtoV2_3>(updated, "Food updated successfully"));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

        }

        [MapToApiVersion("2.0")]
        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission:foods:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteFood(int id)
        {
            try
            {


                var food = await _foodService.GetByIdAsyncV2(id);
                await _systemLogService.AddSystemLog(new AddSystemLogDto
                {
                    BranchId = food.BranchId,
                    UserId = 1,
                    Note = "đã xóa món ăn " + food.Name,
                    Date = food.Date
                });
                var deleted = await _foodService.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new ApiResponseBase<object>(null, "Food not found", "error"));



                return Ok(new ApiResponseBase<object>(null, "Food deleted successfully"));
            }
            catch (Exception ex)
            {

                return BadRequest(new { status = "error", message = ex.Message });
            }
        }
    }
}