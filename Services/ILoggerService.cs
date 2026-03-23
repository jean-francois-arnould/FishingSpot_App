namespace FishingSpot.PWA.Services
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical
    }

    public interface ILoggerService
    {
        void Log(LogLevel level, string message, Exception? exception = null, Dictionary<string, object>? properties = null);
        void LogTrace(string message, Dictionary<string, object>? properties = null);
        void LogDebug(string message, Dictionary<string, object>? properties = null);
        void LogInformation(string message, Dictionary<string, object>? properties = null);
        void LogWarning(string message, Exception? exception = null, Dictionary<string, object>? properties = null);
        void LogError(string message, Exception? exception = null, Dictionary<string, object>? properties = null);
        void LogCritical(string message, Exception exception, Dictionary<string, object>? properties = null);
    }
}
