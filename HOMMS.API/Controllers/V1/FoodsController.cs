
using Asp.Versioning;
using AutoMapper;

using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodService _foodService;
        private readonly IMapper _mapper;
        private readonly IBranchContext _branchContext;
        private readonly IWebHostEnvironment _env;

        public FoodsController(IFoodService foodService, IMapper mapper, IBranchContext branchContext, IWebHostEnvironment env)
        {
            _foodService = foodService;
            _mapper = mapper;
            _branchContext = branchContext;
            _env = env;
        }

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

        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:foods:view")]
        public async Task<ActionResult<ApiResponseBase<FoodDto>>> GetFood(int id)
        {
            var food = await _foodService.GetByIdAsync(id);
            if (food == null)
                return NotFound(new ApiResponseBase<FoodDto>(null, "Food not found", "error"));
            return Ok(new ApiResponseBase<FoodDto>(food, "Food retrieved successfully"));
        }

        [HttpPost]
        [Authorize(Policy = "Permission:foods:add")]
        public async Task<ActionResult<ApiResponseBase<FoodDto>>> CreateFood([FromBody] FoodDto dto)
        {
                var created = await _foodService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetFood), new { id = created.Id }, new ApiResponseBase<FoodDto>(created, "Food created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Permission:foods:edit")]
        public async Task<ActionResult<ApiResponseBase<FoodDto>>> UpdateFood(int id, [FromBody] FoodDto dto)
        {
            var updated = await _foodService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new ApiResponseBase<FoodDto>(null, "Food not found", "error"));
            return Ok(new ApiResponseBase<FoodDto>(updated, "Food updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission:foods:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>> DeleteFood(int id)
        {
            var deleted = await _foodService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponseBase<object>(null, "Food not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "Food deleted successfully"));
        }


        //update url image and save image to root folder

        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetFoodsByBranch(int branchId)
        {
            var foods = await _foodService.GetFoodsByBranchAsync(branchId);
            return Ok(foods);
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateFood([FromForm] FoodCreateRequest request)
        {
            try
            {
                var foodDto = new FoodDto
                {
                    Name = request.Name,
                    Description = request.Description,
                    CategoryId = request.CategoryId,
                    IsAddOn = request.IsAddOn,
                    IsSetDish = request.IsSetDish,
                    PriceForGuest = request.PriceForGuest,
                    PriceForPatient = request.PriceForPatient,
                    PriceForStaff = request.PriceForStaff,
                    Sort = request.Sort,
                    BranchId = request.BranchId
                };

                var result = await _foodService.CreateFoodAsync(foodDto, request.Image, _env.WebRootPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("update/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateFood(int id, [FromForm] FoodCreateRequest request)
        {
            try
            {
                var dto = new FoodDto
                {
                    Name = request.Name,
                    Description = request.Description,
                    CategoryId = request.CategoryId,
                    IsAddOn = request.IsAddOn,
                    IsSetDish = request.IsSetDish,
                    PriceForGuest = request.PriceForGuest,
                    PriceForPatient = request.PriceForPatient,
                    PriceForStaff = request.PriceForStaff,
                    Sort = request.Sort,
                    BranchId = request.BranchId
                };

                var result = await _foodService.UpdateFoodAsync(id, dto, request.Image, _env.WebRootPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}