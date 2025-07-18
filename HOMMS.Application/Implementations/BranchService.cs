using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace HOMMS.Application.Implementations
{
    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBranchContext _branchContext;
        private readonly IMapper _mapper;

        public BranchService(IUnitOfWork unitOfWork, IBranchContext branchContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _branchContext = branchContext;
            _mapper = mapper;
        }

        public async Task<List<BranchDto>> GetBranchesAsync(string userId)
        {
            var branches = await _unitOfWork.BranchRepository.GetUserBranchesAsync(userId);
            return _mapper.Map<List<BranchDto>>(branches);
        }

        public async Task<List<BranchDto>> GetActiveBranchesAsync()
        {
            var branches = await _unitOfWork.BranchRepository.GetActiveBranchesAsync();
            return _mapper.Map<List<BranchDto>>(branches);
        }

        public async Task<BranchDto> GetDefaultBranchAsync(string userId)
        {
            var branch = await _unitOfWork.BranchRepository.GetUserDefaultBranchAsync(userId);
            if (branch == null)
            {
                int branchId = _branchContext.GetCurrentBranchId();
                branch = await _unitOfWork.BranchRepository.GetByIdAsync(branchId);
            }
            if (branch == null)
            {
                var activeBranches = await _unitOfWork.BranchRepository.GetActiveBranchesAsync();
                branch = activeBranches.FirstOrDefault();
            }
            return branch == null ? null : _mapper.Map<BranchDto>(branch);
        }

        public async Task<BranchDto> GetByIdAsync(int branchId)
        {
            var branch = await _unitOfWork.BranchRepository.GetByIdAsync(branchId);
            return branch == null ? null : _mapper.Map<BranchDto>(branch);
        }

        public async Task SetUserDefaultBranchAsync(string userId, int branchId)
        {
            await _unitOfWork.BranchRepository.SetUserDefaultBranchAsync(userId, branchId);
            await _unitOfWork.SaveChangesAsync();
        }

        public int GetCurrentBranchId() => _branchContext.GetCurrentBranchId();
        public void SetCurrentBranchId(int branchId) => _branchContext.SetCurrentBranchId(branchId);
    }
} 