using Xunit;
using FluentAssertions;
using FishingSpot.PWA.Models;

namespace FishingSpot.Tests.Models
{
    public class FishCatchValidationTests
    {
        [Fact]
        public void FishCatch_ShouldHaveValidFishName()
        {
            // Arrange
            var catch1 = new FishCatch { FishName = "Brochet" };

            // Act & Assert
            catch1.FishName.Should().NotBeNullOrEmpty();
            catch1.FishName.Length.Should().BeGreaterOrEqualTo(2);
            catch1.FishName.Length.Should().BeLessOrEqualTo(100);
        }

        [Fact]
        public void FishCatch_Length_ShouldBeInValidRange()
        {
            // Arrange & Act
            var validCatch = new FishCatch { Length = 50 };
            var invalidCatch1 = new FishCatch { Length = -1 };
            var invalidCatch2 = new FishCatch { Length = 501 };

            // Assert
            validCatch.Length.Should().BeInRange(0, 500);
            invalidCatch1.Length.Should().BeLessThan(0);
            invalidCatch2.Length.Should().BeGreaterThan(500);
        }

        [Fact]
        public void FishCatch_Weight_ShouldBeInValidRange()
        {
            // Arrange & Act
            var validCatch = new FishCatch { Weight = 5.5 };

            // Assert
            validCatch.Weight.Should().BeInRange(0, 200);
        }

        [Fact]
        public void FishCatch_LengthHelpers_ShouldWorkCorrectly()
        {
            // Arrange
            var catch1 = new FishCatch();

            // Act
            catch1.LengthMeters = 1;
            catch1.LengthCentimeters = 25;

            // Assert
            catch1.Length.Should().Be(125);
            catch1.LengthMeters.Should().Be(1);
            catch1.LengthCentimeters.Should().Be(25);
        }

        [Fact]
        public void FishCatch_WeightHelpers_ShouldWorkCorrectly()
        {
            // Arrange
            var catch1 = new FishCatch();

            // Act
            catch1.WeightKilograms = 3;
            catch1.WeightGrams = 250;

            // Assert
            catch1.Weight.Should().Be(3.25);
            catch1.WeightKilograms.Should().Be(3);
            catch1.WeightGrams.Should().Be(250);
        }
    }
}
