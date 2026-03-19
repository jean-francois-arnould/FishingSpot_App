# 🎣 Plan de Migration FishingSpot V2.0

## 📋 Résumé des Changements Majeurs

### 1. **Application entièrement en français** ✅
### 2. **Gestion de photos améliorée** 📸
   - Prendre une photo avec l'appareil
   - Sélectionner depuis la bibliothèque

### 3. **Géolocalisation automatique** 📍
   - Bouton "Me localiser"
   - Remplissage automatique des coordonnées GPS

### 4. **Nouvelle architecture Matériel/Setups** 🎣
   - Séparer les équipements (cannes, moulinets, etc.)
   - Créer des setups en combinant les équipements
   - Setup "actuel" par défaut

---

## 🚧 État Actuel

### ✅ Terminé :
- Script SQL V2 créé (`supabase_v2_migration.sql`)
- Modèles C# pour les équipements créés
- Modèle FishingSetup mis à jour

### ⚠️ En cours :
- Pages Razor à mettre à jour (erreurs de compilation)

### 🔜 À faire :
- Service pour gérer les équipements
- Pages de gestion des équipements
- Traduction complète en français
- Upload de photos
- Géolocalisation

---

## 📝 Étapes de Migration

### ÉTAPE 1 : Exécuter le nouveau script SQL ✅
```sql
-- Dans Supabase SQL Editor
-- Exécutez : supabase_v2_migration.sql
```

### ÉTAPE 2 : Mettre à jour les services
- Créer `IEquipmentService`
- Implémenter méthodes CRUD pour chaque type d'équipement
- Mettre à jour `ISupabaseService` pour les nouveaux setups

### ÉTAPE 3 : Supprimer les anciennes pages
- `AddSetup.razor` (incompatible avec la nouvelle structure)
- `Setups.razor` (à recréer)

### ÉTAPE 4 : Créer les nouvelles pages
**Gestion du Matériel :**
- `/equipment` - Liste de tous les équipements
- `/equipment/rods` - Gestion des cannes
- `/equipment/reels` - Gestion des moulinets
- `/equipment/lines` - Gestion des fils
- `/equipment/lures` - Gestion des leurres
- `/equipment/leaders` - Gestion des bas de ligne
- `/equipment/hooks` - Gestion des hameçons

**Gestion des Setups :**
- `/setups` - Liste des setups (avec badge "Actuel")
- `/setups/add` - Créer un setup (sélection d'équipements)
- `/setups/edit/{id}` - Modifier un setup

### ÉTAPE 5 : Traduction en français
- Créer un fichier de ressources
- Traduire toutes les interfaces

### ÉTAPE 6 : Ajouter les fonctionnalités photos
- Utiliser `<input type="file" accept="image/*" capture="camera">`
- Upload vers Supabase Storage
- Générer URL publique

### ÉTAPE 7 : Ajouter la géolocalisation
- Utiliser l'API Navigator Geolocation
- Bouton "Me localiser"
- Géocodage inverse pour obtenir l'adresse

---

## 🎯 Approche Progressive

Vu l'ampleur des changements, je recommande :

### Option A : Migration complète (Plus long mais propre)
1. Exécuter le script SQL
2. Recréer toutes les pages
3. Traduire tout en français
4. Ajouter les nouvelles fonctionnalités

### Option B : Migration progressive (Recommandé)
1. **Phase 1** : Base de données + Services
   - Exécuter SQL
   - Créer services équipements
   - Garder l'ancienne UI temporairement

2. **Phase 2** : Nouvelle UI Matériel
   - Pages de gestion d'équipements
   - Une catégorie à la fois

3. **Phase 3** : Nouvelle UI Setups
   - Recréer pages setups
   - Intégration avec équipements

4. **Phase 4** : Fonctionnalités avancées
   - Photos
   - Géolocalisation
   - Traduction française

---

## 💡 Recommandation

Je vous propose de commencer par **Option B - Phase 1** :

1. ✅ Exécuter le script SQL (déjà créé)
2. 🔧 Créer les services pour les équipements
3. 🔧 Créer UNE page simple pour tester (ex: cannes)
4. ✅ Valider que tout fonctionne
5. 📦 Ensuite continuer avec les autres catégories

**Voulez-vous que je continue avec cette approche progressive, ou préférez-vous que je crée tout d'un coup ?**

---

## ⏱️ Estimation du temps

- **Option A (tout d'un coup)** : ~2-3h de travail
- **Option B (progressif)** : 
  - Phase 1 : 30 min
  - Phase 2 : 1h  
  - Phase 3 : 45 min
  - Phase 4 : 1h

**Total similaire, mais Phase par Phase permet de tester au fur et à mesure !**

---

## 🤔 Question pour vous

Avant de continuer, dites-moi :

1. **Voulez-vous la migration progressive ou tout d'un coup ?**
2. **Les anciennes prises (fish_catches) doivent-elles être migrées ou on repart de zéro ?**
3. **Pour les photos, avez-vous déjà un compte Supabase Storage ou faut-il utiliser une autre solution ?**

Une fois que vous me confirmez, je continue avec l'implémentation ! 🚀
