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

    private sealed class FakeSupabaseService : ISupabaseService
    {
        private readonly List<FishCatch> _catches;

        public FakeSupabaseService(List<FishCatch> catches)
        {
            _catches = catches;
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
        public Task<List<FishingSetup>> GetAllSetupsAsync() => Task.FromResult(new List<FishingSetup>());
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
}
