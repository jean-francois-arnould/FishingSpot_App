namespace FishingSpot.PWA.Services
{
    public class ImageCompressionResult
    {
        public string Base64Data { get; set; } = string.Empty;
        public string Base64Thumbnail { get; set; } = string.Empty;
        public int OriginalSize { get; set; }
        public int CompressedSize { get; set; }
        public int ThumbnailSize { get; set; }
    }

    public interface IImageCompressionService
    {
        Task<ImageCompressionResult> CompressImageAsync(string base64Image, int maxWidth = 1200, int thumbnailSize = 150, int quality = 80);
    }
}
