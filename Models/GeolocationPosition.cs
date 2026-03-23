using System.Text.Json.Serialization;

namespace FishingSpot.PWA.Models
{
    public class GeolocationPosition
    {
        [JsonPropertyName("latitude")]
        public string? Latitude { get; set; } = string.Empty;

        [JsonPropertyName("longitude")]
        public string? Longitude { get; set; } = string.Empty;

        [JsonPropertyName("accuracy")]
        public double? Accuracy { get; set; }
    }
}
