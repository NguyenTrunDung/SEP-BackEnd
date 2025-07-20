using HOMMS.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Entities
{
    public class UserWallet : BaseAuditableEntity<int>
    {
        /// <summary>
        /// Foreign key to ApplicationUser
        /// </summary>
        [Required]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Current wallet balance
        /// </summary>
        [Column(TypeName = "decimal(18,0)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Navigation property for related user
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
