# 🔍 Diagnostic - Erreur "Le serveur n'a pas retourné d'ID"

## 📱 Symptôme

Lors de l'enregistrement d'une prise dans l'application mobile, vous voyez :
```
❌ Erreur lors de la sauvegarde. Le serveur n'a pas retourné d'ID. 
Vérifiez votre connexion et que vous êtes connecté.
```

---

## 🔧 Causes Possibles

### 1. **Header `Prefer` manquant ou incorrect**

**Problème :** Supabase nécessite le header `Prefer: return=representation` pour retourner l'objet créé.

**Solution :** Vérifier dans `Services/SupabaseService.cs` ligne 216-217 :
```csharp
_httpClient.DefaultRequestHeaders.Remove("Prefer");
_httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
```

✅ **Déjà corrigé dans le code**

---

### 2. **Réponse vide de Supabase**

**Problème :** Supabase retourne un statut 200 mais un body vide `[]`.

**Diagnostic :**
- Ouvrir les DevTools du navigateur (F12)
- Onglet Console
- Chercher les logs `Response body:`

**Solution :** 
Vérifier les **RLS Policies** dans Supabase :

```sql
-- Vérifier que la policy INSERT existe
SELECT * FROM pg_policies 
WHERE tablename = 'fish_catches' 
  AND cmd = 'INSERT';

-- Si manquante, créer :
CREATE POLICY "Users insert own catches" ON fish_catches 
FOR INSERT WITH CHECK (auth.uid() = user_id);
```

---

### 3. **Token d'authentification invalide**

**Problème :** Le token JWT a expiré ou est invalide.

**Diagnostic :**
```javascript
// Dans Console DevTools
console.log(localStorage.getItem('supabase_token'));
console.log(localStorage.getItem('supabase_token_expires_at'));
```

**Solution :**
1. Se déconnecter
2. Se reconnecter
3. Réessayer

---

### 4. **Champ manquant ou invalide**

**Problème :** Un champ requis dans la BDD n'est pas envoyé.

**Diagnostic :** Chercher dans Console :
```
Response status: 400
Response body: {"message":"...","details":"..."}
```

**Solution :** Vérifier que tous les champs requis sont remplis :
- `user_id` (automatique)
- `fish_name` (requis)
- `catch_date` (requis)

---

### 5. **Problème de sérialisation JSON**

**Problème :** Les données ne sont pas correctement sérialisées.

**Diagnostic :** Chercher dans Console :
```
Sending catch JSON: {...}
```

**Solution :** Vérifier que le JSON est valide et ne contient pas :
- `Id: 0` ou `Id: -xxxxx` (ID négatif en offline)
- Champs avec valeurs `null` quand `NOT NULL` en BDD

---

## 🛠️ Corrections Appliquées

### ✅ Amélioration du logging

Ajout de logs détaillés dans `SupabaseService.cs` :

```csharp
// Ligne 227-235
if (string.IsNullOrWhiteSpace(responseBody) || responseBody == "[]")
{
    Console.WriteLine($"⚠️ Empty response body from Supabase!");
    Console.WriteLine($"Prefer header value: {_httpClient.DefaultRequestHeaders.GetValues("Prefer").FirstOrDefault()}");
    throw new Exception("Supabase returned empty response...");
}
```

### ✅ Gestion d'erreur améliorée

Ajout de messages d'erreur spécifiques dans `AddCatch.razor` :

```csharp
if (catchId == 0)
{
    Logger.LogError("Server returned ID = 0");
    Toast.ShowError("Erreur de sauvegarde. Vérifiez votre connexion.");
}
```

### ✅ Toast Notifications

Ajout de feedback visuel :
- ✅ Success : "Prise enregistrée avec succès !"
- ℹ️ Info : "Prise enregistrée en mode hors-ligne..."
- ❌ Error : Messages d'erreur spécifiques

---

## 🧪 Tests de Diagnostic

### Test 1 : Vérifier l'authentification

```javascript
// Dans Console DevTools
const token = localStorage.getItem('supabase_token');
if (!token) {
    console.error('❌ Pas de token - reconnexion nécessaire');
} else {
    console.log('✅ Token présent:', token.substring(0, 20) + '...');
}
```

### Test 2 : Vérifier les RLS Policies

```sql
-- Dans Supabase SQL Editor
SELECT * FROM pg_policies 
WHERE tablename = 'fish_catches';
```

Vous devriez voir au moins 4 policies :
- `Users see own catches` (SELECT)
- `Users insert own catches` (INSERT)
- `Users update own catches` (UPDATE)
- `Users delete own catches` (DELETE)

### Test 3 : Test manuel d'insertion

```sql
-- Dans Supabase SQL Editor
INSERT INTO fish_catches (user_id, fish_name, catch_date, length, weight)
VALUES ('your-user-id', 'Test', CURRENT_DATE, 50, 1.5)
RETURNING id;
```

Si cette requête fonctionne → Problème dans l'app  
Si cette requête échoue → Problème BDD

---

## 🔍 Étapes de Résolution

### Étape 1 : Vérifier les Logs
1. Ouvrir F12 (DevTools)
2. Onglet Console
3. Reproduire l'erreur
4. Chercher :
   - `Response status: XXX`
   - `Response body: {...}`
   - Messages d'erreur rouges

### Étape 2 : Vérifier Supabase
1. Aller sur supabase.com
2. Ouvrir votre projet
3. Menu "Authentication" → Vérifier que votre user existe
4. Menu "Table Editor" → Ouvrir `fish_catches`
5. Essayer d'ajouter une ligne manuellement

### Étape 3 : Vérifier RLS
1. Menu "SQL Editor"
2. Exécuter :
```sql
SELECT * FROM pg_policies WHERE tablename = 'fish_catches';
```
3. Si vide ou incomplet → Exécuter `database/improvements.sql`

### Étape 4 : Réinitialiser l'authentification
1. Dans l'app : Menu → Déconnexion
2. Fermer l'app
3. Rouvrir l'app
4. Se reconnecter
5. Réessayer

---

## 📊 Checklist de Vérification

- [ ] Token d'authentification présent et valide
- [ ] Header `Prefer: return=representation` envoyé
- [ ] RLS Policy INSERT existe sur `fish_catches`
- [ ] User authentifié a un `user_id` valide
- [ ] Tous les champs requis sont remplis
- [ ] Pas d'erreur 401/403 dans Console
- [ ] Connexion Internet active
- [ ] Supabase URL et Key corrects dans `appsettings.json`

---

## 💡 Si Rien ne Fonctionne

### Option 1 : Mode Offline
L'application fonctionne en mode hors-ligne :
1. La prise est enregistrée localement (ID négatif)
2. Elle sera synchronisée automatiquement une fois en ligne

### Option 2 : Réinitialisation Complète
```javascript
// Dans Console DevTools
localStorage.clear();
// Puis recharger la page et se reconnecter
```

### Option 3 : Vérifier la Configuration
```json
// wwwroot/appsettings.json
{
  "Supabase": {
    "Url": "https://VOTRE_PROJECT.supabase.co", // ← Vérifier
    "Key": "VOTRE_ANON_KEY" // ← Vérifier
  }
}
```

---

## 📞 Support

Si le problème persiste après toutes ces vérifications :

1. **Copier les logs Console** (F12 → Console → tout copier)
2. **Prendre une capture d'écran** de l'erreur
3. **Noter les étapes** pour reproduire
4. **Créer une issue GitHub** avec ces infos

---

**🎣 Bonne pêche !** 🎣
