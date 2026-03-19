using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FishingSpot.PWA.Models;
using Microsoft.JSInterop;

namespace FishingSpot.PWA.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private User? _currentUser;
        private string? _accessToken;

        public event Action<User?>? OnAuthStateChanged;

        public User? CurrentUser => _currentUser;
        public string? AccessToken => _accessToken;
        public bool IsAuthenticated => _currentUser != null && !string.IsNullOrEmpty(_accessToken);

        public AuthService(HttpClient httpClient, IConfiguration configuration, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _supabaseUrl = configuration["Supabase:Url"] ?? "";
            _supabaseKey = configuration["Supabase:Key"] ?? "";

            if (string.IsNullOrEmpty(_supabaseUrl) || _supabaseUrl.Contains("YOUR_SUPABASE"))
            {
                Console.WriteLine("⚠️ ERREUR: Configuration Supabase manquante! Veuillez configurer wwwroot/appsettings.json");
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "supabase_token");
                var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "supabase_user");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
                {
                    _accessToken = token;
                    _currentUser = JsonSerializer.Deserialize<User>(userJson);
                    OnAuthStateChanged?.Invoke(_currentUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing auth: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> SignUpAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(_supabaseUrl) || _supabaseUrl.Contains("YOUR_SUPABASE"))
                {
                    return (false, "Configuration Supabase manquante. Veuillez configurer wwwroot/appsettings.json avec vos identifiants Supabase.");
                }

                var request = new AuthRequest { Email = email, Password = password };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("apikey");
                _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);

                Console.WriteLine($"🔄 Tentative d'inscription à: {_supabaseUrl}/auth/v1/signup");
                var response = await _httpClient.PostAsync($"{_supabaseUrl}/auth/v1/signup", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📥 Réponse Supabase: {response.StatusCode} - {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (authResponse?.User != null)
                    {
                        return (true, "Compte créé! Vérifiez votre email pour confirmer votre inscription.");
                    }
                    return (false, $"Réponse inattendue de Supabase: {responseContent}");
                }

                // Essayer de parser l'erreur de Supabase
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<SupabaseError>(responseContent);
                    if (errorResponse?.Message != null)
                    {
                        return (false, $"Erreur Supabase: {errorResponse.Message}");
                    }
                }
                catch { }

                return (false, $"Erreur HTTP {response.StatusCode}: {responseContent}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Erreur réseau: {ex.Message}");
                return (false, $"Erreur de connexion à Supabase: {ex.Message}. Vérifiez votre URL Supabase.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de l'inscription: {ex.Message}");
                return (false, $"Erreur: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> SignInAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(_supabaseUrl) || _supabaseUrl.Contains("YOUR_SUPABASE"))
                {
                    return (false, "Configuration Supabase manquante. Veuillez configurer wwwroot/appsettings.json avec vos identifiants Supabase.");
                }

                var request = new AuthRequest { Email = email, Password = password };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("apikey");
                _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);

                var response = await _httpClient.PostAsync($"{_supabaseUrl}/auth/v1/token?grant_type=password", content);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (authResponse?.User != null && !string.IsNullOrEmpty(authResponse.AccessToken))
                    {
                        _currentUser = authResponse.User;
                        _accessToken = authResponse.AccessToken;

                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "supabase_token", _accessToken);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "supabase_user", JsonSerializer.Serialize(_currentUser));

                        OnAuthStateChanged?.Invoke(_currentUser);
                        return (true, "Connexion réussie!");
                    }
                    return (false, "Erreur lors de la connexion.");
                }

                return (false, "Email ou mot de passe incorrect.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error signing in: {ex.Message}");
                return (false, $"Erreur: {ex.Message}");
            }
        }

        public async Task SignOutAsync()
        {
            try
            {
                _currentUser = null;
                _accessToken = null;

                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_token");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_user");

                OnAuthStateChanged?.Invoke(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error signing out: {ex.Message}");
            }
        }
    }
}
