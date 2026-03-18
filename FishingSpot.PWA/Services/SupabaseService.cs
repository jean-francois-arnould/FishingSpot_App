using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FishingSpot.PWA.Models;
using Microsoft.Extensions.Configuration;

namespace FishingSpot.PWA.Services
{
    public class SupabaseService : ISupabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly string _supabaseKey;
        private bool _isInitialized = false;

        public SupabaseService(HttpClient httpClient, IConfiguration configuration, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _supabaseKey = configuration["Supabase:Key"] ?? "";
        }

        private void SetAuthHeaders()
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            var token = _authService.AccessToken ?? _supabaseKey;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            await Task.CompletedTask;
        }

        public async Task<List<FishCatch>> GetAllCatchesAsync()
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<FishCatch>>("/rest/v1/fish_catches?select=*&order=catch_date.desc");
                return response ?? new List<FishCatch>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting catches: {ex.Message}");
                return new List<FishCatch>();
            }
        }

        public async Task<FishCatch?> GetCatchByIdAsync(int id)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<FishCatch>>($"/rest/v1/fish_catches?id=eq.{id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting catch: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddCatchAsync(FishCatch fishCatch)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser != null)
                {
                    fishCatch.UserId = _authService.CurrentUser.Id;
                }

                fishCatch.CreatedAt = DateTime.UtcNow;
                var json = JsonSerializer.Serialize(fishCatch);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/fish_catches", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<FishCatch>>();
                return result?.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding catch: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateCatchAsync(FishCatch fishCatch)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var json = JsonSerializer.Serialize(fishCatch);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"/rest/v1/fish_catches?id=eq.{fishCatch.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating catch: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteCatchAsync(int id)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/fish_catches?id=eq.{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting catch: {ex.Message}");
                return false;
            }
        }
    }
}
