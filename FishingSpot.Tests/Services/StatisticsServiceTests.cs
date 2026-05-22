using FishingSpot.PWA.Models;
using FishingSpot.PWA.Services;

namespace FishingSpot.Tests.Services;

public class StatisticsServiceTests
{
    [Fact]
    public async Task GetStatisticsAsync_CalculatesTotalsAndBests()
    {
        var catches = new List<FishCatch>
        {
            new() { FishName = "Brochet", CatchDate = new DateTime(2026, 5, 1), Length = 80, Weight = 3000, LocationName = "Lac", CatchTime = new TimeSpan(6, 0, 0) },
            new() { FishName = "Perche", CatchDate = new DateTime(2026, 5, 2), Length = 35, Weight = 600, LocationName = "Lac", CatchTime = new TimeSpan(6, 30, 0) },
            new() { FishName = "Brochet", CatchDate = new DateTime(2026, 6, 1), Length = 65, Weight = 2200, LocationName = "Riviere", CatchTime = new TimeSpan(18, 0, 0) }
        };

        var service = new StatisticsService(new FakeSupabaseService(catches), new FakeLoggerService());

        var stats = await service.GetStatisticsAsync();

        Assert.Equal(3, stats.TotalCatches);
        Assert.Equal(2, stats.TotalSpecies);
        Assert.Equal(5800, stats.TotalWeight);
        Assert.Equal("Brochet", stats.BiggestCatch?.FishName);
        Assert.Equal(new TimeSpan(6, 0, 0), stats.BestTimeOfDay);
        Assert.Equal(2, stats.CatchesBySpecies["Brochet"]);
    }

    [Fact]
    public async Task GetStatisticsAsync_AppliesDateRange()
    {
        var catches = new List<FishCatch>
        {
            new() { FishName = "A", CatchDate = new DateTime(2026, 1, 1), Length = 10, Weight = 100 },
            new() { FishName = "B", CatchDate = new DateTime(2026, 5, 1), Length = 20, Weight = 200 }
        };

        var service = new StatisticsService(new FakeSupabaseService(catches), new FakeLoggerService());

        var stats = await service.GetStatisticsAsync(new DateTime(2026, 5, 1), new DateTime(2026, 5, 31));

        Assert.Equal(1, stats.TotalCatches);
        Assert.Equal("B", stats.BiggestCatch?.FishName);
    }

    [Fact]
    public async Task GetDashboardAsync_BuildsConditionInsights()
    {
        var today = DateTime.Today;
        var catches = new List<FishCatch>
        {
            new()
            {
                FishName = "Brochet",
                CatchDate = today.AddDays(-2),
                CatchTime = new TimeSpan(6, 15, 0),
                Length = 82,
                Weight = 3200,
                LocationName = "Lac",
                WeatherCondition = "Nuageux",
                WeatherTemperature = 14,
                WindSpeed = 8,
                Humidity = 70
            },
            new()
            {
                FishName = "Perche",
                CatchDate = today.AddDays(-1),
                CatchTime = new TimeSpan(7, 30, 0),
                Length = 34,
                Weight = 550,
                LocationName = "Lac",
                WeatherCondition = "Nuageux",
                WeatherTemperature = 16,
                WindSpeed = 10,
                Humidity = 74
            },
            new()
            {
                FishName = "Sandre",
                CatchDate = today.AddDays(-120),
                CatchTime = new TimeSpan(20, 0, 0),
                Length = 58,
                Weight = 1800,
                LocationName = "Canal"
            }
        };

        var service = new StatisticsService(new FakeSupabaseService(catches), new FakeLoggerService());

        var dashboard = await service.GetDashboardAsync(StatisticsPeriod.Last90Days);

        Assert.Equal(3, dashboard.TotalAvailableCatches);
        Assert.Equal(2, dashboard.Summary.TotalCatches);
        Assert.Equal("06:00 - 09:00", dashboard.Conditions.BestTimeSlot);
        Assert.Equal("Nuageux", dashboard.Conditions.BestWeatherCondition);
        Assert.Equal(15, dashboard.Conditions.AverageTemperature);
        Assert.Equal("Lac", dashboard.Conditions.BestLocation?.LocationName);
        Assert.Equal(24, dashboard.HourlyBreakdown.Count);
    }

