using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletService _walletService;
        private readonly IBranchContext _branchContext;
        private readonly IMapper _mapper;

        public WalletController(IWalletRepository walletRepository, IBranchContext branchContext, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _branchContext = branchContext;
            _mapper = mapper;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            var result = await _walletService.DepositAsync(request.UserId, request.Amount, request.Description);
            return Ok(result);
        }

        [HttpPost("set-balance")]
        public async Task<IActionResult> SetBalance([FromBody] SetBalanceRequest request)
        {
            var result = await _walletService.SetBalanceAsync(request.UserId, request.NewBalance);
            return Ok(result);
        }
    }

    public class DepositRequest
    {
        public string UserId { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class SetBalanceRequest
    {
        public string UserId { get; set; } = string.Empty;
        public long NewBalance { get; set; }
    }
}
