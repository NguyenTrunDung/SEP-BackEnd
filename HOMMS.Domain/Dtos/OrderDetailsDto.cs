using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public int? Qty { get; set; }
        public int? Price { get; set; }
        public int? Total { get; set; }
        public string? Note { get; set; }
        public string? FoodName { get; set; }
        public string? MenuName { get; set; }
    }
}
