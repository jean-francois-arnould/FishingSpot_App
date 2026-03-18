# FishingSpot PWA 🎣

## ✅ CE QUI A ÉTÉ FAIT

### 1. Transformation en PWA (Progressive Web App)
- ✅ Projet Blazor WebAssembly configuré avec Service Worker
- ✅ Manifest.json configuré pour l'installation sur mobile/desktop
- ✅ Interface responsive qui fonctionne hors connexion (une fois cachée)

### 2. Configuration Base de Données - Supabase (PostgreSQL gratuit)
- ✅ Service de connexion REST API vers Supabase
- ✅ Modèles de données (FishCatch)
- ✅ Script SQL de création de tables (`supabase_setup.sql`)
- ✅ CRUD complet (Create, Read, Update, Delete)

### 3. Architecture
```
FishingSpot.PWA/
├── Models/              # Modèles de données
│   └── FishCatch.cs
├── Services/            # Services de connexion DB
│   ├── ISupabaseService.cs
│   └── SupabaseService.cs
├── Pages/               # Pages Blazor
│   ├── Catches.razor   # Liste des prises
│   ├── Home.razor
│   ├── Counter.razor
│   └── Weather.razor
├── wwwroot/
│   ├── appsettings.json      # Configuration Supabase (à configurer!)
│   ├── manifest.webmanifest  # Config PWA
│   └── service-worker.js     # Pour fonctionnement offline
└── supabase_setup.sql        # Script SQL à exécuter
```

## 🔧 CE QU'IL RESTE À FAIRE

### 1. Setup Supabase (GRATUIT - 500MB, bandwidth illimité pour API)

**Pourquoi Supabase?**
- 100% gratuit pour votre usage (<100 connexions/mois)
- PostgreSQL hébergé
- API REST automatique
- 500MB de stockage DB
- Pas de carte bancaire requise

