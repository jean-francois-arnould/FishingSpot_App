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

    public enum StatisticsPeriod
    {
        Last30Days,
        Last90Days,
        CurrentYear,
        AllTime
    }

    public class StatisticsDashboard
    {
        public StatisticsPeriod Period { get; set; }
        public string PeriodLabel { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Today;
        public int TotalAvailableCatches { get; set; }
        public FishingStatistics Summary { get; set; } = new();
        public ConditionsSummary Conditions { get; set; } = new();
        public List<FishCatch> Catches { get; set; } = new();
        public List<SpeciesBreakdown> SpeciesBreakdown { get; set; } = new();
        public List<WeatherBreakdown> WeatherBreakdown { get; set; } = new();
        public List<HourlyBreakdown> HourlyBreakdown { get; set; } = new();
        public List<LocationStats> TopLocations { get; set; } = new();
        public List<MonthlyTrend> MonthlyTrends { get; set; } = new();
        public List<FishCatch> PersonalBests { get; set; } = new();
        public AdvancedFishingInsights AdvancedInsights { get; set; } = new();
    }

    public class ConditionsSummary
    {
        public string BestTimeSlot { get; set; } = "Non renseignée";
        public int BestTimeSlotCount { get; set; }
        public string BestWeatherCondition { get; set; } = "Non renseignée";
        public int BestWeatherCount { get; set; }
        public double? AverageTemperature { get; set; }
        public double? AverageWindSpeed { get; set; }
        public double? AverageHumidity { get; set; }
        public int WeatherSampleCount { get; set; }
        public LocationStats? BestLocation { get; set; }
    }

    public class SpeciesBreakdown
    {
        public string FishName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double AverageLength { get; set; }
        public double TotalWeight { get; set; }
        public double BiggestLength { get; set; }
        public double HeaviestWeight { get; set; }
    }

    public class WeatherBreakdown
    {
        public string Condition { get; set; } = string.Empty;
        public int Count { get; set; }
        public int? WeatherCode { get; set; }
        public double? AverageTemperature { get; set; }
        public double? AverageWindSpeed { get; set; }
    }

    public class HourlyBreakdown
    {
        public int Hour { get; set; }
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
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
        public DateTime MonthDate { get; set; }
        public int Count { get; set; }
        public double TotalWeight { get; set; }
    }

    public class AdvancedFishingInsights
    {
        public List<SpotProductivity> ProductiveSpots { get; set; } = new();
        public List<SetupEfficiency> SetupEfficiency { get; set; } = new();
        public List<SessionScore> SessionScores { get; set; } = new();
        public List<ReturnSuggestion> ReturnSuggestions { get; set; } = new();
        public List<SpeciesRecord> SpeciesRecords { get; set; } = new();
    }

    public class SpotProductivity
    {
        public string SpotKey { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool HasCoordinates => Latitude.HasValue && Longitude.HasValue;
        public int Count { get; set; }
        public int SpeciesCount { get; set; }
        public double TotalWeight { get; set; }
        public double AverageLength { get; set; }
        public double AverageWeight { get; set; }
        public double ProductivityScore { get; set; }
        public double BestSessionScore { get; set; }
        public DateTime LastCatchDate { get; set; }
        public FishCatch? BestCatch { get; set; }
        public List<string> Species { get; set; } = new();
    }

    public class SetupEfficiency
    {
        public int SetupId { get; set; }
        public string SetupName { get; set; } = string.Empty;
        public int Count { get; set; }
        public int SpeciesCount { get; set; }
        public double TotalWeight { get; set; }
        public double AverageLength { get; set; }
        public double AverageWeight { get; set; }
        public double EfficiencyScore { get; set; }
        public double BestSessionScore { get; set; }
        public DateTime LastCatchDate { get; set; }
        public FishCatch? BestCatch { get; set; }
    }

    public class SessionScore
    {
        public int CatchId { get; set; }
        public FishCatch Catch { get; set; } = new();
        public double Score { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string PrimaryReason { get; set; } = string.Empty;
        public double LengthScore { get; set; }
        public double WeightScore { get; set; }
        public double RarityScore { get; set; }
        public double WeatherScore { get; set; }
        public bool IsPersonalRecord { get; set; }
    }

    public class ReturnSuggestion
    {
        public string SpotName { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime SuggestedAt { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string WeatherSummary { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
        public int HistoricalMatches { get; set; }
        public bool UsesForecast { get; set; }
        public int? WeatherCode { get; set; }
        public double? Temperature { get; set; }
        public double? WindSpeed { get; set; }
        public int? Humidity { get; set; }
        public int? PrecipitationProbability { get; set; }
    }

    public class SpeciesRecord
    {
        public string FishName { get; set; } = string.Empty;
        public FishCatch Catch { get; set; } = new();
        public double Length { get; set; }
        public double Weight { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime CatchDate { get; set; }
        public string LocationName { get; set; } = string.Empty;
    }

    public interface IStatisticsService
    {
        Task<FishingStatistics> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<StatisticsDashboard> GetDashboardAsync(StatisticsPeriod period = StatisticsPeriod.Last90Days);
        Task<Dictionary<string, int>> GetCatchesByHourAsync();
        Task<List<FishCatch>> GetPersonalBestsAsync();
    }
}
