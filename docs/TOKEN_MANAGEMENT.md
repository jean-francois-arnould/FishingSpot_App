# 🔐 Gestion des Tokens JWT et Expiration de Session

## 📋 Problème résolu

### **Symptômes avant correction**
- ❌ Erreurs 401 "JWT expired" partout dans les logs
- ❌ L'app essayait d'appeler l'API avec un token expiré
- ❌ Pas de redirection automatique vers login
- ❌ Le mode offline ne fonctionnait pas à cause des erreurs 401

### **Causes identifiées**
1. Les tokens JWT Supabase expirent après **1 heure** (normal)
2. Le refresh automatique était implémenté mais pas appelé au bon moment
3. Pas de vérification du token avant les appels API
4. Pas de redirection automatique en cas d'expiration

---

## ✅ Solutions implémentées

### 1. **Vérification du token avant chaque appel API**

#### `IAuthService.cs` - Nouvelles méthodes
```csharp
bool IsTokenExpired { get; }  // Vérifie si le token est expiré
event Action? OnSessionExpired;  // Événement déclenché à l'expiration
Task<bool> EnsureValidTokenAsync();  // Rafraîchit le token si nécessaire
```

#### `AuthService.cs` - Implémentation
```csharp
public bool IsTokenExpired => 
    _tokenExpiresAt.HasValue && _tokenExpiresAt.Value <= DateTime.UtcNow;

public async Task<bool> EnsureValidTokenAsync()
{
    // Si le token expire dans moins de 5 minutes, le rafraîchir
    if (_tokenExpiresAt.HasValue && _tokenExpiresAt.Value <= DateTime.UtcNow.AddMinutes(5))
    {
        return await RefreshTokenAsync();
    }
    return true;
}
```

### 2. **Redirection automatique en cas d'expiration**

#### Nouveau composant `SessionWatcher.razor`
```razor
@inject IAuthService AuthService
@inject NavigationManager Navigation

@code {
    protected override void OnInitialized()
    {
        AuthService.OnSessionExpired += HandleSessionExpired;
    }

    private void HandleSessionExpired()
    {
        Navigation.NavigateTo("/FishingSpot_App/login?expired=true", forceLoad: true);
    }
}
```

Ce composant est ajouté dans `App.razor` et écoute l'événement d'expiration.

### 3. **Vérification du token dans les services offline**

#### `OfflineSupabaseService.cs`
```csharp
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
            // Appeler l'API avec un token valide
            var catches = await _onlineService.GetAllCatchesAsync();
            // ... cache les résultats ...
        }
        catch (Exception ex)
        {
            // Fallback au cache en cas d'erreur
        }
    }

    // Mode offline : retourner le cache
    return await _indexedDb.GetAllItemsAsync<FishCatch>(CATCHES_STORE);
}
```

### 4. **Message d'information sur la page de login**

#### `Login.razor`
```razor
@if (sessionExpired)
{
    <div class="alert alert-warning" role="alert">
        <strong>⏱️ Session expirée</strong><br/>
        Votre session a expiré. Veuillez vous reconnecter.
    </div>
}

@code {
    private bool sessionExpired;

    protected override async Task OnInitializedAsync()
    {
        var uri = new Uri(Navigation.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        sessionExpired = query["expired"] == "true";
    }
}
```

---

## 🔄 Flux de gestion du token

### **Cas 1 : Token valide**
```
Utilisateur → Service offline → Vérifie token → ✅ Valide → Appel API → Succès
```

### **Cas 2 : Token expire dans 5 minutes**
```
Utilisateur → Service offline → Vérifie token → ⏱️ Expire bientôt 
→ Refresh automatique → ✅ Nouveau token → Appel API → Succès
```

### **Cas 3 : Token expiré sans refresh token**
```
Utilisateur → Service offline → Vérifie token → ❌ Expiré 
→ Utilise cache offline → ⚠️ Notification → 🚪 Redirection login
```

