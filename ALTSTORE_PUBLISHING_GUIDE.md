# 📱 Guide de Publication AltStore - FishingSpot

## 🎯 Vue d'ensemble

Ce guide explique comment publier FishingSpot sur AltStore, permettant aux utilisateurs d'installer l'application sans passer par l'App Store.

## 📋 Prérequis Développeur

### 1. Compte Développeur Apple
- Compte gratuit Apple Developer (suffisant pour AltStore)
- OU compte payant Apple Developer ($99/an)

### 2. Environnement de développement
- macOS avec Xcode installé
- Visual Studio 2022+ avec workload .NET MAUI
- Certificat de développement iOS installé
- Provisioning Profile configuré

## 🔨 Étape 1 : Configuration du Signing

### Sur Mac avec Xcode :

1. **Ouvrir Xcode**
2. **Préférences > Accounts**
3. **Ajouter votre Apple ID**
4. **Gérer les certificats** > Créer "Apple Development"

### Dans le projet :

Le fichier `.csproj` est déjà configuré pour le signing automatique.

## 🏗️ Étape 2 : Build de l'application

### Option A : PowerShell (Windows)

```powershell
# Exécuter le script de build
.\build-altstore.ps1 -Version "1.0.0"
```

### Option B : Commande manuelle (Mac/Windows)

```bash
# Build pour iOS
dotnet publish FishingSpot/FishingSpot.csproj \
  -c Release \
  -f net10.0-ios \
  -p:RuntimeIdentifier=ios-arm64 \
  -p:ArchiveOnBuild=true \
  -p:CreatePackage=true
```

Le fichier `.ipa` sera généré dans :
`FishingSpot/bin/Release/net10.0-ios/ios-arm64/publish/`

## 📦 Étape 3 : Préparer les assets

### 1. Icône de l'application (512x512 px)
Créez `assets/fishingspot-icon.png`

### 2. Captures d'écran (1170x2532 px pour iPhone)
Créez :
- `assets/screenshot1.png` - Page principale
- `assets/screenshot2.png` - Capture avec détails
- `assets/screenshot3.png` - Statistiques

### 3. En-tête (1200x600 px)
Créez `assets/header.png`

## 🌐 Étape 4 : Hébergement

### Option A : GitHub Pages (Recommandé - Gratuit)

1. **Activer GitHub Pages** :
   - Repo Settings > Pages
   - Source: GitHub Actions

2. **Créer la structure** :
```
publish/
  ios/
    FishingSpot-1.0.0.ipa
assets/
  fishingspot-icon.png
  screenshot1.png
  screenshot2.png
  screenshot3.png
  header.png
  icon.png
altstore-source.json
```

3. **Commit et push** :
```bash
git add .
git commit -m "Add AltStore distribution"
git push
```

Le workflow GitHub Actions déploiera automatiquement.

### Option B : Autres hébergeurs

- **Netlify** : Drag & drop du dossier
- **Vercel** : Import du repo
- **CloudFlare Pages** : Connexion au repo

## ⚙️ Étape 5 : Configuration finale

### 1. Mettre à jour `altstore-source.json`

Remplacez les URLs :
```json
{
  "iconURL": "https://jean-francois-arnould.github.io/FishingSpot_App/assets/icon.png",
  "headerURL": "https://jean-francois-arnould.github.io/FishingSpot_App/assets/header.png",
  "apps": [{
    "iconURL": "https://jean-francois-arnould.github.io/FishingSpot_App/assets/fishingspot-icon.png",
    "versions": [{
      "downloadURL": "https://jean-francois-arnould.github.io/FishingSpot_App/publish/ios/FishingSpot-1.0.0.ipa",
      "size": 52428800
    }]
  }]
}
```

### 2. Obtenir la taille exacte du fichier

```powershell
# PowerShell
(Get-Item "publish\ios\FishingSpot-1.0.0.ipa").Length

# Bash
ls -l publish/ios/FishingSpot-1.0.0.ipa | awk '{print $5}'
```

## 🚀 Étape 6 : Publication

### URL de la source :
```
https://jean-francois-arnould.github.io/FishingSpot_App/altstore-source.json
```

### Partager avec les utilisateurs :

**Tweet/Post exemple** :
```
🎣 FishingSpot est maintenant disponible sur AltStore !

Installez-le facilement :
1. Ouvrir AltStore
2. Sources > + > Ajouter cette URL
3. Installer FishingSpot

🔗 https://jean-francois-arnould.github.io/FishingSpot_App/altstore-source.json

#FishingSpot #AltStore #Pêche
```

## 🔄 Étape 7 : Mises à jour

Pour publier une nouvelle version :

1. **Mettre à jour la version** dans `FishingSpot.csproj` :
```xml
<ApplicationDisplayVersion>1.1.0</ApplicationDisplayVersion>
<ApplicationVersion>2</ApplicationVersion>
```

2. **Build la nouvelle version** :
```powershell
.\build-altstore.ps1 -Version "1.1.0"
```

3. **Ajouter la version** dans `altstore-source.json` :
```json
"versions": [
  {
    "version": "1.1.0",
    "date": "2024-02-01",
    "localizedDescription": "Nouvelles fonctionnalités...",
    "downloadURL": "https://.../FishingSpot-1.1.0.ipa",
    "size": 52428800
  },
  {
    "version": "1.0.0",
    ...
  }
]
```

4. **Commit et push**

AltStore détectera automatiquement la mise à jour.

## 🐛 Dépannage

### Erreur de signing
```
Error: No signing identity found
```
**Solution** : Installer un certificat de développement via Xcode

### Fichier .ipa non généré
```
Error: Archive failed
```
**Solution** : Vérifier que le provisioning profile correspond au bundle ID

### AltStore ne trouve pas la source
**Solution** : Vérifier que le JSON est valide et accessible en HTTPS

## 📊 Statistiques

Vous pouvez suivre les téléchargements via :
- GitHub Traffic (Pages)
- Google Analytics (en ajoutant un tracker)
- Cloudflare Analytics

## 🔒 Sécurité

- ✅ Le fichier .ipa doit être signé avec votre certificat
- ✅ HTTPS obligatoire pour l'hébergement
- ✅ Vérifier régulièrement les mises à jour de sécurité

## 📞 Support

Pour toute question :
- **Issues GitHub** : https://github.com/jean-francois-arnould/FishingSpot_App/issues
- **Discussions** : https://github.com/jean-francois-arnould/FishingSpot_App/discussions

---

**Félicitations !** 🎉 Votre application est maintenant distribuée via AltStore !
