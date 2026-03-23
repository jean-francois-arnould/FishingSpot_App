namespace FishingSpot.PWA.Services
{
    public interface IWeatherService
    {
        Task<Models.WeatherData?> GetCurrentWeatherAsync(double latitude, double longitude);
    }
}
