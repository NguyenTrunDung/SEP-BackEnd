using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? RoomNumber { get; set; }
        public int? Capacity { get; set; }
        public int? Sort { get; set; }
        public bool IsActive { get; set; } = true;
        public int AreaId { get; set; }
        public int BranchId { get; set; }
    }
}
