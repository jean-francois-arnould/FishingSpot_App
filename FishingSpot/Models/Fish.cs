namespace FishingSpot.Models
{
    public class Fish
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Habitat { get; set; } = string.Empty;
        public string BestBait { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double AverageSize { get; set; }
        public double MaxSize { get; set; }
    }
}
