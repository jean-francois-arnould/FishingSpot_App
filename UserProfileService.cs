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
            var token = _authService.AccessToken ?? _supabaseKey;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public async Task<UserProfile?> GetProfileAsync()
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser == null)
                    return null;

                var response = await _httpClient.GetFromJsonAsync<List<UserProfile>>($"/rest/v1/user_profiles?user_id=eq.{_authService.CurrentUser.Id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting profile: {ex.Message}");
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

                profile.UserId = _authService.CurrentUser.Id;
                profile.UpdatedAt = DateTime.UtcNow;

                var existingProfile = await GetProfileAsync();

                if (existingProfile == null)
                {
                    profile.CreatedAt = DateTime.UtcNow;
                    var json = JsonSerializer.Serialize(profile);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Remove("Prefer");
                    _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                    var response = await _httpClient.PostAsync("/rest/v1/user_profiles", content);
                    return response.IsSuccessStatusCode;
                }
                else
                {
                    profile.Id = existingProfile.Id;
                    profile.CreatedAt = existingProfile.CreatedAt;

                    var json = JsonSerializer.Serialize(profile);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PatchAsync($"/rest/v1/user_profiles?user_id=eq.{_authService.CurrentUser.Id}", content);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating/updating profile: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProfileAsync()
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser == null)
                    return false;

                var response = await _httpClient.DeleteAsync($"/rest/v1/user_profiles?user_id=eq.{_authService.CurrentUser.Id}");
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
