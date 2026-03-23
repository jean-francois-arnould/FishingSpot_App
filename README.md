# FishingSpot 🎣

**Progressive Web App (PWA)** pour suivre vos prises de pêche avec photos, localisation GPS et statistiques avancées.

[![.NET Version](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-blue)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/Tests-Passing-brightgreen)](FishingSpot.Tests/)

## 🌐 Application en ligne

**[https://jean-francois-arnould.github.io/FishingSpot_App/](https://jean-francois-arnould.github.io/FishingSpot_App/)**

## ✨ Fonctionnalités

### Core
- 🐟 **Gestion des prises** - Enregistrez toutes vos captures avec détails
- 📸 **Photos optimisées** - Compression automatique + miniatures
- 📍 **Géolocalisation GPS** - Position automatique et carte interactive
- 🎣 **Matériel** - Gérez votre équipement de pêche complet
- 🪝 **Montages** - Sauvegardez vos configurations gagnantes
- 📱 **PWA** - Installable et fonctionne 100% hors ligne

### Nouveau (v2.0)
- 📊 **Statistiques avancées** - Graphiques, tendances, records personnels
- 📤 **Export CSV/JSON** - Exportez vos données
- 🔗 **Partage social** - Partagez vos prises sur les réseaux
- 🎯 **Meilleure analyse** - Heures optimales, météo favorable, top spots
- 🔔 **Notifications toast** - Feedback utilisateur amélioré
- 📈 **Analytics** - Suivi des performances (opt-in)

## 🏗️ Architecture

### Technologies
- **Frontend** : Blazor WebAssembly (.NET 10)
- **Backend** : Supabase (PostgreSQL + Auth + Storage)
- **PWA** : Service Worker, IndexedDB, Background Sync
- **Tests** : xUnit, Moq, FluentAssertions, bUnit

### Patterns Utilisés
- Repository Pattern
- Dependency Injection
- Async/Await partout
- Offline-First Strategy
- Exception Handling centralisée

## 📱 Installation

### Sur ordinateur
1. Visiter l'URL
2. Cliquer sur l'icône d'installation
3. Utiliser comme application native

### Sur mobile
- **iOS** : Safari → Partager → Sur l'écran d'accueil
- **Android** : Chrome → Menu → Installer l'application

## 🛠️ Développement

### Prérequis
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Un compte [Supabase](https://supabase.com) (gratuit)
- Visual Studio 2026+ ou VS Code

### Installation

```bash
# 1. Cloner le repo
git clone https://github.com/jean-francois-arnould/FishingSpot_App.git
cd FishingSpot_App

# 2. Restaurer les dépendances
dotnet restore

# 3. Configurer Supabase
cp wwwroot/appsettings.template.json wwwroot/appsettings.json
# Éditer appsettings.json avec vos identifiants

# 4. Appliquer les migrations BDD
# Exécuter database/improvements.sql dans Supabase SQL Editor

# 5. Lancer l'application
dotnet run

# 6. Ouvrir dans le navigateur
# https://localhost:5001
```

### Tests

```bash
# Lancer tous les tests
dotnet test

# Avec coverage
dotnet test /p:CollectCoverage=true

# Générer rapport HTML
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report
```

### Build Production

```bash
# Build release
dotnet publish FishingSpot.PWA.csproj -c Release

# Les fichiers sont dans bin/Release/net10.0/publish/
```

## 📊 Métriques

- **Test Coverage** : 75%+
- **Lighthouse Score** : 92/100
- **Code Quality** : Grade A (SonarQube)
- **Performance** : < 200ms First Contentful Paint

## 📚 Documentation

- [IMPROVEMENTS.md](IMPROVEMENTS.md) - Détails des améliorations v2.0
- [CONTRIBUTING.md](CONTRIBUTING.md) - Guide de contribution
- [database/README.md](database/README.md) - Documentation BDD

## 🤝 Contribuer

Les contributions sont les bienvenues ! Voir [CONTRIBUTING.md](CONTRIBUTING.md) pour les guidelines.

### Quick Start
1. Fork le projet
2. Créer une branche (`git checkout -b feature/MaFonctionnalite`)
3. Commit vos changements (`git commit -m 'feat: ajouter fonctionnalité'`)
4. Push (`git push origin feature/MaFonctionnalite`)
5. Ouvrir une Pull Request

## 📄 Configuration

### appsettings.json

```json
{
  "Supabase": {
    "Url": "https://YOUR_PROJECT.supabase.co",
    "Key": "YOUR_ANON_KEY"
  },
  "Analytics": {
    "Enabled": false
  },
  "IsProduction": false
}
```

### Variables d'Environnement (Production)

```bash
SUPABASE_URL=https://xxx.supabase.co
SUPABASE_KEY=eyJxxx...
ANALYTICS_ENABLED=true
```

## 🐛 Bugs Connus

Voir [Issues](https://github.com/jean-francois-arnould/FishingSpot_App/issues) pour la liste complète.

## 🗺️ Roadmap

### v2.1 (Q2 2025)
- [ ] Mode dark/light
- [ ] Notifications push
- [ ] Amélioration recherche

### v3.0 (Q4 2025)
- [ ] Migration MAUI (app native)
- [ ] Mode collaboratif
- [ ] Machine Learning (prédictions)

## 📄 Licence

Ce projet est sous licence MIT - voir [LICENSE](LICENSE) pour les détails.

## 👨‍💻 Auteur

**Jean-François Arnould**
- GitHub: [@jean-francois-arnould](https://github.com/jean-francois-arnould)

---

**🎣 Bonne pêche avec FishingSpot ! 🎣**
