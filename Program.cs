using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FishingSpot.PWA;
using FishingSpot.PWA.Services;
using FishingSpot.PWA.Services.Offline;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration Supabase
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? "https://placeholder.supabase.co";
var supabaseKey = builder.Configuration["Supabase:Key"] ?? "";

// HttpClient par défaut (pour les ressources statiques)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Offline Services
builder.Services.AddScoped<INetworkStatusService, NetworkStatusService>();
builder.Services.AddScoped<IIndexedDbService, IndexedDbService>();
builder.Services.AddScoped<ISyncService, SyncService>();

// Core Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddScoped<IImageCompressionService, ImageCompressionService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IShareService, ShareService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// WeatherService avec son propre HttpClient
builder.Services.AddScoped<IWeatherService>(sp =>
{
    var httpClient = new HttpClient();
    return new WeatherService(httpClient);
});

// UserProfileService avec son propre HttpClient configuré pour Supabase
builder.Services.AddScoped<IUserProfileService>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(supabaseUrl) };
    httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
    var config = sp.GetRequiredService<IConfiguration>();
    var authService = sp.GetRequiredService<IAuthService>();
    return new UserProfileService(httpClient, config, authService);
});

// EquipmentService avec son propre HttpClient configuré pour Supabase
// Online service
builder.Services.AddScoped<EquipmentService>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(supabaseUrl) };
    httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
    var config = sp.GetRequiredService<IConfiguration>();
    var authService = sp.GetRequiredService<IAuthService>();
    return new EquipmentService(httpClient, config, authService);
});

// Offline wrapper
builder.Services.AddScoped<IEquipmentService>(sp =>
{
    var onlineService = sp.GetRequiredService<EquipmentService>();
    var networkStatus = sp.GetRequiredService<INetworkStatusService>();
    var indexedDb = sp.GetRequiredService<IIndexedDbService>();
    var syncService = sp.GetRequiredService<ISyncService>();
    return new OfflineEquipmentService(onlineService, networkStatus, indexedDb, syncService);
});

// SupabaseService avec son propre HttpClient configuré pour Supabase
// Online service
builder.Services.AddScoped<SupabaseService>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(supabaseUrl) };
    httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
    var config = sp.GetRequiredService<IConfiguration>();
    var authService = sp.GetRequiredService<IAuthService>();
    return new SupabaseService(httpClient, config, authService);
});

// Offline wrapper
builder.Services.AddScoped<ISupabaseService>(sp =>
{
    var onlineService = sp.GetRequiredService<SupabaseService>();
    var networkStatus = sp.GetRequiredService<INetworkStatusService>();
    var indexedDb = sp.GetRequiredService<IIndexedDbService>();
    var syncService = sp.GetRequiredService<ISyncService>();
    var authService = sp.GetRequiredService<IAuthService>();
    return new OfflineSupabaseService(onlineService, networkStatus, indexedDb, syncService, authService);
});

// Initialize offline services
var app = builder.Build();

var networkStatus = app.Services.GetRequiredService<INetworkStatusService>();
var indexedDb = app.Services.GetRequiredService<IIndexedDbService>();
var syncService = app.Services.GetRequiredService<ISyncService>();

await networkStatus.InitializeAsync();
await indexedDb.InitializeAsync();
await syncService.InitializeAsync();

await app.RunAsync();
