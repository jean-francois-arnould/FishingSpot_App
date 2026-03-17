using FishingSpot.Models;

namespace FishingSpot.Services
{
    public class SocialShareService
    {
        public async Task<bool> ShareCatchAsync(FishCatch fishCatch, string? photoPath = null)
        {
            try
            {
                var shareText = $"🎣 J'ai pêché un {fishCatch.FishName} !\n" +
                               $"📏 Taille: {fishCatch.Length:F1} cm\n" +
                               $"⚖️ Poids: {fishCatch.Weight:F2} kg\n" +
                               $"📍 Lieu: {fishCatch.LocationName}\n" +
                               $"📅 Date: {fishCatch.CatchDate:dd/MM/yyyy}\n" +
                               $"\n#Pêche #FishingSpot #Poisson";

                if (!string.IsNullOrEmpty(photoPath) && File.Exists(photoPath))
                {
                    await Share.Default.RequestAsync(new ShareMultipleFilesRequest
                    {
                        Title = "Partager ma prise",
                        Files = new List<ShareFile> { new ShareFile(photoPath) }
                    });

                    await Share.Default.RequestAsync(new ShareTextRequest
                    {
                        Title = "Partager ma prise",
                        Text = shareText
                    });
                }
                else
                {
                    await Share.Default.RequestAsync(new ShareTextRequest
                    {
                        Title = "Partager ma prise",
                        Text = shareText
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sharing catch: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ShareStatisticsAsync(string statisticsText)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = "Partager mes statistiques de pêche",
                    Text = statisticsText
                });
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sharing statistics: {ex.Message}");
                return false;
            }
        }
    }
}
