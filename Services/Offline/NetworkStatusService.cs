using Microsoft.JSInterop;

namespace FishingSpot.PWA.Services.Offline
{
    /// <summary>
    /// Service for monitoring network connectivity status using JavaScript interop
    /// </summary>
    public class NetworkStatusService : INetworkStatusService, IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<NetworkStatusService>? _dotNetReference;
        private bool _isOnline = true;

        public bool IsOnline => _isOnline;
        public event EventHandler<bool>? OnlineStatusChanged;

        public NetworkStatusService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            _dotNetReference = DotNetObjectReference.Create(this);

            try
            {
                _isOnline = await _jsRuntime.InvokeAsync<bool>("networkStatus.initialize", _dotNetReference);
                Console.WriteLine($"🌐 Network status initialized: {(_isOnline ? "Online" : "Offline")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error initializing network status: {ex.Message}");
                _isOnline = true; // Default to online if detection fails
            }
        }

        [JSInvokable]
        public void UpdateNetworkStatus(bool isOnline)
        {
            if (_isOnline != isOnline)
            {
                _isOnline = isOnline;
                Console.WriteLine($"🌐 Network status changed: {(isOnline ? "Online ✅" : "Offline ❌")}");
                OnlineStatusChanged?.Invoke(this, isOnline);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_dotNetReference != null)
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync("networkStatus.dispose");
                }
                catch { }

                _dotNetReference.Dispose();
            }
        }
    }
}
