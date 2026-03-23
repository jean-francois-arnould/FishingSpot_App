namespace FishingSpot.PWA.Services.Offline
{
    /// <summary>
    /// Service for storing and retrieving data from IndexedDB
    /// </summary>
    public interface IIndexedDbService
    {
        /// <summary>
        /// Initialize the IndexedDB database
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Store an item in the specified store
        /// </summary>
        Task SetItemAsync<T>(string storeName, string key, T value);

        /// <summary>
        /// Retrieve an item from the specified store
        /// </summary>
        Task<T?> GetItemAsync<T>(string storeName, string key);

        /// <summary>
        /// Retrieve all items from the specified store
        /// </summary>
        Task<List<T>> GetAllItemsAsync<T>(string storeName);

        /// <summary>
        /// Delete an item from the specified store
        /// </summary>
        Task DeleteItemAsync(string storeName, string key);

        /// <summary>
        /// Clear all items from the specified store
        /// </summary>
        Task ClearStoreAsync(string storeName);

        /// <summary>
        /// Check if an item exists in the specified store
        /// </summary>
        Task<bool> ExistsAsync(string storeName, string key);
    }
}
