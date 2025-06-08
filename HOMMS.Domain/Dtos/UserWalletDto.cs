using HOMMS.Domain.Enums;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for wallet balance information (simplified for POC)
    /// All amounts are in VND (Vietnamese Dong)
    /// </summary>
    public class WalletBalanceDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// Wallet balance in VND
        /// </summary>
        public long Balance { get; set; }
        public string? CustomerCode { get; set; }
        public bool IsCustomerEnabled { get; set; }
    }

    /// <summary>
    /// DTO for wallet transaction information (simplified for POC)
    /// All amounts are in VND (Vietnamese Dong)
    /// </summary>
    public class WalletTransactionDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public WalletTransactionType TransactionType { get; set; }
        public string TransactionTypeName { get; set; } = string.Empty;
        /// <summary>
        /// Transaction amount in VND
        /// </summary>
        public long Amount { get; set; }
        /// <summary>
        /// Balance after transaction in VND
        /// </summary>
        public long BalanceAfter { get; set; }
        /// <summary>
        /// Transaction description (supports Vietnamese text)
        /// </summary>
        public string Description { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
    }

    /// <summary>
    /// DTO for adding money to wallet (simplified for POC)
    /// Amount in VND (Vietnamese Dong)
    /// </summary>
    public class AddMoneyToWalletDto
    {
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// Amount to add in VND
        /// </summary>
        public long Amount { get; set; }
        /// <summary>
        /// Description (supports Vietnamese text)
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for wallet adjustment
    /// </summary>
    public class WalletAdjustmentDto
    {
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
    }

    /// <summary>
    /// DTO for creating customer account (simplified for POC)
    /// Designed for Vietnamese customers
    /// </summary>
    public class CreateCustomerAccountDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CustomerCode { get; set; }
        public string? Address { get; set; }
        /// <summary>
        /// Customer notes in Vietnamese (allergies, dietary restrictions, etc.)
        /// </summary>
        public string? CustomerNotes { get; set; }
        /// <summary>
        /// Initial wallet balance in VND
        /// </summary>
        public long InitialBalance { get; set; }
    }

    /// <summary>
    /// DTO for customer account information (simplified for POC)
    /// Designed for Vietnamese customers
    /// </summary>
    public class CustomerAccountDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? CustomerCode { get; set; }
        /// <summary>
        /// Customer notes in Vietnamese
        /// </summary>
        public string? CustomerNotes { get; set; }
        /// <summary>
        /// Wallet balance in VND
        /// </summary>
        public long WalletBalance { get; set; }
        public bool IsCustomerAccount { get; set; }
        public bool IsCustomerEnabled { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// DTO for wallet transaction history response (simplified for POC)
    /// All amounts are in VND (Vietnamese Dong)
    /// </summary>
    public class WalletTransactionHistoryDto
    {
        public IEnumerable<WalletTransactionDto> Transactions { get; set; } = new List<WalletTransactionDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// Current wallet balance in VND
        /// </summary>
        public long CurrentBalance { get; set; }
    }
} 