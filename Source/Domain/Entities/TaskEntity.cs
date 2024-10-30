using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskListMaui.Source.Shared.Entities;

namespace TaskListMaui.Source.Main.Entities
{
    public class TaskEntity : Entity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartTime { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly Deadline { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

    }
}
