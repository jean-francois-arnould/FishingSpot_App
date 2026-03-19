# 🔐 Configuration des Secrets GitHub pour Supabase

## 📋 Secrets à configurer

Vous devez ajouter **2 secrets** dans votre dépôt GitHub pour que l'application fonctionne sur GitHub Pages.

## 🔧 Comment ajouter les secrets

### 1️⃣ Aller sur la page des secrets

**URL directe** : https://github.com/jean-francois-arnould/FishingSpot_App/settings/secrets/actions

Ou manuellement :
1. Aller sur votre dépôt : https://github.com/jean-francois-arnould/FishingSpot_App
2. Cliquer sur **Settings** (⚙️)
3. Dans la barre latérale, cliquer sur **Secrets and variables** → **Actions**

### 2️⃣ Ajouter le premier secret : SUPABASE_URL

1. Cliquer sur **New repository secret**
2. **Name** : `SUPABASE_URL`
3. **Value** : `https://kejapcjuczjhyfdeshrv.supabase.co`
4. Cliquer sur **Add secret**

### 3️⃣ Ajouter le deuxième secret : SUPABASE_KEY

1. Cliquer sur **New repository secret**
2. **Name** : `SUPABASE_KEY`
3. **Value** : 
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtlamFwY2p1Y3pqaHlmZGVzaHJ2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzM4MzE3MDYsImV4cCI6MjA4OTQwNzcwNn0.H3DHAmmC_aXekMXiUZfWq1FPTMYlzHqPx70O4A0ui3Y
```
4. Cliquer sur **Add secret**

## ✅ Vérification

Une fois les secrets ajoutés, vous devriez voir :

```
SUPABASE_URL
SUPABASE_KEY
```

Dans la liste des secrets.

## 🚀 Comment ça fonctionne

### Pendant le déploiement

Le workflow GitHub Actions :

1. **Crée automatiquement** `wwwroot/appsettings.json` avec vos secrets
2. **Compile** le projet avec cette configuration
3. **Déploie** sur GitHub Pages

### Fichier créé pendant le build

```json
{
  "Supabase": {
    "Url": "https://kejapcjuczjhyfdeshrv.supabase.co",
    "Key": "eyJhbGci..."
  }
}
```

## 🔒 Sécurité

### ✅ Bonnes pratiques respectées

- ✅ Les secrets ne sont **jamais** visibles dans les logs
- ✅ Le fichier `appsettings.json` local n'est **pas** commité (`.gitignore`)
- ✅ Les secrets sont **injectés** uniquement pendant le build
- ✅ Seuls les admins du dépôt peuvent voir/modifier les secrets

### ⚠️ Note importante

La clé API Supabase est une **clé publique** (anon/public). Elle est conçue pour être utilisée côté client et n'expose pas de données sensibles. Les règles de sécurité (RLS) dans Supabase protègent vos données.

## 🧪 Tester après configuration

1. **Ajouter les secrets** comme décrit ci-dessus
2. **Pousser un commit** (ou re-déclencher le workflow manuellement)
3. **Attendre le déploiement** (~2-3 minutes)
4. **Tester l'application** : https://jean-francois-arnould.github.io/FishingSpot_App/
5. **Essayer de créer un compte** et de se connecter

## 🐛 Dépannage

### Si l'authentification ne fonctionne pas

1. **Vérifier les secrets** sont bien configurés (Settings → Secrets)
2. **Vérifier les logs du workflow** :
   - Aller sur Actions
   - Cliquer sur le dernier workflow
   - Vérifier qu'il n'y a pas d'erreurs

3. **Ouvrir la console du navigateur** (F12) :
   - Onglet Console
   - Chercher des erreurs Supabase

### Erreur "Configuration Supabase manquante"

Si vous voyez ce message, cela signifie que les secrets ne sont pas correctement injectés. Vérifiez :
- Les noms des secrets (sensible à la casse)
- Que vous avez bien poussé le nouveau workflow

## 📝 Modifier les secrets

Si vous devez changer les clés Supabase :

1. Aller sur Settings → Secrets and variables → Actions
2. Cliquer sur le secret à modifier
3. Cliquer sur **Update secret**
4. Entrer la nouvelle valeur
5. Sauvegarder

Le prochain déploiement utilisera automatiquement les nouvelles valeurs.

---

## 🎯 Prochaine étape

**Allez maintenant ajouter les secrets sur GitHub** puis revenez pour tester l'application ! 🚀

https://github.com/jean-francois-arnould/FishingSpot_App/settings/secrets/actions
