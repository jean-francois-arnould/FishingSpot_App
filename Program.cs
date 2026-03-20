using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FishingSpot.PWA;
using FishingSpot.PWA.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration Supabase
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? "https://placeholder.supabase.co";
var supabaseKey = builder.Configuration["Supabase:Key"] ?? "";

// HttpClient par défaut (pour les ressources statiques)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Services
builder.Services.AddScoped<IAuthService, AuthService>();

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
builder.Services.AddScoped<IEquipmentService>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(supabaseUrl) };
    httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
    var config = sp.GetRequiredService<IConfiguration>();
    var authService = sp.GetRequiredService<IAuthService>();
    return new EquipmentService(httpClient, config, authService);
});

// SupabaseService avec son propre HttpClient configuré pour Supabase
builder.Services.AddScoped<ISupabaseService>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(supabaseUrl) };
    httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
    var config = sp.GetRequiredService<IConfiguration>();
    var authService = sp.GetRequiredService<IAuthService>();
    return new SupabaseService(httpClient, config, authService);
});

await builder.Build().RunAsync();
