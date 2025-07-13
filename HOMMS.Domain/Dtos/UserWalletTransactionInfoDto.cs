using System;

namespace HOMMS.Domain.Dtos
{
    public class UserWalletTransactionInfoDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public long Amount { get; set; }
        public long BalanceAfter { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
} 