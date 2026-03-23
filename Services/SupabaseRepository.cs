using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Generic repository service for common CRUD operations on Supabase tables
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public class SupabaseRepository<T> where T : class
    {
        private readonly HttpClient _httpClient;
        private readonly string _tableName;

        public SupabaseRepository(HttpClient httpClient, string tableName)
        {
            _httpClient = httpClient;
            _tableName = tableName;
        }

        public async Task<List<T>> GetAllAsync(string? orderBy = null)
        {
            try
            {
                var url = $"/rest/v1/{_tableName}?select=*";
                if (!string.IsNullOrEmpty(orderBy))
                {
                    url += $"&order={orderBy}";
                }

                var response = await _httpClient.GetFromJsonAsync<List<T>>(url);
                return response ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all {_tableName}: {ex.Message}");
                return new List<T>();
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<T>>($"/rest/v1/{_tableName}?id=eq.{id}&select=*");
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting {_tableName} by id {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddAsync(T entity)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                };
                var json = JsonSerializer.Serialize(entity, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Prefer");
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var response = await _httpClient.PostAsync($"/rest/v1/{_tableName}", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<T>>();

                // Try to get the Id property dynamically
                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty != null && result?.FirstOrDefault() != null)
                {
                    var id = idProperty.GetValue(result.First());
                    return id != null ? Convert.ToInt32(id) : 0;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding {_tableName}: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateAsync(int id, T entity)
        {
            try
            {
                var json = JsonSerializer.Serialize(entity);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"/rest/v1/{_tableName}?id=eq.{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating {_tableName}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/{_tableName}?id=eq.{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting {_tableName}: {ex.Message}");
                return false;
            }
        }
    }
}
