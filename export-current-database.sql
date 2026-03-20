-- ===================================================================
-- SCRIPT D'EXPORT DE LA STRUCTURE ACTUELLE DE LA BASE DE DONNÉES
-- ===================================================================
-- Exécutez ce script dans Supabase SQL Editor
-- Puis copiez TOUS les résultats et envoyez-les pour analyse
-- ===================================================================

-- ==========================================
-- 1. LISTE DES TABLES EXISTANTES
-- ==========================================
\echo '========== TABLES EXISTANTES =========='
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
  AND table_type = 'BASE TABLE'
ORDER BY table_name;

-- ==========================================
-- 2. STRUCTURE DE fish_catches
-- ==========================================
\echo '========== STRUCTURE: fish_catches =========='
SELECT 
    column_name,
    CASE 
        WHEN data_type = 'character varying' THEN 'varchar(' || character_maximum_length || ')'
        WHEN data_type = 'numeric' THEN 'numeric(' || numeric_precision || ',' || numeric_scale || ')'
        WHEN data_type = 'double precision' THEN 'double precision'
        WHEN data_type = 'integer' THEN 'integer'
        WHEN data_type = 'bigint' THEN 'bigint'
        WHEN data_type = 'text' THEN 'text'
        WHEN data_type = 'timestamp with time zone' THEN 'timestamptz'
        WHEN data_type = 'timestamp without time zone' THEN 'timestamp'
        WHEN data_type = 'date' THEN 'date'
        WHEN data_type = 'time without time zone' THEN 'time'
        WHEN data_type = 'boolean' THEN 'boolean'
        WHEN data_type = 'uuid' THEN 'uuid'
        ELSE data_type
    END as type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'fish_catches'
ORDER BY ordinal_position;

-- ==========================================
-- 3. STRUCTURE DE fishing_setups
-- ==========================================
\echo '========== STRUCTURE: fishing_setups =========='
SELECT 
    column_name,
    CASE 
        WHEN data_type = 'character varying' THEN 'varchar(' || character_maximum_length || ')'
        WHEN data_type = 'numeric' THEN 'numeric(' || numeric_precision || ',' || numeric_scale || ')'
        WHEN data_type = 'double precision' THEN 'double precision'
        WHEN data_type = 'integer' THEN 'integer'
        WHEN data_type = 'bigint' THEN 'bigint'
        WHEN data_type = 'text' THEN 'text'
        WHEN data_type = 'timestamp with time zone' THEN 'timestamptz'
        WHEN data_type = 'timestamp without time zone' THEN 'timestamp'
        WHEN data_type = 'date' THEN 'date'
        WHEN data_type = 'time without time zone' THEN 'time'
        WHEN data_type = 'boolean' THEN 'boolean'
        WHEN data_type = 'uuid' THEN 'uuid'
        ELSE data_type
    END as type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'fishing_setups'
ORDER BY ordinal_position;

-- ==========================================
-- 4. STRUCTURE DE user_profiles
-- ==========================================
\echo '========== STRUCTURE: user_profiles =========='
SELECT 
    column_name,
    CASE 
        WHEN data_type = 'character varying' THEN 'varchar(' || character_maximum_length || ')'
        WHEN data_type = 'numeric' THEN 'numeric(' || numeric_precision || ',' || numeric_scale || ')'
        WHEN data_type = 'double precision' THEN 'double precision'
        WHEN data_type = 'integer' THEN 'integer'
        WHEN data_type = 'bigint' THEN 'bigint'
        WHEN data_type = 'text' THEN 'text'
        WHEN data_type = 'timestamp with time zone' THEN 'timestamptz'
        WHEN data_type = 'timestamp without time zone' THEN 'timestamp'
        WHEN data_type = 'date' THEN 'date'
        WHEN data_type = 'time without time zone' THEN 'time'
        WHEN data_type = 'boolean' THEN 'boolean'
        WHEN data_type = 'uuid' THEN 'uuid'
        ELSE data_type
    END as type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'user_profiles'
ORDER BY ordinal_position;

-- ==========================================
-- 5. TOUTES LES CONTRAINTES
-- ==========================================
\echo '========== CONTRAINTES (PK, FK, etc.) =========='
SELECT
    tc.table_name, 
    tc.constraint_name, 
    tc.constraint_type,
    kcu.column_name,
    COALESCE(ccu.table_name, '') AS references_table,
    COALESCE(ccu.column_name, '') AS references_column
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
LEFT JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
WHERE tc.table_schema = 'public'
ORDER BY tc.table_name, tc.constraint_type, tc.constraint_name;

-- ==========================================
-- 6. INDEX
-- ==========================================
\echo '========== INDEX =========='
SELECT
    schemaname,
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'public'
ORDER BY tablename, indexname;

-- ==========================================
-- 7. POLITIQUES RLS (Row Level Security)
-- ==========================================
\echo '========== POLITIQUES RLS =========='
SELECT 
    schemaname,
    tablename,
    policyname,
    permissive,
    roles::text,
    cmd,
    CASE 
        WHEN qual IS NOT NULL THEN 'USING: ' || qual
        ELSE 'No USING clause'
    END as using_clause,
    CASE 
        WHEN with_check IS NOT NULL THEN 'WITH CHECK: ' || with_check
        ELSE 'No WITH CHECK clause'
    END as with_check_clause
FROM pg_policies
WHERE schemaname = 'public'
ORDER BY tablename, policyname;

-- ==========================================
-- 8. TRIGGERS
-- ==========================================
\echo '========== TRIGGERS =========='
SELECT 
    event_object_table as table_name,
    trigger_name,
    event_manipulation as event,
    action_timing as timing,
    action_statement
FROM information_schema.triggers
WHERE trigger_schema = 'public'
ORDER BY event_object_table, trigger_name;

-- ==========================================
-- 9. FONCTIONS CUSTOM
-- ==========================================
\echo '========== FONCTIONS PERSONNALISÉES =========='
SELECT 
    routine_name,
    routine_type,
    data_type as return_type,
    routine_definition
FROM information_schema.routines
WHERE routine_schema = 'public'
ORDER BY routine_name;

-- ==========================================
-- 10. STATISTIQUES DES DONNÉES
-- ==========================================
\echo '========== STATISTIQUES (nombre de lignes) =========='
SELECT 
    schemaname,
    relname as table_name,
    n_live_tup as row_count,
    n_dead_tup as dead_rows,
    last_vacuum,
    last_autovacuum
FROM pg_stat_user_tables
WHERE schemaname = 'public'
ORDER BY relname;

-- ===================================================================
-- FIN DU SCRIPT D'EXPORT
-- ===================================================================
-- Copiez TOUS les résultats de toutes les sections ci-dessus
-- et envoyez-les pour que je puisse créer un script de migration précis
-- ===================================================================
