using System.Text.Json.Serialization;

namespace FishingSpot.PWA.Models
{
    public class WeatherData
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("wind_speed")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("pressure")]
        public double Pressure { get; set; }

        [JsonIgnore]
        public string WeatherDescription => GetWeatherDescription(WeatherCode);

        [JsonIgnore]
        public string WeatherEmoji => GetWeatherEmoji(WeatherCode);

        private static string GetWeatherDescription(int code)
        {
            return code switch
            {
                0 => "Ciel dégagé",
                1 => "Principalement dégagé",
                2 => "Partiellement nuageux",
                3 => "Couvert",
                45 or 48 => "Brouillard",
                51 or 53 or 55 => "Bruine",
                61 or 63 or 65 => "Pluie",
                71 or 73 or 75 => "Neige",
                77 => "Grésil",
                80 or 81 or 82 => "Averses",
                85 or 86 => "Averses de neige",
                95 => "Orage",
                96 or 99 => "Orage avec grêle",
                _ => "Inconnu"
            };
        }

        private static string GetWeatherEmoji(int code)
        {
            return code switch
            {
                0 => "☀️",
                1 => "🌤️",
                2 => "⛅",
                3 => "☁️",
                45 or 48 => "🌫️",
                51 or 53 or 55 => "🌦️",
                61 or 63 or 65 => "🌧️",
                71 or 73 or 75 => "🌨️",
                77 => "🌨️",
                80 or 81 or 82 => "🌧️",
                85 or 86 => "🌨️",
                95 => "⛈️",
                96 or 99 => "⛈️",
                _ => "🌡️"
            };
        }
    }

    public class OpenMeteoResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int Humidity { get; set; }

        [JsonPropertyName("surface_pressure")]
        public double Pressure { get; set; }
    }
}
