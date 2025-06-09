using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using HOMMS.Application.BaseServices;

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
    }
} 