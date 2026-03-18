# 🎣 FishingSpot PWA - Guide Complet des Fonctionnalités

## 📱 Pages Disponibles

### 🏠 Pages Publiques
- **`/`** - Page d'accueil (sans navigation)
- **`/login`** - Connexion
- **`/register`** - Inscription

### 🔐 Pages Authentifiées

#### Mes Prises (`/catches`)
- Liste de toutes vos prises de poisson
- Affichage en cartes avec:
  - Nom du poisson
  - Date, lieu, taille, poids
  - Photo (si disponible)
  - Notes
- Bouton **"Add New Catch"** pour ajouter une prise
- Bouton **"Edit"** sur chaque carte

#### Ajouter une Prise (`/catches/add`)
Formulaire complet avec:
- 🐟 **Informations sur le poisson:**
  - Espèce (requis)
  - Longueur (cm)
  - Poids (kg)

- 📍 **Localisation & Temps:**
  - Nom du lieu (requis)
  - Coordonnées GPS (latitude/longitude - optionnel)
  - Date (requis)
  - Heure

- 📝 **Détails additionnels:**
  - URL de la photo
  - Notes (conditions météo, technique utilisée, etc.)
  - **Sélection d'un setup de pêche** (dropdown avec vos setups existants)

#### Modifier une Prise (`/catches/edit/{id}`)
- Tous les champs modifiables
- Bouton **"Delete"** avec modal de confirmation
- Retour automatique vers la liste après sauvegarde

#### Mes Setups (`/setups`)
- Liste de tous vos équipements de pêche
- Affichage en cartes avec:
  - Nom du setup
  - Badge **⭐ Favorite** pour les favoris
  - Détails complets de l'équipement
- Bouton **"Add New Setup"** pour ajouter un setup
- Bouton **"Edit"** sur chaque carte

#### Ajouter un Setup (`/setups/add`)
Formulaire complet avec:
- 📝 **Informations générales:**
  - Nom du setup (requis)
  - Marquer comme favori ⭐

- 🎣 **Canne à pêche:**
  - Marque
  - Modèle
  - Longueur

- 🎡 **Moulinet:**
  - Marque
  - Modèle

- 🧵 **Ligne & Hameçons:**
  - Type de ligne (monofilament, tressé, fluorocarbone)
  - Résistance de la ligne
  - Leurre/Appât
  - Taille d'hameçon

- 📝 **Notes additionnelles**

#### Mon Profil (`/profile`)
- Informations personnelles:
  - Prénom, Nom
  - Téléphone
  - Pays, Ville
  - Spot favori
  - Bio
- Bouton **"Enregistrer"**
- Zone dangereuse:
  - Déconnexion
  - Suppression de compte

## 🗂️ Base de Données Supabase

### Tables
1. **`user_profiles`** - Profils utilisateurs
2. **`fishing_setups`** - Équipements de pêche
3. **`fish_catches`** - Prises de poisson (avec référence au setup utilisé)

### Relations
- `fish_catches.setup_id` → `fishing_setups.id`
- Toutes les tables liées à `auth.users(id)`
- RLS (Row Level Security) activé partout

## 🎨 Navigation

### Barre de navigation (sur toutes les pages authentifiées)
```
🎣 FishingSpot | My Catches | Setups | Profile | 👋 email@example.com | [Déconnexion]
```

### Flux utilisateur typique
1. **S'inscrire** → `/register`
2. **Se connecter** → `/login`
3. **Créer des setups** → `/setups/add`
4. **Marquer un favori** ⭐
5. **Enregistrer des prises** → `/catches/add`
6. **Lier la prise à un setup** (dropdown dans le formulaire)
7. **Modifier/Supprimer** → Boutons sur les cartes
8. **Compléter le profil** → `/profile`

## ✨ Fonctionnalités Clés

### ✅ CRUD Complet
- **Create** - Ajouter prises & setups
- **Read** - Lister prises & setups
- **Update** - Modifier prises & setups
- **Delete** - Supprimer avec confirmation

### ⭐ Favoris
- Marquer des setups comme favoris
- Les favoris apparaissent en premier dans les listes
- Badge visuel sur les cartes

### 🔗 Relations
- Lier une prise à un setup
- Dropdown intelligent dans le formulaire d'ajout
- Affichage des setups favoris en premier

### 🎨 UI/UX
- Design Bootstrap responsive
- Icônes emoji pour plus de clarté
- Cartes visuelles attractives
- Spinners de chargement
- Messages de succès/erreur
- Modals de confirmation

### 🔐 Sécurité
- Authentification Supabase
- Row Level Security
- Seules vos propres données sont visibles/modifiables

## 📋 TODO / Améliorations Possibles

- [ ] Page Edit Setup (`/setups/edit/{id}`)
- [ ] Upload de photos (au lieu d'URLs)
- [ ] Géolocalisation automatique (GPS du téléphone)
- [ ] Statistiques (nombre de prises, poids total, etc.)
- [ ] Filtres et recherche
- [ ] Carte interactive avec les spots de pêche
- [ ] Graphiques de performance
- [ ] Export des données (PDF, CSV)
- [ ] Partage sur les réseaux sociaux
- [ ] Mode hors ligne (PWA)

## 🚀 Installation & Déploiement

### Prérequis
1. Compte Supabase configuré
2. Tables créées avec `supabase_complete.sql`
3. `wwwroot/appsettings.json` configuré

### Lancer l'application
```bash
dotnet run --launch-profile https
```

### Build pour production
```bash
dotnet publish -c Release
```

## 📞 Support

En cas de problème:
1. Vérifier la console du navigateur (F12)
2. Vérifier les logs Supabase
3. Vérifier que les tables existent
4. Vérifier les politiques RLS

---

**Créé avec ❤️ pour les passionnés de pêche** 🎣
