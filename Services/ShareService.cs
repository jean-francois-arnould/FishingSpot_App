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

        // ============================================
        // NOUVELLES MÉTHODES POUR LE PARTAGE AVEC IMAGE
        // ============================================

        public async Task<bool> ShareCatchImageAsync(FishCatch fishCatch)
        {
            try
            {
                _logger.LogInformation("Generating share image for catch", new Dictionary<string, object>
                {
                    { "CatchId", fishCatch.Id },
                    { "FishName", fishCatch.FishName }
                });

                // Préparer les données pour le générateur d'image JavaScript
                var catchData = new
                {
                    fishName = fishCatch.FishName,
                    length = fishCatch.Length,
                    weight = fishCatch.Weight,
                    locationName = fishCatch.LocationName,
                    catchDate = fishCatch.CatchDate.ToString("O"), // Format ISO
                    photoUrl = fishCatch.PhotoUrl,
                    weatherTemperature = fishCatch.WeatherTemperature,
                    weatherCondition = fishCatch.WeatherCondition,
                    windSpeed = fishCatch.WindSpeed
                };

                // Générer l'image via JavaScript
                var base64Image = await _jsRuntime.InvokeAsync<string>(
                    "shareImageGenerator.blobToBase64",
                    await _jsRuntime.InvokeAsync<object>("shareImageGenerator.generateShareImage", catchData)
                );

                if (string.IsNullOrEmpty(base64Image))
                {
                    _logger.LogError("Failed to generate share image");
                    return false;
                }

                // Convertir en byte array
                var imageBytes = Convert.FromBase64String(base64Image);

                // Partager l'image
                var fileName = $"fishingspot_{fishCatch.FishName.ToLower().Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.jpg";
                var title = $"Ma prise : {fishCatch.FishName} 🎣";
                var text = $"{fishCatch.FishName} • {fishCatch.Length}cm • {fishCatch.Weight}kg";

                return await ShareFileAsync(title, text, imageBytes, fileName, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating or sharing catch image", ex);
                return false;
            }
        }

        public async Task<bool> ShareCatchToWhatsAppAsync(FishCatch fishCatch)
        {
            try
            {
                _logger.LogInformation("Sharing catch to WhatsApp", new Dictionary<string, object>
                {
                    { "CatchId", fishCatch.Id },
                    { "FishName", fishCatch.FishName }
                });

                Console.WriteLine($"📤 Génération de l'image pour WhatsApp...");
                Console.WriteLine($"📸 PhotoUrl: {fishCatch.PhotoUrl}");

                // Préparer les données pour le générateur d'image JavaScript
                var catchData = new
                {
                    fishName = fishCatch.FishName,
                    length = fishCatch.Length,
                    weight = fishCatch.Weight,
                    locationName = fishCatch.LocationName,
                    catchDate = fishCatch.CatchDate.ToString("O"), // Format ISO
                    photoUrl = fishCatch.PhotoUrl,
                    weatherTemperature = fishCatch.WeatherTemperature,
                    weatherCondition = fishCatch.WeatherCondition,
                    windSpeed = fishCatch.WindSpeed
                };

                try
                {
                    // Générer l'image via JavaScript
                    Console.WriteLine("🎨 Appel de generateShareImage...");
                    var generatedBlob = await _jsRuntime.InvokeAsync<object>("shareImageGenerator.generateShareImage", catchData);

                    if (generatedBlob == null)
                    {
                        Console.WriteLine("❌ generateShareImage a retourné null");
                        _logger.LogError("Failed to generate share image - blob is null");
                        return false;
                    }

                    Console.WriteLine("✅ Blob généré, conversion en base64...");
                    var base64Image = await _jsRuntime.InvokeAsync<string>("shareImageGenerator.blobToBase64", generatedBlob);

                    if (string.IsNullOrEmpty(base64Image))
                    {
                        Console.WriteLine("❌ Conversion base64 échouée - résultat vide");
                        return false;
                    }

                    Console.WriteLine($"✅ Base64 générée (longueur: {base64Image.Length})");

                    // Convertir en byte array
                    var imageBytes = Convert.FromBase64String(base64Image);
                    Console.WriteLine($"✅ Image convertie en bytes: {imageBytes.Length} bytes");

                    // Créer le texte de caption
                    var text = $"🎣 Ma prise du jour !\n\n" +
                              $"🐟 {fishCatch.FishName}\n" +
                              $"📏 {fishCatch.Length} cm\n" +
                              $"⚖️ {fishCatch.Weight} kg\n";

                    if (!string.IsNullOrEmpty(fishCatch.LocationName))
                    {
                        text += $"📍 {fishCatch.LocationName}\n";
                    }

                    if (fishCatch.WeatherTemperature.HasValue)
                    {
                        text += $"🌡️ {fishCatch.WeatherTemperature}°C";
                        if (!string.IsNullOrEmpty(fishCatch.WeatherCondition))
                        {
                            text += $" - {fishCatch.WeatherCondition}";
                        }
                        text += "\n";
                    }

                    text += $"\n📅 {fishCatch.CatchDate:dd/MM/yyyy}\n";
                    text += "\n#peche #fishing #fishingspot";

                    var fileName = $"fishingspot_{fishCatch.FishName.ToLower().Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.jpg";
                    var title = $"Ma prise : {fishCatch.FishName} 🎣";

                    Console.WriteLine($"📤 Tentative de partage/téléchargement...");

                    // Essayer d'abord Web Share API (mobile)
                    // Si échec, télécharger l'image automatiquement (desktop)
                    return await ShareFileAsync(title, text, imageBytes, fileName, "image/jpeg");
                }
                catch (Exception jsEx)
                {
                    Console.WriteLine($"❌ Erreur JavaScript: {jsEx.Message}");
                    Console.WriteLine($"Stack trace: {jsEx.StackTrace}");
                    _logger.LogError("JavaScript error during image generation", jsEx);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sharing to WhatsApp", ex);
                Console.WriteLine($"❌ Erreur WhatsApp: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> ShareCatchToFacebookAsync(FishCatch fishCatch)
        {
            try
            {
                _logger.LogInformation("Sharing catch to Facebook", new Dictionary<string, object>
                {
                    { "CatchId", fishCatch.Id },
                    { "FishName", fishCatch.FishName }
                });

                // Pour Facebook, on utilise l'image générée
                // Facebook ne permet pas de partage de texte direct via URL scheme
                // On utilise donc le partage d'image
                return await ShareCatchImageAsync(fishCatch);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sharing to Facebook", ex);
                return false;
            }
        }

        public async Task<bool> ShareCatchToInstagramAsync(FishCatch fishCatch)
        {
            try
            {
                _logger.LogInformation("Sharing catch to Instagram", new Dictionary<string, object>
                {
                    { "CatchId", fishCatch.Id },
                    { "FishName", fishCatch.FishName }
                });

                // Instagram nécessite une image
                return await ShareCatchImageAsync(fishCatch);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sharing to Instagram", ex);
                return false;
            }
        }
    }
}
