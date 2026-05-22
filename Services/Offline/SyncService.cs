using FishingSpot.PWA.Models;
using FishingSpot.PWA.Models.Equipment;
using FishingSpot.PWA.Services.Offline.Models;
using System.Text.Json;

namespace FishingSpot.PWA.Services.Offline
{
    /// <summary>
    /// Service for managing synchronization of offline data with Supabase.
    /// </summary>
    public class SyncService : ISyncService
    {
        private const string SYNC_QUEUE_STORE = "syncQueue";
        private const string CATCHES_STORE = "catches";
        private const string SPECIES_STORE = "species";
        private const string SETUPS_STORE = "setups";
        private const string BRANDS_STORE = "brands";
        private const string DATA_IMAGE_PREFIX = "data:image/";
        private const int MAX_RETRY_COUNT = 3;

        private readonly IIndexedDbService _indexedDb;
        private readonly INetworkStatusService _networkStatus;
        private readonly SupabaseService _supabaseService;
        private readonly EquipmentService _equipmentService;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private bool _isSyncing;
        private int _pendingItemsCount;

        public bool IsSyncing => _isSyncing;
        public int PendingItemsCount => _pendingItemsCount;

        public event EventHandler<bool>? SyncStatusChanged;
        public event EventHandler<int>? SyncQueueChanged;

        public SyncService(
            IIndexedDbService indexedDb,
            INetworkStatusService networkStatus,
            SupabaseService supabaseService,
            EquipmentService equipmentService)
        {
            _indexedDb = indexedDb;
            _networkStatus = networkStatus;
            _supabaseService = supabaseService;
            _equipmentService = equipmentService;
        }

        public async Task InitializeAsync()
        {
            _networkStatus.OnlineStatusChanged += async (_, isOnline) =>
            {
                if (isOnline)
                {
                    Console.WriteLine("Network back online, starting offline sync.");
                    await SyncOfflineCatchesAsync();
                    await SyncAllAsync();
                }
            };

            await UpdatePendingCountAsync();
            Console.WriteLine($"Sync service initialized. Pending items: {_pendingItemsCount}");
        }

        public async Task QueueActionAsync(SyncAction action, string entityType, string entityId, object data)
        {
            var item = new SyncQueueItem
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Data = JsonSerializer.Serialize(data, _jsonOptions)
            };

            await _indexedDb.SetItemAsync(SYNC_QUEUE_STORE, item.Id, item);
            await UpdatePendingCountAsync();

            if (_networkStatus.IsOnline && !_isSyncing)
            {
                _ = Task.Run(SyncAllAsync);
            }
        }

