namespace FishingSpot.PWA.Models
{
    public static class FishSpeciesHelper
    {
        public static readonly List<int> LengthMetersOptions = Enumerable.Range(0, 3).ToList(); // 0-2m
        public static readonly List<int> LengthCentimetersOptions = Enumerable.Range(0, 100).ToList(); // 0-99cm

        public static readonly List<int> WeightKilogramsOptions = Enumerable.Range(0, 51).ToList(); // 0-50kg
        public static readonly List<int> WeightGramsOptions = Enumerable.Range(0, 1000).Where(x => x % 10 == 0).ToList(); // 0-990g par pas de 10
    }
}
