# FishingSpot 🎣

**Progressive Web App (PWA)** pour suivre vos prises de pêche avec photos, localisation GPS et statistiques.

## 🌐 Application en ligne

**URL de production** : [https://jean-francois-arnould.github.io/FishingSpot_App/](https://jean-francois-arnould.github.io/FishingSpot_App/)

## ✨ Fonctionnalités


### 🐟 Mes Prises (Catches)
- Liste de toutes vos prises de pêche
- Affichage des détails : photo, localisation, date/heure, mesures
- Modification et suppression des prises
- Filtrage et statistiques

### ➕ Ajouter une Prise
- Capture photo avec l'appareil (caméra mobile ou upload)
- **Géolocalisation GPS automatique** du lieu de capture
- Enregistrement automatique de la date et l'heure
- Mesures : longueur (cm ou m) et poids (g ou kg)
- Notes personnalisées
- Sélection du matériel et montage utilisés

### 👤 Profil Utilisateur
- Gestion du profil
- Statistiques de pêche
- Historique des prises

### 🎣 Matériel & Montages
- Gestion de votre matériel de pêche
- Enregistrement de vos montages favoris
- Association aux prises

## 🚀 Technologies

- **Blazor WebAssembly** (.NET 10)
- **Progressive Web App** (PWA) - Fonctionne hors ligne
- **Supabase** - Base de données et authentification
- **Geolocation API** - GPS natif
- **Service Worker** - Cache et mode hors ligne

## 📱 Installation PWA

### Sur ordinateur (Chrome/Edge)
1. Visiter [https://jean-francois-arnould.github.io/FishingSpot_App/](https://jean-francois-arnould.github.io/FishingSpot_App/)
2. Cliquer sur l'icône d'installation dans la barre d'adresse
3. Cliquer sur "Installer"

### Sur mobile
1. Ouvrir l'URL dans le navigateur
2. **iOS (Safari)** : Partager → Sur l'écran d'accueil
3. **Android (Chrome)** : Menu → Installer l'application

## 🛠️ Développement local

### Prérequis
- .NET 10 SDK (Preview)
- Visual Studio 2026 ou VS Code

### Lancer en local
```bash
dotnet run
```

### Publier
```bash
dotnet publish FishingSpot.PWA.csproj -c Release -o output
```

## 📋 Configuration

### Supabase (Base de données)

1. Créer un compte sur [Supabase](https://supabase.com)
2. Créer un nouveau projet
3. Copier `wwwroot/appsettings.template.json` vers `wwwroot/appsettings.json`
4. Remplir avec vos identifiants Supabase :
```json
{
  "Supabase": {
    "Url": "https://votre-projet.supabase.co",
    "Key": "votre-cle-api-publique"
  }
}
```

### Permissions PWA

L'application demande les permissions suivantes (uniquement en ligne) :
- **Caméra** : Pour prendre des photos des prises
- **Localisation GPS** : Pour enregistrer automatiquement le lieu de pêche

Ces permissions sont gérées par le navigateur.

## 📁 Structure du projet

```
```
FishingSpot.PWA/
├── Pages/                    # Pages Blazor (.razor)
│   ├── Home.razor           # Page d'accueil
│   ├── Login.razor          # Connexion
│   ├── Register.razor       # Inscription
│   ├── Catches.razor        # Liste des prises
│   ├── AddCatch.razor       # Ajouter une prise (avec GPS)
│   ├── EditCatch.razor      # Modifier une prise
│   └── Profile.razor        # Profil utilisateur
├── Services/                 # Services C#
│   ├── AuthService.cs       # Authentification Supabase
│   ├── SupabaseService.cs   # API Supabase
│   ├── EquipmentService.cs  # Gestion du matériel
│   └── UserProfileService.cs # Gestion du profil
├── Models/                   # Modèles de données
│   ├── FishCatch.cs         # Modèle de prise
│   ├── Equipment.cs         # Modèle de matériel
│   ├── FishingSetup.cs      # Modèle de montage
│   └── UserProfile.cs       # Modèle de profil
├── wwwroot/                  # Assets statiques
│   ├── index.html           # Point d'entrée HTML
│   ├── manifest.webmanifest # Manifeste PWA
│   ├── service-worker.js    # Service worker (cache)
│   ├── css/                 # Styles CSS
│   └── lib/                 # Bibliothèques (Bootstrap)
├── database/                 # Scripts SQL Supabase
└── .github/workflows/        # CI/CD GitHub Actions
```

## 🔧 Déploiement

### Automatique via GitHub Actions

Le déploiement sur GitHub Pages est automatique à chaque push sur `main` :

1. Le workflow compile le projet
2. Configure les chemins pour GitHub Pages
3. Déploie sur `https://jean-francois-arnould.github.io/FishingSpot_App/`

Voir `.github/workflows/blazor-deploy.yml` pour les détails.

## 📝 Documentation

- **SUCCESS_DEPLOYMENT.md** - Guide de déploiement et URLs
- **PWA-README.md** - Guide d'utilisation de la PWA
- **DEPLOYMENT_VERIFICATION.md** - Vérification du déploiement
- **CHANGELOG.md** - Historique des versions

## 🤝 Contribution

Les contributions sont les bienvenues ! N'hésitez pas à :
1. Fork le projet
2. Créer une branche (`git checkout -b feature/AmazingFeature`)
3. Commit vos changements (`git commit -m 'Add AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

## 📄 Licence

Ce projet est sous licence MIT.

## 👨‍💻 Auteur

**Jean-François Arnould**

- GitHub: [@jean-francois-arnould](https://github.com/jean-francois-arnould)
- Application: [FishingSpot PWA](https://jean-francois-arnould.github.io/FishingSpot_App/)

---

**🎣 Bonne pêche avec FishingSpot ! 🎣**
2. Enregistrer la position exacte de la prise
3. Afficher les coordonnées à l'écran pour confirmation

**Activation :**
- Cliquez sur le bouton "📍 Utiliser ma position actuelle"
- Autorisez l'accès à la localisation si demandé
- Les coordonnées GPS s'affichent instantanément

## Prochaines améliorations possibles

- [ ] Stockage persistant avec SQLite
- [ ] Export des données en CSV/PDF
- [ ] Statistiques et graphiques
- [ ] Météo en temps réel au lieu de capture
- [ ] Partage sur les réseaux sociaux
- [ ] Mode hors ligne complet
- [ ] Carnet de pêche avec calendrier
- [ ] Vue carte avec tous les spots de pêche (optionnel)

## Licence

Ce projet est créé à des fins éducatives et personnelles.
