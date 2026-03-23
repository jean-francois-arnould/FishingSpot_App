namespace FishingSpot.PWA.Services
{
    public interface IExportService
    {
        Task<byte[]> ExportToCsvAsync(List<Models.FishCatch> catches);
        Task<string> ExportToJsonAsync(List<Models.FishCatch> catches);
        // Pour le PDF, nécessite une lib comme QuestPDF (ajout ultérieur si besoin)
    }
}
