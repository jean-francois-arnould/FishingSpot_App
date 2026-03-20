# Guide d'export de la base de données Supabase

## Méthode 1 : Via l'interface Supabase (la plus simple)

### Étape 1 : Obtenir la structure actuelle

1. Connectez-vous à **Supabase** : https://app.supabase.com
2. Sélectionnez votre projet
3. Allez dans **Database** → **Tables**
4. Pour chaque table, cliquez dessus pour voir sa structure

### Étape 2 : Copier les informations

Pour chaque table (`fish_catches`, `fishing_setups`, `user_profiles`), notez :
- Les **colonnes** (nom, type, nullable, default)
- Les **contraintes** (primary key, foreign keys)
- Les **index**
- Les **politiques RLS** (Row Level Security)

### Étape 3 : Utiliser SQL Editor pour export

1. Allez dans **SQL Editor**
2. Exécutez cette requête pour obtenir la structure de toutes les tables :

```sql
-- Obtenir la structure de fish_catches
SELECT 
    column_name,
    data_type,
    character_maximum_length,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'fish_catches'
ORDER BY ordinal_position;

-- Obtenir la structure de fishing_setups
SELECT 
    column_name,
    data_type,
    character_maximum_length,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'fishing_setups'
ORDER BY ordinal_position;

-- Obtenir la structure de user_profiles
SELECT 
    column_name,
    data_type,
    character_maximum_length,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'user_profiles'
ORDER BY ordinal_position;

-- Obtenir les contraintes (foreign keys)
SELECT
    tc.table_name, 
    tc.constraint_name, 
    tc.constraint_type,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
  AND tc.table_schema = kcu.table_schema
LEFT JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
  AND ccu.table_schema = tc.table_schema
WHERE tc.table_schema = 'public'
ORDER BY tc.table_name, tc.constraint_name;

-- Obtenir les politiques RLS
SELECT 
    schemaname,
    tablename,
    policyname,
    permissive,
    roles,
    cmd,
    qual,
    with_check
FROM pg_policies
WHERE schemaname = 'public'
ORDER BY tablename, policyname;
```

3. Copiez les résultats et envoyez-les moi

---

## Méthode 2 : Export via pg_dump (pour utilisateurs avancés)

### Prérequis
Installer PostgreSQL client tools

### Commande
```bash
pg_dump -h db.xxxxxxxxxxxxx.supabase.co \
  -U postgres \
  -d postgres \
  --schema-only \
  --no-owner \
  --no-privileges \
  -f current-database-structure.sql
```

Remplacez `xxxxxxxxxxxxx` par votre ID de projet Supabase.

Le mot de passe se trouve dans **Settings** → **Database** → **Connection string**

---

## Méthode 3 : Script SQL complet (plus rapide)

Exécutez ce script dans **SQL Editor** de Supabase et copiez-moi le résultat :

```sql
-- SCRIPT D'EXPORT COMPLET DE LA STRUCTURE

-- ==========================================
-- 1. LISTE DES TABLES
-- ==========================================
SELECT 'TABLES:' as section;
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
  AND table_type = 'BASE TABLE';

-- ==========================================
-- 2. STRUCTURE DÉTAILLÉE DE CHAQUE TABLE
-- ==========================================

-- fish_catches
SELECT '=== FISH_CATCHES ===' as section;
SELECT 
    ordinal_position,
    column_name,
    data_type,
    CASE 
        WHEN character_maximum_length IS NOT NULL 
        THEN data_type || '(' || character_maximum_length || ')'
        WHEN numeric_precision IS NOT NULL 
        THEN data_type || '(' || numeric_precision || ',' || numeric_scale || ')'
        ELSE data_type
    END as full_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'fish_catches'
ORDER BY ordinal_position;

-- fishing_setups
SELECT '=== FISHING_SETUPS ===' as section;
SELECT 
    ordinal_position,
    column_name,
    data_type,
    CASE 
        WHEN character_maximum_length IS NOT NULL 
        THEN data_type || '(' || character_maximum_length || ')'
        WHEN numeric_precision IS NOT NULL 
        THEN data_type || '(' || numeric_precision || ',' || numeric_scale || ')'
        ELSE data_type
    END as full_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'fishing_setups'
ORDER BY ordinal_position;

-- user_profiles
SELECT '=== USER_PROFILES ===' as section;
SELECT 
    ordinal_position,
    column_name,
    data_type,
    CASE 
        WHEN character_maximum_length IS NOT NULL 
        THEN data_type || '(' || character_maximum_length || ')'
        WHEN numeric_precision IS NOT NULL 
        THEN data_type || '(' || numeric_precision || ',' || numeric_scale || ')'
        ELSE data_type
    END as full_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'user_profiles'
ORDER BY ordinal_position;

-- ==========================================
-- 3. CONTRAINTES ET RELATIONS
-- ==========================================
SELECT '=== CONSTRAINTS ===' as section;
SELECT
    tc.table_name, 
    tc.constraint_name, 
    tc.constraint_type,
    kcu.column_name,
    COALESCE(ccu.table_name, '') AS foreign_table_name,
    COALESCE(ccu.column_name, '') AS foreign_column_name
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
  AND tc.table_schema = kcu.table_schema
LEFT JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
  AND ccu.table_schema = tc.table_schema
WHERE tc.table_schema = 'public'
ORDER BY tc.table_name, tc.constraint_name;

-- ==========================================
-- 4. INDEX
-- ==========================================
SELECT '=== INDEXES ===' as section;
SELECT
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'public'
ORDER BY tablename, indexname;

-- ==========================================
-- 5. POLITIQUES RLS
-- ==========================================
SELECT '=== RLS POLICIES ===' as section;
SELECT 
    tablename,
    policyname,
    cmd,
    qual,
    with_check
FROM pg_policies
WHERE schemaname = 'public'
ORDER BY tablename, policyname;

-- ==========================================
-- 6. TRIGGERS
-- ==========================================
SELECT '=== TRIGGERS ===' as section;
SELECT 
    trigger_name,
    event_manipulation,
    event_object_table,
    action_statement
FROM information_schema.triggers
WHERE trigger_schema = 'public'
ORDER BY event_object_table, trigger_name;
```

---

## Ce que je ferai avec ces informations

Une fois que vous m'aurez envoyé les résultats, je pourrai :

1. ✅ Créer un script de migration précis
2. ✅ Identifier les différences avec le nouveau schéma
3. ✅ Préserver les données importantes si nécessaire
4. ✅ Créer un script de rollback (retour en arrière)
5. ✅ Optimiser les index et les politiques RLS

---

## Format de réponse idéal

Envoyez-moi soit :
- **Capture d'écran** des résultats du SQL Editor
- **Copier-coller** du texte des résultats
- **Fichier .sql** si vous utilisez pg_dump

Je pourrai alors créer un script de migration sur mesure ! 🎯
