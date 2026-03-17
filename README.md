# FishingSpot 🎣

Application mobile de gestion de pêche développée avec .NET MAUI pour iOS et Android.

## Fonctionnalités


### 🐟 Mes Poissons
- Liste de toutes vos prises de pêche
- Affichage des détails : photo, localisation, date/heure, mesures
- Suppression par glissement

### ➕ Ajouter un Poisson
- Capture photo avec l'appareil
- **Géolocalisation GPS automatique** du lieu de capture
- Enregistrement de la date et l'heure
- Mesures : longueur et poids
- Notes personnalisées

## Configuration

### Permissions requises

L'application demande les permissions suivantes :
- **Caméra** : Pour prendre des photos des prises
- **Localisation GPS** : Pour enregistrer automatiquement le lieu de pêche
- **Stockage/Photos** : Pour sauvegarder les images

Ces permissions sont déjà configurées dans :
- `Platforms/Android/AndroidManifest.xml`
- `Platforms/iOS/Info.plist`

## Compilation et exécution

### Pour Android :
```bash
dotnet build -f net10.0-android
```

### Pour iOS :
```bash
dotnet build -f net10.0-ios
```

### Pour exécuter sur un émulateur/appareil :
Dans Visual Studio, sélectionnez la plateforme cible et cliquez sur Démarrer.

## Structure du projet

```
FishingSpot/
├── Models/
│   ├── Fish.cs              # Modèle d'espèce de poisson
│   └── FishCatch.cs         # Modèle de capture de pêche
├── Services/
│   └── DatabaseService.cs   # Service de gestion des données
├── Views/
│   ├── FishDocumentationPage.xaml  # Documentation des poissons
│   ├── MyCatchesPage.xaml   # Liste des captures
│   └── AddCatchPage.xaml    # Formulaire d'ajout avec GPS
├── Platforms/
│   ├── Android/
│   │   └── AndroidManifest.xml  # Permissions GPS, caméra, stockage
│   └── iOS/
│       └── Info.plist           # Permissions GPS, caméra, photos
├── AppShell.xaml            # Navigation principale (2 onglets)
└── MauiProgram.cs           # Configuration de l'application
```

## Technologies utilisées

- **.NET MAUI** - Framework cross-platform
- **C# 12** avec .NET 10
- **XAML** pour les interfaces utilisateurs
- **Géolocalisation MAUI** - API de géolocalisation intégrée pour capturer automatiquement la position GPS
- **MediaPicker MAUI** - API de capture photo intégrée

## Utilisation de la Géolocalisation

Lors de l'ajout d'un nouveau poisson, l'application utilise la géolocalisation du téléphone pour :
1. Capturer automatiquement les coordonnées GPS (latitude/longitude)
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
