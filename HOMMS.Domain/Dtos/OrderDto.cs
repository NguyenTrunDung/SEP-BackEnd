using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public int BranchId { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
    }

    public class OrderDetailDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? FoodId { get; set; }
        public int? MenuId { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string? Note { get; set; }
    }
}
