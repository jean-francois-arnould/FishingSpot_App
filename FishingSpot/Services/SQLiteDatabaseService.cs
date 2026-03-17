using FishingSpot.Models;
using SQLite;
using System.Collections.ObjectModel;

namespace FishingSpot.Services
{
    public class SQLiteDatabaseService
    {
        private readonly SQLiteAsyncConnection _database;
        private bool _isInitialized = false;

        public SQLiteDatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "fishingspot.db3");
            _database = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            // Créer les tables
            await _database.CreateTableAsync<FishCatch>();
            await _database.CreateTableAsync<FishingMaterial>();
            await _database.CreateTableAsync<FishingSetup>();
            await _database.CreateTableAsync<WeatherData>();

            _isInitialized = true;
        }

        #region FishCatch Operations

        public async Task<ObservableCollection<FishCatch>> GetAllCatchesAsync()
        {
            await InitializeAsync();
            var catches = await _database.Table<FishCatch>()
                .OrderByDescending(c => c.CatchDate)
                .ToListAsync();
            return new ObservableCollection<FishCatch>(catches);
        }

        public async Task<FishCatch?> GetCatchByIdAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<FishCatch>()
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddCatchAsync(FishCatch fishCatch)
        {
            await InitializeAsync();
            return await _database.InsertAsync(fishCatch);
        }

        public async Task<int> UpdateCatchAsync(FishCatch fishCatch)
        {
            await InitializeAsync();
            return await _database.UpdateAsync(fishCatch);
        }

        public async Task<int> DeleteCatchAsync(FishCatch fishCatch)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(fishCatch);
        }

        #endregion

        #region FishingMaterial Operations

        public async Task<ObservableCollection<FishingMaterial>> GetAllMaterialsAsync()
        {
            await InitializeAsync();
            var materials = await _database.Table<FishingMaterial>().ToListAsync();
            return new ObservableCollection<FishingMaterial>(materials);
        }

        public async Task<ObservableCollection<FishingMaterial>> GetMaterialsByTypeAsync(MaterialType type)
        {
            await InitializeAsync();
            var materials = await _database.Table<FishingMaterial>()
                .Where(m => m.Type == type)
                .ToListAsync();
            return new ObservableCollection<FishingMaterial>(materials);
        }

        public async Task<FishingMaterial?> GetMaterialByIdAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<FishingMaterial>()
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddMaterialAsync(FishingMaterial material)
        {
            await InitializeAsync();
            return await _database.InsertAsync(material);
        }

        public async Task<int> UpdateMaterialAsync(FishingMaterial material)
        {
            await InitializeAsync();
            return await _database.UpdateAsync(material);
        }

        public async Task<int> DeleteMaterialAsync(FishingMaterial material)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(material);
        }

        #endregion

        #region FishingSetup Operations

        public async Task<ObservableCollection<FishingSetup>> GetAllSetupsAsync()
        {
            await InitializeAsync();
            var setups = await _database.Table<FishingSetup>()
                .OrderByDescending(s => s.IsActive)
                .ThenByDescending(s => s.CreatedDate)
                .ToListAsync();
            return new ObservableCollection<FishingSetup>(setups);
        }

        public async Task<FishingSetup?> GetSetupByIdAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<FishingSetup>()
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<FishingSetup?> GetActiveSetupAsync()
        {
            await InitializeAsync();
            return await _database.Table<FishingSetup>()
                .Where(s => s.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddSetupAsync(FishingSetup setup)
        {
            await InitializeAsync();
            return await _database.InsertAsync(setup);
        }

        public async Task<int> UpdateSetupAsync(FishingSetup setup)
        {
            await InitializeAsync();
            return await _database.UpdateAsync(setup);
        }

        public async Task<int> DeleteSetupAsync(FishingSetup setup)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(setup);
        }

        public async Task SetActiveSetupAsync(int setupId)
        {
            await InitializeAsync();

            // Désactiver tous les setups
            var allSetups = await _database.Table<FishingSetup>().ToListAsync();
            foreach (var setup in allSetups)
            {
                setup.IsActive = false;
                await _database.UpdateAsync(setup);
            }

            // Activer le setup sélectionné
            var selectedSetup = await GetSetupByIdAsync(setupId);
            if (selectedSetup != null)
            {
                selectedSetup.IsActive = true;
                await _database.UpdateAsync(selectedSetup);
            }
        }

        #endregion

        #region WeatherData Operations

        public async Task<int> AddWeatherDataAsync(WeatherData weatherData)
        {
            await InitializeAsync();
            return await _database.InsertAsync(weatherData);
        }

        public async Task<WeatherData?> GetWeatherDataByCatchIdAsync(int catchId)
        {
            await InitializeAsync();
            return await _database.Table<WeatherData>()
                .Where(w => w.CatchId == catchId)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateWeatherDataAsync(WeatherData weatherData)
        {
            await InitializeAsync();
            return await _database.UpdateAsync(weatherData);
        }

        #endregion
    }
}
