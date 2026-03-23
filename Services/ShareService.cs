using Microsoft.JSInterop;
using FishingSpot.PWA.Models;

namespace FishingSpot.PWA.Services
{
    public class ShareService : IShareService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ILoggerService _logger;

        public ShareService(IJSRuntime jsRuntime, ILoggerService logger)
        {
            _jsRuntime = jsRuntime;
            _logger = logger;
        }

        public async Task<bool> CanShareAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<bool>("shareHelper.canShare");
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ShareCatchAsync(FishCatch fishCatch)
        {
            try
            {
                var title = $"Ma prise : {fishCatch.FishName} 🎣";
                var text = $"J'ai pêché un {fishCatch.FishName} de {fishCatch.Length} cm et {fishCatch.Weight} kg !";

                if (!string.IsNullOrEmpty(fishCatch.LocationName))
                {
                    text += $"\nLieu : {fishCatch.LocationName}";
                }

                var url = $"{GetBaseUrl()}/catches/{fishCatch.Id}";

                _logger.LogInformation("Sharing catch", new Dictionary<string, object>
                {
                    { "CatchId", fishCatch.Id },
                    { "FishName", fishCatch.FishName }
                });

                return await ShareTextAsync(title, text, url);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sharing catch", ex);
                return false;
            }
        }

        public async Task<bool> ShareTextAsync(string title, string text, string? url = null)
        {
            try
            {
                var shareData = new
                {
                    title,
                    text,
                    url
                };

                return await _jsRuntime.InvokeAsync<bool>("shareHelper.share", shareData);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Share cancelled or failed", ex);
                return false;
            }
        }

        public async Task<bool> ShareFileAsync(string title, string text, byte[] fileData, string fileName, string mimeType)
        {
            try
            {
                var base64 = Convert.ToBase64String(fileData);

                var shareData = new
                {
                    title,
                    text,
                    files = new[]
                    {
                        new
                        {
                            name = fileName,
                            data = base64,
                            type = mimeType
                        }
                    }
                };

                return await _jsRuntime.InvokeAsync<bool>("shareHelper.shareFile", shareData);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sharing file", ex);
                return false;
            }
        }

        private string GetBaseUrl()
        {
            // En production, retourner l'URL réelle du site
            return "https://jean-francois-arnould.github.io/FishingSpot_App";
        }
    }
}
