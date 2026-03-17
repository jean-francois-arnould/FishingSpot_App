using FishingSpot.Models;
using System.Collections.ObjectModel;

namespace FishingSpot.Services
{
    public class DatabaseService
    {
        private readonly SQLiteDatabaseService _sqliteService;
        private ObservableCollection<FishCatch> _catches;
        private readonly List<Fish> _fishSpecies;
        private bool _isLoaded = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public DatabaseService()
        {
            _sqliteService = new SQLiteDatabaseService();
            _catches = new ObservableCollection<FishCatch>();
            _fishSpecies = InitializeFishSpecies();

            // Initialiser en arrière-plan sans bloquer
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
                    var catches = await _sqliteService.GetAllCatchesAsync();

                    // Mettre à jour la collection sur le thread UI
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        _catches.Clear();
                        foreach (var fish in catches)
                        {
                            _catches.Add(fish);
                        }
                    });

                    // Ajouter des poissons de test si la base est vide
                    if (_catches.Count == 0)
                    {
                        await AddTestFishAsync();
                    }

                    _isLoaded = true;
                }
            }
            finally
            {
                _initLock.Release();
            }
        }

        private async Task AddTestFishAsync()
        {
            // Poisson de test 1 : Brochet
            var brochet = new FishCatch
            {
                FishName = "Brochet",
                Length = 85.0,
                Weight = 4.5,
                LocationName = "Lac de Sainte-Croix",
                Latitude = 43.7711,
                Longitude = 6.2048,
                CatchDate = DateTime.Now.AddDays(-2),
                CatchTime = new TimeSpan(14, 30, 0),
                Notes = "Belle prise ! Temps ensoleille, appat: leurre artificiel",
                PhotoPath = ""
            };
            await _sqliteService.AddCatchAsync(brochet);
            _catches.Add(brochet);

            // Poisson de test 2 : Truite
            var truite = new FishCatch
            {
                FishName = "Truite Arc-en-ciel",
                Length = 45.0,
                Weight = 1.8,
                LocationName = "Riviere du Verdon",
                Latitude = 43.8352,
                Longitude = 6.5644,
                CatchDate = DateTime.Now.AddDays(-5),
                CatchTime = new TimeSpan(9, 15, 0),
                Notes = "Peche a la mouche, tres combative !",
                PhotoPath = ""
            };
            await _sqliteService.AddCatchAsync(truite);
            _catches.Add(truite);

            // Poisson de test 3 : Carpe
            var carpe = new FishCatch
            {
                FishName = "Carpe Commune",
                Length = 72.0,
                Weight = 8.2,
                LocationName = "Etang de la Bonde",
                Latitude = 43.7234,
                Longitude = 5.4321,
                CatchDate = DateTime.Now.AddDays(-1),
                CatchTime = new TimeSpan(18, 45, 0),
                Notes = "Record personnel ! Bouillette fraise",
                PhotoPath = ""
            };
            await _sqliteService.AddCatchAsync(carpe);
            _catches.Add(carpe);
        }

        private List<Fish> InitializeFishSpecies()
        {
            return new List<Fish>
            {
                new Fish
                {
                    Id = 1,
                    Name = "Truite Arc-en-ciel",
                    ScientificName = "Oncorhynchus mykiss",
                    Description = "Poisson d'eau douce très prisé des pêcheurs sportifs. Reconnaissable à sa bande latérale rose iridescente.",
                    Habitat = "Rivières et lacs d'eau froide et bien oxygénée",
                    BestBait = "Vers, leurres artificiels, mouches",
                    AverageSize = 30,
                    MaxSize = 70
                },
                new Fish
                {
                    Id = 2,
                    Name = "Brochet",
                    ScientificName = "Esox lucius",
                    Description = "Prédateur féroce des eaux douces, reconnaissable à son corps allongé et sa gueule remplie de dents.",
                    Habitat = "Lacs, étangs et rivières à courant lent",
                    BestBait = "Poissons-nageurs, cuillers, leurres souples",
                    AverageSize = 60,
                    MaxSize = 130
                },
                new Fish
                {
                    Id = 3,
                    Name = "Sandre",
                    ScientificName = "Sander lucioperca",
                    Description = "Carnassier apprécié pour sa chair délicate. Actif principalement au crépuscule.",
                    Habitat = "Grands lacs et rivières",
                    BestBait = "Leurres souples, poissons morts maniés",
                    AverageSize = 50,
                    MaxSize = 100
                },
                new Fish
                {
                    Id = 4,
                    Name = "Carpe Commune",
                    ScientificName = "Cyprinus carpio",
                    Description = "Poisson cyprinidé pouvant atteindre une taille impressionnante. Très populaire en pêche sportive.",
                    Habitat = "Étangs, lacs et rivières calmes",
                    BestBait = "Bouillettes, maïs, pellets",
                    AverageSize = 45,
                    MaxSize = 100
                },
                new Fish
                {
                    Id = 5,
                    Name = "Perche",
                    ScientificName = "Perca fluviatilis",
                    Description = "Petit carnassier rayé de bandes verticales sombres, facilement reconnaissable.",
                    Habitat = "Lacs, étangs et rivières",
                    BestBait = "Vers, petits leurres, vifs",
                    AverageSize = 20,
                    MaxSize = 50
                },
                new Fish
                {
                    Id = 6,
                    Name = "Silure",
                    ScientificName = "Silurus glanis",
                    Description = "Le plus grand poisson d'eau douce d'Europe. Peut atteindre des tailles gigantesques.",
                    Habitat = "Grands fleuves et lacs profonds",
                    BestBait = "Gros leurres, poissons morts, bouillettes",
                    AverageSize = 150,
                    MaxSize = 250
                },
                new Fish
                {
                    Id = 7,
                    Name = "Black Bass",
                    ScientificName = "Micropterus salmoides",
                    Description = "Carnassier américain très combatif, prisé en pêche sportive aux leurres.",
                    Habitat = "Lacs et étangs avec végétation",
                    BestBait = "Leurres de surface, spinnerbaits, crankbaits",
                    AverageSize = 35,
                    MaxSize = 60
                }
            };
        }

        public List<Fish> GetAllFishSpecies() => _fishSpecies;

        public ObservableCollection<FishCatch> GetAllCatches()
        {
            // Déclencher le chargement si pas encore fait (non bloquant)
            if (!_isLoaded)
            {
                _ = Task.Run(async () => await EnsureLoadedAsync());
            }
            return _catches;
        }

        public async Task AddCatchAsync(FishCatch fishCatch)
        {
            await EnsureLoadedAsync();
            await _sqliteService.AddCatchAsync(fishCatch);
            await MainThread.InvokeOnMainThreadAsync(() => _catches.Add(fishCatch));
        }

        public async Task DeleteCatchAsync(FishCatch fishCatch)
        {
            await EnsureLoadedAsync();
            await _sqliteService.DeleteCatchAsync(fishCatch);
            await MainThread.InvokeOnMainThreadAsync(() => _catches.Remove(fishCatch));
        }

        public async Task UpdateCatchAsync(FishCatch fishCatch)
        {
            await EnsureLoadedAsync();
            await _sqliteService.UpdateCatchAsync(fishCatch);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var existingCatch = _catches.FirstOrDefault(c => c.Id == fishCatch.Id);
                if (existingCatch != null)
                {
                    var index = _catches.IndexOf(existingCatch);
                    _catches[index] = fishCatch;
                }
            });
        }

        public FishCatch? GetCatchById(int id)
        {
            return _catches.FirstOrDefault(c => c.Id == id);
        }

        // Méthodes synchrones pour compatibilité (déprécié - utiliser les versions async)
        public int AddCatch(FishCatch fishCatch)
        {
            Task.Run(async () => await AddCatchAsync(fishCatch)).Wait();
            return fishCatch.Id;
        }

        public void DeleteCatch(FishCatch fishCatch)
        {
            _ = Task.Run(async () => await DeleteCatchAsync(fishCatch));
        }

        public void UpdateCatch(FishCatch fishCatch)
        {
            _ = Task.Run(async () => await UpdateCatchAsync(fishCatch));
        }
    }
}
