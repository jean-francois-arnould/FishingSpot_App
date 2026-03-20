using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FishingSpot.PWA.Models;
using Microsoft.Extensions.Configuration;

namespace FishingSpot.PWA.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly string _supabaseKey;

        public UserProfileService(HttpClient httpClient, IConfiguration configuration, IAuthService authService)
        {
            _authService = authService;
            var supabaseUrl = configuration["Supabase:Url"] ?? "https://placeholder.supabase.co";
            _supabaseKey = configuration["Supabase:Key"] ?? "";

            _httpClient = new HttpClient { BaseAddress = new Uri(supabaseUrl) };
            if (!string.IsNullOrEmpty(_supabaseKey))
            {
                _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            }
        }

        private void SetAuthHeaders()
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Remove("apikey");

            var token = _authService.AccessToken ?? _supabaseKey;

            // Ajouter l'apikey (requis par Supabase)
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);

            // Ajouter l'Authorization avec le token d'authentification
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            Console.WriteLine($"🔑 Headers set - apikey: {_supabaseKey.Substring(0, 10)}..., token: {token?.Substring(0, 10)}...");
        }

        public async Task<UserProfile?> GetProfileAsync()
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser == null)
                    return null;

                // Utiliser 'id' au lieu de 'user_id' car la clé primaire est 'id'
                var url = $"/rest/v1/user_profiles?id=eq.{_authService.CurrentUser.Id}&select=*";
                Console.WriteLine($"🔍 Fetching profile from: {url}");

                var httpResponse = await _httpClient.GetAsync(url);
                var content = await httpResponse.Content.ReadAsStringAsync();

                Console.WriteLine($"📥 Response status: {httpResponse.StatusCode}");
                Console.WriteLine($"📥 Response content: {content}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error getting profile: {httpResponse.StatusCode} - {content}");
                    return null;
                }

                var response = JsonSerializer.Deserialize<List<UserProfile>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception getting profile: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<bool> CreateOrUpdateProfileAsync(UserProfile profile)
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser == null)
                    return false;

                profile.Id = _authService.CurrentUser.Id;
                profile.Email = _authService.CurrentUser.Email;

                Console.WriteLine($"🔄 Attempting to save profile for user: {profile.Id}");

                var existingProfile = await GetProfileAsync();

                if (existingProfile == null)
                {
                    // Créer un nouveau profil
                    Console.WriteLine("📝 Creating new profile...");

                    var profileData = new
                    {
                        id = profile.Id,
                        email = profile.Email,
                        display_name = profile.DisplayName ?? string.Empty,
                        avatar_url = profile.AvatarUrl ?? string.Empty
                    };

                    var json = JsonSerializer.Serialize(profileData);
                    Console.WriteLine($"📤 JSON to send: {json}");

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Remove("Prefer");
                    _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                    var response = await _httpClient.PostAsync("/rest/v1/user_profiles", content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"📤 Create profile response: {response.StatusCode} - {responseContent}");

                    // Si erreur 409 (Conflict), c'est que le profil existe déjà, on fait un UPDATE
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        Console.WriteLine("⚠️ Profile already exists, switching to UPDATE...");
                        return await UpdateProfileAsync(profile);
                    }

                    return response.IsSuccessStatusCode;
                }
                else
                {
                    // Mettre à jour le profil existant
                    return await UpdateProfileAsync(profile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating/updating profile: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private async Task<bool> UpdateProfileAsync(UserProfile profile)
        {
            Console.WriteLine($"📝 Updating existing profile for user: {profile.Id}");

            var profileData = new
            {
                display_name = profile.DisplayName ?? string.Empty,
                avatar_url = profile.AvatarUrl ?? string.Empty
            };

            var json = JsonSerializer.Serialize(profileData);
            Console.WriteLine($"📤 JSON to send: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"/rest/v1/user_profiles?id=eq.{profile.Id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"📤 Update profile response: {response.StatusCode} - {responseContent}");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProfileAsync()
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser == null)
                    return false;

                var response = await _httpClient.DeleteAsync($"/rest/v1/user_profiles?id=eq.{_authService.CurrentUser.Id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting profile: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync()
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser == null)
                    return false;

                await DeleteProfileAsync();

                var response = await _httpClient.DeleteAsync($"/rest/v1/fish_catches?user_id=eq.{_authService.CurrentUser.Id}");

                if (response.IsSuccessStatusCode)
                {
                    await _authService.SignOutAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting account: {ex.Message}");
                return false;
            }
        }
    }
}
