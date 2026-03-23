using System.Text.Json.Serialization;

namespace FishingSpot.PWA.Models.Equipment
{
    /// <summary>
    /// Base class for all equipment items with common properties
    /// </summary>
    public abstract class BaseEquipment
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("notes")]
        public string Notes { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public abstract string DisplayName { get; }
    }

    public class Rod : BaseEquipment
    {
        [JsonPropertyName("brand")]
        public string Brand { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("length")]
        public string Length { get; set; } = string.Empty;

        [JsonPropertyName("power")]
        public string Power { get; set; } = string.Empty;

        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonIgnore]
        public override string DisplayName => $"{Brand} {Model}";
    }

    public class Reel : BaseEquipment
    {
        [JsonPropertyName("brand")]
        public string Brand { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("gear_ratio")]
        public string GearRatio { get; set; } = string.Empty;

        [JsonIgnore]
        public override string DisplayName => $"{Brand} {Model}";
    }

    public class Line : BaseEquipment
    {
        [JsonPropertyName("brand")]
        public string Brand { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("strength")]
        public string Strength { get; set; } = string.Empty;

        [JsonPropertyName("diameter")]
        public string Diameter { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;

        [JsonIgnore]
        public override string DisplayName => $"{Brand} {Type} ({Strength})";
    }

    public class Lure : BaseEquipment
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;

        [JsonPropertyName("weight")]
        public string Weight { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public string Size { get; set; } = string.Empty;

        [JsonIgnore]
        public override string DisplayName => $"{Name} ({Type})";
    }

    public class Leader : BaseEquipment
    {
        [JsonPropertyName("material")]
        public string Material { get; set; } = string.Empty;

        [JsonPropertyName("strength")]
        public string Strength { get; set; } = string.Empty;

        [JsonPropertyName("length")]
        public string Length { get; set; } = string.Empty;

        [JsonIgnore]
        public override string DisplayName => $"{Material} - {Strength}";
    }

    public class Hook : BaseEquipment
    {
        [JsonPropertyName("size")]
        public string Size { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("brand")]
        public string Brand { get; set; } = string.Empty;

        [JsonIgnore]
        public override string DisplayName => $"Taille {Size} - {Type}";
    }
}
