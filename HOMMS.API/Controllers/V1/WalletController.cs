using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HOMMS.Infrastructure.Repositories.Implementations.WalletRepository;

namespace HOMMS.API.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            var result = await _walletService.DepositAsync(request.UserId, request.Amount);
            return Ok(result);
        }

        [HttpPost("set-balance")]
        public async Task<IActionResult> SetBalance([FromBody] SetBalanceRequest request)
        {
            var result = await _walletService.SetBalanceAsync(request.UserId, request.NewBalance);
            return Ok(result);
        }
        [HttpGet("credit-history")]
        public async Task<IActionResult> GetWalletCreditHistory([FromQuery] string userId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                Console.WriteLine($"Calling GetWalletCreditHistory: userId={userId}, pageNumber={pageNumber}, pageSize={pageSize}");

                var result = await _walletService.GetWalletCreditHistoryAsync(userId, pageNumber, pageSize);

                if (result == null)
                {
                    Console.WriteLine("Result is null.");
                    return NotFound(new { status = "error", message = "No data found." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR in GetWalletCreditHistory: {ex.Message}");
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        [HttpGet("branch-transactions")]
        public async Task<IActionResult> GetWalletTransactionsByBranch([FromQuery] int branchId)
        {
            try
            {
                var result = await _walletService.GetWalletTransactionsByBranchAsync(branchId);

                return Ok(new
                {
                    status = "success",
                    data = result,
                    totalCount = result.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }


        }
        [HttpGet("purchase-history")]
        public async Task<IActionResult> GetPurchaseHistory([FromQuery] string userId)
        {
            try
            {
                var result = await _walletService.GetPurchaseHistoryByUserIdAsync(userId);
                return Ok(new
                {
                    status = "success",
                    data = result,
                    total = result.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        [HttpPost("add-transaction")]
        public async Task<IActionResult> AddWalletTransaction([FromBody] CreateUserWalletTransactionDto dto)
        {
            try
            {
                var result = await _walletService.AddWalletTransactionAsync(dto);
                return Ok(new
                {
                    status = "success",
                    data = new
                    {
                        result.Id,
                        result.UserId,
                        result.Amount,
                        result.BalanceAfter,
                        result.Description,
                        result.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }
        [HttpDelete("deactivate/{id}")]
        public async Task<IActionResult> DeactivateTransaction(int id)
        {
            var result = await _walletService.DeactivateTransactionAsync(id);

            if (!result)
                return NotFound(new { message = "Không tìm thấy giao dịch ví." });

            return Ok(new { message = "Đã ẩn giao dịch ví thành công." });
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateWalletTransaction([FromBody] UpdateUserWalletTransactionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _walletService.UpdateWalletTransactionAsync(dto);
            if (!result)
                return NotFound("Transaction not found or already deleted");

            return Ok("Transaction updated successfully");
        }
        /// <summary>
        /// Tạo ví mới cho người dùng
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequestDto dto)
        {
            var result = await _walletService.CreateWalletAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật số dư ví người dùng
        /// </summary>
        [HttpPut("updates")]
        public async Task<IActionResult> UpdateWallet([FromBody] UpdateWalletRequestDto dto)
        {
            var result = await _walletService.UpdateWalletAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Vô hiệu hóa (deactivate) ví người dùng
        /// </summary>
        [HttpDelete("deactivates/{id}")]
        public async Task<IActionResult> DeactivateWallet(int id)
        {
            var success = await _walletService.DeactivateWalletAsync(id);
            if (!success)
                return NotFound("Wallet not found.");

            return Ok(new { message = "Wallet deactivated successfully" });
        }
    }
}

    public class DepositRequest
    {
        public string UserId { get; set; } = string.Empty;
        public long Amount { get; set; }
    }

    public class SetBalanceRequest
    {
        public string UserId { get; set; } = string.Empty;
        public long NewBalance { get; set; }
    }

