using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FishingSpot.PWA.Models.Auth;
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
        private string? _refreshToken;
        private DateTime? _tokenExpiresAt;
        private System.Timers.Timer? _refreshTimer;

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
                var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "supabase_refresh_token");
                var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "supabase_user");
                var expiresAtStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "supabase_token_expires_at");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
                {
                    _accessToken = token;
                    _refreshToken = refreshToken;
                    _currentUser = JsonSerializer.Deserialize<User>(userJson);

                    if (!string.IsNullOrEmpty(expiresAtStr) && DateTime.TryParse(expiresAtStr, out var expiresAt))
                    {
                        _tokenExpiresAt = expiresAt;

                        // Vérifier si le token est expiré ou va expirer bientôt
                        if (_tokenExpiresAt <= DateTime.UtcNow.AddMinutes(5))
                        {
                            Console.WriteLine("🔄 Token expiré ou proche de l'expiration, rafraîchissement...");
                            var refreshSuccess = await RefreshTokenAsync();

                            // Si le rafraîchissement échoue, déconnecter l'utilisateur
                            if (!refreshSuccess)
                            {
                                Console.WriteLine("❌ Impossible de rafraîchir le token, déconnexion...");
                                await SignOutAsync();
                            }
                        }
                        else
                        {
                            // Planifier le rafraîchissement automatique
                            ScheduleTokenRefresh();
                        }
                    }
                    else if (string.IsNullOrEmpty(expiresAtStr))
                    {
                        // Pas d'expiration enregistrée, déconnecter par sécurité
                        Console.WriteLine("⚠️ Date d'expiration du token manquante, déconnexion par sécurité...");
                        await SignOutAsync();
                    }

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
                        _refreshToken = authResponse.RefreshToken;

                        // Le token expire dans 1 heure (3600 secondes)
                        _tokenExpiresAt = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn > 0 ? authResponse.ExpiresIn : 3600);

                        // Sauvegarder dans localStorage
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "supabase_token", _accessToken);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "supabase_refresh_token", _refreshToken ?? "");
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "supabase_user", JsonSerializer.Serialize(_currentUser));
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "supabase_token_expires_at", _tokenExpiresAt.Value.ToString("O"));

                        Console.WriteLine($"✅ Connexion réussie ! Token expire à {_tokenExpiresAt:HH:mm:ss}");

                        // Planifier le rafraîchissement automatique
                        ScheduleTokenRefresh();

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
                _refreshToken = null;
                _tokenExpiresAt = null;

                // Arrêter le timer de rafraîchissement
                _refreshTimer?.Stop();
                _refreshTimer?.Dispose();
                _refreshTimer = null;

                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_token");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_refresh_token");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_user");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_token_expires_at");

                OnAuthStateChanged?.Invoke(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error signing out: {ex.Message}");
            }
        }

        private async Task<bool> RefreshTokenAsync()
        {
            if (string.IsNullOrEmpty(_refreshToken))
            {
                Console.WriteLine("⚠️ No refresh token available");
                await NotifySessionExpired();
                return false;
            }

            try
            {
                var request = new { refresh_token = _refreshToken };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("apikey");
                _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);

                var response = await _httpClient.PostAsync($"{_supabaseUrl}/auth/v1/token?grant_type=refresh_token", content);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (authResponse != null && !string.IsNullOrEmpty(authResponse.AccessToken))
                    {
                        _accessToken = authResponse.AccessToken;
                        _refreshToken = authResponse.RefreshToken ?? _refreshToken;
                        _tokenExpiresAt = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn > 0 ? authResponse.ExpiresIn : 3600);

                        // Mettre à jour localStorage
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "supabase_token", _accessToken);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "supabase_refresh_token", _refreshToken);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "supabase_token_expires_at", _tokenExpiresAt.Value.ToString("O"));

                        Console.WriteLine($"✅ Token rafraîchi avec succès ! Expire à {_tokenExpiresAt:HH:mm:ss}");

                        // Replanifier le prochain rafraîchissement
                        ScheduleTokenRefresh();

                        return true;
                    }
                }

                Console.WriteLine($"❌ Échec du rafraîchissement du token: {response.StatusCode}");
                await NotifySessionExpired();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing token: {ex.Message}");
                await NotifySessionExpired();
                return false;
            }
        }

        private async Task NotifySessionExpired()
        {
            try
            {
                // Déconnecter l'utilisateur
                _currentUser = null;
                _accessToken = null;
                _refreshToken = null;
                _tokenExpiresAt = null;

                // Nettoyer localStorage
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_token");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_refresh_token");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_user");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "supabase_token_expires_at");

                // Afficher une notification élégante à l'utilisateur
                await _jsRuntime.InvokeVoidAsync("showSessionExpiredNotification");

                OnAuthStateChanged?.Invoke(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error notifying session expired: {ex.Message}");
                // Fallback : redirection directe
                try
                {
                    await _jsRuntime.InvokeVoidAsync("eval", "window.location.href = '/FishingSpot_App/login';");
                }
                catch
                {
                    // Si tout échoue, au moins nettoyer l'état
                    OnAuthStateChanged?.Invoke(null);
                }
            }
        }

        private void ScheduleTokenRefresh()
        {
            // Arrêter le timer existant
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();

            if (_tokenExpiresAt.HasValue)
            {
                // Rafraîchir 5 minutes avant l'expiration
                var refreshAt = _tokenExpiresAt.Value.AddMinutes(-5);
                var delay = refreshAt - DateTime.UtcNow;

                if (delay.TotalSeconds > 0)
                {
                    Console.WriteLine($"🔄 Rafraîchissement automatique planifié dans {delay.TotalMinutes:F1} minutes");

                    _refreshTimer = new System.Timers.Timer(delay.TotalMilliseconds);
                    _refreshTimer.Elapsed += async (sender, e) =>
                    {
                        await RefreshTokenAsync();
                    };
                    _refreshTimer.AutoReset = false;
                    _refreshTimer.Start();
                }
                else
                {
                    // Le token expire bientôt, rafraîchir immédiatement
                    Task.Run(async () => await RefreshTokenAsync());
                }
            }
        }
    }
}
