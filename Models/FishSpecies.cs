using System.Text.Json.Serialization;

namespace FishingSpot.PWA.Models
{
    public class FishSpecies
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("common_name")]
        public string CommonName { get; set; } = string.Empty;

        [JsonPropertyName("scientific_name")]
        public string? ScientificName { get; set; }

        [JsonPropertyName("family")]
        public string? Family { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("min_legal_size")]
        public int? MinLegalSize { get; set; }

        [JsonPropertyName("icon_emoji")]
        public string IconEmoji { get; set; } = "🐟";

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; } = true;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        // Helper pour l'affichage dans les listes
        [JsonIgnore]
        public string DisplayName => $"{IconEmoji} {CommonName}";

        [JsonIgnore]
        public string CategoryDisplay => Category switch
        {
            "carnassier" => "🦈 Carnassier",
            "salmonidé" => "🌊 Salmonidé",
            "migrateur" => "➡️ Migrateur",
            "cyprinidé" => "🐠 Cyprinidé",
            "autre" => "🐟 Autre",
            _ => "🐟 Non catégorisé"
        };
    }
}
