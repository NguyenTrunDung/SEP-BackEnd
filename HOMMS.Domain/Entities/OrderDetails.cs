using HOMMS.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOMMS.Domain.Entities
{
    public class OrderDetails : BaseAuditableEntity<int>
    {
        public int OrderId { get; set; }
        public int? FoodId { get; set; }
        public int? MenuId { get; set; }
        public int? Qty { get; set; }
        public int? Price { get; set; }
        public int? Total { get; set; }
        public string? Note { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Food? Food { get; set; }
        public virtual Menu? Menu { get; set; }
    }

}
