using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Common.Helpers
{
    public class FoodCreateRequest
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public int? CategoryId { get; set; }

        public bool IsSetDish { get; set; }

        public bool IsAddOn { get; set; }

        public int? PriceForGuest { get; set; }

        public int? PriceForPatient { get; set; }

        public int? PriceForStaff { get; set; }

        public int? Sort { get; set; }

        public int BranchId { get; set; }

        public IFormFile? Image { get; set; }
    }
}
