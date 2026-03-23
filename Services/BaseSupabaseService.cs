using Microsoft.Extensions.Configuration;

namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Base class for Supabase services providing common authentication header management
    /// </summary>
    public abstract class BaseSupabaseService
    {
        protected readonly HttpClient _httpClient;
        protected readonly IAuthService _authService;
        protected readonly string _supabaseKey;

        protected BaseSupabaseService(HttpClient httpClient, IConfiguration configuration, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _supabaseKey = configuration["Supabase:Key"] ?? "";
        }

        protected void SetAuthHeaders()
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Remove("apikey");

            var token = _authService.AccessToken;

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("⚠️ WARNING: No access token found, using API key only");
                token = _supabaseKey;
            }

            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    }
}
