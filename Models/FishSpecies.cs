namespace FishingSpot.PWA.Models
{
    public static class FishSpecies
    {
        public static readonly List<string> RiverFish = new()
        {
            "Brochet",
            "Sandre",
            "Perche",
            "Black-bass",
            "Truite fario",
            "Truite arc-en-ciel",
            "Ombre commun",
            "Saumon",
            "Carpe commune",
            "Carpe miroir",
            "Carpe cuir",
            "Carpe koï",
            "Gardon",
            "Rotengle",
            "Brème",
            "Tanche",
            "Chevesne",
            "Barbeau",
            "Ablette",
            "Vandoise",
            "Hotu",
            "Goujon",
            "Vairon",
            "Loche franche",
            "Silure",
            "Anguille",
            "Lamproie",
            "Breme bordelière",
            "Carassin",
            "Ide mélanote",
            "Aspe",
            "Toxostome",
            "Bouvière",
            "Spirlin"
        };

        public static readonly List<int> LengthMetersOptions = Enumerable.Range(0, 3).ToList(); // 0-2m
        public static readonly List<int> LengthCentimetersOptions = Enumerable.Range(0, 100).ToList(); // 0-99cm

        public static readonly List<int> WeightKilogramsOptions = Enumerable.Range(0, 51).ToList(); // 0-50kg
        public static readonly List<int> WeightGramsOptions = Enumerable.Range(0, 1000).Where(x => x % 10 == 0).ToList(); // 0-990g par pas de 10
    }
}
