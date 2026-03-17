using CsvHelper;
using CsvHelper.Configuration;
using FishingSpot.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using PdfColors = QuestPDF.Helpers.Colors;

namespace FishingSpot.Services
{
    public class ExportService
    {
        private readonly SQLiteDatabaseService _databaseService;

        public ExportService(SQLiteDatabaseService databaseService)
        {
            _databaseService = databaseService;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<string> ExportToCsvAsync()
        {
            try
            {
                var catches = await _databaseService.GetAllCatchesAsync();
                var fileName = $"FishingSpot_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.WriteRecords(catches);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to CSV: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ExportToPdfAsync()
        {
            try
            {
                var catches = await _databaseService.GetAllCatchesAsync();
                var fileName = $"FishingSpot_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(PdfColors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .Text("Carnet de Pêche - FishingSpot")
                            .SemiBold().FontSize(20).FontColor(PdfColors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(10);

                                foreach (var catchItem in catches)
                                {
                                    column.Item().BorderBottom(1).BorderColor(PdfColors.Grey.Lighten2).PaddingBottom(10).Column(catchColumn =>
                                    {
                                        catchColumn.Item().Text($"Poisson: {catchItem.FishName}").Bold();
                                        catchColumn.Item().Text($"Date: {catchItem.CatchDate:dd/MM/yyyy} à {catchItem.CatchTime}");
                                        catchColumn.Item().Text($"Lieu: {catchItem.LocationName}");
                                        catchColumn.Item().Text($"Taille: {catchItem.Length} cm - Poids: {catchItem.Weight} kg");
                                        if (!string.IsNullOrEmpty(catchItem.Notes))
                                        {
                                            catchColumn.Item().Text($"Notes: {catchItem.Notes}");
                                        }
                                    });
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                                x.Span(" sur ");
                                x.TotalPages();
                            });
                    });
                }).GeneratePdf(filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to PDF: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ShareFileAsync(string filePath)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Partager mon carnet de pêche",
                    File = new ShareFile(filePath)
                });
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sharing file: {ex.Message}");
                return false;
            }
        }
    }
}
