namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Service de monitoring et analytics
    /// </summary>
    public interface IAnalyticsService
    {
        Task TrackPageViewAsync(string pageName);
        Task TrackEventAsync(string eventName, Dictionary<string, object>? properties = null);
        Task TrackExceptionAsync(Exception exception, Dictionary<string, object>? properties = null);
        Task TrackPerformanceAsync(string metricName, double value, Dictionary<string, object>? properties = null);
    }
}
