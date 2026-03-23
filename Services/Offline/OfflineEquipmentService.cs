using FishingSpot.PWA.Models.Equipment;
using FishingSpot.PWA.Services.Offline;
using FishingSpot.PWA.Services.Offline.Models;

namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Offline-capable wrapper for EquipmentService
    /// </summary>
    public class OfflineEquipmentService : IEquipmentService
    {
        private const string RODS_STORE = "rods";
        private const string REELS_STORE = "reels";
        private const string LINES_STORE = "lines";
        private const string LURES_STORE = "lures";
        private const string LEADERS_STORE = "leaders";
        private const string HOOKS_STORE = "hooks";

        private readonly IEquipmentService _onlineService;
        private readonly INetworkStatusService _networkStatus;
        private readonly IIndexedDbService _indexedDb;
        private readonly ISyncService _syncService;

        public OfflineEquipmentService(
            IEquipmentService onlineService,
            INetworkStatusService networkStatus,
            IIndexedDbService indexedDb,
            ISyncService syncService)
        {
            _onlineService = onlineService;
            _networkStatus = networkStatus;
            _indexedDb = indexedDb;
            _syncService = syncService;
        }

        // Rods
        public async Task<List<Rod>> GetAllRodsAsync()
        {
            return await GetAllItemsAsync<Rod>(RODS_STORE, _onlineService.GetAllRodsAsync);
        }

        public async Task<Rod?> GetRodByIdAsync(int id)
        {
            return await GetItemByIdAsync<Rod>(RODS_STORE, id, _onlineService.GetRodByIdAsync);
        }

        public async Task<int> AddRodAsync(Rod rod)
        {
            return await AddItemAsync(RODS_STORE, "rod", rod, rod.Id, () => _onlineService.AddRodAsync(rod));
        }

        public async Task<bool> UpdateRodAsync(Rod rod)
        {
            return await UpdateItemAsync(RODS_STORE, "rod", rod, rod.Id, () => _onlineService.UpdateRodAsync(rod));
        }

        public async Task<bool> DeleteRodAsync(int id)
        {
            return await DeleteItemAsync(RODS_STORE, "rod", id, () => _onlineService.DeleteRodAsync(id));
        }

        // Reels
        public async Task<List<Reel>> GetAllReelsAsync()
        {
            return await GetAllItemsAsync<Reel>(REELS_STORE, _onlineService.GetAllReelsAsync);
        }

        public async Task<Reel?> GetReelByIdAsync(int id)
        {
            return await GetItemByIdAsync<Reel>(REELS_STORE, id, _onlineService.GetReelByIdAsync);
        }

        public async Task<int> AddReelAsync(Reel reel)
        {
            return await AddItemAsync(REELS_STORE, "reel", reel, reel.Id, () => _onlineService.AddReelAsync(reel));
        }

        public async Task<bool> UpdateReelAsync(Reel reel)
        {
            return await UpdateItemAsync(REELS_STORE, "reel", reel, reel.Id, () => _onlineService.UpdateReelAsync(reel));
        }

        public async Task<bool> DeleteReelAsync(int id)
        {
            return await DeleteItemAsync(REELS_STORE, "reel", id, () => _onlineService.DeleteReelAsync(id));
        }

        // Lines
        public async Task<List<Line>> GetAllLinesAsync()
        {
            return await GetAllItemsAsync<Line>(LINES_STORE, _onlineService.GetAllLinesAsync);
        }

        public async Task<Line?> GetLineByIdAsync(int id)
        {
            return await GetItemByIdAsync<Line>(LINES_STORE, id, _onlineService.GetLineByIdAsync);
        }

        public async Task<int> AddLineAsync(Line line)
        {
            return await AddItemAsync(LINES_STORE, "line", line, line.Id, () => _onlineService.AddLineAsync(line));
        }

        public async Task<bool> UpdateLineAsync(Line line)
        {
            return await UpdateItemAsync(LINES_STORE, "line", line, line.Id, () => _onlineService.UpdateLineAsync(line));
        }

        public async Task<bool> DeleteLineAsync(int id)
        {
            return await DeleteItemAsync(LINES_STORE, "line", id, () => _onlineService.DeleteLineAsync(id));
        }

        // Lures
        public async Task<List<Lure>> GetAllLuresAsync()
        {
            return await GetAllItemsAsync<Lure>(LURES_STORE, _onlineService.GetAllLuresAsync);
        }

        public async Task<Lure?> GetLureByIdAsync(int id)
        {
            return await GetItemByIdAsync<Lure>(LURES_STORE, id, _onlineService.GetLureByIdAsync);
        }

        public async Task<int> AddLureAsync(Lure lure)
        {
            return await AddItemAsync(LURES_STORE, "lure", lure, lure.Id, () => _onlineService.AddLureAsync(lure));
        }

        public async Task<bool> UpdateLureAsync(Lure lure)
        {
            return await UpdateItemAsync(LURES_STORE, "lure", lure, lure.Id, () => _onlineService.UpdateLureAsync(lure));
        }

        public async Task<bool> DeleteLureAsync(int id)
        {
            return await DeleteItemAsync(LURES_STORE, "lure", id, () => _onlineService.DeleteLureAsync(id));
        }

        // Leaders
        public async Task<List<Leader>> GetAllLeadersAsync()
        {
            return await GetAllItemsAsync<Leader>(LEADERS_STORE, _onlineService.GetAllLeadersAsync);
        }

        public async Task<Leader?> GetLeaderByIdAsync(int id)
        {
            return await GetItemByIdAsync<Leader>(LEADERS_STORE, id, _onlineService.GetLeaderByIdAsync);
        }

        public async Task<int> AddLeaderAsync(Leader leader)
        {
            return await AddItemAsync(LEADERS_STORE, "leader", leader, leader.Id, () => _onlineService.AddLeaderAsync(leader));
        }

        public async Task<bool> UpdateLeaderAsync(Leader leader)
        {
            return await UpdateItemAsync(LEADERS_STORE, "leader", leader, leader.Id, () => _onlineService.UpdateLeaderAsync(leader));
        }

        public async Task<bool> DeleteLeaderAsync(int id)
        {
            return await DeleteItemAsync(LEADERS_STORE, "leader", id, () => _onlineService.DeleteLeaderAsync(id));
        }

        // Hooks
        public async Task<List<Hook>> GetAllHooksAsync()
        {
            return await GetAllItemsAsync<Hook>(HOOKS_STORE, _onlineService.GetAllHooksAsync);
        }

        public async Task<Hook?> GetHookByIdAsync(int id)
        {
            return await GetItemByIdAsync<Hook>(HOOKS_STORE, id, _onlineService.GetHookByIdAsync);
        }

        public async Task<int> AddHookAsync(Hook hook)
        {
            return await AddItemAsync(HOOKS_STORE, "hook", hook, hook.Id, () => _onlineService.AddHookAsync(hook));
        }

        public async Task<bool> UpdateHookAsync(Hook hook)
        {
            return await UpdateItemAsync(HOOKS_STORE, "hook", hook, hook.Id, () => _onlineService.UpdateHookAsync(hook));
        }

        public async Task<bool> DeleteHookAsync(int id)
        {
            return await DeleteItemAsync(HOOKS_STORE, "hook", id, () => _onlineService.DeleteHookAsync(id));
        }

        // Generic helper methods
        private async Task<List<T>> GetAllItemsAsync<T>(string storeName, Func<Task<List<T>>> onlineMethod)
        {
            if (_networkStatus.IsOnline)
            {
                try
                {
                    var items = await onlineMethod();

                    await _indexedDb.ClearStoreAsync(storeName);
                    foreach (var item in items)
                    {
                        var id = GetItemId(item);
                        await _indexedDb.SetItemAsync(storeName, id, item);
                    }

                    return items;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error fetching {storeName} online, falling back to cache: {ex.Message}");
                }
            }

            Console.WriteLine($"📦 Loading {storeName} from cache...");
            return await _indexedDb.GetAllItemsAsync<T>(storeName);
        }

        private async Task<T?> GetItemByIdAsync<T>(string storeName, int id, Func<int, Task<T?>> onlineMethod)
        {
            if (_networkStatus.IsOnline)
            {
                try
                {
                    var item = await onlineMethod(id);
                    if (item != null)
                    {
                        await _indexedDb.SetItemAsync(storeName, id.ToString(), item);
                    }
                    return item;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error fetching {storeName} item online, falling back to cache: {ex.Message}");
                }
            }

            return await _indexedDb.GetItemAsync<T>(storeName, id.ToString());
        }

        private async Task<int> AddItemAsync<T>(string storeName, string entityType, T item, int currentId, Func<Task<int>> onlineMethod)
        {
            var id = currentId;
            if (id == 0)
            {
                id = -new Random().Next(1, 1000000);
                SetItemId(item, id);
            }

            await _indexedDb.SetItemAsync(storeName, id.ToString(), item);

            if (_networkStatus.IsOnline)
            {
                try
                {
                    var newId = await onlineMethod();

                    if (id < 0)
                    {
                        await _indexedDb.DeleteItemAsync(storeName, id.ToString());
                    }
                    SetItemId(item, newId);
                    await _indexedDb.SetItemAsync(storeName, newId.ToString(), item);

                    return newId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error adding {entityType} online, queued for sync: {ex.Message}");
                }
            }

            await _syncService.QueueActionAsync(SyncAction.Create, entityType, id.ToString(), item);
            return id;
        }

        private async Task<bool> UpdateItemAsync<T>(string storeName, string entityType, T item, int id, Func<Task<bool>> onlineMethod)
        {
            await _indexedDb.SetItemAsync(storeName, id.ToString(), item);

            if (_networkStatus.IsOnline)
            {
                try
                {
                    return await onlineMethod();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error updating {entityType} online, queued for sync: {ex.Message}");
                }
            }

            await _syncService.QueueActionAsync(SyncAction.Update, entityType, id.ToString(), item);
            return true;
        }

        private async Task<bool> DeleteItemAsync(string storeName, string entityType, int id, Func<Task<bool>> onlineMethod)
        {
            await _indexedDb.DeleteItemAsync(storeName, id.ToString());

            if (_networkStatus.IsOnline)
            {
                try
                {
                    return await onlineMethod();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error deleting {entityType} online, queued for sync: {ex.Message}");
                }
            }

            await _syncService.QueueActionAsync(SyncAction.Delete, entityType, id.ToString(), new { id });
            return true;
        }

        private string GetItemId<T>(T item)
        {
            var idProp = item?.GetType().GetProperty("Id");
            return idProp?.GetValue(item)?.ToString() ?? Guid.NewGuid().ToString();
        }

        private void SetItemId<T>(T item, int id)
        {
            var idProp = item?.GetType().GetProperty("Id");
            idProp?.SetValue(item, id);
        }
    }
}
