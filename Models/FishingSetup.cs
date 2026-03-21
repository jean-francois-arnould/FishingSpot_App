using System.Text.Json.Serialization;

namespace FishingSpot.PWA.Models
{
    public class FishingSetup
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        // Rod (Canne)
        [JsonPropertyName("rod_brand")]
        public string? RodBrand { get; set; }

        [JsonPropertyName("rod_model")]
        public string? RodModel { get; set; }

        [JsonPropertyName("rod_length")]
        public double? RodLength { get; set; }

        [JsonPropertyName("rod_power")]
        public string? RodPower { get; set; }

        // Reel (Moulinet)
        [JsonPropertyName("reel_brand")]
        public string? ReelBrand { get; set; }

        [JsonPropertyName("reel_model")]
        public string? ReelModel { get; set; }

        [JsonPropertyName("reel_type")]
        public string? ReelType { get; set; }

        // Line (Ligne/Fil)
        [JsonPropertyName("line_type")]
        public string? LineType { get; set; }

        [JsonPropertyName("line_diameter")]
        public double? LineDiameter { get; set; }

        [JsonPropertyName("line_breaking_strength")]
        public double? LineBreakingStrength { get; set; }

        // Hook & Bait
        [JsonPropertyName("hook_size")]
        public string? HookSize { get; set; }

        [JsonPropertyName("bait_type")]
        public string? BaitType { get; set; }

        // General
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
