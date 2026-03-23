using System.Text.Json.Serialization;

namespace FishingSpot.PWA.Services.Offline.Models
{
    public class SyncQueueItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("action")]
        public SyncAction Action { get; set; }

        [JsonPropertyName("entityType")]
        public string EntityType { get; set; } = string.Empty;

        [JsonPropertyName("entityId")]
        public string EntityId { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("retryCount")]
        public int RetryCount { get; set; } = 0;

        [JsonPropertyName("status")]
        public SyncStatus Status { get; set; } = SyncStatus.Pending;

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }

    public enum SyncAction
    {
        Create,
        Update,
        Delete
    }

    public enum SyncStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed
    }
}
