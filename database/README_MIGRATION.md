# 📋 GUIDE D'EXÉCUTION - MIGRATION SQL

## 🎯 Objectif
Migrer la table `fishing_setups` pour utiliser des **foreign keys** au lieu de champs texte.

---

## ⚠️ AVERTISSEMENTS

1. **Les montages existants seront supprimés** lors de la migration
2. Assurez-vous d'avoir **du matériel enregistré** (cannes, moulinets, etc.) avant de créer des montages
3. **Sauvegardez vos données** si nécessaire

---

## 📝 ÉTAPES D'EXÉCUTION

### **Étape 1 : Ouvrir Supabase SQL Editor**

1. Allez sur https://supabase.com
2. Sélectionnez votre projet **FishingSpot**
3. Cliquez sur **SQL Editor** dans le menu de gauche

---

### **Étape 2 : Exécuter le script de migration**

1. Créez une **New Query**
2. Copiez-collez le contenu du fichier :
   ```
   database/MIGRATION_EXECUTE_THIS.sql
   ```
3. Cliquez sur **Run** (ou appuyez sur Ctrl+Enter)
4. Attendez que le script se termine

**Résultat attendu :**
```
✅ ALTER TABLE succeeded
✅ ADD CONSTRAINT succeeded
✅ CREATE INDEX succeeded
```

---

### **Étape 3 : Vérifier la migration**

1. Dans une **nouvelle query**
2. Copiez-collez le contenu du fichier :
   ```
   database/VERIFICATION_POST_MIGRATION.sql
   ```
3. Cliquez sur **Run**
4. Vérifiez les résultats :

**Colonnes attendues :**
- ✅ `id` (integer)
- ✅ `user_id` (uuid)
- ✅ `rod_id` (bigint)
- ✅ `reel_id` (bigint)
- ✅ `line_id` (bigint)
- ✅ `lure_id` (bigint)
- ✅ `leader_id` (bigint)
- ✅ `hook_id` (bigint)
- ✅ `description` (text)
- ✅ `is_current` (boolean)
- ✅ `is_favorite` (boolean)
- ✅ `created_at` (timestamp)
- ✅ `updated_at` (timestamp)

**Contraintes FK attendues :**
- ✅ `fishing_setups_rod_id_fkey` → `rods(id)`
- ✅ `fishing_setups_reel_id_fkey` → `reels(id)`
- ✅ `fishing_setups_line_id_fkey` → `lines(id)`
- ✅ `fishing_setups_lure_id_fkey` → `lures(id)`
- ✅ `fishing_setups_leader_id_fkey` → `leaders(id)`
- ✅ `fishing_setups_hook_id_fkey` → `hooks(id)`

---

### **Étape 4 : Tester dans l'application**

1. **Ajoutez du matériel** (si ce n'est pas déjà fait) :
   - Allez dans "Mon Matériel"
   - Ajoutez au moins :
     - 1 canne
     - 1 moulinet
     - 1 fil

2. **Créez un montage** :
   - Allez dans "Mes Montages"
   - Cliquez sur "Nouveau montage"
   - Sélectionnez votre matériel dans les dropdowns
   - Enregistrez

3. **Vérifiez l'affichage** :
   - Le montage devrait s'afficher avec les détails du matériel
   - Ex: "🎣 Shimano Zodias (2.4m)"

---

## 🔍 EN CAS DE PROBLÈME

### Erreur : "relation does not exist"
**Solution :** Assurez-vous que les tables `rods`, `reels`, `lines`, etc. existent.
Vérifiez avec :
```sql
SELECT tablename FROM pg_tables WHERE schemaname = 'public';
```

### Erreur : "cannot add foreign key constraint"
**Solution :** Il existe peut-être des données invalides. Nettoyez d'abord :
```sql
DELETE FROM fishing_setups;
```
Puis réexécutez la migration.

### L'application ne charge pas les montages
**Solution :** 
1. Ouvrez la **Console du navigateur** (F12)
2. Vérifiez les erreurs
3. Vérifiez que le token d'authentification est valide

---

## 📊 SCHÉMA AVANT/APRÈS

### AVANT (champs texte)
```
fishing_setups
├── id
├── user_id
├── rod_brand (TEXT)      ❌
├── rod_model (TEXT)      ❌
├── rod_length (NUMERIC)  ❌
├── reel_brand (TEXT)     ❌
...
```

### APRÈS (foreign keys)
```
fishing_setups
├── id
├── user_id
├── rod_id (FK → rods)      ✅
├── reel_id (FK → reels)    ✅
├── line_id (FK → lines)    ✅
├── lure_id (FK → lures)    ✅
├── leader_id (FK → leaders)✅
├── hook_id (FK → hooks)    ✅
├── description
├── is_current
├── is_favorite
```

---

## ✅ CHECKLIST

- [ ] Script `MIGRATION_EXECUTE_THIS.sql` exécuté
- [ ] Script `VERIFICATION_POST_MIGRATION.sql` exécuté
- [ ] 13 colonnes dans `fishing_setups`
- [ ] 6 contraintes FK créées
- [ ] 6 index créés
- [ ] Matériel ajouté dans l'application
- [ ] Montage de test créé et affiché correctement

---

## 🎉 C'EST TERMINÉ !

Votre application utilise maintenant des **foreign keys** pour les montages.

**Avantages :**
- ✅ Sélection rapide du matériel
- ✅ Pas de duplication de données
- ✅ Cohérence garantie
- ✅ Statistiques possibles

**Profitez de la nouvelle fonctionnalité ! 🎣**
