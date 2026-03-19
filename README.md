# FishingSpot 🎣

**Progressive Web App (PWA)** pour suivre vos prises de pêche avec photos, localisation GPS et statistiques.

## 🌐 Application en ligne

**[https://jean-francois-arnould.github.io/FishingSpot_App/](https://jean-francois-arnould.github.io/FishingSpot_App/)**

## ✨ Fonctionnalités

- 🐟 **Gestion des prises** - Enregistrez toutes vos captures
- 📸 **Photos** - Capturez vos plus belles prises
- 📍 **Géolocalisation GPS** - Position automatique
- 🎣 **Matériel** - Gérez votre équipement de pêche
- 🪝 **Montages** - Sauvegardez vos configurations
- 👤 **Profil** - Statistiques et historique
- 📱 **PWA** - Installable et fonctionne hors ligne

## 🚀 Technologies

- Blazor WebAssembly (.NET 10)
- Progressive Web App
- Supabase (base de données)
- Geolocation API

## 📱 Installation

### Sur ordinateur
1. Visiter l'URL
2. Cliquer sur l'icône d'installation
3. Utiliser comme application native

### Sur mobile
- **iOS** : Safari → Partager → Sur l'écran d'accueil
- **Android** : Chrome → Menu → Installer l'application

## 🛠️ Développement

```bash
# Cloner le repo
git clone https://github.com/jean-francois-arnould/FishingSpot_App.git

# Lancer en local
dotnet run

# Publier
dotnet publish FishingSpot.PWA.csproj -c Release
```

## 📄 Configuration

Copier `wwwroot/appsettings.template.json` vers `wwwroot/appsettings.json` et configurer avec vos identifiants Supabase.

## 👨‍💻 Auteur

**Jean-François Arnould**
- GitHub: [@jean-francois-arnould](https://github.com/jean-francois-arnould)

---

**🎣 Bonne pêche avec FishingSpot ! 🎣**
