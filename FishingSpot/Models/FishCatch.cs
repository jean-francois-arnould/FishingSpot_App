using SQLite;

namespace FishingSpot.Models
{
    [Table("FishCatches")]
    public class FishCatch
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string FishName { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public DateTime CatchDate { get; set; }
        public TimeSpan CatchTime { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Setup utilisé (remplace les IDs individuels de matériel)
        public int? SetupId { get; set; }
    }
}
