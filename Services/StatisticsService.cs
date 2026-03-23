using FishingSpot.PWA.Models;

namespace FishingSpot.PWA.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ISupabaseService _supabaseService;
        private readonly ILoggerService _logger;

        public StatisticsService(ISupabaseService supabaseService, ILoggerService logger)
        {
            _supabaseService = supabaseService;
            _logger = logger;
        }

        public async Task<FishingStatistics> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Calculating fishing statistics");

                var allCatches = await _supabaseService.GetAllCatchesAsync();

                // Filtrer par date si nécessaire
                if (startDate.HasValue)
                    allCatches = allCatches.Where(c => c.CatchDate >= startDate.Value).ToList();
                if (endDate.HasValue)
                    allCatches = allCatches.Where(c => c.CatchDate <= endDate.Value).ToList();

                var stats = new FishingStatistics
                {
                    TotalCatches = allCatches.Count,
                    TotalSpecies = allCatches.Select(c => c.FishName).Distinct().Count(),
                    TotalWeight = allCatches.Sum(c => c.Weight),
                    AverageWeight = allCatches.Any() ? allCatches.Average(c => c.Weight) : 0,
                    AverageLength = allCatches.Any() ? allCatches.Average(c => c.Length) : 0,
                    BiggestCatch = allCatches.OrderByDescending(c => c.Length).FirstOrDefault(),
                    HeaviestCatch = allCatches.OrderByDescending(c => c.Weight).FirstOrDefault(),

                    // Par espèce
                    CatchesBySpecies = allCatches
                        .GroupBy(c => c.FishName)
                        .ToDictionary(g => g.Key, g => g.Count()),

                    AverageSizeBySpecies = allCatches
                        .GroupBy(c => c.FishName)
                        .ToDictionary(g => g.Key, g => g.Average(c => c.Length)),

                    // Par mois
                    CatchesByMonth = allCatches
                        .GroupBy(c => c.CatchDate.ToString("yyyy-MM"))
                        .OrderBy(g => g.Key)
                        .ToDictionary(g => g.Key, g => g.Count()),

                    // Top locations
                    TopLocations = allCatches
                        .Where(c => !string.IsNullOrEmpty(c.LocationName))
                        .GroupBy(c => c.LocationName)
                        .Select(g => new LocationStats
                        {
                            LocationName = g.Key,
                            Count = g.Count(),
                            AverageSize = g.Average(c => c.Length)
                        })
                        .OrderByDescending(l => l.Count)
                        .Take(5)
                        .ToList(),

                    // Meilleure heure
                    BestTimeOfDay = allCatches
                        .Where(c => c.CatchTime.HasValue)
                        .GroupBy(c => c.CatchTime!.Value.Hours)
                        .OrderByDescending(g => g.Count())
                        .Select(g => TimeSpan.FromHours(g.Key))
                        .FirstOrDefault(),

                    // Par météo
                    CatchesByWeather = allCatches
                        .Where(c => !string.IsNullOrEmpty(c.WeatherCondition))
                        .GroupBy(c => c.WeatherCondition!)
                        .ToDictionary(g => g.Key, g => g.Count()),

                    // Tendances mensuelles
                    MonthlyTrends = allCatches
                        .GroupBy(c => c.CatchDate.ToString("MMM yyyy"))
                        .Select(g => new MonthlyTrend
                        {
                            Month = g.Key,
                            Count = g.Count(),
                            TotalWeight = g.Sum(c => c.Weight)
                        })
                        .OrderBy(t => t.Month)
                        .ToList()
                };

                _logger.LogInformation("Statistics calculated successfully", new Dictionary<string, object>
                {
                    { "TotalCatches", stats.TotalCatches },
                    { "TotalSpecies", stats.TotalSpecies }
                });

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error calculating statistics", ex);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetCatchesByHourAsync()
        {
            try
            {
                var catches = await _supabaseService.GetAllCatchesAsync();

                return catches
                    .Where(c => c.CatchTime.HasValue)
                    .GroupBy(c => $"{c.CatchTime!.Value.Hours:00}:00")
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting catches by hour", ex);
                return new Dictionary<string, int>();
            }
        }

        public async Task<List<FishCatch>> GetPersonalBestsAsync()
        {
            try
            {
                var catches = await _supabaseService.GetAllCatchesAsync();

                var personalBests = catches
                    .GroupBy(c => c.FishName)
                    .Select(g => g.OrderByDescending(c => c.Length).First())
                    .OrderByDescending(c => c.Length)
                    .Take(10)
                    .ToList();

                return personalBests;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting personal bests", ex);
                return new List<FishCatch>();
            }
        }
    }
}
