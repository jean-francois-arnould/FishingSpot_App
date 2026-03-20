# 🎣 Guide de Migration - FishingSpot v2.0

## ⚠️ IMPORTANT
- Cette migration **SUPPRIME TOUTES LES DONNÉES**
- Aucune sauvegarde ne sera effectuée
- Les anciennes tables (rods, reels, lines, lures, leaders, hooks) seront supprimées
- La nouvelle structure est **simplifiée** et correspond au code actuel

---

## 🚀 Étapes de migration (5 minutes)

### Étape 1 : Ouvrir Supabase
1. Allez sur https://app.supabase.com
2. Sélectionnez votre projet **FishingSpot**
3. Cliquez sur **SQL Editor** dans le menu de gauche

### Étape 2 : Exécuter la migration
1. Cliquez sur **New query** (nouvelle requête)
2. Ouvrez le fichier `database-migration.sql` dans VS Code
3. **Copiez TOUT le contenu** du fichier (Ctrl+A puis Ctrl+C)
4. **Collez-le** dans l'éditeur SQL de Supabase (Ctrl+V)
5. Cliquez sur **RUN** (ou appuyez sur F5)
6. ⏳ Attendez quelques secondes...

### Étape 3 : Vérifier le résultat
Vous devriez voir dans les résultats :
```
✅ Toutes les anciennes tables ont été supprimées
🎣 Migration terminée avec succès !
```

Et un tableau montrant les 3 tables créées :
- `fish_catches`
- `fishing_setups`
- `user_profiles`

### Étape 4 : Vérification post-migration (optionnel)
1. Créez une **nouvelle requête** dans SQL Editor
2. Ouvrez le fichier `verify-migration.sql`
3. Copiez et collez tout son contenu
4. Cliquez sur **RUN**
5. Vérifiez que tous les tests affichent ✅

---

## 📊 Ce qui change

### Anciennes tables (SUPPRIMÉES)
- ❌ `rods` (cannes)
- ❌ `reels` (moulinets)
- ❌ `lines` (lignes)
- ❌ `lures` (leurres)
- ❌ `leaders` (bas de ligne)
- ❌ `hooks` (hameçons)

### Nouvelles tables (CRÉÉES)
- ✅ `user_profiles` - Profils utilisateurs simplifiés
- ✅ `fishing_setups` - Montages tout-en-un (toutes les infos dans une seule table)
- ✅ `fish_catches` - Prises de pêche avec unités correctes

### Structure de fishing_setups (simplifié)
Au lieu de 6 tables séparées, tout est dans une seule table :
```sql
- rod_brand, rod_model, rod_length, rod_power
- reel_brand, reel_model, reel_type
- line_type, line_diameter, line_breaking_strength
- hook_size, bait_type
- description, notes
- is_current, is_favorite
```

### Structure de fish_catches
```sql
- fish_name (TEXT)
- latitude, longitude (TEXT) ← Changé de double precision
- length (DOUBLE PRECISION) ← En CENTIMÈTRES
- weight (DOUBLE PRECISION) ← En GRAMMES
- photo_url, location_name, notes
- catch_date, catch_time
- setup_id (référence vers fishing_setups)
```

---

## 🎯 Unités de mesure

### ✅ AVANT (dans l'interface)
- Longueur : Sélection séparée **Mètres + Centimètres**
- Poids : Sélection séparée **Kilogrammes + Grammes**

### ✅ APRÈS (dans la base de données)
- `length` = **Total en centimètres**
  - Exemple : 1m 50cm → stocké comme `150`
- `weight` = **Total en grammes**
  - Exemple : 2kg 500g → stocké comme `2500`

### 🔄 Conversion automatique
Le code C# fait la conversion automatiquement :
```csharp
// UI → DB
length_cm = (meters * 100) + centimeters
weight_g = (kilograms * 1000) + grams

// DB → UI  
meters = length_cm / 100
centimeters = length_cm % 100
kilograms = weight_g / 1000
grams = weight_g % 1000
```

---

## 🔐 Sécurité (RLS activé)

Chaque utilisateur voit **uniquement ses propres données** :
- ✅ Isolation complète entre utilisateurs
- ✅ Politiques RLS sur toutes les tables
- ✅ Foreign keys vers `auth.users`

---

## 🧪 Tests après migration

### Test 1 : Connexion
1. Lancez l'application
2. Connectez-vous ou créez un compte
3. Vérifiez que vous arrivez sur la page d'accueil

### Test 2 : Ajouter une prise
1. Cliquez sur **➕ Ajouter une prise**
2. Sélectionnez un poisson dans la liste (ex: "Brochet")
3. Saisissez la longueur : **1m 50cm**
4. Saisissez le poids : **2kg 500g**
5. Cliquez sur **✅ Enregistrer**
6. Vérifiez que vous êtes redirigé vers la liste

### Test 3 : Voir la prise
1. Cliquez sur la prise dans la liste
2. Vérifiez que les mesures s'affichent correctement
3. Vérifiez la géolocalisation
4. Testez le bouton **📋 Copier GPS**

### Test 4 : Créer un montage
1. Allez dans **Montages**
2. Créez un nouveau montage
3. Remplissez les informations
4. Marquez-le comme **favori** ou **actuel**

---

## 🐛 En cas de problème

### Erreur "relation does not exist"
→ Relancez le script de migration, une table n'a pas été créée

### Erreur "permission denied"
→ Vérifiez que vous êtes connecté avec le rôle `postgres`

### Erreur lors de l'insertion
→ Vérifiez dans la console (F12) les logs détaillés
→ Relancez le script `verify-migration.sql`

### Les données ne s'affichent pas
→ Ouvrez la console (F12) et cherchez les erreurs
→ Vérifiez que vous êtes bien connecté
→ Vérifiez les politiques RLS dans Supabase

---

## 📞 Checklist finale

Avant de dire que la migration est terminée, vérifiez :

- [ ] Le script `database-migration.sql` s'est exécuté sans erreur
- [ ] Le script `verify-migration.sql` affiche des ✅ partout
- [ ] Vous pouvez vous connecter à l'application
- [ ] Vous pouvez ajouter une prise avec mesures m/cm et kg/g
- [ ] La géolocalisation fonctionne automatiquement
- [ ] Le bouton de copie GPS fonctionne
- [ ] Vous pouvez créer un montage
- [ ] La liste des prises se rafraîchit automatiquement

---

## 🎉 C'est tout !

Une fois ces étapes terminées, votre base de données est prête !

L'application devrait fonctionner parfaitement avec :
- ✅ 34 poissons pré-définis
- ✅ Mesures avec unités séparées (m/cm, kg/g)
- ✅ Géolocalisation automatique
- ✅ Copie GPS
- ✅ Rafraîchissement automatique
- ✅ Structure simplifiée et performante

**Bonne pêche ! 🎣**
