using System.Text.Json;
using FishingSpot.PWA.Models;

namespace FishingSpot.PWA.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://api.open-meteo.com/v1/forecast";

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherData?> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            try
            {
                var url = $"{BASE_URL}?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&current=temperature_2m,weather_code,wind_speed_10m,relative_humidity_2m,surface_pressure&timezone=auto";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var openMeteoResponse = JsonSerializer.Deserialize<OpenMeteoResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (openMeteoResponse?.Current == null)
                {
                    return null;
                }

                return new WeatherData
                {
                    Temperature = openMeteoResponse.Current.Temperature,
                    WeatherCode = openMeteoResponse.Current.WeatherCode,
                    WindSpeed = openMeteoResponse.Current.WindSpeed,
                    Humidity = openMeteoResponse.Current.Humidity,
                    Pressure = openMeteoResponse.Current.Pressure
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
