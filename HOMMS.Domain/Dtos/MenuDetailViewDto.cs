using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class MenuDetailViewDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? TimeOfDay { get; set; }
        public bool IsTime { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public string? Name { get; set; }

        public List<MenuDetailsDto> Details { get; set; } = new();
    }
    public class UpdateMenuDto
    {
        public int Id { get; set; }   // ID của Menu
        public DateTime Date { get; set; }
        public string? TimeOfDay { get; set; }
        public bool IsTime { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public string? Name { get; set; }

        public List<MenuDetailsDto> Details { get; set; } = new(); // update toàn bộ danh sách
    }
    public class CreateMenuDto
    {
        public int Id { get; set; }   // ID của Menu
        public DateTime Date { get; set; }
        public string? TimeOfDay { get; set; }
        public bool IsTime { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public string? Name { get; set; }

        public List<MenuDetailsDto> Details { get; set; } = new(); // update toàn bộ danh sách
    }
}
