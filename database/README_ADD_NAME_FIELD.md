# Migration : Ajout du champ Name aux montages

## 📋 Résumé
Cette migration ajoute un champ **`name` (nom/titre)** obligatoire à la table `fishing_setups` pour identifier facilement les montages.

## 🎯 Objectifs
1. Ajouter un champ `name TEXT NOT NULL` à la table `fishing_setups`
2. Migrer les données existantes (utiliser description ou générer un nom)
3. Mettre à jour le code pour utiliser le nouveau champ

## 📦 Étape 1 : Exécuter la migration SQL

### Dans Supabase SQL Editor :

```sql
-- Fichier: database/ADD_NAME_TO_FISHING_SETUPS.sql
```

1. Allez sur **Supabase Dashboard** → votre projet
2. Cliquez sur **SQL Editor** dans le menu de gauche
3. Créez une nouvelle requête
4. Copiez-collez le contenu du fichier `database/ADD_NAME_TO_FISHING_SETUPS.sql`
5. Cliquez sur **Run** (Exécuter)

### Vérifications après migration :

```sql
-- Vérifier que toutes les colonnes sont bien présentes
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_name = 'fishing_setups'
ORDER BY ordinal_position;

-- Vérifier les données migrées
SELECT id, name, description, is_current, is_favorite 
FROM fishing_setups 
ORDER BY created_at DESC 
LIMIT 10;
```

## ✅ Résultat attendu

Après la migration, la table `fishing_setups` aura cette structure :

| Colonne | Type | Nullable | Description |
|---------|------|----------|-------------|
| id | serial | NOT NULL | Identifiant unique |
| user_id | uuid | NOT NULL | Propriétaire du montage |
| **name** | **text** | **NOT NULL** | **Nom/titre du montage** ⭐ NOUVEAU |
| description | text | NULL | Description détaillée (optionnel) |
| rod_id | bigint | NULL | Référence à la canne |
| reel_id | bigint | NULL | Référence au moulinet |
| line_id | bigint | NULL | Référence au fil |
| lure_id | bigint | NULL | Référence au leurre |
| leader_id | bigint | NULL | Référence au bas de ligne |
| hook_id | bigint | NULL | Référence à l'hameçon |
| is_current | boolean | NULL | Montage actuel ? |
| is_favorite | boolean | NULL | Montage favori ? |
| created_at | timestamptz | NULL | Date de création |
| updated_at | timestamptz | NULL | Date de modification |

## 🔄 Changements dans le code

### 1. Modèle (`Models/FishingSetup.cs`)
- ✅ Ajout de la propriété `Name` avec `[JsonPropertyName("name")]`

### 2. Service (`SupabaseService.cs`)
- ✅ Ajout du champ `name` dans `AddSetupAsync()`
- ✅ Ajout du champ `name` dans `UpdateSetupAsync()`

### 3. Formulaire de création (`Pages/Montages/Ajouter.razor`)
- ✅ Ajout d'un champ texte pour le nom (obligatoire)
- ✅ Validation : le nom ne peut pas être vide

### 4. Formulaire de modification (`Pages/Montages/Modifier.razor`)
- ✅ Ajout d'un champ texte pour modifier le nom

### 5. Page liste (`Pages/Montages/Index.razor`)
- ✅ Affichage de `setup.Name` au lieu de `GetSetupTitle()`
- ✅ Sélecteur dropdown utilise `setup.Name`
- ✅ Accordéons fonctionnels (Favoris + Tous les montages)

### 6. Page détails (`Pages/Montages/Details.razor`)
- ✅ Affichage de `setup.Name` comme titre

## 🎨 Améliorations UX

### Page Index (`/montages`)
1. **Sélecteur de montage actuel** en haut
   - Dropdown avec tous les montages (par nom)
   - Bouton "Voir détails" pour le montage sélectionné

2. **Accordéon "Mes Favoris"** ⭐
   - Affiche uniquement les montages favoris
   - Ouvert par défaut si des favoris existent
   - Compte affiché : "⭐ Mes Favoris (3)"

3. **Accordéon "Mes Montages"** 🎣
   - Affiche tous les montages
   - Fermé par défaut si des favoris existent
   - Compte affiché : "🎣 Mes Montages (10)"

## 🧪 Tests recommandés

1. **Créer un nouveau montage** avec un nom
2. **Modifier un montage existant** et changer le nom
3. **Vérifier les accordéons** se déplient/replient correctement
4. **Changer le montage actuel** via le dropdown
5. **Ajouter/retirer des favoris** et voir l'accordéon Favoris

## ⚠️ Important

- Le champ **`name` est obligatoire** lors de la création/modification
- Les anciens montages ont été migrés automatiquement avec :
  - Leur `description` si elle existait
  - Ou un nom généré : `"Montage #<ID>"`

## 📚 Documentation

- **Migration SQL** : `database/ADD_NAME_TO_FISHING_SETUPS.sql`
- **Modèle** : `Models/FishingSetup.cs`
- **Formulaires** : 
  - `Pages/Montages/Ajouter.razor`
  - `Pages/Montages/Modifier.razor`
- **Vues** :
  - `Pages/Montages/Index.razor`
  - `Pages/Montages/Details.razor`
