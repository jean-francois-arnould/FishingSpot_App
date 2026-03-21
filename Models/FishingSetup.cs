using System.Text.Json.Serialization;

namespace FishingSpot.PWA.Models
{
    public class FishingSetup
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        // Equipment References (Foreign Keys)
        [JsonPropertyName("rod_id")]
        public long? RodId { get; set; }

        [JsonPropertyName("reel_id")]
        public long? ReelId { get; set; }

        [JsonPropertyName("line_id")]
        public long? LineId { get; set; }

        [JsonPropertyName("lure_id")]
        public long? LureId { get; set; }

        [JsonPropertyName("leader_id")]
        public long? LeaderId { get; set; }

        [JsonPropertyName("hook_id")]
        public long? HookId { get; set; }

        // Setup Properties
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("is_current")]
        public bool IsCurrent { get; set; }

        [JsonPropertyName("is_favorite")]
        public bool IsFavorite { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
