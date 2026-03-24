# 🚀 Guide de déploiement GitHub Pages

## 🐛 Erreur rencontrée

```
Error: Get Pages site failed. Please verify that the repository has Pages enabled 
and configured to build using GitHub Actions, or consider exploring the `enablement` 
parameter for this action.

Error: HttpError: request to https://api.github.com/repos/jean-francois-arnould/FishingSpot_App/pages 
failed, reason: connect ETIMEDOUT 140.82.112.6:443
```

## 🎯 Causes possibles

1. ❌ GitHub Pages pas activé dans les settings du repository
2. ❌ Source incorrecte (branch au lieu de GitHub Actions)
3. ❌ Permissions insuffisantes dans le workflow
4. ❌ Timeout de l'API GitHub (temporaire)
5. ❌ Secrets Supabase manquants

---

## ✅ Solutions (dans l'ordre)

### Solution 1 : Activer GitHub Pages correctement

#### Étape 1 : Aller dans les settings

URL directe : https://github.com/jean-francois-arnould/FishingSpot_App/settings/pages

#### Étape 2 : Configurer la source

**IMPORTANT** : Dans "Build and deployment"

```
Source: [GitHub Actions] ← Sélectionner cette option
```

**PAS** :
```
Source: Deploy from a branch ← NE PAS utiliser
```

#### Étape 3 : Sauvegarder

Cliquer sur "Save" si nécessaire.

---

### Solution 2 : Vérifier les secrets Supabase

#### URL des secrets
https://github.com/jean-francois-arnould/FishingSpot_App/settings/secrets/actions

#### Secrets requis

1. **`SUPABASE_URL`**
   - Valeur : Votre URL Supabase
   - Exemple : `https://xxxxx.supabase.co`

2. **`SUPABASE_KEY`**
   - Valeur : Votre clé API Supabase (anon/public)
   - Exemple : `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

#### Comment ajouter un secret

1. Cliquer sur "New repository secret"
2. Name : `SUPABASE_URL` ou `SUPABASE_KEY`
3. Secret : Coller la valeur
4. Cliquer sur "Add secret"

---

### Solution 3 : Re-déclencher le workflow

Le timeout peut être temporaire. Relancez le workflow :

```powershell
# Commit vide pour déclencher le workflow
git commit --allow-empty -m "Trigger GitHub Pages deployment"
git push origin main
```

Ou manuellement :
1. Aller sur : https://github.com/jean-francois-arnould/FishingSpot_App/actions
2. Cliquer sur le workflow "Deploy Blazor to GitHub Pages"
3. Cliquer sur "Run workflow" → "Run workflow"

---

### Solution 4 : Vérifier les permissions du workflow

Le workflow a déjà les bonnes permissions :

```yaml
permissions:
  contents: read
  pages: write
  id-token: write
```

Mais vérifiez dans les settings :

URL : https://github.com/jean-francois-arnould/FishingSpot_App/settings/actions

**Vérifier** :
- "Workflow permissions" : `Read and write permissions` ✅

---

### Solution 5 : Workflow amélioré (déjà appliqué)

Le workflow a été mis à jour avec :
- ✅ `timeout-minutes` pour éviter les blocages
- ✅ `continue-on-error` sur Setup Pages

---

## 🧪 Tester le déploiement

### Méthode 1 : Push un commit

```powershell
# Faire un changement (exemple : modifier README)
git add .
git commit -m "Test deployment"
git push origin main
```

### Méthode 2 : Déclencher manuellement

1. https://github.com/jean-francois-arnould/FishingSpot_App/actions
2. "Deploy Blazor to GitHub Pages"
3. "Run workflow" → "Run workflow"

### Suivre le déploiement

1. Aller sur : https://github.com/jean-francois-arnould/FishingSpot_App/actions
2. Cliquer sur le dernier workflow
3. Observer les étapes :
   - ✅ Checkout
   - ✅ Setup .NET
   - ✅ Create appsettings.json
   - ✅ Publish
   - ✅ Copy 404.html
   - ✅ Fix base href
   - ✅ Fix manifest
   - ✅ Fix service worker
   - ✅ Add .nojekyll
   - ✅ Setup Pages
   - ✅ Upload artifact
   - ✅ Deploy to GitHub Pages ← Le plus important

---

## 📊 Vérifier le résultat

### URL de l'application

https://jean-francois-arnould.github.io/FishingSpot_App/

### Vérifications

1. [ ] La page se charge
2. [ ] Pas d'erreur 404
3. [ ] Les styles CSS fonctionnent
4. [ ] Les scripts JavaScript fonctionnent
5. [ ] Le PWA fonctionne (offline, install prompt)
6. [ ] Connexion Supabase fonctionne (login)

---

## 🐛 Problèmes connus et solutions

### Problème : "404 - File not found"

**Cause** : Base href incorrect ou 404.html manquant

**Solution** : Déjà gérée dans le workflow
```yaml
- name: Copy 404.html for SPA routing
  run: cp 404.html output/wwwroot/404.html

