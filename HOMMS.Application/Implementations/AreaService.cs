using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AreaService(IAreaRepository areaRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AreaDto>> GetAllAreasAsync()
        {
            var areas = await _areaRepository.GetAllAreasAsync();
            return _mapper.Map<IEnumerable<AreaDto>>(areas);
        }

        public async Task<bool> CreateAreaAsync(AreaDto areaDto)
        {
            var area = _mapper.Map<Area>(areaDto);
            return await _areaRepository.CreateAreaAsync(area);
        }

        public async Task<bool> UpdateAreaAsync(int id, string newName)
        {
            return await _areaRepository.UpdateAreaAsync(id, newName);
        }

        public async Task<bool> DeleteAreaAsync(int id)
        {
            return await _areaRepository.DeleteAreaAsync(id);
        }
    }
}
