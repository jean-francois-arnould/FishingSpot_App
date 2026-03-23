using FishingSpot.PWA.Models;

namespace FishingSpot.PWA.Services
{
    public class FishingStatistics
    {
        public int TotalCatches { get; set; }
        public int TotalSpecies { get; set; }
        public double TotalWeight { get; set; }
        public double AverageWeight { get; set; }
        public double AverageLength { get; set; }
        public FishCatch? BiggestCatch { get; set; }
        public FishCatch? HeaviestCatch { get; set; }
        public Dictionary<string, int> CatchesBySpecies { get; set; } = new();
        public Dictionary<string, double> AverageSizeBySpecies { get; set; } = new();
        public Dictionary<string, int> CatchesByMonth { get; set; } = new();
        public List<LocationStats> TopLocations { get; set; } = new();
        public TimeSpan? BestTimeOfDay { get; set; }
        public Dictionary<string, int> CatchesByWeather { get; set; } = new();
        public List<MonthlyTrend> MonthlyTrends { get; set; } = new();
    }

    public class LocationStats
    {
        public string LocationName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double AverageSize { get; set; }
    }

    public class MonthlyTrend
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
        public double TotalWeight { get; set; }
    }

    public interface IStatisticsService
    {
        Task<FishingStatistics> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, int>> GetCatchesByHourAsync();
        Task<List<FishCatch>> GetPersonalBestsAsync();
    }
}
