# 📱 Build Android sur Windows (PLUS SIMPLE)

En attendant de configurer iOS, vous pouvez **immédiatement** builder et distribuer la version Android !

## 🚀 Build Android APK (2 minutes)

### Option 1 : Via Visual Studio (Interface)

1. **Ouvrez** `FishingSpot.sln` dans Visual Studio
2. **Configuration** : Release
3. **Plateforme** : Android
4. **Clic droit** sur le projet > **Publier**
5. **Ad Hoc** > **Créer un nouveau keystore** (ou utilisez un existant)
6. **Publier** > Le fichier APK sera généré

### Option 2 : Via PowerShell (Ligne de commande)

```powershell
# Script automatique de build Android
.\build-android.ps1
```

Créons ce script maintenant...
