using Microsoft.JSInterop;
using System.Text.Json;

namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Service de logging pour Blazor WebAssembly
    /// Log vers la console du navigateur et peut être étendu vers Application Insights
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly bool _isProduction;

        public LoggerService(IJSRuntime jsRuntime, IConfiguration configuration)
        {
            _jsRuntime = jsRuntime;
            _isProduction = configuration.GetValue<bool>("IsProduction", false);
        }

        public void Log(LogLevel level, string message, Exception? exception = null, Dictionary<string, object>? properties = null)
        {
            var logEntry = new
            {
                timestamp = DateTime.UtcNow,
                level = level.ToString(),
                message,
                exception = exception != null ? new
                {
                    type = exception.GetType().Name,
                    message = exception.Message,
                    stackTrace = exception.StackTrace
                } : null,
                properties
            };

            var logJson = JsonSerializer.Serialize(logEntry);

            // Log vers la console du navigateur
            _ = LogToConsole(level, logJson);

            // En production, vous pouvez envoyer vers Application Insights ou autre
            if (_isProduction && level >= LogLevel.Warning)
            {
                _ = SendToMonitoring(logEntry);
            }
        }

        public void LogTrace(string message, Dictionary<string, object>? properties = null)
            => Log(LogLevel.Trace, message, null, properties);

        public void LogDebug(string message, Dictionary<string, object>? properties = null)
            => Log(LogLevel.Debug, message, null, properties);

        public void LogInformation(string message, Dictionary<string, object>? properties = null)
            => Log(LogLevel.Information, message, null, properties);

        public void LogWarning(string message, Exception? exception = null, Dictionary<string, object>? properties = null)
            => Log(LogLevel.Warning, message, exception, properties);

        public void LogError(string message, Exception? exception = null, Dictionary<string, object>? properties = null)
            => Log(LogLevel.Error, message, exception, properties);

        public void LogCritical(string message, Exception exception, Dictionary<string, object>? properties = null)
            => Log(LogLevel.Critical, message, exception, properties);

        private async Task LogToConsole(LogLevel level, string logJson)
        {
            try
            {
                var consoleMethod = level switch
                {
                    LogLevel.Trace or LogLevel.Debug => "console.debug",
                    LogLevel.Information => "console.info",
                    LogLevel.Warning => "console.warn",
                    LogLevel.Error or LogLevel.Critical => "console.error",
                    _ => "console.log"
                };

                await _jsRuntime.InvokeVoidAsync("eval", $"{consoleMethod}({logJson})");
            }
            catch
            {
                // Fallback silencieux si JS non disponible
            }
        }

        private async Task SendToMonitoring(object logEntry)
        {
            // TODO: Implémenter l'envoi vers Application Insights ou Sentry
            // Pour l'instant, juste un placeholder
            await Task.CompletedTask;
        }
    }
}
