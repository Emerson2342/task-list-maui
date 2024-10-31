using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskListMaui.Source.Domain.Main.Entities;

namespace TaskListMaui.Source.Domain.Main.UseCase.ResponseCase
{
    public class Response : Shared.UseCases.Response
    {
        public User? User { get; set; }
        public TaskEntity? Task{ get; set; }
        public List<TaskEntity> TaskList { get; set; } = [];
    }
}
