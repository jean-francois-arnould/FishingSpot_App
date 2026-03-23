using FishingSpot.PWA.Models;
using FishingSpot.PWA.Services.Offline;
using FishingSpot.PWA.Services.Offline.Models;

namespace FishingSpot.PWA.Services
{
    /// <summary>
    /// Offline-capable wrapper for SupabaseService
    /// Automatically handles caching and synchronization
    /// </summary>
    public class OfflineSupabaseService : ISupabaseService
    {
        private const string CATCHES_STORE = "catches";
        private const string SPECIES_STORE = "species";
        private const string SETUPS_STORE = "setups";
        private const string BRANDS_STORE = "brands";

        private readonly ISupabaseService _onlineService;
        private readonly INetworkStatusService _networkStatus;
        private readonly IIndexedDbService _indexedDb;
        private readonly ISyncService _syncService;
        private readonly IAuthService _authService;

        public OfflineSupabaseService(
            ISupabaseService onlineService,
            INetworkStatusService networkStatus,
            IIndexedDbService indexedDb,
            ISyncService syncService,
            IAuthService authService)
        {
            _onlineService = onlineService;
            _networkStatus = networkStatus;
            _indexedDb = indexedDb;
            _syncService = syncService;
            _authService = authService;
        }

        public Task InitializeAsync()
        {
            return _onlineService.InitializeAsync();
        }

        // Fish Catches
        public async Task<List<FishCatch>> GetAllCatchesAsync()
        {
            if (_networkStatus.IsOnline && !_authService.IsTokenExpired)
            {
                // Vérifier et rafraîchir le token si nécessaire
                var tokenValid = await _authService.EnsureValidTokenAsync();
                if (!tokenValid)
                {
                    Console.WriteLine("⚠️ Token invalide, utilisation du cache offline");
                    return await _indexedDb.GetAllItemsAsync<FishCatch>(CATCHES_STORE);
                }

                try
                {
                    var catches = await _onlineService.GetAllCatchesAsync();

                    // IMPORTANT: Récupérer les prises offline (ID négatifs) AVANT de nettoyer
                    var cachedCatches = await _indexedDb.GetAllItemsAsync<FishCatch>(CATCHES_STORE);
                    var offlineCatches = cachedCatches.Where(c => c.Id < 0).ToList();

                    if (offlineCatches.Any())
                    {
                        Console.WriteLine($"📦 {offlineCatches.Count} prises offline détectées, préservation...");
                    }

                    // Vider le cache
                    await _indexedDb.ClearStoreAsync(CATCHES_STORE);

                    // Remettre les prises du serveur
                    foreach (var catchItem in catches)
                    {
                        await _indexedDb.SetItemAsync(CATCHES_STORE, catchItem.Id.ToString(), catchItem);
                    }

                    // REMETTRE les prises offline qui n'ont pas encore été synchronisées
                    foreach (var offlineCatch in offlineCatches)
                    {
                        await _indexedDb.SetItemAsync(CATCHES_STORE, offlineCatch.Id.ToString(), offlineCatch);
                        Console.WriteLine($"📦 Prise offline préservée: {offlineCatch.FishName} (ID: {offlineCatch.Id})");
                    }

                    // Retourner la combinaison des deux
                    var allCatches = catches.Concat(offlineCatches).ToList();
                    return allCatches;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error fetching catches online, falling back to cache: {ex.Message}");
                }
            }

            // Offline or error: return cached data
            Console.WriteLine("📦 Loading catches from cache...");
            return await _indexedDb.GetAllItemsAsync<FishCatch>(CATCHES_STORE);
        }

        public async Task<FishCatch?> GetCatchByIdAsync(int id)
        {
            // Si l'ID est négatif, c'est une prise offline, chercher directement dans le cache
            if (id < 0)
            {
                Console.WriteLine($"📦 Loading offline catch {id} from cache...");
                return await _indexedDb.GetItemAsync<FishCatch>(CATCHES_STORE, id.ToString());
            }

            // ID positif : essayer d'abord l'API si online
            if (_networkStatus.IsOnline && !_authService.IsTokenExpired)
            {
                var tokenValid = await _authService.EnsureValidTokenAsync();
                if (tokenValid)
                {
                    try
                    {
                        var catchItem = await _onlineService.GetCatchByIdAsync(id);
                        if (catchItem != null)
                        {
                            await _indexedDb.SetItemAsync(CATCHES_STORE, id.ToString(), catchItem);
                        }
                        return catchItem;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error fetching catch online, falling back to cache: {ex.Message}");
                    }
                }
            }

            // Fallback au cache
            return await _indexedDb.GetItemAsync<FishCatch>(CATCHES_STORE, id.ToString());
        }

