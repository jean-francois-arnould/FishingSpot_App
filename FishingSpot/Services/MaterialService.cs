using FishingSpot.Models;
using System.Collections.ObjectModel;

namespace FishingSpot.Services
{
    public class MaterialService
    {
        private readonly SQLiteDatabaseService _sqliteService;
        private ObservableCollection<FishingMaterial> _materials;
        private bool _isLoaded = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public MaterialService()
        {
            _sqliteService = new SQLiteDatabaseService();
            _materials = new ObservableCollection<FishingMaterial>();

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
                    var materials = await _sqliteService.GetAllMaterialsAsync();

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        _materials.Clear();
                        foreach (var mat in materials)
                        {
                            _materials.Add(mat);
                        }
                    });

                    // Ajouter du matériel de test si la base est vide
                    if (_materials.Count == 0)
                    {
                        await AddTestMaterialsAsync();
                    }

                    _isLoaded = true;
                }
            }
            finally
            {
                _initLock.Release();
            }
        }

        private async Task AddTestMaterialsAsync()
        {
            // Cannes
            await AddMaterialAsync(new FishingMaterial
            {
                Type = MaterialType.Canne,
                Name = "Spinning Pro 2000",
                Brand = "Shimano",
                Length = "2.40 m",
                Description = "Canne spinning légère",
                PurchaseDate = DateTime.Now.AddMonths(-6)
            });

            await AddMaterialAsync(new FishingMaterial
            {
                Type = MaterialType.Canne,
                Name = "Carpe Master",
                Brand = "Daiwa",
                Length = "3.60 m",
                Description = "Canne pour carpe",
                PurchaseDate = DateTime.Now.AddYears(-1)
            });

            // Fils
            await AddMaterialAsync(new FishingMaterial
            {
                Type = MaterialType.Fil,
                Name = "PowerPro",
                Brand = "Shimano",
                Strength = "0.15 mm - 10 kg",
                Description = "Tresse 4 brins",
                PurchaseDate = DateTime.Now.AddMonths(-3)
            });

            // Bas de ligne
            await AddMaterialAsync(new FishingMaterial
            {
                Type = MaterialType.BasDeLigne,
                Name = "Fluorocarbone",
                Brand = "Seaguar",
                Strength = "0.30 mm - 6 kg",
                Description = "Bas de ligne invisible",
                PurchaseDate = DateTime.Now.AddMonths(-2)
            });

            // Leurres
            await AddMaterialAsync(new FishingMaterial
            {
                Type = MaterialType.AppAtOuLeurre,
                Name = "Rapala Original",
                Brand = "Rapala",
                Color = "Perche",
                Description = "Poisson nageur",
                PurchaseDate = DateTime.Now.AddMonths(-4)
            });
        }

        public ObservableCollection<FishingMaterial> GetAllMaterials()
        {
            if (!_isLoaded)
            {
                _ = Task.Run(async () => await EnsureLoadedAsync());
            }
            return _materials;
        }

        public ObservableCollection<FishingMaterial> GetMaterialsByType(MaterialType type)
        {
            if (!_isLoaded)
            {
                _ = Task.Run(async () => await EnsureLoadedAsync());
            }
            var filtered = new ObservableCollection<FishingMaterial>(
                _materials.Where(m => m.Type == type).ToList()
            );
            return filtered;
        }

        public async Task AddMaterialAsync(FishingMaterial material)
        {
            await EnsureLoadedAsync();
            await _sqliteService.AddMaterialAsync(material);
            await MainThread.InvokeOnMainThreadAsync(() => _materials.Add(material));
        }

        public async Task UpdateMaterialAsync(FishingMaterial material)
        {
            await EnsureLoadedAsync();
            await _sqliteService.UpdateMaterialAsync(material);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var existing = _materials.FirstOrDefault(m => m.Id == material.Id);
                if (existing != null)
                {
                    var index = _materials.IndexOf(existing);
                    _materials[index] = material;
                }
            });
        }

        public async Task DeleteMaterialAsync(FishingMaterial material)
        {
            await EnsureLoadedAsync();
            await _sqliteService.DeleteMaterialAsync(material);
            await MainThread.InvokeOnMainThreadAsync(() => _materials.Remove(material));
        }

        public FishingMaterial? GetMaterialById(int id)
        {
            return _materials.FirstOrDefault(m => m.Id == id);
        }

        // Méthodes synchrones pour compatibilité (déprécié - utiliser les versions async)
        public void AddMaterial(FishingMaterial material)
        {
            _ = Task.Run(async () => await AddMaterialAsync(material));
        }

        public void UpdateMaterial(FishingMaterial material)
        {
            _ = Task.Run(async () => await UpdateMaterialAsync(material));
        }

        public void DeleteMaterial(FishingMaterial material)
        {
            _ = Task.Run(async () => await DeleteMaterialAsync(material));
        }
    }
}
