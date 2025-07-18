using HOMMS.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Entities
{
    public class SystemLog : BaseAuditableEntity<int>
    {
        public int? BranchId { get; set; }
        public virtual Branch? Branch { get; set; }

        public string? UserId { get; set; }
        [StringLength(255)]
        public string? Note { get; set; }

    }
}
