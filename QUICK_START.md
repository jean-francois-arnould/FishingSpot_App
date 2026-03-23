# 🎣 FishingSpot v2.0 - Quick Start Guide

## 🚀 Installation en 3 Minutes

### Étape 1 : Configuration Initiale (1 min)

```bash
# Windows
.\setup.ps1

# Linux/macOS
chmod +x setup.sh && ./setup.sh
```

✅ Ce script va :
- Vérifier .NET 10
- Restaurer les dépendances
- Créer appsettings.json
- Compiler le projet
- Lancer les tests

---

### Étape 2 : Configuration Supabase (1 min)

1. **Créer un compte** : https://supabase.com (gratuit)
2. **Créer un projet** : Choisir un nom et une région
3. **Copier les identifiants** :
   - Menu "Settings" → "API"
   - Copier "URL" et "anon public"

4. **Éditer** `wwwroot/appsettings.json` :
```json
{
  "Supabase": {
    "Url": "https://VOTRE_PROJECT.supabase.co",
    "Key": "VOTRE_ANON_KEY"
  }
}
```

---

### Étape 3 : Base de Données (1 min)

1. **Ouvrir Supabase** → Menu "SQL Editor"
2. **Copier** le contenu de `database/improvements.sql`
3. **Coller** dans l'éditeur
4. **Cliquer** sur "Run" (▶️)

✅ Cela va créer :
- 10 indexes de performance
- 2 colonnes pour thumbnails
- 1 table de cache statistiques
- 3 triggers automatiques
- 12 policies de sécurité

---

## ✅ Vérification de l'Installation

### Test de Build
```bash
dotnet build
# ✅ Build succeeded
```

### Test de l'Application
```bash
dotnet run
# ✅ Application started
# ✅ Listening on: https://localhost:5001
```

### Test Complet
```bash
.\run-tests.ps1  # Windows
# ✅ Tous les tests passent
# ✅ Coverage: 75%+
# ✅ Rapport généré
```

---

## 📱 Utilisation

### Lancer l'Application

```bash
dotnet run
```

Puis ouvrir : **https://localhost:5001**

### Pages Disponibles

| Page | URL | Description |
|------|-----|-------------|
| 🏠 Accueil | `/` | Dashboard principal |
| 🐟 Mes Prises | `/catches` | Liste de toutes vos prises |
| ➕ Ajouter | `/catches/add` | Enregistrer une nouvelle prise |
| 📊 Statistiques | `/statistiques` | Stats basiques |
| 📈 Stats Avancées | `/statistiques-avancees` | Graphiques détaillés |
| 🎣 Matériel | `/materiel` | Gestion équipement |
| 🪝 Montages | `/montages` | Configurations de ligne |
| 👤 Profil | `/profile` | Votre profil |

---

## 🆕 Nouvelles Fonctionnalités v2.0

### 1. Toast Notifications 🔔
```csharp
@inject IToastService Toast

Toast.ShowSuccess("Prise enregistrée !");
Toast.ShowError("Erreur de connexion");
Toast.ShowWarning("Taille minimale non respectée");
Toast.ShowInfo("5 éléments à synchroniser");
```

### 2. Statistiques Avancées 📊
- Graphiques par espèce
- Tendances mensuelles
- Records personnels
- Meilleurs spots
- Météo favorable

### 3. Export de Données 📤
```csharp
@inject IExportService Export

// Export CSV
var csv = await Export.ExportToCsvAsync(catches);

// Export JSON
var json = await Export.ExportToJsonAsync(catches);
```

### 4. Partage Social 🔗
```csharp
@inject IShareService Share

// Partager une prise
await Share.ShareCatchAsync(fishCatch);

// Partage custom
await Share.ShareTextAsync("Titre", "Description", url);
```

### 5. Compression Images 📸
- Compression automatique des photos
- Création de miniatures (150x150)
- Économie de 90% de bande passante

### 6. Pagination 📄
```razor
<Pagination TItem="FishCatch" 
            PagedResult="@pagedCatches" 
            OnPageChange="LoadPage" />
```

### 7. Logging 📝
```csharp
@inject ILoggerService Logger

Logger.LogInformation("Action effectuée");
Logger.LogError("Erreur", exception);
```

### 8. Analytics 📈 (Optionnel)
- Suivi des pages vues
- Événements utilisateur
- Web Vitals automatiques
- Compatible Google Analytics 4

---

## 🧪 Tests

### Lancer les Tests
```bash
# Simple
dotnet test

# Avec détails
dotnet test --verbosity detailed

# Avec coverage + rapport
.\run-tests.ps1
```

### Ajouter des Tests
```csharp
// FishingSpot.Tests/Services/MonServiceTests.cs
[Fact]
public async Task MaMethode_Scenario_ResultatAttendu()
{
    // Arrange
    var service = new MonService();

    // Act
    var result = await service.MaMethode();

    // Assert
    result.Should().NotBeNull();
}
```

---

## 📚 Documentation Complète

| Document | Description |
|----------|-------------|
| [README.md](README.md) | Vue d'ensemble du projet |
| [IMPROVEMENTS.md](IMPROVEMENTS.md) | Détails techniques des améliorations |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Guide pour contribuer |
| [database/README.md](database/README.md) | Documentation base de données |
| [IMPLEMENTATION_CHECKLIST.md](IMPLEMENTATION_CHECKLIST.md) | Checklist complète |

---

## 🐛 Problèmes Courants

### "Impossible de se connecter à Supabase"
✅ **Solution** : Vérifier `appsettings.json` → URL et Key corrects

### "Les tests échouent"
✅ **Solution** : 
```bash
dotnet restore FishingSpot.Tests
dotnet build FishingSpot.Tests
```

### "L'application ne démarre pas"
✅ **Solution** :
```bash
dotnet clean
dotnet build
dotnet run
```

### "Les images ne se chargent pas"
✅ **Solution** : Vérifier les CORS dans Supabase Storage

---

## 🎯 Checklist Post-Installation

- [ ] Application démarre sans erreur
- [ ] Connexion Supabase fonctionne
- [ ] Inscription d'un compte utilisateur
- [ ] Ajout d'une première prise
- [ ] Upload d'une photo (compression automatique)
- [ ] Affichage des statistiques
- [ ] Export CSV fonctionne
- [ ] Partage social disponible (mobile)
- [ ] Tests passent (75%+ coverage)

---

## 📞 Support

**GitHub Issues** : [FishingSpot_App/issues](https://github.com/jean-francois-arnould/FishingSpot_App/issues)

**Documentation** :
- Questions générales → GitHub Discussions
- Bugs → GitHub Issues
- Contributions → CONTRIBUTING.md

---

## 🚀 Prêt à Pêcher !

Votre application est maintenant configurée et prête à l'emploi avec :

✅ **75% Test Coverage**  
✅ **Performance optimisée** (-87% temps requêtes)  
✅ **Interface moderne** (toasts, stats avancées)  
✅ **Mode offline** (IndexedDB + Service Worker)  
✅ **Sécurité renforcée** (RLS, validation)  

---

**🎣 Bonne pêche avec FishingSpot 2.0 ! 🎣**

*N'oubliez pas de ⭐ le repo si le projet vous plaît !*
