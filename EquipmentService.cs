using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FishingSpot.PWA.Models.Equipment;

namespace FishingSpot.PWA.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly string _supabaseKey;

        public EquipmentService(HttpClient httpClient, IConfiguration configuration, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _supabaseKey = configuration["Supabase:Key"] ?? "";
        }

        private void SetAuthHeaders()
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

        // ============================================
        // CANNES
        // ============================================
        public async Task<List<Rod>> GetAllRodsAsync()
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Rod>>("/rest/v1/rods?select=*&order=created_at.desc");
                return response ?? new List<Rod>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des cannes: {ex.Message}");
                return new List<Rod>();
            }
        }

        public async Task<Rod?> GetRodByIdAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Rod>>($"/rest/v1/rods?id=eq.{id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddRodAsync(Rod rod)
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser != null)
                    rod.UserId = _authService.CurrentUser.Id;

                rod.CreatedAt = DateTime.UtcNow;

                // Options pour ignorer les valeurs par défaut comme id: 0
                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                };
                var json = JsonSerializer.Serialize(rod, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/rods", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<Rod>>();
                return result?.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateRodAsync(Rod rod)
        {
            SetAuthHeaders();
            try
            {
                var json = JsonSerializer.Serialize(rod);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"/rest/v1/rods?id=eq.{rod.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteRodAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/rods?id=eq.{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        // ============================================
        // MOULINETS (même pattern)
        // ============================================
        public async Task<List<Reel>> GetAllReelsAsync()
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Reel>>("/rest/v1/reels?select=*&order=created_at.desc");
                return response ?? new List<Reel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return new List<Reel>();
            }
        }

        public async Task<Reel?> GetReelByIdAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Reel>>($"/rest/v1/reels?id=eq.{id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddReelAsync(Reel reel)
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser != null)
                    reel.UserId = _authService.CurrentUser.Id;

                reel.CreatedAt = DateTime.UtcNow;

                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                };
                var json = JsonSerializer.Serialize(reel, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/reels", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<Reel>>();
                return result?.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateReelAsync(Reel reel)
        {
            SetAuthHeaders();
            try
            {
                var json = JsonSerializer.Serialize(reel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"/rest/v1/reels?id=eq.{reel.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteReelAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/reels?id=eq.{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        // ============================================
        // FILS
        // ============================================
        public async Task<List<Line>> GetAllLinesAsync()
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Line>>("/rest/v1/lines?select=*&order=created_at.desc");
                return response ?? new List<Line>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return new List<Line>();
            }
        }

        public async Task<Line?> GetLineByIdAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Line>>($"/rest/v1/lines?id=eq.{id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddLineAsync(Line line)
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser != null)
                    line.UserId = _authService.CurrentUser.Id;

                line.CreatedAt = DateTime.UtcNow;

                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                };
                var json = JsonSerializer.Serialize(line, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/lines", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<Line>>();
                return result?.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateLineAsync(Line line)
        {
            SetAuthHeaders();
            try
            {
                var json = JsonSerializer.Serialize(line);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"/rest/v1/lines?id=eq.{line.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteLineAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/lines?id=eq.{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        // ============================================
        // LEURRES
        // ============================================
        public async Task<List<Lure>> GetAllLuresAsync()
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Lure>>("/rest/v1/lures?select=*&order=created_at.desc");
                return response ?? new List<Lure>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return new List<Lure>();
            }
        }

        public async Task<Lure?> GetLureByIdAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Lure>>($"/rest/v1/lures?id=eq.{id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddLureAsync(Lure lure)
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser != null)
                    lure.UserId = _authService.CurrentUser.Id;

                lure.CreatedAt = DateTime.UtcNow;

                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                };
                var json = JsonSerializer.Serialize(lure, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/lures", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<Lure>>();
                return result?.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateLureAsync(Lure lure)
        {
            SetAuthHeaders();
            try
            {
                var json = JsonSerializer.Serialize(lure);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"/rest/v1/lures?id=eq.{lure.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteLureAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/lures?id=eq.{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        // ============================================
        // BAS DE LIGNE
        // ============================================
        public async Task<List<Leader>> GetAllLeadersAsync()
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Leader>>("/rest/v1/leaders?select=*&order=created_at.desc");
                return response ?? new List<Leader>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return new List<Leader>();
            }
        }

        public async Task<Leader?> GetLeaderByIdAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Leader>>($"/rest/v1/leaders?id=eq.{id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddLeaderAsync(Leader leader)
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser != null)
                    leader.UserId = _authService.CurrentUser.Id;

                leader.CreatedAt = DateTime.UtcNow;

                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                };
                var json = JsonSerializer.Serialize(leader, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/leaders", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<Leader>>();
                return result?.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateLeaderAsync(Leader leader)
        {
            SetAuthHeaders();
            try
            {
                var json = JsonSerializer.Serialize(leader);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"/rest/v1/leaders?id=eq.{leader.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteLeaderAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/leaders?id=eq.{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        // ============================================
        // HAMEÇONS
        // ============================================
        public async Task<List<Hook>> GetAllHooksAsync()
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Hook>>("/rest/v1/hooks?select=*&order=created_at.desc");
                return response ?? new List<Hook>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return new List<Hook>();
            }
        }

        public async Task<Hook?> GetHookByIdAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Hook>>($"/rest/v1/hooks?id=eq.{id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddHookAsync(Hook hook)
        {
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser != null)
                    hook.UserId = _authService.CurrentUser.Id;

                hook.CreatedAt = DateTime.UtcNow;

                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                };
                var json = JsonSerializer.Serialize(hook, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/hooks", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<Hook>>();
                return result?.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateHookAsync(Hook hook)
        {
            SetAuthHeaders();
            try
            {
                var json = JsonSerializer.Serialize(hook);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"/rest/v1/hooks?id=eq.{hook.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteHookAsync(int id)
        {
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/hooks?id=eq.{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
                return false;
            }
        }
    }
}
