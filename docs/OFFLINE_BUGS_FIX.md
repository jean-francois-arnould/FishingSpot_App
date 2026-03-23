# 🐛 Fix : Écran noir et perte de données offline

## Problèmes identifiés

### 1. **Écran noir après sauvegarde offline** 🖥️❌
**Symptôme** :
- En mode offline, après avoir sauvegardé une prise, l'utilisateur tombe sur un écran noir
- Safari affiche : "Safari ne peut pas ouvrir la page car votre iPhone n'est pas connecté à Internet"

**Cause** :
```csharp
Navigation.NavigateTo("/FishingSpot_App/catches", forceLoad: true);
```
- `forceLoad: true` force un rechargement complet depuis le serveur
- En mode offline, le serveur n'est pas accessible → écran noir

**Solution** ✅ :
```csharp
Navigation.NavigateTo("/FishingSpot_App/catches");  // Sans forceLoad
```
- Navigation SPA normale, pas de rechargement serveur
- Fonctionne offline car tout est dans le cache du navigateur

---

### 2. **Perte des prises offline lors de la synchronisation** 📦❌

**Symptôme** :
- En mode offline, l'utilisateur crée une prise → elle apparaît dans "Mes Prises" ✅
- Il repasse en ligne → synchronisation automatique
- La prise créée offline **disparaît** ❌

**Cause** :
```csharp
// Dans GetAllCatchesAsync()
await _indexedDb.ClearStoreAsync(CATCHES_STORE);  // ❌ EFFACE TOUT !
foreach (var catchItem in catches) {
    await _indexedDb.SetItemAsync(CATCHES_STORE, catchItem.Id.ToString(), catchItem);
}
```
Le `ClearStoreAsync()` efface **toutes** les prises, y compris celles avec des ID négatifs (offline).

**Solution** ✅ :
```csharp
// 1. Récupérer les prises offline AVANT de nettoyer
var cachedCatches = await _indexedDb.GetAllItemsAsync<FishCatch>(CATCHES_STORE);
var offlineCatches = cachedCatches.Where(c => c.Id < 0).ToList();

// 2. Nettoyer le cache
await _indexedDb.ClearStoreAsync(CATCHES_STORE);

// 3. Remettre les prises du serveur
foreach (var catchItem in catches) {
    await _indexedDb.SetItemAsync(CATCHES_STORE, catchItem.Id.ToString(), catchItem);
}

// 4. REMETTRE les prises offline
foreach (var offlineCatch in offlineCatches) {
    await _indexedDb.SetItemAsync(CATCHES_STORE, offlineCatch.Id.ToString(), offlineCatch);
}

// 5. Retourner la combinaison des deux
return catches.Concat(offlineCatches).ToList();
```

---

### 3. **Prises offline non synchronisées automatiquement** 🔄❌

**Symptôme** :
- Les prises avec ID négatifs restent dans le cache
- Elles ne sont jamais envoyées au serveur
- L'utilisateur doit manuellement aller sur `/sync-status` pour les synchroniser

**Cause** :
- La queue de synchronisation était vide pour les prises créées offline
- Le `SyncService` ne détectait pas les prises avec ID négatifs

**Solution** ✅ :
```csharp
// Nouvelle méthode dans SyncService
public async Task SyncOfflineCatchesAsync()
{
    var cachedCatches = await _indexedDb.GetAllItemsAsync<FishCatch>(CATCHES_STORE);
    var offlineCatches = cachedCatches.Where(c => c.Id < 0).ToList();

    foreach (var offlineCatch in offlineCatches)
    {
        await QueueActionAsync(SyncAction.Create, "catch", offlineCatch.Id.ToString(), offlineCatch);
    }
}

// Appelée automatiquement au retour en ligne
_networkStatus.OnlineStatusChanged += async (sender, isOnline) =>
{
    if (isOnline)
    {
        await SyncOfflineCatchesAsync();  // ✅ Sync auto
        await SyncAllAsync();
    }
};
```

---

## Flux corrigé

### **Scénario offline → online**

#### Avant ❌
```
1. User crée prise offline
   → Sauvegardé avec ID négatif (-12345)
   → Navigation avec forceLoad → ❌ Écran noir

2. User clique sur "Prises"
   → Voit sa prise ✅

3. User repasse online
   → GetAllCatchesAsync()
   → ClearStoreAsync() → ❌ Prise effacée
   → Télécharge prises du serveur (ne contient pas -12345)
   → ❌ Prise perdue définitivement
```

