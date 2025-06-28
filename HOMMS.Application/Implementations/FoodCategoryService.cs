using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class FoodCategoryService : IFoodCategoryService
    {
        private readonly IFoodCategoryRepository _foodCategoryRepository;
        private readonly IMapper _mapper;

        public FoodCategoryService(IFoodCategoryRepository foodCategoryRepository, IMapper mapper)
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


            var category = _mapper.Map<FoodCategory>(dto);
            var created = await _foodCategoryRepository.AddAsync(category);
            return _mapper.Map<FoodCategoryDto>(created);
        }

        public async Task<FoodCategoryDto> UpdateAsync(int id, FoodCategoryDto dto)
        {

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
    }
}