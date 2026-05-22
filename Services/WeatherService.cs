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

        public async Task<WeatherForecast?> GetHourlyForecastAsync(double latitude, double longitude, int forecastDays = 7)
        {
            try
            {
                forecastDays = Math.Clamp(forecastDays, 1, 16);
                var lat = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var lon = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var url = $"{BASE_URL}?latitude={lat}&longitude={lon}&hourly=temperature_2m,relative_humidity_2m,weather_code,wind_speed_10m,precipitation_probability&forecast_days={forecastDays}&timezone=auto";

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

                if (openMeteoResponse?.Hourly == null || openMeteoResponse.Hourly.Time.Count == 0)
                {
                    return null;
                }

                var hourly = openMeteoResponse.Hourly;
                var hours = hourly.Time
                    .Select((time, index) => new HourlyForecast
                    {
                        Time = time,
                        Temperature = GetAt(hourly.Temperature, index),
                        WeatherCode = GetAt(hourly.WeatherCode, index),
                        WindSpeed = GetAt(hourly.WindSpeed, index),
                        Humidity = GetAt(hourly.Humidity, index),
                        PrecipitationProbability = GetAt(hourly.PrecipitationProbability, index)
                    })
                    .ToList();

                return new WeatherForecast
                {
                    Hours = hours
                };
            }
            catch
            {
                return null;
            }
        }

        private static T? GetAt<T>(IReadOnlyList<T?> values, int index) where T : struct
        {
            return index >= 0 && index < values.Count ? values[index] : null;
        }
    }
}
