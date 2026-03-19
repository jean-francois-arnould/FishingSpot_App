# Configuration PWA pour FishingSpot

## ✅ Corrections effectuées

1. **Structure corrigée** : Tous les fichiers ont été déplacés dans le dossier `wwwroot/`
   - `index.html`
   - `manifest.webmanifest`
   - `service-worker.js` et `service-worker.published.js`
   - Icônes PWA (`icon-192.png`, `icon-512.png`, `favicon.png`)
   - Ressources CSS et bibliothèques

2. **Workflow GitHub Actions mis à jour** : Le fichier `.github/workflows/blazor-deploy.yml` corrige automatiquement les chemins pour GitHub Pages lors du déploiement

3. **Service Worker configuré** : Le service worker est correctement configuré avec le manifeste d'assets

4. **Manifest PWA configuré** : Le fichier `manifest.webmanifest` contient toutes les informations nécessaires pour installer l'app

## 🚀 Comment tester la PWA localement

### Option 1 : Tester avec le serveur de développement (.NET)

```powershell
dotnet run
```

⚠️ **Note** : En mode développement, le service worker ne met pas en cache les fichiers (voir `wwwroot/service-worker.js`)

### Option 2 : Tester la version de production localement

1. Publier l'application :
```powershell
dotnet publish -c Release -o publish
```

2. Installer un serveur HTTP local (si ce n'est pas déjà fait) :
```powershell
dotnet tool install --global dotnet-serve
```

3. Lancer le serveur :
```powershell
dotnet serve -d publish/wwwroot -p 5000
```

4. Ouvrir dans le navigateur : `https://localhost:5000`

5. Tester l'installation PWA :
   - Chrome/Edge : Cliquer sur l'icône "Installer" dans la barre d'adresse
   - Firefox : Menu → Installer cette application
   - Safari (iOS) : Partager → Sur l'écran d'accueil

### Option 3 : Déployer sur GitHub Pages

Le déploiement se fait automatiquement via GitHub Actions lors d'un push sur `main` ou `WAP`.

1. Assurez-vous que GitHub Pages est activé dans les paramètres du dépôt :
   - Settings → Pages → Source : GitHub Actions

2. Push vos modifications :
```powershell
git add .
git commit -m "Fix PWA configuration"
git push origin main
```

3. Le workflow déploiera automatiquement sur : `https://jean-francois-arnould.github.io/FishingSpot_App/`

## 🔍 Vérifier que la PWA fonctionne

### Dans Chrome DevTools :

1. Ouvrir DevTools (F12)
2. Onglet **Application**
3. Vérifier :
   - **Manifest** : Doit afficher toutes les informations (nom, icônes, etc.)
   - **Service Workers** : Doit montrer le service worker enregistré et actif
   - **Cache Storage** : Doit contenir les assets en cache (en production uniquement)

### Test d'installation :

1. Ouvrir l'application dans le navigateur
2. Une invitation à installer l'application devrait apparaître
3. Une fois installée, l'application doit :
   - S'ouvrir dans une fenêtre autonome (sans barre d'adresse)
   - Fonctionner hors ligne (en mode production)
   - Être accessible depuis le menu démarrer / écran d'accueil

## 📱 Test sur mobile

1. Ouvrir l'URL dans Safari (iOS) ou Chrome (Android)
2. Pour iOS : Partager → Sur l'écran d'accueil
3. Pour Android : Menu → Installer l'application

## 🐛 Dépannage

### Le service worker ne s'enregistre pas

- Vérifiez que vous êtes en HTTPS ou localhost
- Vérifiez dans DevTools → Console les éventuelles erreurs
- Effacez le cache et rechargez (Ctrl+Shift+R)

### L'application ne fonctionne pas hors ligne

- Le mode développement ne met pas en cache les fichiers
- Utilisez une build de production pour tester le mode hors ligne

### Les icônes ne s'affichent pas

- Vérifiez que les fichiers `icon-192.png` et `icon-512.png` existent dans `wwwroot/`
- Vérifiez les chemins dans `manifest.webmanifest`

### L'installation PWA n'est pas proposée

- Vérifiez que le manifest est accessible : `/manifest.webmanifest`
- Vérifiez que le service worker est enregistré
- Assurez-vous d'être en HTTPS (sauf localhost)
- Certains navigateurs nécessitent plusieurs visites avant de proposer l'installation

## 📚 Ressources

- [Documentation Blazor PWA](https://learn.microsoft.com/aspnet/core/blazor/progressive-web-app)
- [Web App Manifest](https://developer.mozilla.org/en-US/docs/Web/Manifest)
- [Service Worker API](https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API)
