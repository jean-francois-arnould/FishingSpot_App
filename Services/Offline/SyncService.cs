using FishingSpot.PWA.Services.Offline.Models;
using System.Text.Json;

namespace FishingSpot.PWA.Services.Offline
{
    /// <summary>
    /// Service for managing synchronization of offline data with the server
    /// </summary>
    public class SyncService : ISyncService
    {
        private const string SYNC_QUEUE_STORE = "syncQueue";
        private const string CATCHES_STORE = "catches";
        private const int MAX_RETRY_COUNT = 3;

        private readonly IIndexedDbService _indexedDb;
        private readonly INetworkStatusService _networkStatus;
        private bool _isSyncing = false;
        private int _pendingItemsCount = 0;

        public bool IsSyncing => _isSyncing;
        public int PendingItemsCount => _pendingItemsCount;

        public event EventHandler<bool>? SyncStatusChanged;
        public event EventHandler<int>? SyncQueueChanged;

        public SyncService(
            IIndexedDbService indexedDb,
            INetworkStatusService networkStatus)
        {
            _indexedDb = indexedDb;
            _networkStatus = networkStatus;
        }

        public async Task InitializeAsync()
        {
            // Subscribe to network status changes
            _networkStatus.OnlineStatusChanged += async (sender, isOnline) =>
            {
                if (isOnline)
                {
                    Console.WriteLine("🔄 Network back online, checking offline catches...");

                    // D'abord synchroniser les prises offline
                    await SyncOfflineCatchesAsync();

                    // Puis traiter la queue normale
                    await SyncAllAsync();
                }
            };

            // Load pending items count
            await UpdatePendingCountAsync();

            Console.WriteLine($"🔄 Sync service initialized. Pending items: {_pendingItemsCount}");
        }

        public async Task QueueActionAsync(SyncAction action, string entityType, string entityId, object data)
        {
            var item = new SyncQueueItem
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Data = JsonSerializer.Serialize(data)
            };

            await _indexedDb.SetItemAsync(SYNC_QUEUE_STORE, item.Id, item);

            await UpdatePendingCountAsync();

            Console.WriteLine($"📋 Queued {action} action for {entityType} ({entityId})");

            // Auto-sync if online
            if (_networkStatus.IsOnline && !_isSyncing)
            {
                _ = Task.Run(async () => await SyncAllAsync());
            }
        }

        public async Task SyncAllAsync()
        {
            if (_isSyncing)
            {
                Console.WriteLine("⚠️ Sync already in progress, skipping...");
                return;
            }

            if (!_networkStatus.IsOnline)
            {
                Console.WriteLine("⚠️ Cannot sync while offline");
                return;
            }

            SetSyncStatus(true);

            try
            {
                var items = await _indexedDb.GetAllItemsAsync<SyncQueueItem>(SYNC_QUEUE_STORE);
                var pendingItems = items.Where(i => i.Status == SyncStatus.Pending || i.Status == SyncStatus.Failed)
                                       .OrderBy(i => i.Timestamp)
                                       .ToList();

                Console.WriteLine($"🔄 Starting sync of {pendingItems.Count} items...");

                foreach (var item in pendingItems)
                {
                    try
                    {
                        await ProcessSyncItemAsync(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error syncing item {item.Id}: {ex.Message}");
                        item.RetryCount++;
                        item.ErrorMessage = ex.Message;

                        if (item.RetryCount >= MAX_RETRY_COUNT)
                        {
                            item.Status = SyncStatus.Failed;
                            Console.WriteLine($"❌ Item {item.Id} failed after {MAX_RETRY_COUNT} retries");
                        }

                        await _indexedDb.SetItemAsync(SYNC_QUEUE_STORE, item.Id, item);
                    }
                }

                await UpdatePendingCountAsync();
                Console.WriteLine("✅ Sync completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during sync: {ex.Message}");
            }
            finally
            {
                SetSyncStatus(false);
            }
        }

        public async Task<List<SyncQueueItem>> GetPendingItemsAsync()
        {
            var items = await _indexedDb.GetAllItemsAsync<SyncQueueItem>(SYNC_QUEUE_STORE);
            return items.Where(i => i.Status == SyncStatus.Pending || i.Status == SyncStatus.Failed)
                       .OrderBy(i => i.Timestamp)
                       .ToList();
        }

        public async Task ClearCompletedItemsAsync()
        {
            var items = await _indexedDb.GetAllItemsAsync<SyncQueueItem>(SYNC_QUEUE_STORE);
            var completedItems = items.Where(i => i.Status == SyncStatus.Completed).ToList();

            foreach (var item in completedItems)
            {
                await _indexedDb.DeleteItemAsync(SYNC_QUEUE_STORE, item.Id);
            }

            await UpdatePendingCountAsync();
            Console.WriteLine($"🗑️ Cleared {completedItems.Count} completed sync items");
        }

        public async Task SyncOfflineCatchesAsync()
        {
            if (!_networkStatus.IsOnline)
            {
                Console.WriteLine("⚠️ Cannot sync offline catches while offline");
                return;
            }

            Console.WriteLine("🔄 Checking for offline catches to sync...");

            var cachedCatches = await _indexedDb.GetAllItemsAsync<FishingSpot.PWA.Models.FishCatch>(CATCHES_STORE);
            var offlineCatches = cachedCatches.Where(c => c.Id < 0).ToList();

            if (!offlineCatches.Any())
            {
                Console.WriteLine("✅ No offline catches to sync");
                return;
            }

            Console.WriteLine($"📤 Found {offlineCatches.Count} offline catches to sync");

            foreach (var offlineCatch in offlineCatches)
            {
                // Queue the catch for sync
                await QueueActionAsync(SyncAction.Create, "catch", offlineCatch.Id.ToString(), offlineCatch);
                Console.WriteLine($"📋 Queued offline catch: {offlineCatch.FishName} (ID: {offlineCatch.Id})");
            }

            Console.WriteLine($"✅ {offlineCatches.Count} offline catches queued for synchronization");
        }

        private async Task ProcessSyncItemAsync(SyncQueueItem item)
        {
            item.Status = SyncStatus.InProgress;
            await _indexedDb.SetItemAsync(SYNC_QUEUE_STORE, item.Id, item);

            // Here you would call the appropriate service method based on entityType and action
            // For now, we'll mark it as completed (to be implemented with actual API calls)
            Console.WriteLine($"🔄 Processing {item.Action} for {item.EntityType} ({item.EntityId})");

            // Simulate processing
            await Task.Delay(100);

            item.Status = SyncStatus.Completed;
            await _indexedDb.SetItemAsync(SYNC_QUEUE_STORE, item.Id, item);
        }

        private async Task UpdatePendingCountAsync()
        {
            var items = await _indexedDb.GetAllItemsAsync<SyncQueueItem>(SYNC_QUEUE_STORE);
            var newCount = items.Count(i => i.Status == SyncStatus.Pending || i.Status == SyncStatus.Failed);

            if (_pendingItemsCount != newCount)
            {
                _pendingItemsCount = newCount;
                SyncQueueChanged?.Invoke(this, _pendingItemsCount);
            }
        }

        private void SetSyncStatus(bool syncing)
        {
            if (_isSyncing != syncing)
            {
                _isSyncing = syncing;
                SyncStatusChanged?.Invoke(this, _isSyncing);
            }
        }
    }
}
