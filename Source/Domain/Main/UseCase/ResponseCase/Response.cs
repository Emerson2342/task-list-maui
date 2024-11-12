using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskListMaui.Source.Domain.Main.Entities;

namespace TaskListMaui.Source.Domain.Main.UseCase.ResponseCase
{
    public class Response
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public int Status { get; set; } = 400;
        [JsonPropertyName("isSucess")]
        public bool IsSuccess => Status is >= 200 and <= 299;

        [JsonPropertyName("user")]
        public User? User { get; set; }
        [JsonPropertyName("taskList")]
        public TaskEntity? Task{ get; set; }
        [JsonPropertyName("tasksList")]
        public List<TaskEntity> TaskList { get; set; } = [];

        [JsonConstructor]
        public Response()
        {
            
        }

        public Response(string message, int status)
        {
            Message = message;
            Status = status;
        }
    }
}
