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
        public DateTime? ReceiveDate { get; set; }
        public string? ReceiveTime { get; set; }
        public string? Status { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public int? Total { get; set; }
        public string? Code { get; set; }
    }
}
