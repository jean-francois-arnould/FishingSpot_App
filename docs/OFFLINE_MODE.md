# 🌐 Mode Hors Ligne - FishingSpot PWA

## 📋 Vue d'ensemble

Cette application Blazor WebAssembly dispose d'un **mode hors ligne complet** permettant aux utilisateurs de continuer à utiliser l'application sans connexion Internet.

## ✨ Fonctionnalités

### 1. **Détection automatique de la connexion**
- Détection en temps réel du statut réseau (online/offline)
- Indicateur visuel en haut de l'écran
- Basculement automatique entre les modes

### 2. **Stockage local (IndexedDB)**
- Cache de toutes les données (prises, matériel, montages, etc.)
- Persistance des données entre les sessions
- Accès rapide aux données en mode offline

### 3. **Queue de synchronisation**
- Toutes les actions effectuées hors ligne sont mises en queue
- Synchronisation automatique dès le retour de la connexion
- Gestion des erreurs et des réessais automatiques
- Affichage du nombre d'éléments en attente

### 4. **Gestion des conflits**
- IDs temporaires pour les éléments créés offline (négatifs)
- Remplacement par les vrais IDs lors de la sync
- Préservation de l'intégrité des données

## 🏗️ Architecture

```
┌─────────────────────────────────────────────┐
│           Composants Blazor                 │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│    Offline Wrappers (OfflineXxxService)     │
│  - OfflineSupabaseService                   │
│  - OfflineEquipmentService                  │
└───────┬─────────────────┬───────────────────┘
        │                 │
        │                 │
    ┌───▼────┐       ┌────▼──────┐
    │ Online │       │  Offline  │
    │Services│       │  Services │
    └────────┘       └─────┬─────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
┌───────▼────┐   ┌─────────▼──────┐  ┌───────▼──────┐
│ Network    │   │   IndexedDB    │  │     Sync     │
│   Status   │   │    Service     │  │   Service    │
└────────────┘   └────────────────┘  └──────────────┘
```

## 📦 Structure des fichiers

### Services Offline
- **`Services/Offline/INetworkStatusService.cs`** - Interface de détection réseau
- **`Services/Offline/NetworkStatusService.cs`** - Implémentation de la détection
- **`Services/Offline/IIndexedDbService.cs`** - Interface IndexedDB
- **`Services/Offline/IndexedDbService.cs`** - Gestion du cache local
- **`Services/Offline/ISyncService.cs`** - Interface de synchronisation
- **`Services/Offline/SyncService.cs`** - Queue et synchronisation
- **`Services/Offline/Models/SyncQueueItem.cs`** - Modèle de queue

### Wrappers Offline
- **`Services/Offline/OfflineSupabaseService.cs`** - Wrapper pour les prises et données
- **`Services/Offline/OfflineEquipmentService.cs`** - Wrapper pour le matériel

### UI
- **`Components/NetworkStatusIndicator.razor`** - Indicateur de connexion
- **`wwwroot/js/offline-support.js`** - Code JavaScript pour IndexedDB et détection réseau

## 🚀 Utilisation

### Pour l'utilisateur

1. **Mode Online (normal)**
   - L'app fonctionne normalement avec Supabase
   - Les données sont automatiquement mises en cache

2. **Passage en Offline**
   - Un bandeau rouge s'affiche : "Mode hors ligne"
   - Toutes les fonctionnalités restent disponibles
   - Les données sont lues depuis le cache
   - Les modifications sont stockées localement

3. **Retour en Online**
   - Un bandeau bleu s'affiche : "Synchronisation..."
   - Les données sont automatiquement synchronisées
   - Le bandeau disparaît une fois la sync terminée

### Pour le développeur

#### Ajouter le support offline à un nouveau service

