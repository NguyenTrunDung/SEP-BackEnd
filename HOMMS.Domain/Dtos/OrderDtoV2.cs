using HOMMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using HOMMS.Domain.Enums;


namespace HOMMS.Domain.Dtos
    {
        public class OrderDtoV2
        {
            public int BranchId { get; set; }
            public string? UserId { get; set; }
            public string? PatientId { get; set; }
            public bool IsPatientOrder { get; set; }

            public DateTime OrderDate { get; set; }
            public DateTime? ReceiveDate { get; set; }
            public string? ReceiveTime { get; set; }
            public string? ReceiveType { get; set; }
            public string? Type { get; set; }
            public string? Status { get; set; }

            public string? CustomerName { get; set; }
            public string? CustomerPhone { get; set; }
            public string? CustomerAddress { get; set; }

            public int? Total { get; set; }
            public int? ShippingFee { get; set; }
            public int? FoodToolFee { get; set; }
            public OrderPaymentMethod PaymentMethod { get; set; }
            public bool IsPaid { get; set; }
            public long? WalletAmountUsed { get; set; }

            public string? Code { get; set; }
            public string? Note { get; set; }

            public int? LocationId { get; set; }

            public List<OrderDetailsDto> OrderDetails { get; set; } = new();
        }
}
