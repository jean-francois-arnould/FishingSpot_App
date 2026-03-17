using Microsoft.Extensions.Logging;
using FishingSpot.Services;
using FishingSpot.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace FishingSpot
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Enregistrement des services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<SQLiteDatabaseService>();
            builder.Services.AddSingleton<MaterialService>();
            builder.Services.AddSingleton<SetupService>();
            builder.Services.AddSingleton<WeatherService>();
            builder.Services.AddSingleton<ExportService>();
            builder.Services.AddSingleton<StatisticsService>();
            builder.Services.AddSingleton<SocialShareService>();

            // Enregistrement des pages
            builder.Services.AddTransient<FishDocumentationPage>();
            builder.Services.AddTransient<MyCatchesPage>();
            builder.Services.AddTransient<AddCatchPage>();
            builder.Services.AddTransient<FishCardDetailPage>();
            builder.Services.AddTransient<MaterialListPage>();
            builder.Services.AddTransient<AddMaterialPage>();
            builder.Services.AddTransient<MaterialManagementPage>();
            builder.Services.AddTransient<AddSetupPage>();
            builder.Services.AddTransient<SelectSetupPage>();
            builder.Services.AddTransient<StatisticsPage>();
            builder.Services.AddTransient<CalendarPage>();

            return builder.Build();
        }
    }
}
