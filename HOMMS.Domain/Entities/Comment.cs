using HOMMS.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Entities
{
    public class Comment : BaseAuditableEntity<int>, IBranchEntity
    {
        public int? Star { get; set; }

        [StringLength(255)]
        public string? CommentLines { get; set; }

        public int? FoodId { get; set; }
        public virtual Food? Food { get; set; }

        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }


        public string UserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }



        public int BranchId { get; set; }

        public virtual Branch? Branch { get; set; }





    }
}



