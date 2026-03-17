using Microsoft.Extensions.Logging;
using FishingSpot.Services;
using FishingSpot.Views;

namespace FishingSpot
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
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
            builder.Services.AddSingleton<MaterialService>();
            builder.Services.AddSingleton<SetupService>();

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

            return builder.Build();
        }
    }
}
