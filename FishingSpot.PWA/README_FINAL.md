# 🎣 FishingSpot PWA - Documentation Complète

> **Progressive Web App** pour suivre vos prises de pêche avec gestion avancée du matériel et des montages

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com/)
[![Blazor WebAssembly](https://img.shields.io/badge/Blazor-WebAssembly-512BD4)](https://blazor.net/)
[![Supabase](https://img.shields.io/badge/Supabase-PostgreSQL-3ECF8E)](https://supabase.com/)

---

## 📋 Table des Matières

1. [Fonctionnalités](#-fonctionnalités)
2. [Architecture](#-architecture)
3. [Installation](#-installation)
4. [Configuration](#-configuration)
5. [Structure du Projet](#-structure-du-projet)
6. [Guide d'Utilisation](#-guide-dutilisation)
7. [Documentation Technique](#-documentation-technique)

---

## ✨ Fonctionnalités

### 🎣 Gestion des Prises
- ✅ Enregistrer les détails d'une prise (espèce, poids, longueur, date/heure)
- ✅ **Upload de photos** (caméra ou galerie)
- ✅ **Géolocalisation automatique** ou manuelle
- ✅ Association à un montage de pêche
- ✅ Notes personnalisées
- ✅ Liste et édition des prises

### 🛠️ Gestion du Matériel (6 catégories)
1. **🎣 Cannes** : marque, modèle, longueur, puissance, action
2. **🎡 Moulinets** : marque, modèle, type, ratio de récupération
3. **🧵 Fils** : marque, type, résistance, diamètre, couleur
4. **🪝 Leurres** : nom, type, couleur, poids, taille
5. **⛓️ Bas de ligne** : matériau, résistance, longueur
6. **🪝 Hameçons** : taille, type, marque

**Fonctionnalités** :
- ✅ CRUD complet (Create, Read, Update, Delete)
- ✅ Interface intuitive avec cartes colorées
- ✅ Compteurs en temps réel
- ✅ Modal de confirmation pour les suppressions

### 🎯 Gestion des Montages
- ✅ Créer des montages (combinaisons d'équipements)
- ✅ **Définir un montage comme "actuel"**
- ✅ Marquer des montages favoris ⭐
- ✅ **Pré-sélection automatique** du montage actuel dans les formulaires
- ✅ Affichage détaillé de tout l'équipement du montage

### 👤 Profil Utilisateur
- ✅ Authentification Supabase (inscription/connexion)
- ✅ Profil personnalisé
- ✅ Données isolées par utilisateur (Row Level Security)

---

## 🏗️ Architecture

### Stack Technique

**Frontend**
- .NET 10
- Blazor WebAssembly (mode standalone)
- Bootstrap 5
- Progressive Web App (PWA)

**Backend**
- Supabase (PostgreSQL + Auth + Storage)
- REST API
- Row Level Security (RLS)

**Stockage**
- PostgreSQL (base de données)
- Supabase Storage (photos)

### Base de Données

**9 tables principales** :
```
user_profiles          → Profils utilisateurs
├── rods              → Cannes à pêche
├── reels             → Moulinets
├── lines             → Fils de pêche
├── lures             → Leurres
├── leaders           → Bas de ligne
├── hooks             → Hameçons
└── fishing_setups     → Montages (références aux équipements)
    └── fish_catches   → Prises de poisson
```

**Sécurité** :
- 36 politiques RLS
- 13 index de performance
- Contrainte unique pour un montage actuel par utilisateur

---

## 🚀 Installation

### Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio 2026 Preview](https://visualstudio.microsoft.com/vs/preview/) ou VS Code
- Compte [Supabase](https://supabase.com/) (gratuit)

### Étapes

1. **Cloner le repository**
   ```powershell
   git clone https://github.com/jean-francois-arnould/FishingSpot_App.git
   cd FishingSpot\FishingSpot.PWA
   ```

2. **Configurer Supabase**
   - Créer un projet sur https://supabase.com/dashboard
   - Exécuter le script SQL : `supabase_create_all.sql`
   - Créer le bucket Storage : `fishing-photos` (voir `GUIDE_SUPABASE_BUCKET.md`)

3. **Configuration**
   - Copier `wwwroot/appsettings.json.example` vers `wwwroot/appsettings.json`
   - Remplir avec vos clés Supabase :
     ```json
     {
       "Supabase": {
         "Url": "https://[VOTRE_PROJET].supabase.co",
         "Key": "[VOTRE_CLE_ANON]"
       }
     }
     ```

4. **Lancer l'application**
   ```powershell
   dotnet run
   ```

5. **Ouvrir le navigateur**
   - Aller sur : https://localhost:5001

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "Supabase": {
    "Url": "https://kejapcjuczjhyfdeshrv.supabase.co",
    "Key": "votre_anon_key"
  }
}
```

### Variables d'environnement (optionnel)

```bash
export SUPABASE_URL="https://kejapcjuczjhyfdeshrv.supabase.co"
export SUPABASE_KEY="votre_anon_key"
```

---

## 📁 Structure du Projet

```
FishingSpot.PWA/
├── Models/                     # Modèles de données
│   ├── Equipment/
│   │   └── Equipment.cs       # 6 types d'équipement
│   ├── FishCatch.cs           # Modèle Prise
│   ├── FishingSetup.cs        # Modèle Montage
│   ├── UserProfile.cs         # Modèle Profil
│   └── GeolocationPosition.cs # Géolocalisation
│
├── Services/                   # Couche services
│   ├── ISupabaseService.cs    # Interface Supabase
│   ├── SupabaseService.cs     # Implémentation
│   ├── IEquipmentService.cs   # Interface Équipement
│   ├── EquipmentService.cs    # Implémentation
│   ├── IAuthService.cs        # Interface Auth
│   ├── AuthService.cs         # Implémentation
│   └── UserProfileService.cs  # Service Profil
│
├── Pages/                      # Pages Blazor
│   ├── Materiel/              # 22 pages matériel
│   │   ├── Index.razor        # Hub
│   │   ├── Cannes.razor       # Liste cannes
│   │   ├── AjouterCanne.razor # Ajouter canne
│   │   ├── ModifierCanne.razor# Modifier canne
│   │   └── ... (x6 catégories)
│   │
│   ├── Montages/              # 3 pages montages
│   │   ├── Index.razor        # Liste montages
│   │   ├── Ajouter.razor      # Créer montage
│   │   └── Modifier.razor     # Modifier montage
│   │
│   ├── Catches.razor          # Liste prises
│   ├── AddCatch.razor         # Ajouter prise
│   ├── EditCatch.razor        # Modifier prise
│   ├── Profile.razor          # Profil utilisateur
│   ├── Login.razor            # Connexion
│   └── Register.razor         # Inscription
│
├── wwwroot/
│   ├── index.html             # Page HTML + JS Geolocation
│   ├── appsettings.json       # Configuration (gitignored)
│   ├── manifest.webmanifest   # Manifest PWA
│   └── service-worker.js      # Service Worker
│
├── SQL/                        # Scripts SQL
│   ├── supabase_create_all.sql      # Création DB complète
│   └── supabase_clean_all.sql       # Nettoyage DB
│
└── Documentation/
    ├── IMPLEMENTATION_COMPLETE.md   # Résumé implémentation
    ├── GUIDE_SUPABASE_BUCKET.md     # Guide bucket Storage
    ├── AUDIT_COMPLET.md             # Audit technique
    ├── PROGRESS_MATERIEL.md         # Progression matériel
    └── PROGRESS_MONTAGES.md         # Progression montages
```

---

## 📖 Guide d'Utilisation

### 1. Inscription / Connexion
1. Ouvrir l'application
2. Cliquer sur **"S'inscrire"**
3. Entrer email et mot de passe (min 6 caractères)
4. Se connecter

### 2. Ajouter du Matériel
1. Aller sur **"Matériel"**
2. Choisir une catégorie (ex: Cannes)
3. Cliquer sur **"Ajouter une canne"**
4. Remplir le formulaire
5. **Enregistrer**

Répéter pour les 6 catégories d'équipement.

### 3. Créer un Montage
1. Aller sur **"Montages"**
2. Cliquer sur **"Créer un montage"**
3. Donner un nom au montage
4. Sélectionner l'équipement dans les dropdowns :
   - Canne
   - Moulinet
   - Fil
   - Leurre
   - Bas de ligne
   - Hameçon
5. Cocher **"⭐ Marquer comme favori"** (optionnel)
6. Cocher **"✅ Définir comme montage actuel"** (optionnel)
7. **Enregistrer**

### 4. Enregistrer une Prise
1. Aller sur **"Mes Prises"**
2. Cliquer sur **"Add New Catch"**
3. Remplir le formulaire :
   - **Espèce** : Nom du poisson *
   - **Poids et Longueur** : Mesures
   - **Lieu** : Nom de l'endroit *
   - **Coordonnées GPS** : 
     - Saisir manuellement OU
     - Cliquer sur **"📍 Me localiser"**
   - **Date et Heure** : Date de la prise *
   - **Photo** :
     - Cliquer sur **"Choisir un fichier"** OU
     - Cliquer sur **"📷 Caméra"** (mobile)
   - **Montage** : Sélectionné automatiquement (montage actuel)
   - **Notes** : Remarques personnelles
4. **Enregistrer**

### 5. Définir un Montage comme Actuel
1. Aller sur **"Montages"**
2. Sur un montage, cliquer sur **"Définir comme actuel"**
3. Le montage a maintenant un badge **"✅ ACTUEL"**
4. Ce montage sera pré-sélectionné lors de l'ajout d'une nouvelle prise

---

## 🔧 Documentation Technique

### Services

#### SupabaseService
```csharp
// CRUD Prises
Task<List<FishCatch>> GetAllCatchesAsync();
Task<int> AddCatchAsync(FishCatch catch);
Task<bool> UpdateCatchAsync(FishCatch catch);
Task<bool> DeleteCatchAsync(int id);

// CRUD Montages
Task<List<FishingSetup>> GetAllSetupsAsync();
Task<int> AddSetupAsync(FishingSetup setup);
Task<bool> UpdateSetupAsync(FishingSetup setup);
Task<bool> DeleteSetupAsync(int id);
Task<FishingSetup?> GetCurrentSetupAsync();
Task<bool> SetCurrentSetupAsync(int setupId);

// Upload Photos
Task<string?> UploadPhotoAsync(Stream photoStream, string fileName);
```

#### EquipmentService
```csharp
// Chaque type d'équipement (Rod, Reel, Line, Lure, Leader, Hook)
Task<List<T>> GetAll{Type}sAsync();
Task<T?> Get{Type}ByIdAsync(int id);
Task<int> Add{Type}Async(T item);
Task<bool> Update{Type}Async(T item);
Task<bool> Delete{Type}Async(int id);
```

### Géolocalisation JavaScript

```javascript
window.getCurrentPosition = function() {
    return new Promise((resolve, reject) => {
        navigator.geolocation.getCurrentPosition(
            (position) => {
                resolve({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude,
                    accuracy: position.coords.accuracy
                });
            },
            (error) => reject(new Error(error.message)),
            { enableHighAccuracy: true, timeout: 10000, maximumAge: 0 }
        );
    });
};
```

### Row Level Security (RLS)

Toutes les tables utilisent RLS pour isoler les données par utilisateur :

```sql
-- Exemple pour fish_catches
CREATE POLICY "Users can view their own catches" ON fish_catches
    FOR SELECT USING (auth.uid() = user_id);

CREATE POLICY "Users can insert their own catches" ON fish_catches
    FOR INSERT WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update their own catches" ON fish_catches
    FOR UPDATE USING (auth.uid() = user_id);

CREATE POLICY "Users can delete their own catches" ON fish_catches
    FOR DELETE USING (auth.uid() = user_id);
```

---

## 📊 Statistiques du Projet

| Métrique | Valeur |
|----------|--------|
| **Pages Blazor** | 27 |
| **Services C#** | 5 |
| **Modèles** | 12 |
| **Tables SQL** | 9 |
| **Politiques RLS** | 36 |
| **Lignes de code C#** | ~3500 |
| **Lignes de SQL** | ~800 |

---

## 🐛 Dépannage

### L'upload de photos ne fonctionne pas
➡️ Vérifier que le bucket `fishing-photos` existe sur Supabase Storage  
➡️ Vérifier qu'il est **public**  
➡️ Voir : `GUIDE_SUPABASE_BUCKET.md`

### La géolocalisation ne fonctionne pas
➡️ Vérifier que le navigateur demande l'autorisation de localisation  
➡️ Sur mobile, vérifier les permissions de l'application  
➡️ Utiliser HTTPS (requis pour l'API Geolocation)

### Les montages ne s'affichent pas dans le dropdown
➡️ Vérifier que des montages ont été créés (`/montages`)  
➡️ Vérifier la console du navigateur (F12) pour les erreurs  
➡️ Vérifier la connexion Supabase

### Erreur "Failed to fetch"
➡️ Vérifier `appsettings.json`  
➡️ Vérifier que Supabase URL et Key sont corrects  
➡️ Vérifier que l'utilisateur est authentifié

---

## 📝 TODO / Améliorations Futures

- [ ] Mode hors ligne avec synchronisation
- [ ] Carte interactive pour visualiser les prises
- [ ] Statistiques avancées (graphiques)
- [ ] Export PDF des prises
- [ ] Partage social
- [ ] Notifications push
- [ ] Recherche et filtres avancés
- [ ] Mode sombre

---

## 📄 Licence

Ce projet est sous licence MIT.

---

## 👤 Auteur

**Jean-François Arnould**
- GitHub: [@jean-francois-arnould](https://github.com/jean-francois-arnould)

---

## 🙏 Remerciements

- [Supabase](https://supabase.com/) pour le backend
- [Blazor](https://blazor.net/) pour le framework
- [Bootstrap](https://getbootstrap.com/) pour l'UI

---

**🎣 Bonne pêche avec FishingSpot PWA ! 🎣**
