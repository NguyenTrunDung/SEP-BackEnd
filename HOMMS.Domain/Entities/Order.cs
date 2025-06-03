<<<<<<< HEAD
using HOMMS.Domain.Entities.Base;
using HOMMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
=======
﻿using HOMMS.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4

namespace HOMMS.Domain.Entities
{
    public class Order : BaseAuditableEntity<int>, IBranchEntity
    {
        public int BranchId { get; set; }
        public int? BranchUserId { get; set; } // For user-branch context, nullable for guest
<<<<<<< HEAD
        
        /// <summary>
        /// Gets or sets the user ID if this order is placed by a registered user (customer account)
        /// </summary>
        public string? UserId { get; set; }
        
=======
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        public DateTime OrderDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        [StringLength(10)]
        public string? ReceiveTime { get; set; }
        [StringLength(10)]
        public string? ReceiveType { get; set; }
        [StringLength(20)]
        public string? Type { get; set; }
        [StringLength(20)]
        public string? Status { get; set; }
<<<<<<< HEAD
        
        // Customer information (for both guest and registered customers)
=======
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        public int? CustomerId { get; set; }
        [StringLength(100)]
        public string? CustomerName { get; set; }
        [StringLength(20)]
        public string? CustomerPhone { get; set; }
        [StringLength(10)]
        public string? CustomerDob { get; set; }
        [StringLength(255)]
        public string? CustomerAddress { get; set; }
<<<<<<< HEAD
        
        // VAT information
=======
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        public bool? HasVat { get; set; }
        [StringLength(30)]
        public string? VatTaxCode { get; set; }
        [StringLength(100)]
        public string? VatName { get; set; }
        [StringLength(255)]
        public string? VatAddress { get; set; }
        [StringLength(255)]
        public string? VatEmail { get; set; }
<<<<<<< HEAD
        
        // Pricing and fees
=======
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        public int? Total { get; set; }
        public int? ShippingFee { get; set; }
        public bool? FoodTool { get; set; }
        public int? FoodToolFee { get; set; }
<<<<<<< HEAD
        // Payment information
        /// <summary>
        /// Gets or sets the payment method used for this order
        /// </summary>
        public OrderPaymentMethod PaymentMethod { get; set; } = OrderPaymentMethod.Cash;
        
        /// <summary>
        /// Gets or sets whether payment has been completed
        /// </summary>
        public bool IsPaid { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the amount paid from wallet in VND (if applicable)
        /// </summary>
        [Column(TypeName = "bigint")]
        public long? WalletAmountUsed { get; set; }
        
        // Order processing
=======
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        public bool? Printed { get; set; }
        public int? ConfirmedBy { get; set; }
        public DateTime? TimeConfirmed { get; set; }
        public DateTime? KitchenCompletionTime { get; set; }
        public int? DeliveryBy { get; set; }
        public DateTime? DeliveryCompletion { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        public int? LocationId { get; set; }
        public string? Note { get; set; }

<<<<<<< HEAD
        // Navigation properties
        public virtual Branch? Branch { get; set; }
        public virtual BranchUser? BranchUser { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public virtual ICollection<UserWalletTransaction> WalletTransactions { get; set; } = new List<UserWalletTransaction>();
    }
}
=======
        public virtual Branch? Branch { get; set; }
        public virtual BranchUser? BranchUser { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
