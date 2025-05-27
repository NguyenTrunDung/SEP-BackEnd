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
    }
} 