using FishingSpot.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace FishingSpot.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly SQLiteDatabaseService _databaseService;
        private const string API_KEY = "VOTRE_CLE_API_OPENWEATHERMAP";
        private const string BASE_URL = "https://api.openweathermap.org/data/2.5/weather";

        public WeatherService(SQLiteDatabaseService databaseService)
        {
            _httpClient = new HttpClient();
            _databaseService = databaseService;
        }

        public async Task<WeatherData?> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            try
            {
                var url = $"{BASE_URL}?lat={latitude}&lon={longitude}&appid={API_KEY}&units=metric&lang=fr";
                var response = await _httpClient.GetFromJsonAsync<OpenWeatherMapResponse>(url);

                if (response != null)
                {
                    var weatherData = new WeatherData
                    {
                        Latitude = latitude,
                        Longitude = longitude,
                        Timestamp = DateTime.Now,
                        Temperature = response.Main.Temp,
                        Description = response.Weather[0].Description,
                        Icon = response.Weather[0].Icon,
                        WindSpeed = response.Wind.Speed,
                        Humidity = response.Main.Humidity,
                        Pressure = response.Main.Pressure
                    };

                    await _databaseService.AddWeatherDataAsync(weatherData);
                    return weatherData;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching weather: {ex.Message}");
            }

            return null;
        }

        public async Task<WeatherData?> GetWeatherForCatchAsync(int catchId)
        {
            return await _databaseService.GetWeatherDataByCatchIdAsync(catchId);
        }

        public async Task UpdateWeatherForCatchAsync(WeatherData weatherData)
        {
            await _databaseService.UpdateWeatherDataAsync(weatherData);
        }

        private class OpenWeatherMapResponse
        {
            public MainData Main { get; set; } = new();
            public List<WeatherDescription> Weather { get; set; } = new();
            public WindData Wind { get; set; } = new();
        }

        private class MainData
        {
            public double Temp { get; set; }
            public int Humidity { get; set; }
            public double Pressure { get; set; }
        }

        private class WeatherDescription
        {
            public string Description { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
        }

        private class WindData
        {
            public double Speed { get; set; }
        }
    }
}
