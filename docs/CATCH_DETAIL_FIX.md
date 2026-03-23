# 🐛 Fix: "Prise non trouvée" pour les prises offline

## Problème identifié

**Symptôme** :
- En mode offline, l'utilisateur crée une prise
- La prise apparaît dans la liste ✅
- Mais quand l'utilisateur clique dessus pour voir les détails → **"Prise non trouvée"** ❌

**Scénario de reproduction** :
```
1. Mode offline → Créer prise "Black-bass"
   → ID négatif : -12345
   → Apparaît dans la liste ✅

2. Clic sur la prise pour voir les détails
   → GetCatchByIdAsync(-12345)
   → Essaie d'appeler l'API avec ID -12345
   → API ne trouve pas (normal)
   → Retourne null
   → "Prise non trouvée" ❌

3. Même problème après retour en ligne avant la sync
```

---

## Cause racine

### Dans `OfflineSupabaseService.GetCatchByIdAsync()`

**Code bugué** ❌ :
```csharp
public async Task<FishCatch?> GetCatchByIdAsync(int id)
{
    if (_networkStatus.IsOnline)
    {
        // ❌ Essaie d'appeler l'API même pour les IDs négatifs
        var catchItem = await _onlineService.GetCatchByIdAsync(id);
        // L'API ne connaît pas les IDs négatifs → null
        return catchItem;
    }
    return await _indexedDb.GetItemAsync<FishCatch>(CATCHES_STORE, id.ToString());
}
```

**Problème** :
- Les IDs négatifs sont des identifiants **temporaires offline**
- Ils n'existent **que dans le cache local**
- L'API Supabase ne les connaît pas
- Donc l'appel API retourne toujours `null`

---

## Solution implémentée ✅

### Vérifier si l'ID est négatif AVANT d'appeler l'API

```csharp
public async Task<FishCatch?> GetCatchByIdAsync(int id)
{
    // ✅ Si l'ID est négatif, c'est une prise offline
    if (id < 0)
    {
        Console.WriteLine($"📦 Loading offline catch {id} from cache...");
        return await _indexedDb.GetItemAsync<FishCatch>(CATCHES_STORE, id.ToString());
    }

    // ID positif : essayer l'API si online
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
```

### Flux corrigé

```
GetCatchByIdAsync(-12345)
    ↓
Est-ce que id < 0 ? → OUI
    ↓
📦 Charger depuis le cache
    ↓
✅ Prise trouvée et affichée
```

```
GetCatchByIdAsync(156)  // ID positif
    ↓
Est-ce que id < 0 ? → NON
    ↓
Online ? → OUI
    ↓
Appeler l'API Supabase
    ↓
✅ Prise trouvée depuis le serveur
```

---

## Corrections supplémentaires

### Retirer `forceLoad: true` dans CatchDetail.razor

**Avant** ❌ :
```csharp
private void GoToCatchesList()
{
    Navigation.NavigateTo("/FishingSpot_App/catches", forceLoad: true);
}
```

**Après** ✅ :
```csharp
private void GoToCatchesList()
{
    Navigation.NavigateTo("/FishingSpot_App/catches");
}
```

**Raison** : `forceLoad: true` cause un écran noir en mode offline.

---

## Tests de validation

### Test 1 : Détail prise offline ✅

```
1. Mode offline
2. Créer prise "Brochet"
   → ID: -12345
3. Clic sur la prise dans la liste
4. **Attendu** : Page de détails s'affiche avec toutes les infos
5. **Logs** :
   🔍 Loading catch with ID: -12345
   📦 Loading offline catch -12345 from cache...
   ✅ Catch loaded: Brochet
```

### Test 2 : Détail prise après sync ✅

```
1. Offline : créer prise "Perche"
   → ID: -98765
2. Voir les détails → ✅ Fonctionne
3. Repasser online → Sync automatique
   → ID devient 234 (positif)
4. Voir les détails
5. **Attendu** : Page de détails charge depuis l'API
6. **Logs** :
   🔍 Loading catch with ID: 234
   Online, calling API...
   ✅ Catch loaded: Perche
```

### Test 3 : Détail prise normale ✅

```
1. Online : créer prise normale
   → ID: 567
2. Clic sur la prise
3. **Attendu** : Charge depuis l'API
4. **Logs** :
   🔍 Loading catch with ID: 567
   Online, calling API...
   ✅ Catch loaded from API
```

---

## Logs de debugging

### Avant (bugué) ❌

```
User clique sur prise offline (ID -12345):
🔍 Loading catch with ID: -12345
Calling API with ID -12345...
❌ API error: 404 Not Found
❌ Catch -12345 not found
→ Affiche "Prise non trouvée"
```

### Après (corrigé) ✅

```
User clique sur prise offline (ID -12345):
🔍 Loading catch with ID: -12345
📦 Loading offline catch -12345 from cache...
✅ Catch loaded: Black-bass
→ Affiche les détails complets
```

```
User clique sur prise après sync (ID 234):
🔍 Loading catch with ID: 234
Online, calling API...
✅ Catch loaded from API: Black-bass
→ Affiche les détails depuis le serveur
```

---

## Fichiers modifiés

1. **Services/Offline/OfflineSupabaseService.cs** (méthode `GetCatchByIdAsync`)
   - ✅ Vérification `if (id < 0)` en premier
   - ✅ Chargement direct depuis le cache pour IDs négatifs
   - ✅ Vérification du token pour IDs positifs
   - ✅ Fallback au cache en cas d'erreur

2. **Components/Pages/CatchDetail.razor** (méthode `GoToCatchesList`)
   - ✅ Retirer `forceLoad: true`

---

## Résumé des corrections

| ID | Type | Comportement |
|----|------|--------------|
| **< 0** | Offline | 📦 Cache direct (pas d'appel API) |
| **> 0** | Normal | 🌐 API si online, sinon cache |

### Avantages

- ✅ Les prises offline sont toujours accessibles en détail
- ✅ Pas d'appel API inutile pour les IDs négatifs
- ✅ Meilleure performance (pas d'attente réseau)
- ✅ Fonctionne même après retour en ligne avant la sync
- ✅ Transition transparente après synchronisation

---

## Scénario complet de bout en bout

```
1. User offline → Crée prise "Sandre"
   → ID: -55555
   → Dans cache: ✅

2. User clique sur "Sandre" dans la liste
   → GetCatchByIdAsync(-55555)
   → id < 0 ? OUI
   → 📦 Cache direct
   → ✅ Détails affichés (photo, lieu, dimensions)

3. User repasse online
   → Sync auto démarre
   → Envoie prise -55555 au serveur
   → Serveur retourne ID 999
   → Cache mis à jour: -55555 supprimé, 999 ajouté

4. Liste se rafraîchit
   → "Sandre" apparaît avec ID 999

5. User clique sur "Sandre" (ID 999)
   → GetCatchByIdAsync(999)
   → id < 0 ? NON
   → Online ? OUI
   → 🌐 Appel API
   → ✅ Charge depuis le serveur
   → ✅ Détails affichés

→ Transition transparente de offline à online !
```

---

## Checklist de validation

- [x] Détails des prises offline s'affichent correctement
- [x] Pas d'appel API pour les IDs négatifs
- [x] Appel API normal pour les IDs positifs
- [x] Fallback au cache en cas d'erreur réseau
- [x] Vérification du token avant appel API
- [x] Pas d'écran noir (forceLoad retiré)
- [x] Logs clairs pour le debugging

---

**La page de détails fonctionne maintenant parfaitement en mode online et offline !** ✅
