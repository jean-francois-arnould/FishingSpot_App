namespace FishingSpot.PWA.Services
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    public class ToastMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Message { get; set; } = string.Empty;
        public ToastType Type { get; set; }
        public int DurationMs { get; set; } = 3000;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public interface IToastService
    {
        event Action<ToastMessage>? OnShow;
        event Action<string>? OnHide;

        void ShowSuccess(string message, int durationMs = 3000);
        void ShowError(string message, int durationMs = 5000);
        void ShowWarning(string message, int durationMs = 4000);
        void ShowInfo(string message, int durationMs = 3000);
        void Hide(string id);
    }
}
