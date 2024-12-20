﻿using System.Text.Json.Serialization;

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

        [JsonPropertyName("photoFile")]
        public Stream? PhotoFile { get; set; }

        [JsonPropertyName("photoTask")]
        public string? PhotoTask { get; set; } = string.Empty;

        [JsonConstructor]
        public RequestTask()
        {

        }
    }
}
