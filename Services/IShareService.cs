namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Service pour partager du contenu via Web Share API
    /// </summary>
    public interface IShareService
    {
        Task<bool> CanShareAsync();
        Task<bool> ShareCatchAsync(Models.FishCatch fishCatch);
        Task<bool> ShareTextAsync(string title, string text, string? url = null);
        Task<bool> ShareFileAsync(string title, string text, byte[] fileData, string fileName, string mimeType);

        // Nouvelles méthodes pour le partage avec image générée
        Task<bool> ShareCatchImageAsync(Models.FishCatch fishCatch);
        Task<bool> ShareCatchToWhatsAppAsync(Models.FishCatch fishCatch);
        Task<bool> ShareCatchToFacebookAsync(Models.FishCatch fishCatch);
        Task<bool> ShareCatchToInstagramAsync(Models.FishCatch fishCatch);
    }
}