        public async Task<int> AddCatchAsync(FishCatch fishCatch)
        {
            Console.WriteLine($"🔍 [AddCatchAsync] ENTRÉE - ID actuel: {fishCatch.Id}");

            // ⚠️ PROTECTION : Si l'ID est déjà positif, c'est une tentative de double sauvegarde
            if (fishCatch.Id > 0)
            {
                Console.WriteLine($"❌❌❌ DOUBLE SAUVEGARDE DÉTECTÉE! ID déjà attribué: {fishCatch.Id}");
                Console.WriteLine($"❌ Cette prise a déjà été sauvegardée. Retour immédiat.");
                return fishCatch.Id;
            }

            // Generate temporary ID for offline
            if (fishCatch.Id == 0)
            {
                fishCatch.Id = -new Random().Next(1, 1000000); // Negative ID for offline items
                Console.WriteLine($"🆔 ID temporaire généré: {fishCatch.Id}");
            }

            // Save to cache immediately
            Console.WriteLine($"💾 Sauvegarde dans IndexedDB avec ID: {fishCatch.Id}");
            await _indexedDb.SetItemAsync(CATCHES_STORE, fishCatch.Id.ToString(), fishCatch);

            if (_networkStatus.IsOnline && !_authService.IsTokenExpired)
            {
                // Vérifier et rafraîchir le token si nécessaire
                var tokenValid = await _authService.EnsureValidTokenAsync();
                if (!tokenValid)
                {
                    Console.WriteLine("⚠️ Token invalide, mode offline activé");
                    await _syncService.QueueActionAsync(SyncAction.Create, "catch", fishCatch.Id.ToString(), fishCatch);
                    return fishCatch.Id;
                }

                try
                {
                    Console.WriteLine("🌐 Tentative de sauvegarde en ligne...");
                    var newId = await _onlineService.AddCatchAsync(fishCatch);

                    if (newId == 0)
                    {
                        Console.WriteLine("❌ Le serveur a retourné ID = 0");
                        throw new Exception("Le serveur n'a pas retourné un ID valide");
                    }

                    Console.WriteLine($"✅ Prise enregistrée en ligne avec ID: {newId}");

                    // Update with real ID
                    if (fishCatch.Id < 0)
                    {
                        Console.WriteLine($"🗑️ Suppression de l'ancien ID temporaire: {fishCatch.Id}");
                        await _indexedDb.DeleteItemAsync(CATCHES_STORE, fishCatch.Id.ToString());
                    }

                    Console.WriteLine($"🔄 Mise à jour avec le nouvel ID: {newId}");
                    fishCatch.Id = newId;
                    await _indexedDb.SetItemAsync(CATCHES_STORE, newId.ToString(), fishCatch);

                    Console.WriteLine($"🔍 [AddCatchAsync] SORTIE - ID final: {fishCatch.Id}");
                    return newId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error adding catch online: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    // NE PAS avaler l'exception - la propager à l'UI
                    throw;
                }
            }

            // Mode offline ou pas de connexion
            Console.WriteLine($"📋 Mode offline: Prise mise en queue pour synchronisation");
            await _syncService.QueueActionAsync(SyncAction.Create, "catch", fishCatch.Id.ToString(), fishCatch);

            Console.WriteLine($"🔍 [AddCatchAsync] SORTIE - ID offline: {fishCatch.Id}");
            return fishCatch.Id;
        }

        public async Task<bool> UpdateCatchAsync(FishCatch fishCatch)
        {
            // Update cache immediately
            await _indexedDb.SetItemAsync(CATCHES_STORE, fishCatch.Id.ToString(), fishCatch);

            if (_networkStatus.IsOnline)
            {
                try
                {
                    return await _onlineService.UpdateCatchAsync(fishCatch);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error updating catch online, queued for sync: {ex.Message}");
                }
            }

            // Queue for synchronization
            await _syncService.QueueActionAsync(SyncAction.Update, "catch", fishCatch.Id.ToString(), fishCatch);
            Console.WriteLine($"📋 Catch update queued for sync (offline mode)");

            return true;
        }