**Étapes :**
1. Aller sur [supabase.com](https://supabase.com)
2. Créer un compte gratuit (avec GitHub ou email)
3. Créer un nouveau projet
   - Nom: `fishingspot`
   - Mot de passe DB: choisir un mot de passe fort
   - Région: Europe West (London) - la plus proche
4. Une fois le projet créé, aller dans **SQL Editor** (icône dans le menu gauche)
5. Copier-coller le contenu du fichier `FishingSpot.PWA/supabase_setup.sql` et cliquer **Run**
6. Aller dans **Settings > API**
   - Copier **Project URL** (ex: `https://xxxxx.supabase.co`)
   - Copier **anon public** key (PAS la service_role!)

### 2. Configuration de l'app

**Fichier à modifier: `FishingSpot.PWA/wwwroot/appsettings.json`**
```json
{
  "Supabase": {
    "Url": "https://VOTRE_PROJECT_ID.supabase.co",
    "Key": "VOTRE_ANON_KEY_ICI"
  }
}
```

Remplacez:
- `VOTRE_PROJECT_ID` par votre URL Supabase
- `VOTRE_ANON_KEY_ICI` par votre clé anon publique

### 3. Hébergement GRATUIT

#### ⭐ Option Recommandée: GitHub Pages

**Avantages:**
- Totalement gratuit
- Setup simple
- Déploiement automatique
- URL: `https://jean-francois-arnould.github.io/FishingSpot_App/`

**Étapes:**

1. Créer le fichier `.github/workflows/deploy-pwa.yml` avec ce contenu:
```yaml
name: Deploy PWA to GitHub Pages

on:
  push:
    branches: [ WAP ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'

    - name: Publish
      run: dotnet publish FishingSpot.PWA/FishingSpot.PWA.csproj -c Release -o release

    - name: Change base tag
      run: sed -i 's/<base href="\/" \/>/<base href="\/FishingSpot_App\/" \/>/g' release/wwwroot/index.html

    - name: Add .nojekyll
      run: touch release/wwwroot/.nojekyll

    - name: Deploy
      uses: peaceiris/actions-gh-pages@v3
      with:
        github_token: ${{ secrets.GITHUB_TOKEN }}
        publish_dir: release/wwwroot
```

2. Dans votre repo GitHub:
   - Aller dans **Settings > Pages**
   - Source: "Deploy from a branch"
   - Branch: `gh-pages` / `root`
   - Save

3. Commiter et push votre code sur la branche `WAP`:
```bash
git add .
git commit -m "Setup PWA with Supabase"
git push origin WAP
```

4. L'app sera automatiquement déployée à:
   `https://jean-francois-arnould.github.io/FishingSpot_App/`

**Limites:** 1GB espace, 100GB bandwidth/mois (largement suffisant!)

#### Alternative: Netlify
1. Aller sur [netlify.com](https://netlify.com)
2. "Add new site" > "Import from Git"
3. Sélectionner votre repo + branche WAP
4. Build command: `dotnet publish FishingSpot.PWA/FishingSpot.PWA.csproj -c Release -o release`
5. Publish directory: `release/wwwroot`
6. Deploy!

### 4. Test en local

```bash
cd FishingSpot.PWA
dotnet run
```
Puis ouvrir: `https://localhost:5001`

## 📱 COMMENT ÇA FONCTIONNE

### Architecture Simplifiée
```
┌─────────────────┐
│  Navigateur     │  ← PWA installable (comme une app native)
│  (iOS, Android, │
│   Desktop)      │
└────────┬────────┘
         │ HTTPS
         ▼
┌─────────────────┐
│  API Supabase   │  ← API REST automatique
│  (PostgreSQL)   │
└─────────────────┘
```

### Flux de données
1. L'utilisateur ouvre l'app dans son navigateur
2. L'app charge depuis GitHub Pages (fichiers statiques)
3. Les requêtes de données vont vers Supabase via HTTPS
4. Supabase retourne les données en JSON
5. L'app affiche les données

### Avantages
✅ **Coût: 0€** (Supabase gratuit + GitHub Pages gratuit)
✅ **Installation:** Peut être installé comme une app sur mobile/desktop
✅ **Offline:** Service Worker permet de consulter en cache
✅ **Cross-platform:** iOS, Android, Windows, Mac, Linux
✅ **Pas de store:** Pas besoin de licence Apple Developer (99$/an)
✅ **Updates:** Instantanés (pas de validation store)
✅ **Accessible:** N'importe qui peut accéder via URL

### Limites
❌ Pas d'accès aux fonctionnalités natives profondes (Bluetooth, NFC)
❌ Performance légèrement inférieure à une app native (mais acceptable)
⚠️ Nécessite connexion internet pour charger/enregistrer données

## 🔐 SÉCURITÉ

### Configuration actuelle (ATTENTION)
- ⚠️ **Données publiques**: N'importe qui avec l'URL peut voir/modifier les données
- Cette config est OK pour test, mais PAS pour production

### Pour sécuriser (optionnel):
1. Dans Supabase: **Authentication** > Enable Email auth
2. Modifier les RLS policies dans SQL Editor:
```sql
-- Désactiver l'accès public
DROP POLICY IF EXISTS "enable_read_access_for_all_users" ON fish_catches;
DROP POLICY IF EXISTS "enable_insert_for_all_users" ON fish_catches;
DROP POLICY IF EXISTS "enable_update_for_all_users" ON fish_catches;
DROP POLICY IF EXISTS "enable_delete_for_all_users" ON fish_catches;

-- Activer uniquement pour utilisateurs authentifiés
CREATE POLICY "Authenticated users can read" ON fish_catches
    FOR SELECT USING (auth.role() = 'authenticated');

CREATE POLICY "Authenticated users can insert" ON fish_catches
    FOR INSERT WITH CHECK (auth.role() = 'authenticated');

CREATE POLICY "Authenticated users can update" ON fish_catches
    FOR UPDATE USING (auth.role() = 'authenticated');

CREATE POLICY "Authenticated users can delete" ON fish_catches
    FOR DELETE USING (auth.role() = 'authenticated');
```

3. Ajouter l'authentification dans le code (tutoriel Supabase Auth + Blazor)

## 📊 LIMITES GRATUITES

### Supabase Free Tier
- ✅ 500 MB database
- ✅ 1 GB file storage
- ✅ 2 GB bandwidth/mois
- ✅ 50k requêtes authentification/mois
- ✅ API calls illimitées

**Pour 100 connexions/mois:** Très largement suffisant!

### GitHub Pages
- ✅ 1 GB storage
- ✅ 100 GB bandwidth/mois
- ✅ Builds illimités

## 🚀 PROCHAINES FONCTIONNALITÉS (optionnel)

- [ ] Page d'ajout de nouvelles prises (formulaire)
- [ ] Upload de photos vers Supabase Storage
- [ ] Carte interactive avec localisation des prises
- [ ] Statistiques et graphiques
- [ ] Export PDF des prises
- [ ] Mode offline complet avec synchronisation
- [ ] Authentification utilisateur
- [ ] Partage de prises sur réseaux sociaux

## 📝 RÉSUMÉ RAPIDE

1. **Créer compte Supabase** + exécuter `supabase_setup.sql`
2. **Copier URL et Key** dans `appsettings.json`
3. **Créer fichier GitHub Actions** (`.github/workflows/deploy-pwa.yml`)
4. **Activer GitHub Pages** dans settings du repo
5. **Push le code** → Déploiement automatique!

**Coût total: 0€**
**Temps estimé: 15-20 minutes**

---

**Note:** Cette branche `WAP` est dédiée à la PWA. Le projet MAUI original reste sur `main`.