#### Après ✅
```
1. User crée prise offline
   → Sauvegardé avec ID négatif (-12345)
   → Navigation SANS forceLoad → ✅ Affichage normal

2. User voit sa prise dans "Mes Prises" ✅

3. User repasse online
   → NetworkStatus détecte : Online !
   → SyncOfflineCatchesAsync()
     → Détecte prise ID -12345
     → Met en queue de sync

   → GetAllCatchesAsync()
     → Récupère prises offline AVANT ClearStoreAsync()
     → Télécharge prises du serveur
     → Nettoie le cache
     → Remet prises serveur + prises offline

   → SyncAllAsync()
     → Traite la queue
     → Envoie prise -12345 au serveur
     → Reçoit nouvel ID (ex: 156)
     → Remplace -12345 par 156 dans le cache

   → ✅ Prise synchronisée et visible avec le vrai ID
```

---

## Logs de debugging

### Avant (avec bugs) ❌
```
User crée prise offline:
✅ Catch saved with ID: -12345
📋 Catch queued for sync (offline mode)
→ Navigation avec forceLoad
❌ Écran noir

User repasse online:
🔄 Network back online, starting auto-sync...
📦 Loading catches from API...
🗑️ Cleared IndexedDB store: catches
→ Prise -12345 perdue
```

### Après (corrigé) ✅
```
User crée prise offline:
✅ Catch saved with ID: -12345
📋 Catch queued for sync (offline mode)
→ Navigation SANS forceLoad
✅ Page "Mes Prises" affichée normalement

User repasse online:
🌐 Network status changed: Online ✅
🔄 Network back online, checking offline catches...
📤 Found 1 offline catches to sync
📋 Queued offline catch: Brochet (ID: -12345)
✅ 1 offline catches queued for synchronization

🔄 Starting sync of 1 items...
🔄 Processing Create for catch (-12345)
✅ Sync completed successfully

📦 1 prises offline détectées, préservation...
📦 Prise offline préservée: Brochet (ID: -12345)
✅ Données chargées et mises en cache

[Après sync complète]
→ Prise -12345 remplacée par ID 156
✅ Prise visible avec le vrai ID
```

---

## Tests de validation

### Test 1 : Navigation offline ✅
1. Passer en mode offline
2. Créer une prise
3. **Attendu** : Page "Mes Prises" s'affiche (pas d'écran noir)

### Test 2 : Préservation des prises offline ✅
1. Mode offline : créer une prise "Brochet"
2. Vérifier qu'elle apparaît dans "Mes Prises"
3. Repasser en ligne
4. **Attendu** : La prise "Brochet" est toujours visible
5. Attendre la synchronisation (voir logs)
6. **Attendu** : La prise a maintenant un ID positif

### Test 3 : Synchronisation automatique ✅
1. Mode offline : créer 2 prises
2. Repasser en ligne
3. Ouvrir la console (F12)
4. **Attendu** :
   ```
   📤 Found 2 offline catches to sync
   ✅ 2 offline catches queued for synchronization
   ```

### Test 4 : Combinaison serveur + offline ✅
1. Mode online : créer 2 prises normales
2. Passer offline : créer 1 prise
3. Vérifier "Mes Prises" : **3 prises visibles**
4. Repasser online
5. **Attendu** : **3 prises toujours visibles**
6. Après sync : **3 prises avec IDs positifs**

---

## Fichiers modifiés

1. **AddCatch.razor** (ligne 870)
   - ❌ `Navigation.NavigateTo(..., forceLoad: true)`
   - ✅ `Navigation.NavigateTo(...)`  // Sans forceLoad

2. **Services/Offline/OfflineSupabaseService.cs** (méthode `GetAllCatchesAsync`)
   - ✅ Préservation des prises offline avant `ClearStoreAsync()`
   - ✅ Remise des prises offline après mise en cache
   - ✅ Retour de la combinaison (serveur + offline)

3. **Services/Offline/ISyncService.cs**
   - ✅ Nouvelle méthode `Task SyncOfflineCatchesAsync()`

4. **Services/Offline/SyncService.cs**
   - ✅ Ajout de `CATCHES_STORE` constante
   - ✅ Implémentation de `SyncOfflineCatchesAsync()`
   - ✅ Appel automatique au retour en ligne dans `InitializeAsync()`

---

## Résumé des corrections

| Problème | Cause | Solution | Statut |
|----------|-------|----------|--------|
| Écran noir offline | `forceLoad: true` | Retirer `forceLoad` | ✅ Corrigé |
| Perte de prises | `ClearStoreAsync()` trop agressif | Préserver les ID négatifs | ✅ Corrigé |
| Pas de sync auto | Prises offline non détectées | `SyncOfflineCatchesAsync()` | ✅ Corrigé |

---

## ✅ Validation finale

- [x] Plus d'écran noir en mode offline
- [x] Les prises offline sont préservées lors de la sync
- [x] Synchronisation automatique au retour en ligne
- [x] Combinaison correcte des données serveur + offline
- [x] Logs clairs pour le debugging
- [x] Tests validés

---

**Tous les bugs de mode offline sont maintenant corrigés !** 📦✅
