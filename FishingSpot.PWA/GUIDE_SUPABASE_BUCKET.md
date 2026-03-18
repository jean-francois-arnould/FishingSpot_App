# 📸 Guide : Créer le Bucket Supabase Storage

## ⚠️ ACTION REQUISE AVANT D'UTILISER L'UPLOAD DE PHOTOS

L'application est maintenant prête à uploader des photos, **MAIS** vous devez d'abord créer le bucket de stockage sur Supabase.

---

## 🚀 ÉTAPES (5 minutes)

### 1. Connexion à Supabase Dashboard
- Aller sur : **https://supabase.com/dashboard**
- Se connecter avec votre compte
- Sélectionner le projet : **kejapcjuczjhyfdeshrv** (ou votre projet FishingSpot)

### 2. Accéder à Storage
- Dans le menu de gauche, cliquer sur **"Storage"** 📦
- Vous verrez la liste des buckets existants (probablement vide)

### 3. Créer le Nouveau Bucket
- Cliquer sur le bouton **"New bucket"** en haut à droite
- Une fenêtre popup s'ouvre

### 4. Configuration du Bucket
Remplir les champs suivants :

| Champ | Valeur | Important |
|-------|--------|-----------|
| **Name** | `fishing-photos` | ⚠️ EXACTEMENT ce nom (minuscules, tiret) |
| **Public bucket** | ✅ **Coché (OUI)** | ⚠️ OBLIGATOIRE pour que les URLs fonctionnent |
| **File size limit** | `5242880` (5 MB) | Optionnel mais recommandé |
| **Allowed MIME types** | `image/jpeg, image/png, image/webp, image/jpg` | Optionnel |

**Screenshot de la configuration** :
```
┌─────────────────────────────────────────────┐
│ Create a new bucket                          │
├─────────────────────────────────────────────┤
│                                               │
│ Name *                                        │
│ ┌───────────────────────────────────────┐   │
│ │ fishing-photos                        │   │
│ └───────────────────────────────────────┘   │
│                                               │
│ ☑ Public bucket                              │
│   Allow public access to all files           │
│                                               │
│ File size limit (bytes)                      │
│ ┌───────────────────────────────────────┐   │
│ │ 5242880                               │   │
│ └───────────────────────────────────────┘   │
│                                               │
│ Allowed MIME types (optional)                │
│ ┌───────────────────────────────────────┐   │
│ │ image/jpeg, image/png, image/webp    │   │
│ └───────────────────────────────────────┘   │
│                                               │
│         [Cancel]  [Create bucket]            │
└─────────────────────────────────────────────┘
```

### 5. Créer le Bucket
- Cliquer sur **"Create bucket"**
- Le bucket `fishing-photos` apparaît dans la liste

---

## ✅ Vérification

Après création, vous devriez voir :
```
Storage > fishing-photos
├── 📁 catches/     (sera créé automatiquement lors du 1er upload)
└── Status: Public ✅
```

---

## 🔒 Politiques de Sécurité (Optionnel mais Recommandé)

Par défaut, le bucket public permet à tout le monde de **lire** les fichiers, mais **vous seuls** pouvez uploader.

Si vous voulez restreindre l'upload aux utilisateurs authentifiés uniquement :

### 1. Aller dans "Policies" du bucket
- Cliquer sur `fishing-photos` dans la liste des buckets
- Onglet **"Policies"**

### 2. Créer une politique INSERT
Cliquer sur **"New policy"** puis **"Create a policy from scratch"**

**Configuration** :
```sql
Policy name: "Users can upload their own photos"
Operation: INSERT
Policy definition:

(bucket_id = 'fishing-photos'::text)
AND
(auth.role() = 'authenticated')
```

### 3. Créer une politique SELECT (lecture publique)
Cliquer sur **"New policy"** puis **"Create a policy from scratch"**

**Configuration** :
```sql
Policy name: "Public read access"
Operation: SELECT
Policy definition:

(bucket_id = 'fishing-photos'::text)
```

### 4. Créer une politique DELETE (optionnel)
Si vous voulez permettre aux utilisateurs de supprimer leurs propres photos :

```sql
Policy name: "Users can delete their own photos"
Operation: DELETE
Policy definition:

(bucket_id = 'fishing-photos'::text)
AND
(auth.role() = 'authenticated')
```

---

## 🧪 Tester l'Upload

### 1. Lancer l'application
```powershell
cd "C:\Users\J.Arnould\OneDrive - EY\Documents\GitHub\FishingSpot\FishingSpot.PWA\"
dotnet run
```

### 2. Aller sur Add Catch
- Naviguer vers `/catches/add`

### 3. Uploader une photo
- Cliquer sur **"Choisir un fichier"**
- Sélectionner une image (max 5 MB)
- Attendre le message : **"✅ Photo uploadée avec succès !"**

### 4. Vérifier dans Supabase
- Retourner sur Storage > fishing-photos
- Vous devriez voir un dossier `catches/` avec votre photo uploadée

---

## ❌ Dépannage

### Erreur : "Bucket not found"
- ✅ Vérifier que le bucket s'appelle exactement `fishing-photos` (minuscules, tiret)
- ✅ Vérifier que vous êtes sur le bon projet Supabase

### Erreur : "Permission denied"
- ✅ Vérifier que **"Public bucket"** est coché
- ✅ Vérifier les politiques RLS (voir section ci-dessus)

### Erreur : "File too large"
- ✅ Réduire la taille de l'image (max 5 MB)
- ✅ Augmenter la limite dans la configuration du bucket

### La photo ne s'affiche pas après upload
- ✅ Vérifier que le bucket est **public**
- ✅ Vérifier l'URL retournée dans la console du navigateur (F12)
- ✅ Format attendu : `https://kejapcjuczjhyfdeshrv.supabase.co/storage/v1/object/public/fishing-photos/catches/[uuid]_[filename]`

---

## 📚 Ressources

- Documentation Supabase Storage : https://supabase.com/docs/guides/storage
- API Upload : https://supabase.com/docs/reference/javascript/storage-from-upload

---

**Une fois le bucket créé, l'upload de photos fonctionnera immédiatement dans votre application !** 🎉
