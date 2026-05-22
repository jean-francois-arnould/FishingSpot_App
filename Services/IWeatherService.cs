namespace FishingSpot.PWA.Services
{
    public interface IWeatherService
    {
        Task<Models.WeatherData?> GetCurrentWeatherAsync(double latitude, double longitude);
        Task<Models.WeatherForecast?> GetHourlyForecastAsync(double latitude, double longitude, int forecastDays = 7);
    }
}