        public async Task<bool> DeleteCatchAsync(int id)
        {
            // Remove from cache immediately
            await _indexedDb.DeleteItemAsync(CATCHES_STORE, id.ToString());

            if (_networkStatus.IsOnline)
            {
                try
                {
                    return await _onlineService.DeleteCatchAsync(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error deleting catch online, queued for sync: {ex.Message}");
                }
            }

            // Queue for synchronization
            await _syncService.QueueActionAsync(SyncAction.Delete, "catch", id.ToString(), new { id });
            Console.WriteLine($"📋 Catch deletion queued for sync (offline mode)");

            return true;
        }

        // Fish Species
        public async Task<List<FishSpecies>> GetAllFishSpeciesAsync()
        {
            if (_networkStatus.IsOnline)
            {
                try
                {
                    var species = await _onlineService.GetAllFishSpeciesAsync();

                    await _indexedDb.ClearStoreAsync(SPECIES_STORE);
                    foreach (var speciesItem in species)
                    {
                        await _indexedDb.SetItemAsync(SPECIES_STORE, speciesItem.Id.ToString(), speciesItem);
                    }

                    return species;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error fetching species online, falling back to cache: {ex.Message}");
                }
            }

            Console.WriteLine("📦 Loading species from cache...");
            return await _indexedDb.GetAllItemsAsync<FishSpecies>(SPECIES_STORE);
        }

        public async Task<int> AddFishSpeciesAsync(FishSpecies fishSpecies)
        {
            if (fishSpecies.Id == 0)
            {
                fishSpecies.Id = -new Random().Next(1, 1000000);
            }

            await _indexedDb.SetItemAsync(SPECIES_STORE, fishSpecies.Id.ToString(), fishSpecies);

            if (_networkStatus.IsOnline)
            {
                try
                {
                    var newId = await _onlineService.AddFishSpeciesAsync(fishSpecies);

                    if (fishSpecies.Id < 0)
                    {
                        await _indexedDb.DeleteItemAsync(SPECIES_STORE, fishSpecies.Id.ToString());
                    }
                    fishSpecies.Id = newId;
                    await _indexedDb.SetItemAsync(SPECIES_STORE, newId.ToString(), fishSpecies);

                    return newId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error adding species online, queued for sync: {ex.Message}");
                }
            }

            await _syncService.QueueActionAsync(SyncAction.Create, "species", fishSpecies.Id.ToString(), fishSpecies);
            return fishSpecies.Id;
        }

        // Fishing Brands
        public async Task<List<FishingBrand>> GetBrandsByCategoryAsync(string category)
        {
            if (_networkStatus.IsOnline)
            {
                try
                {
                    var brands = await _onlineService.GetBrandsByCategoryAsync(category);

                    foreach (var brand in brands)
                    {
                        await _indexedDb.SetItemAsync(BRANDS_STORE, $"{category}_{brand.Id}", brand);
                    }

                    return brands;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error fetching brands online, falling back to cache: {ex.Message}");
                }
            }

            var allBrands = await _indexedDb.GetAllItemsAsync<FishingBrand>(BRANDS_STORE);
            return allBrands.Where(b => b.Category == category).ToList();
        }

        public async Task<int> AddFishingBrandAsync(FishingBrand brand)
        {
            if (brand.Id == 0)
            {
                brand.Id = -new Random().Next(1, 1000000);
            }

            await _indexedDb.SetItemAsync(BRANDS_STORE, $"{brand.Category}_{brand.Id}", brand);

            if (_networkStatus.IsOnline)
            {
                try
                {
                    var newId = await _onlineService.AddFishingBrandAsync(brand);

                    if (brand.Id < 0)
                    {
                        await _indexedDb.DeleteItemAsync(BRANDS_STORE, $"{brand.Category}_{brand.Id}");
                    }
                    brand.Id = newId;
                    await _indexedDb.SetItemAsync(BRANDS_STORE, $"{brand.Category}_{newId}", brand);

                    return newId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error adding brand online, queued for sync: {ex.Message}");
                }
            }

            await _syncService.QueueActionAsync(SyncAction.Create, "brand", brand.Id.ToString(), brand);
            return brand.Id;
        }

        // Fishing Setups
        public async Task<List<FishingSetup>> GetAllSetupsAsync()
        {
            if (_networkStatus.IsOnline)
            {
                try
                {
                    var setups = await _onlineService.GetAllSetupsAsync();

                    await _indexedDb.ClearStoreAsync(SETUPS_STORE);
                    foreach (var setup in setups)
                    {
                        await _indexedDb.SetItemAsync(SETUPS_STORE, setup.Id.ToString(), setup);
                    }

                    return setups;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error fetching setups online, falling back to cache: {ex.Message}");
                }
            }

            Console.WriteLine("📦 Loading setups from cache...");
            return await _indexedDb.GetAllItemsAsync<FishingSetup>(SETUPS_STORE);
        }

        public async Task<FishingSetup?> GetSetupByIdAsync(int id)
        {
            if (_networkStatus.IsOnline)
            {
                try
                {
                    var setup = await _onlineService.GetSetupByIdAsync(id);
                    if (setup != null)
                    {
                        await _indexedDb.SetItemAsync(SETUPS_STORE, id.ToString(), setup);
                    }
                    return setup;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error fetching setup online, falling back to cache: {ex.Message}");
                }
            }

            return await _indexedDb.GetItemAsync<FishingSetup>(SETUPS_STORE, id.ToString());
        }

        public async Task<FishingSetup?> GetCurrentSetupAsync()
        {
            if (_networkStatus.IsOnline)
            {
                try
                {
                    return await _onlineService.GetCurrentSetupAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error fetching current setup online, falling back to cache: {ex.Message}");
                }
            }

            var allSetups = await _indexedDb.GetAllItemsAsync<FishingSetup>(SETUPS_STORE);
            return allSetups.FirstOrDefault(s => s.IsCurrent);
        }

        public async Task<int> AddSetupAsync(FishingSetup setup)
        {
            if (setup.Id == 0)
            {
                setup.Id = -new Random().Next(1, 1000000);
            }

            await _indexedDb.SetItemAsync(SETUPS_STORE, setup.Id.ToString(), setup);

            if (_networkStatus.IsOnline)
            {
                try
                {
                    var newId = await _onlineService.AddSetupAsync(setup);

                    if (setup.Id < 0)
                    {
                        await _indexedDb.DeleteItemAsync(SETUPS_STORE, setup.Id.ToString());
                    }
                    setup.Id = newId;
                    await _indexedDb.SetItemAsync(SETUPS_STORE, newId.ToString(), setup);

                    return newId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error adding setup online, queued for sync: {ex.Message}");
                }
            }

            await _syncService.QueueActionAsync(SyncAction.Create, "setup", setup.Id.ToString(), setup);
            return setup.Id;
        }

        public async Task<bool> UpdateSetupAsync(FishingSetup setup)
        {
            await _indexedDb.SetItemAsync(SETUPS_STORE, setup.Id.ToString(), setup);

            if (_networkStatus.IsOnline)
            {
                try
                {
                    return await _onlineService.UpdateSetupAsync(setup);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error updating setup online, queued for sync: {ex.Message}");
                }
            }

            await _syncService.QueueActionAsync(SyncAction.Update, "setup", setup.Id.ToString(), setup);
            return true;
        }

        public async Task<bool> DeleteSetupAsync(int id)
        {
            await _indexedDb.DeleteItemAsync(SETUPS_STORE, id.ToString());

            if (_networkStatus.IsOnline)
            {
                try
                {
                    return await _onlineService.DeleteSetupAsync(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error deleting setup online, queued for sync: {ex.Message}");
                }
            }

            await _syncService.QueueActionAsync(SyncAction.Delete, "setup", id.ToString(), new { id });
            return true;
        }

        public async Task<bool> SetCurrentSetupAsync(int setupId)
        {
            // Update cache
            var allSetups = await _indexedDb.GetAllItemsAsync<FishingSetup>(SETUPS_STORE);
            foreach (var setup in allSetups)
            {
                setup.IsCurrent = (setup.Id == setupId);
                await _indexedDb.SetItemAsync(SETUPS_STORE, setup.Id.ToString(), setup);
            }

            if (_networkStatus.IsOnline)
            {
                try
                {
                    return await _onlineService.SetCurrentSetupAsync(setupId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error setting current setup online, queued for sync: {ex.Message}");
                }
            }

            await _syncService.QueueActionAsync(SyncAction.Update, "setup_current", setupId.ToString(), new { setupId });
            return true;
        }

        // Photo Upload (requires online)
        public async Task<string?> UploadPhotoAsync(Stream photoStream, string fileName)
        {
            if (!_networkStatus.IsOnline)
            {
                Console.WriteLine("⚠️ Cannot upload photo while offline. Photo will be stored locally.");
                // TODO: Implement local photo storage with File System Access API or IndexedDB
                return $"offline_{fileName}"; // Return placeholder
            }

            try
            {
                return await _onlineService.UploadPhotoAsync(photoStream, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error uploading photo: {ex.Message}");
                return null;
            }
        }
    }
}
