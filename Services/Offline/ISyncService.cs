using FishingSpot.PWA.Services.Offline.Models;

namespace FishingSpot.PWA.Services.Offline
{
    /// <summary>
    /// Service for managing synchronization of offline data
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Gets whether synchronization is currently in progress
        /// </summary>
        bool IsSyncing { get; }

        /// <summary>
        /// Gets the number of pending items in the sync queue
        /// </summary>
        int PendingItemsCount { get; }

        /// <summary>
        /// Event triggered when sync status changes
        /// </summary>
        event EventHandler<bool>? SyncStatusChanged;

        /// <summary>
        /// Event triggered when sync queue changes
        /// </summary>
        event EventHandler<int>? SyncQueueChanged;

        /// <summary>
        /// Initialize the sync service
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Queue an action for synchronization
        /// </summary>
        Task QueueActionAsync(SyncAction action, string entityType, string entityId, object data);

        /// <summary>
        /// Process all pending sync items
        /// </summary>
        Task SyncAllAsync();

        /// <summary>
        /// Get all pending sync items
        /// </summary>
        Task<List<SyncQueueItem>> GetPendingItemsAsync();

        /// <summary>
        /// Clear all completed sync items
        /// </summary>
        Task ClearCompletedItemsAsync();

        /// <summary>
        /// Synchronize offline catches with negative IDs
        /// </summary>
        Task SyncOfflineCatchesAsync();
    }
}
