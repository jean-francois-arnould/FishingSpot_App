using FishingSpot.Models;
using System.Collections.ObjectModel;

namespace FishingSpot.Services
{
    public class SetupService
    {
        private readonly SQLiteDatabaseService _sqliteService;
        private readonly MaterialService _materialService;
        private ObservableCollection<FishingSetup> _setups;
        private bool _isLoaded = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public SetupService(MaterialService materialService)
        {
            _sqliteService = new SQLiteDatabaseService();
            _materialService = materialService;
            _setups = new ObservableCollection<FishingSetup>();

            // Initialiser en arrière-plan
            _ = Task.Run(async () => await EnsureLoadedAsync());
        }

        private async Task EnsureLoadedAsync()
        {
            await _initLock.WaitAsync();
            try
            {
                if (!_isLoaded)
                {
                    await _sqliteService.InitializeAsync();
                    var setups = await _sqliteService.GetAllSetupsAsync();

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        _setups.Clear();
                        foreach (var setup in setups)
                        {
                            _setups.Add(setup);
                        }
                    });

                    // Ajouter un setup de test si la base est vide
                    if (_setups.Count == 0)
                    {
                        await AddTestSetupAsync();
                    }

                    _isLoaded = true;
                }
            }
            finally
            {
                _initLock.Release();
            }
        }

        private async Task AddTestSetupAsync()
        {
            var setup = new FishingSetup
            {
                Name = "Setup Carnassiers",
                Description = "Configuration pour la pêche aux carnassiers",
                CanneId = 1, // Spinning Pro 2000
                FilId = 3,   // PowerPro
                BasDeLigneId = 4, // Fluorocarbone
                AppAtId = 5, // Rapala Original
                CreatedDate = DateTime.Now.AddDays(-10),
                IsActive = true,
                Notes = "Setup polyvalent pour brochet et perche"
            };
            await _sqliteService.AddSetupAsync(setup);
            _setups.Add(setup);
        }

        public ObservableCollection<FishingSetup> GetAllSetups()
        {
            if (!_isLoaded)
            {
                _ = Task.Run(async () => await EnsureLoadedAsync());
            }
            return _setups;
        }

        public FishingSetup? GetActiveSetup()
        {
            return _setups.FirstOrDefault(s => s.IsActive);
        }

        public FishingSetup? GetSetupById(int id)
        {
            return _setups.FirstOrDefault(s => s.Id == id);
        }

        public async Task AddSetupAsync(FishingSetup setup)
        {
            await EnsureLoadedAsync();
            await _sqliteService.AddSetupAsync(setup);
            await MainThread.InvokeOnMainThreadAsync(() => _setups.Add(setup));
        }

        public async Task UpdateSetupAsync(FishingSetup setup)
        {
            await EnsureLoadedAsync();
            await _sqliteService.UpdateSetupAsync(setup);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var existing = _setups.FirstOrDefault(s => s.Id == setup.Id);
                if (existing != null)
                {
                    existing.Name = setup.Name;
                    existing.Description = setup.Description;
                    existing.CanneId = setup.CanneId;
                    existing.FilId = setup.FilId;
                    existing.BasDeLigneId = setup.BasDeLigneId;
                    existing.AppAtId = setup.AppAtId;
                    existing.Notes = setup.Notes;
                    existing.IsActive = setup.IsActive;
                }
            });
        }

        public async Task DeleteSetupAsync(FishingSetup setup)
        {
            await EnsureLoadedAsync();
            await _sqliteService.DeleteSetupAsync(setup);
            _setups.Remove(setup);
        }

        public async Task SetActiveSetupAsync(int setupId)
        {
            await EnsureLoadedAsync();
            await _sqliteService.SetActiveSetupAsync(setupId);

            // Mettre à jour la collection locale
            foreach (var setup in _setups)
            {
                setup.IsActive = (setup.Id == setupId);
            }
        }

        public string GetSetupSummary(FishingSetup setup)
        {
            var parts = new List<string>();

            if (setup.CanneId.HasValue)
            {
                var canne = _materialService.GetMaterialById(setup.CanneId.Value);
                if (canne != null) parts.Add($"🎣 {canne.Name}");
            }

            if (setup.FilId.HasValue)
            {
                var fil = _materialService.GetMaterialById(setup.FilId.Value);
                if (fil != null) parts.Add($"🧵 {fil.Name}");
            }

            if (setup.BasDeLigneId.HasValue)
            {
                var bas = _materialService.GetMaterialById(setup.BasDeLigneId.Value);
                if (bas != null) parts.Add($"🔗 {bas.Name}");
            }

            if (setup.AppAtId.HasValue)
            {
                var appat = _materialService.GetMaterialById(setup.AppAtId.Value);
                if (appat != null) parts.Add($"🐟 {appat.Name}");
            }

            return parts.Any() ? string.Join(" | ", parts) : "Setup vide";
        }

        // Méthodes synchrones pour compatibilité (déprécié - utiliser les versions async)
        public void AddSetup(FishingSetup setup)
        {
            _ = Task.Run(async () => await AddSetupAsync(setup));
        }

        public void UpdateSetup(FishingSetup setup)
        {
            _ = Task.Run(async () => await UpdateSetupAsync(setup));
        }

        public void DeleteSetup(FishingSetup setup)
        {
            _ = Task.Run(async () => await DeleteSetupAsync(setup));
        }

        public void SetActiveSetup(int setupId)
        {
            _ = Task.Run(async () => await SetActiveSetupAsync(setupId));
        }
    }
}
