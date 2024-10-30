using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskListMaui.Source.Domain.Main.DTOs.TaskDTOs
{
    public class RequestTask
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("startTime")]
        public DateOnly StartTime { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [JsonPropertyName("deadLine")]
        public DateOnly Deadline { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        [JsonConstructor]
        public RequestTask()
        {

        }
    }
}
