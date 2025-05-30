using HOMMS.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Entities
{
    public class Order : BaseAuditableEntity<int>, IBranchEntity
    {
        public int BranchId { get; set; }
        public int? BranchUserId { get; set; } // For user-branch context, nullable for guest
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
        public int? CustomerId { get; set; }
        [StringLength(100)]
        public string? CustomerName { get; set; }
        [StringLength(20)]
        public string? CustomerPhone { get; set; }
        [StringLength(10)]
        public string? CustomerDob { get; set; }
        [StringLength(255)]
        public string? CustomerAddress { get; set; }
        public bool? HasVat { get; set; }
        [StringLength(30)]
        public string? VatTaxCode { get; set; }
        [StringLength(100)]
        public string? VatName { get; set; }
        [StringLength(255)]
        public string? VatAddress { get; set; }
        [StringLength(255)]
        public string? VatEmail { get; set; }
        public int? Total { get; set; }
        public int? ShippingFee { get; set; }
        public bool? FoodTool { get; set; }
        public int? FoodToolFee { get; set; }
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

        public virtual Branch? Branch { get; set; }
        public virtual BranchUser? BranchUser { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
