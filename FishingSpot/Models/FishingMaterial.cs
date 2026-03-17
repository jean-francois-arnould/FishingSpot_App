using SQLite;

namespace FishingSpot.Models
{
    public enum MaterialType
    {
        Canne,
        Fil,
        BasDeLigne,
        AppAtOuLeurre
    }

    [Table("FishingMaterials")]
    public class FishingMaterial
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public MaterialType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Propriétés spécifiques selon le type
        public string Length { get; set; } = string.Empty; // Pour Canne
        public string Strength { get; set; } = string.Empty; // Pour Fil et Bas de ligne
        public string Color { get; set; } = string.Empty; // Pour Appât/Leurre
    }
}
