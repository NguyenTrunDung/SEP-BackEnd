
using HOMMS.Application.BaseServices;

using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;

using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class FoodCategoryService : BaseService, IFoodCategoryService
    {
        private readonly IFoodCategoryRepository _foodCategoryRepository;
        private readonly IMapper _mapper;

        public FoodCategoryService(IFoodCategoryRepository foodCategoryRepository, IMapper mapper, IBranchContext branchContext)
            : base(branchContext)
        {
            _foodCategoryRepository = foodCategoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FoodCategoryDto>> GetCategoriesByBranchAsync(int branchId)
        {
            var categories = await _foodCategoryRepository.GetActiveCategoriesByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<FoodCategoryDto>>(categories);
        }

        public async Task<FoodCategoryDto> GetByIdAsync(int id)
        {
            var category = await _foodCategoryRepository.GetByIdAsync(id);
            return _mapper.Map<FoodCategoryDto>(category);
        }

        public async Task<FoodCategoryDto> CreateAsync(FoodCategoryDto dto)
        {
                
            dto.BranchId = EnsureBranchId(dto.BranchId);


            bool isSortExist = await _foodCategoryRepository
                .AnyAsync(x => x.Sort == dto.Sort && x.BranchId == dto.BranchId);

            if (isSortExist)
                throw new Exception($"Số thứ tự {dto.Sort} đã được sử dụng.");



            var category = _mapper.Map<FoodCategory>(dto);
            var created = await _foodCategoryRepository.AddAsync(category);
            return _mapper.Map<FoodCategoryDto>(created);
        }

        public async Task<FoodCategoryDto> UpdateAsync(int id, FoodCategoryDto dto)
        {
            dto.BranchId = EnsureBranchId(dto.BranchId);

            bool isSortExist = await _foodCategoryRepository
              .AnyAsync(x => x.Sort == dto.Sort && x.BranchId == dto.BranchId && x.Id != id);

            if (isSortExist)
                throw new Exception($"Số thứ tự {dto.Sort} đã được sử dụng.");


 
            
 
            var category = await _foodCategoryRepository.GetByIdAsync(id);
            if (category == null) return null;
            _mapper.Map(dto, category);
            await _foodCategoryRepository.UpdateAsync(category);
            return _mapper.Map<FoodCategoryDto>(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _foodCategoryRepository.GetByIdAsync(id);
            if (category == null) return false;
            await _foodCategoryRepository.DeleteAsync(category);
            return true;
        }

        public async Task<IEnumerable<FoodCategoryDto>> GetActiveCategoriesByBranchAsync(int branchId)
        {
            var categories = await _foodCategoryRepository.GetActiveCategoriesByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<FoodCategoryDto>>(categories);
        }

        // Image upload methods
        public async Task<FoodCategoryDto> CreateCategoryAsync(FoodCategoryDto dto, IFormFile? image, string webRootPath)
        {
            string? imagePath = null;
            if (image != null)
            {
                imagePath = await UploadHandler.SaveImageAsync(image, webRootPath, "uploads");
            }

            var category = new FoodCategory
            {
                Name = dto.Name!,
                Image = imagePath ?? dto.ImageUrl,
                Sort = dto.Sort,
                BranchId = EnsureBranchId(dto.BranchId),
                Active = true // Set as active by default
            };

            await _foodCategoryRepository.AddAndSaveAsync(category);

            return new FoodCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.Image,
                Sort = category.Sort ?? 0,
                BranchId = category.BranchId
            };
        }

        public async Task<FoodCategoryDto> UpdateCategoryAsync(int id, FoodCategoryDto dto, IFormFile? image, string webRootPath)
        {
            // Find existing category
            var existingCategory = await _foodCategoryRepository.FindByIdAsync(id);
            if (existingCategory == null)
            {
                throw new Exception("Food category not found.");
            }

            // Update basic information
            existingCategory.Name = dto.Name!;
            existingCategory.Sort = dto.Sort;
            existingCategory.BranchId = EnsureBranchId(dto.BranchId);

            // If there's a new image, save it and update the path with automatic cleanup
            if (image != null)
            {
                // Pass existing image path for automatic cleanup of old file
                var newImagePath = await UploadHandler.SaveImageAsync(image, webRootPath, "uploads", existingCategory.Image);
                existingCategory.Image = newImagePath;
            }
            else if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                // Update with provided ImageUrl if no file upload
                existingCategory.Image = dto.ImageUrl;
            }

            // Save changes
            await _foodCategoryRepository.UpdateAndSaveAsync(existingCategory);

            // Return DTO
            return new FoodCategoryDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                ImageUrl = existingCategory.Image,
                Sort = existingCategory.Sort ?? 0,
                BranchId = existingCategory.BranchId
            };
        }
    }
}