        public async Task SyncAllAsync()
        {
            if (_isSyncing || !_networkStatus.IsOnline)
            {
                return;
            }

            SetSyncStatus(true);

            try
            {
                var items = await _indexedDb.GetAllItemsAsync<SyncQueueItem>(SYNC_QUEUE_STORE);
                var pendingItems = items
                    .Where(i => i.Status is SyncStatus.Pending or SyncStatus.Failed)
                    .OrderBy(i => i.Timestamp)
                    .ToList();

                foreach (var item in pendingItems)
                {
                    try
                    {
                        await ProcessSyncItemAsync(item);
                        item.Status = SyncStatus.Completed;
                        item.ErrorMessage = null;
                        await _indexedDb.SetItemAsync(SYNC_QUEUE_STORE, item.Id, item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error syncing item {item.Id}: {ex.Message}");
                        item.RetryCount++;
                        item.ErrorMessage = ex.Message;
                        item.Status = item.RetryCount >= MAX_RETRY_COUNT ? SyncStatus.Failed : SyncStatus.Pending;
                        await _indexedDb.SetItemAsync(SYNC_QUEUE_STORE, item.Id, item);
                    }
                }

                await UpdatePendingCountAsync();
            }
            finally
            {
                SetSyncStatus(false);
            }
        }

        public async Task<List<SyncQueueItem>> GetPendingItemsAsync()
        {
            var items = await _indexedDb.GetAllItemsAsync<SyncQueueItem>(SYNC_QUEUE_STORE);
            return items
                .Where(i => i.Status is SyncStatus.Pending or SyncStatus.Failed)
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
        }

        public async Task SyncOfflineCatchesAsync()
        {
            if (!_networkStatus.IsOnline)
            {
                return;
            }

            var cachedCatches = await _indexedDb.GetAllItemsAsync<FishCatch>(CATCHES_STORE);
            var offlineCatches = cachedCatches.Where(c => c.Id < 0).ToList();
            if (offlineCatches.Count == 0)
            {
                return;
            }

            var queuedCatchIds = (await GetPendingItemsAsync())
                .Where(i => i.EntityType == "catch" && i.Action == SyncAction.Create)
                .Select(i => i.EntityId)
                .ToHashSet(StringComparer.Ordinal);

            foreach (var offlineCatch in offlineCatches)
            {
                if (!queuedCatchIds.Contains(offlineCatch.Id.ToString()))
                {
                    await QueueActionAsync(SyncAction.Create, "catch", offlineCatch.Id.ToString(), offlineCatch);
                }
            }
        }

        private async Task ProcessSyncItemAsync(SyncQueueItem item)
        {
            item.Status = SyncStatus.InProgress;
            await _indexedDb.SetItemAsync(SYNC_QUEUE_STORE, item.Id, item);

            switch (item.EntityType)
            {
                case "catch":
                    await ProcessCatchAsync(item);
                    break;
                case "species":
                    await ProcessSpeciesAsync(item);
                    break;
                case "brand":
                    await ProcessBrandAsync(item);
                    break;
                case "setup":
                    await ProcessSetupAsync(item);
                    break;
                case "setup_current":
                    await EnsureSuccessAsync(await _supabaseService.SetCurrentSetupAsync(ParseEntityId(item)), "set current setup");
                    break;
                case "rod":
                    await ProcessEquipmentAsync<Rod>(item, "rods", _equipmentService.AddRodAsync, _equipmentService.UpdateRodAsync, _equipmentService.DeleteRodAsync);
                    break;
                case "reel":
                    await ProcessEquipmentAsync<Reel>(item, "reels", _equipmentService.AddReelAsync, _equipmentService.UpdateReelAsync, _equipmentService.DeleteReelAsync);
                    break;
                case "line":
                    await ProcessEquipmentAsync<Line>(item, "lines", _equipmentService.AddLineAsync, _equipmentService.UpdateLineAsync, _equipmentService.DeleteLineAsync);
                    break;
                case "lure":
                    await ProcessEquipmentAsync<Lure>(item, "lures", _equipmentService.AddLureAsync, _equipmentService.UpdateLureAsync, _equipmentService.DeleteLureAsync);
                    break;
                case "leader":
                    await ProcessEquipmentAsync<Leader>(item, "leaders", _equipmentService.AddLeaderAsync, _equipmentService.UpdateLeaderAsync, _equipmentService.DeleteLeaderAsync);
                    break;
                case "hook":
                    await ProcessEquipmentAsync<Hook>(item, "hooks", _equipmentService.AddHookAsync, _equipmentService.UpdateHookAsync, _equipmentService.DeleteHookAsync);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported sync entity type '{item.EntityType}'.");
            }
        }

        private async Task ProcessCatchAsync(SyncQueueItem item)
        {
            if (item.Action == SyncAction.Delete)
            {
                var id = ParseEntityId(item);
                await _indexedDb.DeleteItemAsync(CATCHES_STORE, id.ToString());
                if (id > 0)
                {
                    await EnsureSuccessAsync(await _supabaseService.DeleteCatchAsync(id), "delete catch");
                }
                return;
            }

            var fishCatch = DeserializeRequired<FishCatch>(item.Data);

            if (item.Action == SyncAction.Create || fishCatch.Id < 0)
            {
                var oldId = fishCatch.Id;
                if (fishCatch.Id < 0)
                {
                    fishCatch.Id = 0;
                }

                await UploadLocalPhotoIfNeededAsync(fishCatch);
                var newId = await _supabaseService.AddCatchAsync(fishCatch);
                if (newId <= 0)
                {
                    throw new InvalidOperationException("Server did not return a valid catch ID.");
                }

                fishCatch.Id = newId;
                if (oldId < 0)
                {
                    await _indexedDb.DeleteItemAsync(CATCHES_STORE, oldId.ToString());
                }
                await _indexedDb.SetItemAsync(CATCHES_STORE, newId.ToString(), fishCatch);
                return;
            }

            await UploadLocalPhotoIfNeededAsync(fishCatch);
            await EnsureSuccessAsync(await _supabaseService.UpdateCatchAsync(fishCatch), "update catch");
            await _indexedDb.SetItemAsync(CATCHES_STORE, fishCatch.Id.ToString(), fishCatch);
        }

        private async Task ProcessSpeciesAsync(SyncQueueItem item)
        {
            var species = DeserializeRequired<FishSpecies>(item.Data);
            if (item.Action != SyncAction.Create)
            {
                return;
            }

            var oldId = species.Id;
            if (species.Id < 0)
            {
                species.Id = 0;
            }

            var newId = await _supabaseService.AddFishSpeciesAsync(species);
            if (newId <= 0)
            {
                throw new InvalidOperationException("Server did not return a valid species ID.");
            }

            species.Id = newId;
            if (oldId < 0)
            {
                await _indexedDb.DeleteItemAsync(SPECIES_STORE, oldId.ToString());
            }
            await _indexedDb.SetItemAsync(SPECIES_STORE, newId.ToString(), species);
        }

        private async Task ProcessBrandAsync(SyncQueueItem item)
        {
            var brand = DeserializeRequired<FishingBrand>(item.Data);
            if (item.Action != SyncAction.Create)
            {
                return;
            }

            var oldKey = $"{brand.Category}_{brand.Id}";
            if (brand.Id < 0)
            {
                brand.Id = 0;
            }

            var newId = await _supabaseService.AddFishingBrandAsync(brand);
            if (newId <= 0)
            {
                throw new InvalidOperationException("Server did not return a valid brand ID.");
            }

            brand.Id = newId;
            await _indexedDb.DeleteItemAsync(BRANDS_STORE, oldKey);
            await _indexedDb.SetItemAsync(BRANDS_STORE, $"{brand.Category}_{newId}", brand);
        }

        private async Task ProcessSetupAsync(SyncQueueItem item)
        {
            if (item.Action == SyncAction.Delete)
            {
                var id = ParseEntityId(item);
                await _indexedDb.DeleteItemAsync(SETUPS_STORE, id.ToString());
                if (id > 0)
                {
                    await EnsureSuccessAsync(await _supabaseService.DeleteSetupAsync(id), "delete setup");
                }
                return;
            }

            var setup = DeserializeRequired<FishingSetup>(item.Data);
            if (item.Action == SyncAction.Create || setup.Id < 0)
            {
                var oldId = setup.Id;
                if (setup.Id < 0)
                {
                    setup.Id = 0;
                }

                var newId = await _supabaseService.AddSetupAsync(setup);
                if (newId <= 0)
                {
                    throw new InvalidOperationException("Server did not return a valid setup ID.");
                }

                setup.Id = newId;
                if (oldId < 0)
                {
                    await _indexedDb.DeleteItemAsync(SETUPS_STORE, oldId.ToString());
                }
                await _indexedDb.SetItemAsync(SETUPS_STORE, newId.ToString(), setup);
                return;
            }

            await EnsureSuccessAsync(await _supabaseService.UpdateSetupAsync(setup), "update setup");
            await _indexedDb.SetItemAsync(SETUPS_STORE, setup.Id.ToString(), setup);
        }

        private async Task ProcessEquipmentAsync<T>(
            SyncQueueItem item,
            string storeName,
            Func<T, Task<int>> addAsync,
            Func<T, Task<bool>> updateAsync,
            Func<int, Task<bool>> deleteAsync) where T : class
        {
            if (item.Action == SyncAction.Delete)
            {
                var deleteId = ParseEntityId(item);
                await _indexedDb.DeleteItemAsync(storeName, deleteId.ToString());
                if (deleteId > 0)
                {
                    await EnsureSuccessAsync(await deleteAsync(deleteId), $"delete {item.EntityType}");
                }
                return;
            }

            var entity = DeserializeRequired<T>(item.Data);
            var entityId = GetEntityId(entity);

            if (item.Action == SyncAction.Create || entityId < 0)
            {
                var oldId = entityId;
                if (entityId < 0)
                {
                    SetEntityId(entity, 0);
                }

                var newId = await addAsync(entity);
                if (newId <= 0)
                {
                    throw new InvalidOperationException($"Server did not return a valid {item.EntityType} ID.");
                }

                SetEntityId(entity, newId);
                if (oldId < 0)
                {
                    await _indexedDb.DeleteItemAsync(storeName, oldId.ToString());
                }
                await _indexedDb.SetItemAsync(storeName, newId.ToString(), entity);
                return;
            }

            await EnsureSuccessAsync(await updateAsync(entity), $"update {item.EntityType}");
            await _indexedDb.SetItemAsync(storeName, entityId.ToString(), entity);
        }

        private async Task UploadLocalPhotoIfNeededAsync(FishCatch fishCatch)
        {
            if (string.IsNullOrWhiteSpace(fishCatch.PhotoUrl) ||
                !fishCatch.PhotoUrl.StartsWith(DATA_IMAGE_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            using var stream = CreateStreamFromDataUrl(fishCatch.PhotoUrl, out var extension);
            var remoteUrl = await _supabaseService.UploadPhotoAsync(
                stream,
                $"catch_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}.{extension}");

            if (string.IsNullOrWhiteSpace(remoteUrl) ||
                remoteUrl.StartsWith(DATA_IMAGE_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Local catch photo could not be uploaded.");
            }

            fishCatch.PhotoUrl = remoteUrl;
        }

        private T DeserializeRequired<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _jsonOptions)
                ?? throw new InvalidOperationException($"Unable to deserialize sync payload as {typeof(T).Name}.");
        }

        private static async Task EnsureSuccessAsync(bool success, string operation)
        {
            if (!success)
            {
                throw new InvalidOperationException($"Unable to {operation}.");
            }

            await Task.CompletedTask;
        }

        private static int ParseEntityId(SyncQueueItem item)
        {
            if (!int.TryParse(item.EntityId, out var id))
            {
                throw new InvalidOperationException($"Invalid entity ID '{item.EntityId}'.");
            }

            return id;
        }

        private static int GetEntityId<T>(T entity)
        {
            var idProperty = entity?.GetType().GetProperty("Id");
            return idProperty?.GetValue(entity) is int id ? id : 0;
        }

        private static void SetEntityId<T>(T entity, int id)
        {
            var idProperty = entity?.GetType().GetProperty("Id");
            idProperty?.SetValue(entity, id);
        }

        private static MemoryStream CreateStreamFromDataUrl(string dataUrl, out string extension)
        {
            var commaIndex = dataUrl.IndexOf(',');
            if (commaIndex < 0)
            {
                throw new InvalidOperationException("Invalid local photo data URL.");
            }

            var header = dataUrl[..commaIndex];
            extension = header.Contains("png", StringComparison.OrdinalIgnoreCase) ? "png" : "jpg";
            var base64 = dataUrl[(commaIndex + 1)..];
            return new MemoryStream(Convert.FromBase64String(base64));
        }

        private async Task UpdatePendingCountAsync()
        {
            var items = await _indexedDb.GetAllItemsAsync<SyncQueueItem>(SYNC_QUEUE_STORE);
            var newCount = items.Count(i => i.Status is SyncStatus.Pending or SyncStatus.Failed);

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
