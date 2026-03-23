using Microsoft.JSInterop;

namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Service d'analytics compatible avec Google Analytics 4 ou Application Insights
    /// </summary>
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ILoggerService _logger;
        private readonly bool _isEnabled;

        public AnalyticsService(IJSRuntime jsRuntime, ILoggerService logger, IConfiguration configuration)
        {
            _jsRuntime = jsRuntime;
            _logger = logger;
            _isEnabled = configuration.GetValue<bool>("Analytics:Enabled", false);
        }

        public async Task TrackPageViewAsync(string pageName)
        {
            if (!_isEnabled) return;

            try
            {
                await _jsRuntime.InvokeVoidAsync("analyticsHelper.trackPageView", pageName);
                _logger.LogDebug($"Page view tracked: {pageName}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to track page view", ex);
            }
        }

        public async Task TrackEventAsync(string eventName, Dictionary<string, object>? properties = null)
        {
            if (!_isEnabled) return;

            try
            {
                await _jsRuntime.InvokeVoidAsync("analyticsHelper.trackEvent", eventName, properties);
                _logger.LogDebug($"Event tracked: {eventName}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to track event", ex);
            }
        }

        public async Task TrackExceptionAsync(Exception exception, Dictionary<string, object>? properties = null)
        {
            if (!_isEnabled) return;

            try
            {
                var exceptionData = new
                {
                    type = exception.GetType().Name,
                    message = exception.Message,
                    stackTrace = exception.StackTrace,
                    properties
                };

                await _jsRuntime.InvokeVoidAsync("analyticsHelper.trackException", exceptionData);
                _logger.LogDebug($"Exception tracked: {exception.GetType().Name}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to track exception", ex);
            }
        }

        public async Task TrackPerformanceAsync(string metricName, double value, Dictionary<string, object>? properties = null)
        {
            if (!_isEnabled) return;

            try
            {
                await _jsRuntime.InvokeVoidAsync("analyticsHelper.trackPerformance", metricName, value, properties);
                _logger.LogDebug($"Performance metric tracked: {metricName} = {value}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to track performance", ex);
            }
        }
    }
}
