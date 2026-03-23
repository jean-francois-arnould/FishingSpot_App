using Microsoft.JSInterop;

namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Service de compression d'images utilisant Canvas API du navigateur
    /// </summary>
    public class ImageCompressionService : IImageCompressionService
    {
        private readonly IJSRuntime _jsRuntime;

        public ImageCompressionService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<ImageCompressionResult> CompressImageAsync(
            string base64Image, 
            int maxWidth = 1200, 
            int thumbnailSize = 150, 
            int quality = 80)
        {
            try
            {
                var result = await _jsRuntime.InvokeAsync<ImageCompressionResult>(
                    "imageCompressionHelper.compressImage",
                    base64Image,
                    maxWidth,
                    thumbnailSize,
                    quality
                );

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error compressing image: {ex.Message}");
                // Fallback: retourner l'image originale
                return new ImageCompressionResult
                {
                    Base64Data = base64Image,
                    Base64Thumbnail = base64Image,
                    OriginalSize = base64Image.Length,
                    CompressedSize = base64Image.Length,
                    ThumbnailSize = base64Image.Length
                };
            }
        }
    }
}