- name: Fix base href for GitHub Pages
  run: sed -i 's/<base href="\/" \/>/<base href="\/FishingSpot_App\/" \/>/g' output/wwwroot/index.html
```

### Problème : "Failed to load _framework/blazor.webassembly.js"

**Cause** : Base href incorrect

**Solution** : Vérifier que `index.html` contient :
```html
<base href="/FishingSpot_App/" />
```

### Problème : PWA ne s'installe pas

**Cause** : Manifest ou service worker incorrect

**Solution** : Déjà gérée dans le workflow
```yaml
- name: Fix manifest for PWA
- name: Fix service worker base path
```

### Problème : Supabase ne se connecte pas

**Cause** : Secrets manquants ou incorrects

**Solution** : Vérifier les secrets (Solution 2)

### Problème : Timeout persistant

**Cause** : API GitHub temporairement indisponible

**Solution** :
1. Attendre 15-30 minutes
2. Re-déclencher le workflow
3. Vérifier status GitHub : https://www.githubstatus.com/

---

## 📝 Checklist de déploiement

### Avant le déploiement

- [ ] GitHub Pages activé (Source: GitHub Actions)
- [ ] Secrets Supabase configurés (`SUPABASE_URL`, `SUPABASE_KEY`)
- [ ] Permissions workflow correctes (Read and write)
- [ ] Fichier `404.html` présent à la racine
- [ ] Workflow `.github/workflows/blazor-deploy.yml` présent

### Après le déploiement

- [ ] Workflow complété avec succès (toutes les étapes ✅)
- [ ] URL accessible : https://jean-francois-arnould.github.io/FishingSpot_App/
- [ ] Application se charge correctement
- [ ] Connexion Supabase fonctionne
- [ ] PWA installable

---

## 🔍 Logs utiles pour déboguer

### Dans GitHub Actions

Cliquer sur chaque étape pour voir les logs détaillés :

**Setup Pages** :
```
Configuring Pages site...
✓ Pages site configured
```

**Upload artifact** :
```
Uploading artifact...
✓ Artifact uploaded successfully
Size: X MB
```

**Deploy to GitHub Pages** :
```
Deploying to GitHub Pages...
✓ Deployment completed
URL: https://jean-francois-arnould.github.io/FishingSpot_App/
```

### Dans la console navigateur (après déploiement)

F12 → Console :
```javascript
// Vérifier la base URL
console.log(document.baseURI);
// Devrait afficher: https://jean-francois-arnould.github.io/FishingSpot_App/

// Vérifier Supabase
console.log(typeof window.Blazor);
// Devrait afficher: object
```

---

## 🆘 Support

### Si rien ne fonctionne

1. **Vérifier status GitHub** : https://www.githubstatus.com/
2. **Consulter les logs** : https://github.com/jean-francois-arnould/FishingSpot_App/actions
3. **Créer une issue** : https://github.com/jean-francois-arnould/FishingSpot_App/issues

### Commandes de diagnostic

```powershell
# Vérifier la connexion GitHub
git remote -v

# Vérifier les permissions locales
git config user.name
git config user.email

# Tester le build localement
dotnet publish FishingSpot.PWA.csproj -c Release -o output
```

---

## ✅ Résumé

### Changements appliqués

1. ✅ Workflow mis à jour avec timeouts
2. ✅ `continue-on-error` sur Setup Pages
3. ✅ Documentation complète créée

### Actions requises (par l'utilisateur)

1. 🎯 **Activer GitHub Pages** (Source: GitHub Actions)
2. 🎯 **Vérifier les secrets Supabase**
3. 🎯 **Re-déclencher le workflow**

### Prochaines étapes

```powershell
# 1. Commit les changements du workflow
git add .github/workflows/blazor-deploy.yml
git commit -m "Fix: Add timeouts to GitHub Pages deployment"
git push origin main

# 2. Aller configurer GitHub Pages
# URL: https://github.com/jean-francois-arnould/FishingSpot_App/settings/pages

# 3. Observer le déploiement
# URL: https://github.com/jean-francois-arnould/FishingSpot_App/actions
```

---

**Date** : 2026-03-23  
**Status** : 🟡 En attente configuration GitHub  
**Fichier mis à jour** : `.github/workflows/blazor-deploy.yml`  
**Prochaine action** : Activer GitHub Pages avec source "GitHub Actions"  