    [Fact]
    public async Task GetDashboardAsync_BuildsAdvancedInsights()
    {
        var today = DateTime.Today;
        var catches = new List<FishCatch>
        {
            new() { Id = 1, FishName = "Brochet", CatchDate = today.AddDays(-5), CatchTime = new TimeSpan(6, 0, 0), Length = 92, Weight = 5200, LocationName = "Grand Lac", Latitude = "49.6110", Longitude = "6.1319", SetupId = 10, WeatherCode = 2, WeatherCondition = "Partiellement nuageux", WeatherTemperature = 13, WindSpeed = 8, Humidity = 72, PhotoUrl = "photo-a.jpg" },
            new() { Id = 2, FishName = "Perche", CatchDate = today.AddDays(-4), CatchTime = new TimeSpan(6, 30, 0), Length = 38, Weight = 750, LocationName = "Grand Lac", Latitude = "49.61103", Longitude = "6.13188", SetupId = 10, WeatherCode = 2, WeatherCondition = "Partiellement nuageux", WeatherTemperature = 14 },
            new() { Id = 3, FishName = "Sandre", CatchDate = today.AddDays(-3), CatchTime = new TimeSpan(20, 0, 0), Length = 64, Weight = 2400, LocationName = "Canal", SetupId = 20 },
            new() { Id = 4, FishName = "Brochet", CatchDate = today.AddDays(-2), CatchTime = new TimeSpan(18, 0, 0), Length = 71, Weight = 3100, LocationName = "Canal", SetupId = 20 }
        };
        var setups = new List<FishingSetup>
        {
            new() { Id = 10, Name = "Shad profond" },
            new() { Id = 20, Name = "Drop shot" }
        };

        var service = new StatisticsService(new FakeSupabaseService(catches, setups), new FakeLoggerService());

        var dashboard = await service.GetDashboardAsync(StatisticsPeriod.Last90Days);
        var insights = dashboard.AdvancedInsights;

        Assert.Equal("Grand Lac", insights.ProductiveSpots.First().LocationName);
        Assert.True(insights.ProductiveSpots.First().HasCoordinates);
        Assert.Equal("Shad profond", insights.SetupEfficiency.First().SetupName);
        Assert.Contains(insights.SpeciesRecords, r => r.FishName == "Brochet" && r.Catch.Id == 1);
        Assert.Equal(4, insights.SessionScores.Count);
        Assert.NotEmpty(insights.ReturnSuggestions);
    }

    [Fact]
    public async Task GetDashboardAsync_AdvancedInsightsHandleEmptyData()
    {
        var service = new StatisticsService(new FakeSupabaseService(new List<FishCatch>()), new FakeLoggerService());

        var dashboard = await service.GetDashboardAsync(StatisticsPeriod.AllTime);

        Assert.Empty(dashboard.AdvancedInsights.ProductiveSpots);
        Assert.Empty(dashboard.AdvancedInsights.SetupEfficiency);
        Assert.Empty(dashboard.AdvancedInsights.SessionScores);
        Assert.Empty(dashboard.AdvancedInsights.SpeciesRecords);
        Assert.Empty(dashboard.AdvancedInsights.ReturnSuggestions);
    }

    [Fact]
    public async Task GetDashboardAsync_UsesForecastForReturnSuggestions()
    {
        var today = DateTime.Today;
        var catches = new List<FishCatch>
        {
            new() { Id = 1, FishName = "Brochet", CatchDate = today.AddDays(-7), CatchTime = new TimeSpan(6, 15, 0), Length = 80, Weight = 3000, LocationName = "Lac", Latitude = "49.6", Longitude = "6.1", WeatherCode = 2 },
            new() { Id = 2, FishName = "Perche", CatchDate = today.AddDays(-6), CatchTime = new TimeSpan(6, 45, 0), Length = 35, Weight = 600, LocationName = "Lac", Latitude = "49.6", Longitude = "6.1", WeatherCode = 2 }
        };
        var forecast = new WeatherForecast
        {
            Hours =
            {
                new() { Time = DateTime.Now.AddDays(1).Date.AddHours(6), WeatherCode = 2, Temperature = 14, WindSpeed = 8, Humidity = 70, PrecipitationProbability = 10 },
                new() { Time = DateTime.Now.AddDays(1).Date.AddHours(15), WeatherCode = 95, Temperature = 27, WindSpeed = 35, Humidity = 80, PrecipitationProbability = 90 }
            }
        };
        var service = new StatisticsService(new FakeSupabaseService(catches), new FakeLoggerService(), new FakeWeatherService(forecast));

        var dashboard = await service.GetDashboardAsync(StatisticsPeriod.Last90Days);
        var suggestion = Assert.Single(dashboard.AdvancedInsights.ReturnSuggestions);

        Assert.True(suggestion.UsesForecast);
        Assert.Equal(6, suggestion.SuggestedAt.Hour);
        Assert.Equal(2, suggestion.WeatherCode);
    }

