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
    public class SystemLogService: ISystemLogService
    {

        private readonly ISystemLogRepository _systemLogRepository;
        private readonly IMapper _mapper;

        public SystemLogService(ISystemLogRepository systemLogRepository, IMapper mapper)
        {
           _systemLogRepository = systemLogRepository;
            _mapper = mapper;
        }

        public async Task<AddSystemLogDto> AddSystemLog(AddSystemLogDto addSystemLogDto)
        {
            var sys = _mapper.Map<SystemLog>(addSystemLogDto);
            var tem = await _systemLogRepository.AddAsync(sys);
            return _mapper.Map<AddSystemLogDto>(tem);
        }

        public async Task<IEnumerable<SystemLogDto>> GetSystemLogAll(int branchId, DateTime dateStart, DateTime dateEnd)
        {
            var sys = await _systemLogRepository.GetSystemLogAll(branchId, dateStart,dateEnd);

            var log = sys.Select( tem => new SystemLogDto
            {

                UserId = tem.UserId,
                Note = tem.Note,
                Date = tem.LastModifiedAt.HasValue
               ? tem.LastModifiedAt.Value
               : tem.CreatedAt
            });


            return _mapper.Map<IEnumerable<SystemLogDto>> (log);
        }
    }
}
