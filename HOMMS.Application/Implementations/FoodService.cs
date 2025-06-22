using AutoMapper;
using HOMMS.Application.BaseServices;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
 

    public class FoodService : BaseService, IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;
        
        public FoodService(IFoodRepository foodRepository, IMapper mapper, IBranchContext branchContext)
            : base(branchContext)
        {
            _foodRepository = foodRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FoodDto>> GetFoodsByBranchAsync(int branchId)
        {
            var foods = await _foodRepository.GetFoodsWithCategoriesByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<FoodDto>>(foods);
        }

        public async Task<FoodDto> GetByIdAsync(int id)
        {
            var food = await _foodRepository.GetFoodWithCategoryAsync(id);
            return _mapper.Map<FoodDto>(food);
        }

        public async Task<FoodDto> CreateAsync(FoodDto dto)
        {
            dto.BranchId = EnsureBranchId(dto.BranchId);
            var food = _mapper.Map<Food>(dto);
            var created = await _foodRepository.AddAsync(food);
            return _mapper.Map<FoodDto>(created);
        }

        public async Task<FoodDto> UpdateAsync(int id, FoodDto dto)
        {
            dto.BranchId = EnsureBranchId(dto.BranchId);
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null) return null;
            _mapper.Map(dto, food);
            await _foodRepository.UpdateAsync(food);
            return _mapper.Map<FoodDto>(food);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null) return false;
            await _foodRepository.DeleteAsync(food);
            return true;
        }

        public async Task<IEnumerable<FoodDto>> GetFoodsByBranchAndCategoryAsync(int branchId, int categoryId)
        {
            var foods = await _foodRepository.GetFoodsByBranchAndCategoryAsync(branchId, categoryId);
            return _mapper.Map<IEnumerable<FoodDto>>(foods);
        }

        public async Task<IEnumerable<FoodDto>> GetFoodsByBranchAndDateAsync(int branchId, System.DateTime date)
        {
            var foods = await _foodRepository.GetFoodsByBranchAndDateAsync(branchId, date);
            return _mapper.Map<IEnumerable<FoodDto>>(foods);
        }

        public async Task<IEnumerable<FoodCategoryDto>> GetCategoriesByBranchAndDateAsync(int branchId, System.DateTime date)
        {
            var categories = await _foodRepository.GetCategoriesByBranchAndDateAsync(branchId, date);
            return _mapper.Map<IEnumerable<FoodCategoryDto>>(categories);
        }

        public async Task<IEnumerable<FoodDto>> GetFoodsByBranchCategoryAndDateAsync(int branchId, int categoryId, System.DateTime date)
        {
            var foods = await _foodRepository.GetFoodsByBranchCategoryAndDateAsync(branchId, categoryId, date);
            return _mapper.Map<IEnumerable<FoodDto>>(foods);
        }


        //////////////////////
        public async Task<FoodDtoV2> CreateAsyncV2(FoodDtoV2 dto)
        {
            var food = _mapper.Map<Food>(dto);
            var created = await _foodRepository.AddAsync(food);
            return _mapper.Map<FoodDtoV2>(created); throw new NotImplementedException();
        }

        public async Task<FoodDtoV2_3> UpdateAsyncV2(int id, FoodDtoV2_3 dto)
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null) return null;
            _mapper.Map(dto, food);
            await _foodRepository.UpdateAsync(food);
            return _mapper.Map<FoodDtoV2_3>(food);
        }

        public async Task<FoodDtoV2> GetByIdAsyncV2(int id)
        {

            var food = await _foodRepository.GetFoodWithCategoryAsync(id);
            return _mapper.Map<FoodDtoV2>(food);
        }

        //update url image and save image to root folder
        public async Task<FoodDto> CreateFoodAsync(FoodDto dto, IFormFile image, string webRootPath)
        {
            string? imagePath = null;
            if (image != null)
            {
                imagePath = await UploadHandler.SaveImageAsync(image, webRootPath, "uploads");
            }
            var food = new Food
            {
                Name = dto.Name!,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                IsAddOn = dto.IsAddOn,
                IsSetDish = dto.IsSetDish,
                ForPatient = true,
                PriceForGuest = dto.PriceForGuest,
                PriceForPatient = dto.PriceForPatient,
                PriceForStaff = dto.PriceForStaff,
                Sort = dto.Sort,
                BranchId = dto.BranchId,
                Image = imagePath
            };

            await _foodRepository.AddAndSaveAsync(food);

            return new FoodDto
            {
                Id = food.Id,
                Name = food.Name,
                Description = food.Description,
                CategoryId = food.CategoryId,
                IsAddOn = food.IsAddOn,
                IsSetDish = food.IsSetDish,
                PriceForGuest = food.PriceForGuest,
                PriceForPatient = food.PriceForPatient,
                PriceForStaff = food.PriceForStaff,
                Sort = food.Sort,
                BranchId = food.BranchId,
                ImageUrl = food.Image
            };
        }
        public async Task<FoodDto> UpdateFoodAsync(int id, FoodDto dto, IFormFile? image, string webRootPath)
        {
            // 1. Tìm món ăn hiện có
            var existingFood = await _foodRepository.FindByIdAsync(id);
            if (existingFood == null)
            {
                throw new Exception("Food not found.");
            }

            // 2. Cập nhật thông tin cơ bản
            existingFood.Name = dto.Name!;
            existingFood.Description = dto.Description;
            existingFood.CategoryId = dto.CategoryId;
            existingFood.IsAddOn = dto.IsAddOn;
            existingFood.IsSetDish = dto.IsSetDish;
            existingFood.PriceForGuest = dto.PriceForGuest;
            existingFood.PriceForPatient = dto.PriceForPatient;
            existingFood.PriceForStaff = dto.PriceForStaff;
            existingFood.Sort = dto.Sort;
            existingFood.BranchId = dto.BranchId;

            // 3. If there's a new image, save it and update the path with automatic cleanup
            if (image != null)
            {
                // Pass existing image path for automatic cleanup of old file
                var newImagePath = await UploadHandler.SaveImageAsync(image, webRootPath, "uploads", existingFood.Image);
                existingFood.Image = newImagePath;
            }

            // 4. Lưu thay đổi
            await _foodRepository.UpdateAndSaveAsync(existingFood);

            // 5. Trả về DTO
            return new FoodDto
            {
                Id = existingFood.Id,
                Name = existingFood.Name,
                Description = existingFood.Description,
                CategoryId = existingFood.CategoryId,
                IsAddOn = existingFood.IsAddOn,
                IsSetDish = existingFood.IsSetDish,
                PriceForGuest = existingFood.PriceForGuest,
                PriceForPatient = existingFood.PriceForPatient,
                PriceForStaff = existingFood.PriceForStaff,
                Sort = existingFood.Sort,
                BranchId = existingFood.BranchId,
                ImageUrl = existingFood.Image
            };
        }

    }
}