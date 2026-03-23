using Microsoft.JSInterop;
using System.Text.Json;

namespace FishingSpot.PWA.Services.Offline
{
    /// <summary>
    /// Service for interacting with IndexedDB via JavaScript interop
    /// </summary>
    public class IndexedDbService : IIndexedDbService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly JsonSerializerOptions _jsonOptions;
        private bool _initialized = false;

        public IndexedDbService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        public async Task InitializeAsync()
        {
            if (_initialized) return;

            try
            {
                await _jsRuntime.InvokeVoidAsync("indexedDb.initialize");
                _initialized = true;
                Console.WriteLine("💾 IndexedDB initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error initializing IndexedDB: {ex.Message}");
                throw;
            }
        }

        public async Task SetItemAsync<T>(string storeName, string key, T value)
        {
            await EnsureInitializedAsync();

            try
            {
                var json = JsonSerializer.Serialize(value, _jsonOptions);
                await _jsRuntime.InvokeVoidAsync("indexedDb.setItem", storeName, key, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error saving to IndexedDB [{storeName}]: {ex.Message}");
                throw;
            }
        }

        public async Task<T?> GetItemAsync<T>(string storeName, string key)
        {
            await EnsureInitializedAsync();

            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("indexedDb.getItem", storeName, key);

                if (string.IsNullOrEmpty(json))
                    return default;

                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error reading from IndexedDB [{storeName}]: {ex.Message}");
                return default;
            }
        }

        public async Task<List<T>> GetAllItemsAsync<T>(string storeName)
        {
            await EnsureInitializedAsync();

            try
            {
                var jsonArray = await _jsRuntime.InvokeAsync<string[]>("indexedDb.getAllItems", storeName);

                var results = new List<T>();
                foreach (var json in jsonArray)
                {
                    if (!string.IsNullOrEmpty(json))
                    {
                        var item = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                        if (item != null)
                            results.Add(item);
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error reading all from IndexedDB [{storeName}]: {ex.Message}");
                return new List<T>();
            }
        }

        public async Task DeleteItemAsync(string storeName, string key)
        {
            await EnsureInitializedAsync();

            try
            {
                await _jsRuntime.InvokeVoidAsync("indexedDb.deleteItem", storeName, key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error deleting from IndexedDB [{storeName}]: {ex.Message}");
                throw;
            }
        }

        public async Task ClearStoreAsync(string storeName)
        {
            await EnsureInitializedAsync();

            try
            {
                await _jsRuntime.InvokeVoidAsync("indexedDb.clearStore", storeName);
                Console.WriteLine($"🗑️ Cleared IndexedDB store: {storeName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error clearing IndexedDB [{storeName}]: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string storeName, string key)
        {
            await EnsureInitializedAsync();

            try
            {
                return await _jsRuntime.InvokeAsync<bool>("indexedDb.exists", storeName, key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error checking existence in IndexedDB [{storeName}]: {ex.Message}");
                return false;
            }
        }

        private async Task EnsureInitializedAsync()
        {
            if (!_initialized)
                await InitializeAsync();
        }
    }
}
