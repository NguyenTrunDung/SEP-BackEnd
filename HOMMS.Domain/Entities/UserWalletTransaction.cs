using HOMMS.Domain.Entities.Base;
using HOMMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a wallet transaction for user accounts (simplified for POC)
    /// Amounts are stored in VND (Vietnamese Dong) as whole numbers
    /// </summary>
    public class UserWalletTransaction : BaseAuditableEntity<int>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the user ID this transaction belongs to
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the branch ID where this transaction occurred
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the transaction type (Credit, OrderPayment, Refund, Adjustment)
        /// </summary>
        [Required]
        public WalletTransactionType TransactionType { get; set; }
        
        /// <summary>
        /// Gets or sets the transaction amount in VND (positive for credits, negative for debits)
        /// VND doesn't use decimal places
        /// </summary>
        [Required]
        [Column(TypeName = "bigint")]
        public long Amount { get; set; }
        
        /// <summary>
        /// Gets or sets the wallet balance after this transaction in VND
        /// </summary>
        [Column(TypeName = "bigint")]
        public long BalanceAfter { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the transaction
        /// Supports Vietnamese text
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the order ID if this transaction is related to an order
        /// </summary>
        public int? OrderId { get; set; }
        
        /// <summary>
        /// Gets or sets the user navigation property
        /// </summary>
        public virtual ApplicationUser User { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the branch navigation property
        /// </summary>
        public virtual Branch Branch { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the order navigation property (if applicable)
        /// </summary>
        public virtual Order? Order { get; set; }
    }
} 