using System.Text.Json.Serialization;

namespace FishingSpot.PWA.Models 
{ 
    public class FishingBrand
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; } = true;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        // Helper pour l'affichage
        [JsonIgnore]
        public string DisplayName => Name;
    }

    // Enum pour les catégories
    public static class BrandCategory
    {
        public const string Rod = "rod";
        public const string Reel = "reel";
        public const string Line = "line";
        public const string Lure = "lure";
    }
}
