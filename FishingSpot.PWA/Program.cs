using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FishingSpot.PWA;
using FishingSpot.PWA.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<ISupabaseService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var authService = sp.GetRequiredService<IAuthService>();
    var supabaseUrl = config["Supabase:Url"] ?? "https://placeholder.supabase.co";
    var supabaseKey = config["Supabase:Key"] ?? "";

    var client = new HttpClient { BaseAddress = new Uri(supabaseUrl) };
    if (!string.IsNullOrEmpty(supabaseKey))
    {
        client.DefaultRequestHeaders.Add("apikey", supabaseKey);
    }

    return new SupabaseService(client, config, authService);
});

await builder.Build().RunAsync();
