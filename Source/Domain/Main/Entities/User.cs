using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskListMaui.Source.Domain.Main.ValueObjects;
using TaskListMaui.Source.Domain.Shared.Entities;
using Email = TaskListMaui.Source.Domain.Main.ValueObjects.Email;

namespace TaskListMaui.Source.Domain.Main.Entities
{
    public class User : Entity
    {
        public string Name { get; set; } = string.Empty;
        public Email MyProperty { get; set; } = new(string.Empty);
        public Password Password { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; } = false;
        public string PhotoTask { get; set; } = string.Empty;

        public IList<TaskEntity>? Tasks { get; set; }
    }
}