    private sealed class FakeSupabaseService : ISupabaseService
    {
        private readonly List<FishCatch> _catches;
        private readonly List<FishingSetup> _setups;

        public FakeSupabaseService(List<FishCatch> catches, List<FishingSetup>? setups = null)
        {
            _catches = catches;
            _setups = setups ?? new List<FishingSetup>();
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public Task<List<FishCatch>> GetAllCatchesAsync() => Task.FromResult(_catches);
        public Task<FishCatch?> GetCatchByIdAsync(int id) => Task.FromResult(_catches.FirstOrDefault(c => c.Id == id));
        public Task<int> AddCatchAsync(FishCatch fishCatch) => Task.FromResult(1);
        public Task<bool> UpdateCatchAsync(FishCatch fishCatch) => Task.FromResult(true);
        public Task<bool> DeleteCatchAsync(int id) => Task.FromResult(true);
        public Task<List<FishSpecies>> GetAllFishSpeciesAsync() => Task.FromResult(new List<FishSpecies>());
        public Task<int> AddFishSpeciesAsync(FishSpecies fishSpecies) => Task.FromResult(1);
        public Task<List<FishingBrand>> GetBrandsByCategoryAsync(string category) => Task.FromResult(new List<FishingBrand>());
        public Task<int> AddFishingBrandAsync(FishingBrand brand) => Task.FromResult(1);
        public Task<List<FishingSetup>> GetAllSetupsAsync() => Task.FromResult(_setups);
        public Task<FishingSetup?> GetSetupByIdAsync(int id) => Task.FromResult<FishingSetup?>(null);
        public Task<FishingSetup?> GetCurrentSetupAsync() => Task.FromResult<FishingSetup?>(null);
        public Task<int> AddSetupAsync(FishingSetup setup) => Task.FromResult(1);
        public Task<bool> UpdateSetupAsync(FishingSetup setup) => Task.FromResult(true);
        public Task<bool> DeleteSetupAsync(int id) => Task.FromResult(true);
        public Task<bool> SetCurrentSetupAsync(int setupId) => Task.FromResult(true);
        public Task<string?> UploadPhotoAsync(Stream photoStream, string fileName) => Task.FromResult<string?>(null);
    }

    private sealed class FakeLoggerService : ILoggerService
    {
        public void Log(FishingSpot.PWA.Services.LogLevel level, string message, Exception? exception = null, Dictionary<string, object>? properties = null) { }
        public void LogTrace(string message, Dictionary<string, object>? properties = null) { }
        public void LogCritical(string message, Exception exception, Dictionary<string, object>? properties = null) { }
        public void LogInformation(string message, Dictionary<string, object>? properties = null) { }
        public void LogWarning(string message, Exception? exception = null, Dictionary<string, object>? properties = null) { }
        public void LogError(string message, Exception? exception = null, Dictionary<string, object>? properties = null) { }
        public void LogDebug(string message, Dictionary<string, object>? properties = null) { }
    }

    private sealed class FakeWeatherService : IWeatherService
    {
        private readonly WeatherForecast _forecast;

        public FakeWeatherService(WeatherForecast forecast)
        {
            _forecast = forecast;
        }

        public Task<WeatherData?> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            return Task.FromResult<WeatherData?>(null);
        }

        public Task<WeatherForecast?> GetHourlyForecastAsync(double latitude, double longitude, int forecastDays = 7)
        {
            return Task.FromResult<WeatherForecast?>(_forecast);
        }
    }
}
