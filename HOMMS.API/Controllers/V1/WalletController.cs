using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Infrastructure.Repositories.Interfaces;
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

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
