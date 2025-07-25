using HOMMS.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Entities
{
    public class Department : BaseAuditableEntity<int>, IBranchEntity
    {


        [StringLength(255)]
        public string? Name { get; set; }
        public int? Sort { get; set; }
        public bool IsActive { get; set; } = true;
        public int BranchId { get; set; }

        public virtual Branch? Branch { get; set; }

        public virtual ICollection<ApplicationUser> user { get; set; } = new List<ApplicationUser>();

    }
}


