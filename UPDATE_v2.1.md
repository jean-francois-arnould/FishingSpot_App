# 🔄 MISE À JOUR v2.1 - Page Profil Utilisateur

## Nouveautés

### ✨ Fonctionnalités ajoutées

1. **Page d'accueil améliorée** (`/`)
   - Affiche les boutons Login/Register si non connecté
   - Affiche les actions rapides si connecté

2. **Page "Mon Compte"** (`/profile`)
   - Formulaire de profil complet:
     - Prénom, Nom
     - Téléphone
     - Pays, Ville
     - Spot de pêche favori
     - Bio
   - Bouton "Se déconnecter"
   - Bouton "Supprimer mon compte" avec confirmation

3. **Nouvelle table Supabase** (`user_profiles`)
   - Stockage des informations utilisateur
   - Liée à auth.users via user_id
   - Sécurisée par RLS

4. **Menu mis à jour**
   - Ajout du lien "Mon Compte"
   - Textes en français

## 📋 Fichiers ajoutés/modifiés

### Nouveaux fichiers
```
FishingSpot.PWA/
├── Models/
│   └── UserProfile.cs              # Nouveau
├── Services/
│   ├── IUserProfileService.cs      # Nouveau
│   └── UserProfileService.cs       # Nouveau
└── Pages/
    ├── Profile.razor               # Nouveau
    └── Home.razor                  # Modifié

supabase_setup.sql                  # Modifié (ajout table user_profiles)
TEST_LOGIN.md                       # Nouveau (guide de test)
```

### Fichiers modifiés
- `FishingSpot.PWA/Pages/Home.razor` - Nouvelle interface
- `FishingSpot.PWA/Layout/NavMenu.razor` - Ajout lien "Mon Compte"
- `FishingSpot.PWA/Program.cs` - Enregistrement UserProfileService
- `FishingSpot.PWA/supabase_setup.sql` - Ajout table user_profiles

## 🚀 Mise à jour

### 1. Mettre à jour la base de données Supabase

Si vous avez déjà une base Supabase configurée:

1. Aller dans votre projet Supabase
2. **SQL Editor**
3. Copier UNIQUEMENT cette partie du script SQL:

```sql
-- TABLE: user_profiles
CREATE TABLE IF NOT EXISTS user_profiles (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL UNIQUE REFERENCES auth.users(id) ON DELETE CASCADE,
    first_name TEXT,
    last_name TEXT,
    phone TEXT,
    country TEXT,
    city TEXT,
    favorite_spot TEXT,
    bio TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_user_profiles_user ON user_profiles(user_id);

ALTER TABLE user_profiles ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Users can view their own profile" ON user_profiles
    FOR SELECT 
    USING (auth.uid() = user_id);

CREATE POLICY "Users can insert their own profile" ON user_profiles
    FOR INSERT 
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update their own profile" ON user_profiles
    FOR UPDATE 
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can delete their own profile" ON user_profiles
    FOR DELETE 
    USING (auth.uid() = user_id);
```

4. Cliquer **RUN**
5. Vérifier dans **Table Editor** que `user_profiles` apparaît

### 2. Mettre à jour le code

Pull les derniers changements:

```bash
git pull origin WAP
```

OU si vous avez des changements locaux:

```bash
git stash
git pull origin WAP
git stash pop
```

### 3. Rebuild l'application

```bash
dotnet build FishingSpot.PWA/FishingSpot.PWA.csproj
```

### 4. Tester en local

```bash
cd FishingSpot.PWA
dotnet run
```

Ouvrir: `https://localhost:5001`

### 5. Déployer

```bash
git add .
git commit -m "Update: Ajout page profil utilisateur"
git push origin WAP
```

L'app sera automatiquement déployée via GitHub Actions.

## 🧪 Tests à effectuer

Voir le fichier `TEST_LOGIN.md` pour les scénarios de test complets.

**Tests rapides:**
1. Se connecter
2. Aller sur `/profile`
3. Remplir le formulaire
4. Cliquer "Enregistrer"
5. Actualiser la page → Les données devraient être sauvegardées
6. Tester "Se déconnecter"
7. Tester "Supprimer mon compte" (avec un compte de test!)

## ⚠️ Notes importantes

### Suppression de compte

La fonctionnalité "Supprimer mon compte" fait:
1. Supprime le profil utilisateur (`user_profiles`)
2. Supprime toutes les prises (`fish_catches`) via CASCADE
3. L'utilisateur reste dans `auth.users` (Supabase Auth)

**Pour supprimer complètement un user de auth.users:**
→ Aller dans Supabase Dashboard
→ Authentication > Users
→ Cliquer sur les "..." du user
→ Delete user

**Note:** Le code ne peut pas supprimer directement de `auth.users` car cela nécessite des privilèges admin (service_role key).

### Cascade DELETE

Grâce à `ON DELETE CASCADE`, quand un user est supprimé de `auth.users`:
- Son profil dans `user_profiles` est automatiquement supprimé
- Ses prises dans `fish_catches` sont automatiquement supprimées

## 🎯 Prochaines étapes

Fonctionnalités à venir:
- [ ] Formulaire d'ajout de prises de pêche
- [ ] Upload de photos de profil
- [ ] Upload de photos de prises
- [ ] Statistiques personnelles
- [ ] Export des données
- [ ] Carte interactive

## 📊 Changements de version

### v2.1 (Actuelle)
- ✅ Page profil utilisateur
- ✅ Suppression de compte
- ✅ Home page améliorée
- ✅ Menu en français

### v2.0
- ✅ Authentification complète
- ✅ Login/Register/Logout
- ✅ RLS Supabase
- ✅ Isolation des données par user

### v1.0
- ✅ PWA initiale
- ✅ Connexion Supabase
- ✅ Liste des prises

---

**Temps de mise à jour:** ~5 minutes
**Compatibilité:** Rétrocompatible (les users existants peuvent continuer à utiliser l'app)
**Breaking changes:** Aucun
