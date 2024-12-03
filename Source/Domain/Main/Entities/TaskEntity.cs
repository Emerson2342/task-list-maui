using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskListMaui.Source.Domain.Shared.Entities;

namespace TaskListMaui.Source.Domain.Main.Entities
{
    public class TaskEntity : Entity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartTime { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly Deadline { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        public string PhotoTask { get; set; } = string.Empty;

    }
}
