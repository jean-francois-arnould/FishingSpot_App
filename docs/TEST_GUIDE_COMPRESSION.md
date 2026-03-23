# 🧪 Guide de test rapide - Compression d'images

## 📋 Checklist de test

### ✅ Préparation

1. [ ] Ouvrir l'application dans le navigateur
2. [ ] Ouvrir les outils développeur (F12)
3. [ ] Aller dans l'onglet "Console"
4. [ ] Naviguer vers "Ajouter une prise"

### 🔬 Test 1 : Photo standard (smartphone)

**Photo suggérée** : Photo de votre téléphone (2-4 MB, ~3000x4000px)

**Étapes** :
1. [ ] Cliquer sur "📷 Caméra" ou "🖼️ Galerie"
2. [ ] Sélectionner une photo
3. [ ] Observer la console

**À vérifier dans la console** :
```
📸 Traitement de la photo : IMG_XXXX.jpg (XXXXX bytes)
✅ Fichier lu en mémoire
✅ Conversion base64 terminée
🗜️ Compression de l'image...
✅ Compression terminée en XXXms        ← Devrait être < 500ms
📊 Taille originale: XXXXkb
📊 Taille compressée: XXXkb             ← Devrait être 300-500 KB
📊 Réduction: XX.X%                     ← Devrait être 80-90%
✅ Aperçu affiché
🌐 Upload vers Supabase...
✅ Upload terminé en XXXms              ← Devrait être < 1000ms sur 4G
```

**Message attendu** :
```
✅ Photo uploadée avec succès ! (-87% taille)
```

**Résultat attendu** :
- [ ] Compression : < 500ms
- [ ] Réduction : 80-90%
- [ ] Upload : < 1 seconde (4G)
- [ ] Qualité visuelle : Bonne

---

### 🔬 Test 2 : Petite photo (déjà optimisée)

**Photo suggérée** : Image web déjà compressée (< 500 KB)

**Résultat attendu** :
- [ ] Compression : < 200ms
- [ ] Réduction : 20-40% (car déjà compressée)
- [ ] Upload : < 500ms
- [ ] Pas de perte de qualité visible

---

### 🔬 Test 3 : Grande photo (haute résolution)

**Photo suggérée** : Photo haute résolution (4-5 MB, 4000x6000px)

**Résultat attendu** :
- [ ] Compression : < 800ms
- [ ] Réduction : 85-95%
- [ ] Upload : < 1.5 seconde
- [ ] Redimensionnement à 1200px largeur max

---

### 🔬 Test 4 : Mode hors-ligne

**Étapes** :
1. [ ] Ouvrir les outils développeur (F12)
2. [ ] Onglet "Network" → "Offline"
3. [ ] Uploader une photo

**Résultat attendu** :
- [ ] Compression fonctionne normalement
- [ ] Message : "📸 Photo ajoutée localement"
- [ ] Aperçu visible
- [ ] Photo en attente de sync

---

## 📊 Tableau de résultats

Remplissez ce tableau avec vos résultats de tests :

| Test | Taille originale | Taille compressée | Réduction | Temps compression | Temps upload | Total | ✅/❌ |
|------|-----------------|-------------------|-----------|-------------------|--------------|-------|-------|
| Photo standard | ___ MB | ___ KB | ___% | ___ ms | ___ ms | ___ ms | [ ] |
| Petite photo | ___ KB | ___ KB | ___% | ___ ms | ___ ms | ___ ms | [ ] |
| Grande photo | ___ MB | ___ KB | ___% | ___ ms | ___ ms | ___ ms | [ ] |
| Mode offline | ___ MB | ___ KB | ___% | ___ ms | N/A | ___ ms | [ ] |

---

## 🎯 Objectifs de performance

### ✅ Succès si :
- Temps de compression : **< 500ms**
- Réduction de taille : **> 80%** pour photos standard
- Temps d'upload : **< 1 seconde** sur connexion rapide
- Qualité visuelle : **Aucune dégradation visible**

### ⚠️ À ajuster si :
- Temps de compression : **> 800ms** → Réduire maxWidth
- Réduction de taille : **< 70%** → Réduire quality
- Qualité visuelle : **Pixelisée** → Augmenter quality ou maxWidth

---

## 🔧 Ajustements possibles

### Si les photos sont trop compressées (qualité insuffisante)

Modifier dans `AddCatch.razor` ligne ~735 :
```csharp
var compressed = await ImageCompression.CompressImageAsync(
    base64DataUrl, 
    maxWidth: 1200,        // Augmenter à 1600
    thumbnailSize: 150,    
    quality: 85            // Augmenter à 90
);
```

### Si la compression est trop lente

```csharp
var compressed = await ImageCompression.CompressImageAsync(
    base64DataUrl, 
    maxWidth: 1200,        // Réduire à 1000
    thumbnailSize: 150,    
    quality: 85            // Réduire à 80
);
```

### Si les fichiers sont encore trop gros

```csharp
var compressed = await ImageCompression.CompressImageAsync(
    base64DataUrl, 
    maxWidth: 1200,        // Réduire à 800
    thumbnailSize: 150,    
    quality: 85            // Réduire à 75
);
```

---

## 📸 Exemples de photos de test

### Bonnes photos de test
- ✅ Photo de smartphone récent (3-5 MB)
- ✅ Photo avec détails (paysage, architecture)
- ✅ Photo de poisson (sujet principal)
- ✅ Photo en plein soleil (couleurs vives)
- ✅ Photo en basse lumière (test de grain)

### Photos à éviter pour les tests
- ❌ Screenshots (déjà compressés)
- ❌ Images web (déjà optimisées)
- ❌ GIF ou PNG (format non optimal)
- ❌ Photos très anciennes (faible résolution)

---

## 🐛 Problèmes courants

### La compression ne fonctionne pas
- [ ] Vérifier que `@inject IImageCompressionService ImageCompression` est présent
- [ ] Vérifier les logs de la console pour erreurs JavaScript
- [ ] Vérifier que `imageCompression.js` est bien chargé

### Upload toujours lent
- [ ] Vérifier dans la console que la taille compressée est bien utilisée
- [ ] Vérifier la connexion réseau (outils dev → Network)
- [ ] Vérifier les logs "Taille compressée" dans la console

### Qualité d'image dégradée
- [ ] Augmenter le paramètre `quality` (85 → 90)
- [ ] Augmenter `maxWidth` (1200 → 1600)
- [ ] Vérifier la photo originale (peut-être déjà de mauvaise qualité)

---

## ✅ Validation finale

Une fois tous les tests effectués :

- [ ] Les temps sont conformes aux objectifs
- [ ] La qualité visuelle est satisfaisante
- [ ] Le mode offline fonctionne
- [ ] Les logs s'affichent correctement
- [ ] Message de succès avec pourcentage de réduction
- [ ] Aucune erreur dans la console

**Status** : _________________ (À compléter après tests)

**Commentaires** :
_____________________________________________
_____________________________________________
_____________________________________________

**Prochaines actions** :
_____________________________________________
_____________________________________________
_____________________________________________
