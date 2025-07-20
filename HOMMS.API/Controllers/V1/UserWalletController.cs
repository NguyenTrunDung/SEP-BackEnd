using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserWalletController : ControllerBase
    {
        private readonly IUserWalletService _userWalletService;

        public UserWalletController(IUserWalletService userWalletService)
        {
            _userWalletService = userWalletService;
        }

        /// <summary>
        /// Gets wallet information for a specific user
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponseBase<UserWalletInfoDto>>> GetUserWallet(string userId)
        {
            try
            {
                var walletDto = await _userWalletService.GetUserWalletInfoAsync(userId);
                if (walletDto == null)
                    return NotFound(ApiResponseBase<UserWalletInfoDto>.Error("User not found"));
                return Ok(ApiResponseBase<UserWalletInfoDto>.Success(walletDto, "Wallet information retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseBase<UserWalletInfoDto>.Error($"Internal server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Deposits money to a user's wallet
        /// </summary>
        [HttpPost("deposit")]
        public async Task<ActionResult<ApiResponseBase<UserWalletTransactionInfoDto>>> Deposit([FromBody] UserWalletDepositRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponseBase<UserWalletTransactionInfoDto>.Error("Invalid request data"));
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                    return Unauthorized(ApiResponseBase<UserWalletTransactionInfoDto>.Error("User not authenticated"));
                var branchIdClaim = User.FindFirst("BranchId")?.Value;
                if (!int.TryParse(branchIdClaim, out int branchId))
                    return BadRequest(ApiResponseBase<UserWalletTransactionInfoDto>.Error("Branch ID not found"));
                var transaction = await _userWalletService.DepositAsync(
                    request.UserId, 
                    request.Amount, 
                    request.Description, 
                    currentUserId, 
                    branchId);
                var transactionDto = new UserWalletTransactionInfoDto
                {
                    Id = transaction.Id,
                    UserId = transaction.UserId,
                    UserFullName = transaction.User?.FullName ?? string.Empty,
                    TransactionType = transaction.TransactionType.ToString(),
                    Amount = transaction.Amount,
                    BalanceAfter = transaction.BalanceAfter,
                    Description = transaction.Description,
                    OrderId = transaction.OrderId,
                    BranchId = transaction.BranchId,
                    BranchName = transaction.Branch?.Name ?? string.Empty,
                    CreatedAt = transaction.CreatedAt,
                    CreatedBy = transaction.CreatedBy
                };
                return Ok(ApiResponseBase<UserWalletTransactionInfoDto>.Success(transactionDto, "Deposit successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseBase<UserWalletTransactionInfoDto>.Error($"Internal server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets deposit history for a user
        /// </summary>
        [HttpGet("{userId}/deposit-history")]
        public async Task<ActionResult<ApiResponseBase<UserWalletTransactionHistoryDto>>> GetDepositHistory(
            string userId, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (transactions, totalCount) = await _userWalletService.GetDepositHistoryAsync(userId, pageNumber, pageSize);
                var transactionDtos = transactions.Select(t => new UserWalletTransactionInfoDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    UserFullName = t.User?.FullName ?? string.Empty,
                    TransactionType = t.TransactionType.ToString(),
                    Amount = t.Amount,
                    BalanceAfter = t.BalanceAfter,
                    Description = t.Description,
                    OrderId = t.OrderId,
                    BranchId = t.BranchId,
                    BranchName = t.Branch?.Name ?? string.Empty,
                    CreatedAt = t.CreatedAt,
                    CreatedBy = t.CreatedBy
                });
                var response = new UserWalletTransactionHistoryDto
                {
                    Transactions = transactionDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };
                return Ok(ApiResponseBase<UserWalletTransactionHistoryDto>.Success(response, "Deposit history retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseBase<UserWalletTransactionHistoryDto>.Error($"Internal server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets purchase history for a user
        /// </summary>
        [HttpGet("{userId}/purchase-history")]
        public async Task<ActionResult<ApiResponseBase<UserWalletTransactionHistoryDto>>> GetPurchaseHistory(
            string userId, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (transactions, totalCount) = await _userWalletService.GetPurchaseHistoryAsync(userId, pageNumber, pageSize);
                var transactionDtos = transactions.Select(t => new UserWalletTransactionInfoDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    UserFullName = t.User?.FullName ?? string.Empty,
                    TransactionType = t.TransactionType.ToString(),
                    Amount = t.Amount,
                    BalanceAfter = t.BalanceAfter,
                    Description = t.Description,
                    OrderId = t.OrderId,
                    BranchId = t.BranchId,
                    BranchName = t.Branch?.Name ?? string.Empty,
                    CreatedAt = t.CreatedAt,
                    CreatedBy = t.CreatedBy
                });
                var response = new UserWalletTransactionHistoryDto
                {
                    Transactions = transactionDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };
                return Ok(ApiResponseBase<UserWalletTransactionHistoryDto>.Success(response, "Purchase history retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseBase<UserWalletTransactionHistoryDto>.Error($"Internal server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets list of users for UserTable
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponseBase<List<UserWalletListItemDto>>>> GetAllUsers()
        {
            var users = await _userWalletService.GetUserWalletListAsync();
            return Ok(ApiResponseBase<List<UserWalletListItemDto>>.Success(users, "User list retrieved successfully"));
        }

        [HttpGet("users-by-branch")]
        public async Task<ActionResult<ApiResponseBase<List<UserWalletInfoDto>>>> GetUsersByBranch([FromQuery] int? branchId)
        {
            int resolvedBranchId = branchId ?? 0;
            if (resolvedBranchId == 0)
            {
                var branchIdHeader = Request.Headers["X-Branch-Id"].FirstOrDefault();
                if (!int.TryParse(branchIdHeader, out resolvedBranchId))
                    return BadRequest(ApiResponseBase<List<UserWalletInfoDto>>.Error("BranchId is required"));
            }
            var users = await _userWalletService.GetUserWalletListByBranchAsync(resolvedBranchId);
            return Ok(ApiResponseBase<List<UserWalletInfoDto>>.Success(users, "User list retrieved successfully"));
        }
    }
} 