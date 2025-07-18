
using HOMMS.Application.BaseServices;

using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;

using Microsoft.EntityFrameworkCore;

using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;

using System.Collections.Generic;
using System.Linq;
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


            bool isSortExist = await _foodCategoryRepository
                .AnyAsync(x => x.Sort == dto.Sort && x.BranchId == dto.BranchId);

            if (isSortExist)
                throw new Exception($"Số thứ tự {dto.Sort} đã được sử dụng.");



            dto.BranchId = EnsureBranchId(dto.BranchId);

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

        // Auto-sort and reordering methods
        public async Task<FoodCategoryDto> CreateCategoryWithAutoSortAsync(FoodCategoryDto dto, IFormFile? image, string webRootPath)
        {
            string? imagePath = null;
            if (image != null)
            {
                imagePath = await UploadHandler.SaveImageAsync(image, webRootPath, "uploads");
            }

            // Auto-assign sort value
            var maxSort = await _foodCategoryRepository.GetMaxSortValueByBranchAsync(EnsureBranchId(dto.BranchId));
            var newSort = maxSort + 1;

            var category = new FoodCategory
            {
                Name = dto.Name!,
                Image = imagePath ?? dto.ImageUrl,
                Sort = newSort,
                BranchId = EnsureBranchId(dto.BranchId),
                Active = true
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

        public async Task<bool> ReorderCategoriesAsync(IEnumerable<(int CategoryId, int Sort)> categoryOrders, int branchId)
        {
            try
            {
                branchId = EnsureBranchId(branchId);

                // Validate that all categories belong to the specified branch
                var categoryIds = categoryOrders.Select(x => x.CategoryId).ToList();
                var existingCategories = await _foodCategoryRepository.GetCategoriesByBranchWithTrackingAsync(branchId);
                var existingCategoryIds = existingCategories.Select(c => c.Id).ToList();

                var invalidCategories = categoryIds.Except(existingCategoryIds).ToList();
                if (invalidCategories.Any())
                {
                    throw new Exception($"Categories with IDs {string.Join(", ", invalidCategories)} do not exist in branch {branchId}");
                }

                // Update sort orders
                await _foodCategoryRepository.UpdateSortOrdersAsync(categoryOrders);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> MoveCategoryAsync(int categoryId, int newPosition, int branchId)
        {
            try
            {
                branchId = EnsureBranchId(branchId);

                // Get all categories for the branch ordered by current sort
                var categories = (await _foodCategoryRepository.GetCategoriesByBranchWithTrackingAsync(branchId)).ToList();
                
                // Find the category to move
                var categoryToMove = categories.FirstOrDefault(c => c.Id == categoryId);
                if (categoryToMove == null)
                {
                    throw new Exception($"Category with ID {categoryId} not found in branch {branchId}");
                }

                // Remove from current position
                categories.Remove(categoryToMove);

                // Insert at new position (ensure position is within bounds)
                newPosition = Math.Max(0, Math.Min(newPosition, categories.Count));
                categories.Insert(newPosition, categoryToMove);

                // Update sort values based on new order
                var updates = categories.Select((category, index) => (category.Id, index + 1));
                await _foodCategoryRepository.UpdateSortOrdersAsync(updates);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}