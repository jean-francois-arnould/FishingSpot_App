using Xunit;
using Moq;
using FishingSpot.PWA.Services;
using FishingSpot.PWA.Models;
using FluentAssertions;

namespace FishingSpot.Tests.Services
{
    public class StatisticsServiceTests
    {
        private readonly Mock<ISupabaseService> _supabaseServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly StatisticsService _statisticsService;

        public StatisticsServiceTests()
        {
            _supabaseServiceMock = new Mock<ISupabaseService>();
            _loggerMock = new Mock<ILoggerService>();
            _statisticsService = new StatisticsService(_supabaseServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetStatisticsAsync_ShouldReturnCorrectTotalCatches()
        {
            // Arrange
            var catches = new List<FishCatch>
            {
                new FishCatch { Id = 1, FishName = "Brochet", Length = 80, Weight = 3.5 },
                new FishCatch { Id = 2, FishName = "Perche", Length = 25, Weight = 0.5 },
                new FishCatch { Id = 3, FishName = "Brochet", Length = 90, Weight = 4.2 }
            };

            _supabaseServiceMock
                .Setup(s => s.GetAllCatchesAsync())
                .ReturnsAsync(catches);

            // Act
            var stats = await _statisticsService.GetStatisticsAsync();

            // Assert
            stats.TotalCatches.Should().Be(3);
            stats.TotalSpecies.Should().Be(2);
            stats.CatchesBySpecies["Brochet"].Should().Be(2);
            stats.CatchesBySpecies["Perche"].Should().Be(1);
        }

        [Fact]
        public async Task GetStatisticsAsync_ShouldCalculateAverages()
        {
            // Arrange
            var catches = new List<FishCatch>
            {
                new FishCatch { Length = 100, Weight = 5.0 },
                new FishCatch { Length = 80, Weight = 3.0 }
            };

            _supabaseServiceMock
                .Setup(s => s.GetAllCatchesAsync())
                .ReturnsAsync(catches);

            // Act
            var stats = await _statisticsService.GetStatisticsAsync();

            // Assert
            stats.AverageLength.Should().Be(90);
            stats.AverageWeight.Should().Be(4.0);
        }
    }
}
