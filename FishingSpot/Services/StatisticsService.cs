using FishingSpot.Models;

namespace FishingSpot.Services
{
    public class StatisticsService
    {
        private readonly SQLiteDatabaseService _databaseService;

        public StatisticsService(SQLiteDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<StatisticsData> GetStatisticsAsync()
        {
            var catches = await _databaseService.GetAllCatchesAsync();

            return new StatisticsData
            {
                TotalCatches = catches.Count,
                TotalWeight = catches.Sum(c => c.Weight),
                AverageWeight = catches.Any() ? catches.Average(c => c.Weight) : 0,
                AverageLength = catches.Any() ? catches.Average(c => c.Length) : 0,
                BiggestFish = catches.OrderByDescending(c => c.Weight).FirstOrDefault(),
                LongestFish = catches.OrderByDescending(c => c.Length).FirstOrDefault(),
                MostCaughtFish = catches.GroupBy(c => c.FishName)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new FishCount { Name = g.Key, Count = g.Count() })
                    .FirstOrDefault(),
                CatchesByMonth = catches
                    .GroupBy(c => new { c.CatchDate.Year, c.CatchDate.Month })
                    .Select(g => new MonthlyCount
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(m => m.Year).ThenBy(m => m.Month)
                    .ToList(),
                CatchesByFish = catches
                    .GroupBy(c => c.FishName)
                    .Select(g => new FishCount { Name = g.Key, Count = g.Count() })
                    .OrderByDescending(f => f.Count)
                    .ToList(),
                CatchesByLocation = catches
                    .GroupBy(c => c.LocationName)
                    .Select(g => new LocationCount { Name = g.Key, Count = g.Count() })
                    .OrderByDescending(l => l.Count)
                    .Take(10)
                    .ToList()
            };
        }

        public async Task<List<FishCatch>> GetCatchesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var allCatches = await _databaseService.GetAllCatchesAsync();
            return allCatches.Where(c => c.CatchDate >= startDate && c.CatchDate <= endDate).ToList();
        }

        public async Task<List<FishCatch>> GetCatchesByFishNameAsync(string fishName)
        {
            var allCatches = await _databaseService.GetAllCatchesAsync();
            return allCatches.Where(c => c.FishName.Equals(fishName, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    public class StatisticsData
    {
        public int TotalCatches { get; set; }
        public double TotalWeight { get; set; }
        public double AverageWeight { get; set; }
        public double AverageLength { get; set; }
        public FishCatch? BiggestFish { get; set; }
        public FishCatch? LongestFish { get; set; }
        public FishCount? MostCaughtFish { get; set; }
        public List<MonthlyCount> CatchesByMonth { get; set; } = new();
        public List<FishCount> CatchesByFish { get; set; } = new();
        public List<LocationCount> CatchesByLocation { get; set; } = new();
    }

    public class FishCount
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MonthlyCount
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    }

    public class LocationCount
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
