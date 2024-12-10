using System.Text.Json.Serialization;

namespace TaskListMaui.Source.Domain.Shared.Entities
{
    public abstract class Entity
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        protected Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}
