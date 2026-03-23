namespace FishingSpot.PWA.Services
{
    public class ToastService : IToastService
    {
        public event Action<ToastMessage>? OnShow;
        public event Action<string>? OnHide;

        public void ShowSuccess(string message, int durationMs = 3000)
        {
            Show(message, ToastType.Success, durationMs);
        }

        public void ShowError(string message, int durationMs = 5000)
        {
            Show(message, ToastType.Error, durationMs);
        }

        public void ShowWarning(string message, int durationMs = 4000)
        {
            Show(message, ToastType.Warning, durationMs);
        }

        public void ShowInfo(string message, int durationMs = 3000)
        {
            Show(message, ToastType.Info, durationMs);
        }

        public void Hide(string id)
        {
            OnHide?.Invoke(id);
        }

        private void Show(string message, ToastType type, int durationMs)
        {
            var toast = new ToastMessage
            {
                Message = message,
                Type = type,
                DurationMs = durationMs
            };

            OnShow?.Invoke(toast);
        }
    }
}
