using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IFoodCategoryService
    {
        Task<IEnumerable<FoodCategoryDto>> GetCategoriesByBranchAsync(int branchId);
        Task<FoodCategoryDto> GetByIdAsync(int id);
        Task<FoodCategoryDto> CreateAsync(FoodCategoryDto dto);
        Task<FoodCategoryDto> UpdateAsync(int id, FoodCategoryDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FoodCategoryDto>> GetActiveCategoriesByBranchAsync(int branchId);

        // Image upload methods
        Task<FoodCategoryDto> CreateCategoryAsync(FoodCategoryDto dto, IFormFile? image, string webRootPath);
        Task<FoodCategoryDto> UpdateCategoryAsync(int id, FoodCategoryDto dto, IFormFile? image, string webRootPath);
    }
} 