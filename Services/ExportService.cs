using System.Text;
using System.Text.Json;
using FishingSpot.PWA.Models;

namespace FishingSpot.PWA.Services
{
    public class ExportService : IExportService
    {
        private readonly ILoggerService _logger;

        public ExportService(ILoggerService logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> ExportToCsvAsync(List<FishCatch> catches)
        {
            try
            {
                _logger.LogInformation("Exporting catches to CSV");

                var csv = new StringBuilder();

                // Header
                csv.AppendLine("Date,Heure,Poisson,Longueur (cm),Poids (kg),Lieu,Latitude,Longitude,Notes");

                // Data
                foreach (var catch in catches.OrderByDescending(c => c.CatchDate))
                {
                    csv.AppendLine($"{catch.CatchDate:yyyy-MM-dd}," +
                                   $"{catch.CatchTimeString}," +
                                   $"\"{catch.FishName}\"," +
                                   $"{catch.Length}," +
                                   $"{catch.Weight}," +
                                   $"\"{catch.LocationName}\"," +
                                   $"{catch.Latitude}," +
                                   $"{catch.Longitude}," +
                                   $"\"{catch.Notes?.Replace("\"", "\"\"")}\"");
                }

                _logger.LogInformation($"CSV export completed: {catches.Count} catches");

                return Encoding.UTF8.GetBytes(csv.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error exporting to CSV", ex);
                throw;
            }
        }

        public async Task<string> ExportToJsonAsync(List<FishCatch> catches)
        {
            try
            {
                _logger.LogInformation("Exporting catches to JSON");

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(catches, options);

                _logger.LogInformation($"JSON export completed: {catches.Count} catches");

                return await Task.FromResult(json);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error exporting to JSON", ex);
                throw;
            }
        }
    }
}
