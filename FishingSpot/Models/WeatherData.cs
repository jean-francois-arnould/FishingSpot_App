using SQLite;

namespace FishingSpot.Models
{
    [Table("WeatherData")]
    public class WeatherData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public double WindSpeed { get; set; }
        public int Humidity { get; set; }
        public double Pressure { get; set; }
        public int? CatchId { get; set; }
    }
}
