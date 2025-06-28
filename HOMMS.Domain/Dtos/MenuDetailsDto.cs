using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class MenuDetailsDto
    {
        public int? Id { get; set; }             
        public int MenuId { get; set; }
        public int FoodId { get; set; }           
        public string? FoodName { get; set; }
        public bool? IsQty { get; set; }
        public int? Qty { get; set; }
        public int? Sold { get; set; }
        public int? PriceForGuest { get; set; }
        public int? PriceForPatient { get; set; }
        public int? PriceForStaff { get; set; }
        public int? DiscountPrice { get; set; }
        public bool? Status { get; set; }
        public string? DiscountFrom { get; set; }
        public string? DiscountTo { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Food information
        public FoodDto? Food { get; set; }
    }
}
