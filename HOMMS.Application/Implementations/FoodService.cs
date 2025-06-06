using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;

        public FoodService(IFoodRepository foodRepository, IMapper mapper)
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
            var food = _mapper.Map<Food>(dto);
            var created = await _foodRepository.AddAsync(food);
            return _mapper.Map<FoodDto>(created);
        }

        public async Task<FoodDto> UpdateAsync(int id, FoodDto dto)
        {
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

        public async Task<IEnumerable<FoodDto>> GetFoodsByBranchAndDateAsync(int branchId, DateTime date)
        {
            var foods = await _foodRepository.GetFoodsByBranchAndDateAsync(branchId, date);
            return _mapper.Map<IEnumerable<FoodDto>>(foods);
        }

        public async Task<IEnumerable<FoodCategoryDto>> GetCategoriesByBranchAndDateAsync(int branchId, DateTime date)
        {
            var categories = await _foodRepository.GetCategoriesByBranchAndDateAsync(branchId, date);
            return _mapper.Map<IEnumerable<FoodCategoryDto>>(categories);
        }

        public async Task<IEnumerable<FoodDto>> GetFoodsByBranchCategoryAndDateAsync(int branchId, int categoryId, DateTime date)
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

        
    }
}