```csharp
// 1. Créer le service online normal
public class MyService : IMyService
{
    public async Task<List<MyData>> GetDataAsync()
    {
        // Appel API
    }
}

// 2. Créer le wrapper offline
public class OfflineMyService : IMyService
{
    private const string STORE_NAME = "mydata";

    private readonly IMyService _onlineService;
    private readonly INetworkStatusService _networkStatus;
    private readonly IIndexedDbService _indexedDb;
    private readonly ISyncService _syncService;

    public async Task<List<MyData>> GetDataAsync()
    {
        if (_networkStatus.IsOnline)
        {
            try
            {
                var data = await _onlineService.GetDataAsync();
                // Cache les données
                await CacheDataAsync(data);
                return data;
            }
            catch
            {
                // Fallback au cache
            }
        }

        // Mode offline : lire le cache
        return await _indexedDb.GetAllItemsAsync<MyData>(STORE_NAME);
    }
}

// 3. Enregistrer dans Program.cs
builder.Services.AddScoped<MyService>(...);
builder.Services.AddScoped<IMyService, OfflineMyService>();
```

## 📊 Stores IndexedDB

L'application utilise les stores suivants :

| Store | Contenu |
|-------|---------|
| `catches` | Prises de poisson |
| `species` | Espèces de poissons |
| `setups` | Montages de pêche |
| `rods` | Cannes |
| `reels` | Moulinets |
| `lines` | Fils |
| `lures` | Leurres |
| `leaders` | Bas de ligne |
| `hooks` | Hameçons |
| `brands` | Marques |
| `syncQueue` | Queue de synchronisation |
| `userProfile` | Profil utilisateur |

## 🔧 Configuration

### Durée de cache
Par défaut, les données restent en cache indéfiniment. Pour ajouter une expiration :

```javascript
// Dans offline-support.js
setItem: function(storeName, key, jsonValue) {
    store.put({
        key: key,
        value: jsonValue,
        timestamp: Date.now(),
        expiresAt: Date.now() + (24 * 60 * 60 * 1000) // 24h
    });
}
```

### Nombre de réessais
Par défaut, 3 tentatives avant échec. Modifiable dans `SyncService.cs` :

```csharp
private const int MAX_RETRY_COUNT = 3; // Changez ici
```

## 🐛 Debugging

### Voir les données en cache
Ouvrez la console du navigateur :
```javascript
// Voir toutes les catches en cache
indexedDb.getAllItems('catches').then(console.log);

// Voir la queue de sync
indexedDb.getAllItems('syncQueue').then(console.log);
```

### Forcer une synchronisation
```csharp
@inject ISyncService SyncService

await SyncService.SyncAllAsync();
```

### Vider le cache
```csharp
@inject IIndexedDbService IndexedDb

await IndexedDb.ClearStoreAsync("catches");
```

## ⚠️ Limitations actuelles

1. **Upload de photos** : Les photos ne peuvent pas être uploadées en mode offline (nécessite connexion)
   - Solution future : Stocker les photos en base64 dans IndexedDB

2. **Conflits de modification** : Si deux appareils modifient les mêmes données offline, la dernière synchronisation écrase
   - Solution future : Détection et résolution de conflits

3. **Taille du cache** : IndexedDB a une limite de ~50MB par origine
   - Solution : Surveillance et nettoyage automatique

## 🔮 Améliorations futures

- [ ] Support des photos offline (base64 dans IndexedDB)
- [ ] Détection et résolution de conflits
- [ ] Nettoyage automatique du cache ancien
- [ ] Synchronisation en arrière-plan (Background Sync API)
- [ ] Mode "lecture seule" offline pour économiser l'espace
- [ ] Statistiques de synchronisation
- [ ] Export/import manuel des données

## 📚 Ressources

- [IndexedDB API](https://developer.mozilla.org/fr/docs/Web/API/IndexedDB_API)
- [Service Workers](https://developer.mozilla.org/fr/docs/Web/API/Service_Worker_API)
- [PWA Offline Patterns](https://web.dev/offline-cookbook/)
- [Blazor WebAssembly](https://learn.microsoft.com/fr-fr/aspnet/core/blazor/hosting-models)

---

**Note**: Ce système est conçu pour être transparent pour l'utilisateur. L'application fonctionne identiquement en mode online et offline, avec synchronisation automatique.
