using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IFoodService
    {
        Task<IEnumerable<FoodDto>> GetFoodsByBranchAsync(int branchId);
        Task<FoodDto> GetByIdAsync(int id);
        Task<FoodDto> CreateAsync(FoodDto dto);
        Task<FoodDto> UpdateAsync(int id, FoodDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FoodDto>> GetFoodsByBranchAndCategoryAsync(int branchId, int categoryId);
        Task<IEnumerable<FoodDto>> GetFoodsByBranchAndDateAsync(int branchId, DateTime date);
        Task<IEnumerable<FoodCategoryDto>> GetCategoriesByBranchAndDateAsync(int branchId, DateTime date);
        Task<IEnumerable<FoodDto>> GetFoodsByBranchCategoryAndDateAsync(int branchId, int categoryId, DateTime date);

        ///////////////

        Task<FoodDtoV2> CreateAsyncV2(FoodDtoV2 dto);
        Task<FoodDtoV2_3> UpdateAsyncV2(int id, FoodDtoV2_3 dto);
        Task<FoodDtoV2> GetByIdAsyncV2(int id);

        //update url image and save image to root folder
        Task<FoodDto> CreateFoodAsync(FoodDto dto, IFormFile image, string webRootPath);


    }
} 