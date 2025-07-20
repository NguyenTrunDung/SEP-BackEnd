using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class BranchRoleCreateUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public bool IsDefault { get; set; }
        public List<string>? Permissions { get; set; }
    }
    public class BranchRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public bool IsDefault { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

}
