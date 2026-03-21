using System.Text.Json.Serialization;

namespace FishingSpot.PWA.Models
{
    public class FishingSetup
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        // Références aux équipements
        [JsonPropertyName("rod_id")]
        public int? RodId { get; set; }

        [JsonPropertyName("reel_id")]
        public int? ReelId { get; set; }

        [JsonPropertyName("line_id")]
        public int? LineId { get; set; }

        [JsonPropertyName("lure_id")]
        public int? LureId { get; set; }

        [JsonPropertyName("leader_id")]
        public int? LeaderId { get; set; }

        [JsonPropertyName("hook_id")]
        public int? HookId { get; set; }

        [JsonPropertyName("is_favorite")]
        public bool IsFavorite { get; set; }

        [JsonPropertyName("is_current")]
        public bool IsCurrent { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime CreatedAt { get; set; }
    }
}
