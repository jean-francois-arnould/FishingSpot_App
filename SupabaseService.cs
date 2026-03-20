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
            _httpClient.DefaultRequestHeaders.Remove("apikey");

            var token = _authService.AccessToken;

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("⚠️ WARNING: No access token found, using API key only");
                token = _supabaseKey;
            }
            else
            {
                Console.WriteLine($"✅ Using user access token: {token.Substring(0, 20)}...");
            }

            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
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

        public async Task<List<FishSpecies>> GetAllFishSpeciesAsync()
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<FishSpecies>>("/rest/v1/fish_species?select=*&is_active=eq.true&order=common_name.asc");
                Console.WriteLine($"✅ Loaded {response?.Count ?? 0} fish species");
                return response ?? new List<FishSpecies>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting fish species: {ex.Message}");
                return new List<FishSpecies>();
            }
        }

        public async Task<int> AddFishSpeciesAsync(FishSpecies fishSpecies)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                // Vérifier si le poisson existe déjà (insensible à la casse)
                var existingSpecies = await GetAllFishSpeciesAsync();
                var duplicate = existingSpecies.FirstOrDefault(f => 
                    f.CommonName.Equals(fishSpecies.CommonName, StringComparison.OrdinalIgnoreCase));

                if (duplicate != null)
                {
                    Console.WriteLine($"⚠️ Fish species '{fishSpecies.CommonName}' already exists with ID {duplicate.Id}");
                    return -1; // Code d'erreur pour doublon
                }

                fishSpecies.CreatedAt = DateTime.UtcNow;
                fishSpecies.IsActive = true;

                var json = JsonSerializer.Serialize(fishSpecies);
                Console.WriteLine($"Adding new fish species: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/fish_species", content);

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response body: {responseBody}");

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<FishSpecies>>();
                var id = result?.FirstOrDefault()?.Id ?? 0;
                Console.WriteLine($"✅ New fish species added with ID: {id}");
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding fish species: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<FishingBrand>> GetBrandsByCategoryAsync(string category)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<FishingBrand>>(
                    $"/rest/v1/fishing_brands?category=eq.{category}&is_active=eq.true&order=name.asc");
                Console.WriteLine($"✅ Loaded {response?.Count ?? 0} brands for category {category}");
                return response ?? new List<FishingBrand>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting brands for category {category}: {ex.Message}");
                return new List<FishingBrand>();
            }
        }

        public async Task<int> AddFishingBrandAsync(FishingBrand brand)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                // Vérifier si la marque existe déjà
                var existingBrands = await GetBrandsByCategoryAsync(brand.Category);
                var duplicate = existingBrands.FirstOrDefault(b => 
                    b.Name.Equals(brand.Name, StringComparison.OrdinalIgnoreCase));

                if (duplicate != null)
                {
                    Console.WriteLine($"⚠️ Brand '{brand.Name}' already exists in category {brand.Category} with ID {duplicate.Id}");
                    return -1; // Code d'erreur pour doublon
                }

                brand.CreatedAt = DateTime.UtcNow;
                brand.IsActive = true;

                var json = JsonSerializer.Serialize(brand);
                Console.WriteLine($"Adding new brand: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/fishing_brands", content);

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response body: {responseBody}");

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<FishingBrand>>();
                var id = result?.FirstOrDefault()?.Id ?? 0;
                Console.WriteLine($"✅ New brand added with ID: {id}");
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding brand: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
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

                // Ne pas envoyer l'ID (auto-généré par la DB)
                var catchId = fishCatch.Id;
                fishCatch.Id = 0;
                fishCatch.CreatedAt = DateTime.UtcNow;

                var json = JsonSerializer.Serialize(fishCatch, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                });
                Console.WriteLine($"Sending catch JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/fish_catches", content);

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response body: {responseBody}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error {response.StatusCode}: {responseBody}");
                    throw new HttpRequestException($"HTTP {response.StatusCode}: {responseBody}");
                }

                var result = await response.Content.ReadFromJsonAsync<List<FishCatch>>();
                var id = result?.FirstOrDefault()?.Id ?? 0;
                Console.WriteLine($"✅ Returned catch ID: {id}");
                return id;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ HTTP Error adding catch: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error adding catch: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
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

        // Fishing Setups Methods
        public async Task<List<FishingSetup>> GetAllSetupsAsync()
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<FishingSetup>>("/rest/v1/fishing_setups?select=*&order=is_favorite.desc,created_at.desc");
                return response ?? new List<FishingSetup>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting setups: {ex.Message}");
                return new List<FishingSetup>();
            }
        }

        public async Task<FishingSetup?> GetSetupByIdAsync(int id)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<FishingSetup>>($"/rest/v1/fishing_setups?id=eq.{id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting setup: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddSetupAsync(FishingSetup setup)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                if (_authService.CurrentUser != null)
                {
                    setup.UserId = _authService.CurrentUser.Id;
                }

                setup.CreatedAt = DateTime.UtcNow;
                var json = JsonSerializer.Serialize(setup);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync("/rest/v1/fishing_setups", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<FishingSetup>>();
                return result?.FirstOrDefault()?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding setup: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateSetupAsync(FishingSetup setup)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var json = JsonSerializer.Serialize(setup);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"/rest/v1/fishing_setups?id=eq.{setup.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating setup: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteSetupAsync(int id)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/fishing_setups?id=eq.{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting setup: {ex.Message}");
                return false;
            }
        }

        // ============================================
        // NOUVEAU : Gestion du setup actuel
        // ============================================
        public async Task<FishingSetup?> GetCurrentSetupAsync()
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<FishingSetup>>("/rest/v1/fishing_setups?is_current=eq.true&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du setup actuel: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SetCurrentSetupAsync(int setupId)
        {
            await InitializeAsync();
            SetAuthHeaders();
            try
            {
                // D'abord, désactiver tous les setups actuels
                var allSetups = await GetAllSetupsAsync();
                foreach (var setup in allSetups.Where(s => s.IsCurrent))
                {
                    setup.IsCurrent = false;
                    await UpdateSetupAsync(setup);
                }

                // Ensuite, activer le setup sélectionné
                var selectedSetup = await GetSetupByIdAsync(setupId);
                if (selectedSetup != null)
                {
                    selectedSetup.IsCurrent = true;
                    return await UpdateSetupAsync(selectedSetup);
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la définition du setup actuel: {ex.Message}");
                return false;
            }
        }

        // ============================================
        // NOUVEAU : Upload de photos vers Supabase Storage
        // ============================================
        public async Task<string?> UploadPhotoAsync(Stream photoStream, string fileName)
        {
            SetAuthHeaders();
            try
            {
                // Créer un nom de fichier unique
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var filePath = $"catches/{uniqueFileName}";

                // Préparer le contenu
                using var content = new StreamContent(photoStream);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                // Upload vers Supabase Storage
                var response = await _httpClient.PostAsync($"/storage/v1/object/fishing-photos/{filePath}", content);

                if (response.IsSuccessStatusCode)
                {
                    // Retourner l'URL publique
                    var supabaseUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/');
                    return $"{supabaseUrl}/storage/v1/object/public/fishing-photos/{filePath}";
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erreur upload photo: {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'upload de la photo: {ex.Message}");
                return null;
            }
        }
    }
}
