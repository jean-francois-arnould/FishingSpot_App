namespace FishingSpot.PWA.Models
{
    public class GeolocationPosition
    {
        public string? Latitude { get; set; } = string.Empty;
        public string? Longitude { get; set; } = string.Empty;
        public double? Accuracy { get; set; }
    }
}