### **Cas 4 : Mode offline**
```
Utilisateur → Service offline → ⚠️ Pas de réseau 
→ Utilise cache offline → ✅ Succès (pas d'appel API)
```

---

## ⏰ Timeline du token

```
T=0      Token créé (login réussi)
↓
T=55min  Refresh automatique planifié (5 min avant expiration)
↓
T=60min  Token expire (si pas de refresh)
↓
         → Tentative de refresh
         → Si échec : déconnexion + redirection login
```

---

## 🔧 Configuration Supabase

### **Durée de vie du token par défaut**
- **Access Token** : 1 heure (3600 secondes)
- **Refresh Token** : 30 jours (par défaut)

### **Modifier la durée dans Supabase Dashboard**

1. Aller sur [Supabase Dashboard](https://app.supabase.com/)
2. Sélectionner votre projet
3. **Authentication** → **Settings**
4. **JWT Expiry** : Modifier la valeur (en secondes)

**Options possibles :**
- `3600` = 1 heure (par défaut)
- `7200` = 2 heures
- `86400` = 24 heures (⚠️ moins sécurisé)
- Maximum recommandé : **24 heures**

### **⚠️ Pourquoi ne pas désactiver l'expiration ?**

Il est **impossible et déconseillé** de faire des tokens qui n'expirent jamais car :

1. **Sécurité** : Si un token est volé, il peut être utilisé indéfiniment
2. **Standards JWT** : Les JWT doivent avoir une date d'expiration
3. **Best practices** : Les tokens courts + refresh tokens longs = plus sécurisé

**Solution recommandée** : Garder l'expiration à 1h et utiliser le refresh automatique ✅

---

## 🧪 Tests

### Test 1 : Vérification du refresh automatique
1. ✅ Se connecter à l'app
2. ✅ Ouvrir la console (F12)
3. ✅ Attendre ~55 minutes
4. ✅ Vérifier le log : "🔄 Token rafraîchi avec succès !"

### Test 2 : Expiration forcée
1. ✅ Se connecter
2. ✅ Dans localStorage, modifier `supabase_token_expires_at` à une date passée
3. ✅ Recharger l'app
4. ✅ Vérifier la redirection vers `/login?expired=true`

### Test 3 : Mode offline avec token expiré
1. ✅ Se connecter et naviguer (pour mettre en cache)
2. ✅ Passer en mode offline (mode avion)
3. ✅ L'app utilise le cache, pas d'erreur 401

---

## 📊 Logs de debugging

Avec les corrections, vous verrez dans la console :

```
✅ Connexion réussie ! Token expire à 15:30:00
🔄 Rafraîchissement automatique planifié dans 55.0 minutes
...
[55 minutes plus tard]
🔄 Token expiré, tentative de rafraîchissement...
✅ Token rafraîchi avec succès ! Expire à 16:30:00
🔄 Rafraîchissement automatique planifié dans 55.0 minutes
```

Si le refresh échoue :
```
❌ Échec du rafraîchissement du token: 401
🚪 Session expirée, déconnexion...
→ Redirection vers /login?expired=true
```

---

## ✅ Checklist finale

- [x] Token vérifié avant chaque appel API
- [x] Refresh automatique toutes les 55 minutes
- [x] Redirection automatique si token invalide
- [x] Mode offline fonctionne même avec token expiré
- [x] Message d'avertissement sur la page de login
- [x] Logs clairs pour le debugging
- [x] Gestion des erreurs 401

---

## 🔮 Améliorations futures possibles

1. **Notification toast** : Afficher "Token rafraîchi" à l'utilisateur
2. **Countdown** : Afficher le temps restant avant expiration
3. **Bouton manuel** : Permettre un refresh manuel
4. **Offline queue** : Stocker les actions offline et les rejouer après refresh

---

**Statut : ✅ Token JWT géré correctement, plus d'erreurs 401 !** 🔐
