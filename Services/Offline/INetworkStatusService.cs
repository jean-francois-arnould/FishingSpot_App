namespace FishingSpot.PWA.Services.Offline
{
    /// <summary>
    /// Service for monitoring network connectivity status
    /// </summary>
    public interface INetworkStatusService
    {
        /// <summary>
        /// Gets whether the application is currently online
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// Event triggered when network status changes
        /// </summary>
        event EventHandler<bool>? OnlineStatusChanged;

        /// <summary>
        /// Initialize the network status monitoring
        /// </summary>
        Task InitializeAsync();
    }
}
