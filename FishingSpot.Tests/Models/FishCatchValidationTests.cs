using FishingSpot.PWA.Models;

namespace FishingSpot.Tests.Models;

public class FishCatchValidationTests
{
    [Fact]
    public void LengthHelpers_CombineMetersAndCentimeters()
    {
        var fishCatch = new FishCatch();

        fishCatch.LengthMeters = 1;
        fishCatch.LengthCentimeters = 27;

        Assert.Equal(127, fishCatch.Length);
        Assert.Equal(1, fishCatch.LengthMeters);
        Assert.Equal(27, fishCatch.LengthCentimeters);
    }

    [Fact]
    public void WeightHelpers_CombineKilogramsAndGrams()
    {
        var fishCatch = new FishCatch();

        fishCatch.WeightKilograms = 2;
        fishCatch.WeightGrams = 350;

        Assert.Equal(2350, fishCatch.Weight);
        Assert.Equal(2, fishCatch.WeightKilograms);
        Assert.Equal(350, fishCatch.WeightGrams);
    }

    [Fact]
    public void CatchTimeString_ParsesValidTimeAndClearsInvalidTime()
    {
        var fishCatch = new FishCatch { CatchTimeString = "06:45" };

        Assert.Equal(new TimeSpan(6, 45, 0), fishCatch.CatchTime);

        fishCatch.CatchTimeString = "invalid";

        Assert.Null(fishCatch.CatchTime);
    }
}
