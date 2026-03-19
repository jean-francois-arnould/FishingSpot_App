# 🎨 Génération des icônes PWA depuis AppIcon.svg

Ce dossier contient les outils pour générer les icônes PNG nécessaires à partir de `AppIcon.svg`.

## 📦 Fichiers nécessaires

- ✅ `AppIcon.svg` - Icône source (déjà présent)
- 🔄 `icon-512.png` - Icône 512x512 (à générer)
- 🔄 `icon-192.png` - Icône 192x192 (à générer)

## 🚀 Méthodes de génération

### Méthode 1 : Via navigateur (Recommandé) ⭐

1. Ouvrez `wwwroot/generate-icons.html` dans votre navigateur
2. Les aperçus des icônes s'affichent automatiquement
3. Cliquez sur "⬇️ Télécharger icon-512.png"
4. Cliquez sur "⬇️ Télécharger icon-192.png"
5. Placez les deux fichiers téléchargés dans `wwwroot/`
6. Commit et push les changements

### Méthode 2 : Via PowerShell avec Inkscape

Si vous avez Inkscape installé :

```powershell
.\generate-icons.ps1
```

Si Inkscape n'est pas installé, téléchargez-le depuis https://inkscape.org/release/

### Méthode 3 : Via convertisseur en ligne

1. Allez sur https://cloudconvert.com/svg-to-png
2. Upload `wwwroot/AppIcon.svg`
3. Configurez la taille à 512x512, téléchargez → sauvegardez comme `icon-512.png`
4. Refaites avec 192x192 → sauvegardez comme `icon-192.png`
5. Placez les deux fichiers dans `wwwroot/`

## ✅ Vérification

Après génération, vérifiez que vous avez :

```
wwwroot/
├── AppIcon.svg ✅
├── icon-512.png ✅
├── icon-192.png ✅
└── manifest.webmanifest ✅
```

## 🎯 Design de l'icône

`AppIcon.svg` représente :
- 🐟 Un poisson (thème pêche)
- 📋 Une checklist de matériel
- 🪝 Un hameçon
- 🌊 Des vagues
- 🎨 Dégradé bleu (#0EA5E9 → #0369A1)

## 🔄 Mise à jour de l'icône

Si vous modifiez `AppIcon.svg` :

1. Éditez le fichier SVG
2. Re-générez les PNG via une des méthodes ci-dessus
3. Commit et push
4. Le déploiement GitHub Actions mettra à jour automatiquement

## 📱 Résultat

L'icône apparaîtra :
- Sur l'écran d'accueil quand l'app est installée
- Dans l'écran de splash au démarrage
- Dans la barre de titre
- Dans le sélecteur d'apps

## 🎨 Thème de couleur

Le manifest utilise maintenant :
- `background_color`: #0EA5E9 (bleu ciel)
- `theme_color`: #0EA5E9 (correspond à l'icône)
