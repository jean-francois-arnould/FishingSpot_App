using FishingSpot.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace FishingSpot.Views
{
    public partial class StatisticsPage : ContentPage
    {
        private readonly StatisticsService _statisticsService;
        private readonly ExportService _exportService;
        private StatisticsData? _statistics;

        public StatisticsPage(StatisticsService statisticsService, ExportService exportService)
        {
            InitializeComponent();
            _statisticsService = statisticsService;
            _exportService = exportService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadStatisticsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            try
            {
                _statistics = await _statisticsService.GetStatisticsAsync();

                TotalCatchesLabel.Text = _statistics.TotalCatches.ToString();
                TotalWeightLabel.Text = $"{_statistics.TotalWeight:F2} kg";
                AverageWeightLabel.Text = $"{_statistics.AverageWeight:F2} kg";
                AverageLengthLabel.Text = $"{_statistics.AverageLength:F2} cm";

                if (_statistics.BiggestFish != null)
                {
                    BiggestFishLabel.Text = $"Plus gros poisson: {_statistics.BiggestFish.FishName} ({_statistics.BiggestFish.Weight:F2} kg)";
                }

                if (_statistics.LongestFish != null)
                {
                    LongestFishLabel.Text = $"Plus long poisson: {_statistics.LongestFish.FishName} ({_statistics.LongestFish.Length:F2} cm)";
                }

                if (_statistics.MostCaughtFish != null)
                {
                    MostCaughtFishLabel.Text = $"Poisson le plus pêché: {_statistics.MostCaughtFish.Name} ({_statistics.MostCaughtFish.Count}x)";
                }

                LocationsCollectionView.ItemsSource = _statistics.CatchesByLocation;

                MonthlyChartView.InvalidateSurface();
                FishChartView.InvalidateSurface();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Impossible de charger les statistiques: {ex.Message}", "OK");
            }
        }

        private void OnMonthlyChartPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_statistics?.CatchesByMonth == null || !_statistics.CatchesByMonth.Any())
                return;

            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            var info = e.Info;
            var padding = 40f;
            var chartWidth = info.Width - 2 * padding;
            var chartHeight = info.Height - 2 * padding;

            var maxCount = _statistics.CatchesByMonth.Max(m => m.Count);
            var barWidth = chartWidth / _statistics.CatchesByMonth.Count;

            using var paint = new SKPaint
            {
                Color = SKColors.DeepSkyBlue,
                IsAntialias = true
            };

            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 12,
                IsAntialias = true
            };

            for (int i = 0; i < _statistics.CatchesByMonth.Count; i++)
            {
                var month = _statistics.CatchesByMonth[i];
                var barHeight = (month.Count / (float)maxCount) * chartHeight;
                var x = padding + i * barWidth;
                var y = info.Height - padding - barHeight;

                canvas.DrawRect(x + 5, y, barWidth - 10, barHeight, paint);

                var monthText = $"{month.Month}/{month.Year:yy}";
                canvas.DrawText(monthText, x + barWidth / 2, info.Height - 10, textPaint);

                canvas.DrawText(month.Count.ToString(), x + barWidth / 2, y - 5, textPaint);
            }
        }

        private void OnFishChartPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_statistics?.CatchesByFish == null || !_statistics.CatchesByFish.Any())
                return;

            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            var info = e.Info;
            var centerX = info.Width / 2f;
            var centerY = info.Height / 2f;
            var radius = Math.Min(centerX, centerY) - 40;

            var total = _statistics.CatchesByFish.Sum(f => f.Count);
            var startAngle = -90f;

            var colors = new[] { 
                SKColors.DeepSkyBlue, SKColors.LightGreen, SKColors.Orange, 
                SKColors.Pink, SKColors.Purple, SKColors.Yellow 
            };

            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 14,
                IsAntialias = true
            };

            for (int i = 0; i < Math.Min(_statistics.CatchesByFish.Count, colors.Length); i++)
            {
                var fish = _statistics.CatchesByFish[i];
                var sweepAngle = (fish.Count / (float)total) * 360f;

                using var paint = new SKPaint
                {
                    Color = colors[i],
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };

                using var path = new SKPath();
                path.MoveTo(centerX, centerY);
                path.ArcTo(
                    new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius),
                    startAngle,
                    sweepAngle,
                    false);
                path.Close();

                canvas.DrawPath(path, paint);

                var labelAngle = startAngle + sweepAngle / 2;
                var labelX = centerX + (radius * 0.7f) * (float)Math.Cos(labelAngle * Math.PI / 180);
                var labelY = centerY + (radius * 0.7f) * (float)Math.Sin(labelAngle * Math.PI / 180);

                canvas.DrawText($"{fish.Count}", labelX, labelY, textPaint);

                startAngle += sweepAngle;
            }
        }

        private async void OnExportCsvClicked(object sender, EventArgs e)
        {
            try
            {
                var filePath = await _exportService.ExportToCsvAsync();
                var result = await DisplayAlert("Export CSV", 
                    "Export réussi ! Voulez-vous partager le fichier ?", 
                    "Oui", "Non");

                if (result)
                {
                    await _exportService.ShareFileAsync(filePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors de l'export: {ex.Message}", "OK");
            }
        }

        private async void OnExportPdfClicked(object sender, EventArgs e)
        {
            try
            {
                var filePath = await _exportService.ExportToPdfAsync();
                var result = await DisplayAlert("Export PDF", 
                    "Export réussi ! Voulez-vous partager le fichier ?", 
                    "Oui", "Non");

                if (result)
                {
                    await _exportService.ShareFileAsync(filePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors de l'export: {ex.Message}", "OK");
            }
        }
    }
}
