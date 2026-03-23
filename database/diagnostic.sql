-- ===================================================
-- FISHINGSPOT DATABASE DIAGNOSTIC SCRIPT
-- ===================================================
-- Ce script vérifie que toutes les améliorations
-- ont été correctement appliquées
-- ===================================================

\echo '===================================================';
\echo 'FISHINGSPOT DATABASE DIAGNOSTIC';
\echo '===================================================';
\echo '';

-- ============================================
-- 1. VÉRIFIER LES TABLES
-- ============================================
\echo '1. TABLES';
\echo '---------';

SELECT 
    'user_statistics' as table_name,
    CASE WHEN EXISTS (
        SELECT FROM pg_tables 
        WHERE schemaname = 'public' 
        AND tablename = 'user_statistics'
    ) THEN '✅ EXISTS' ELSE '❌ MISSING' END as status;

\echo '';

-- ============================================
-- 2. VÉRIFIER LES INDEXES
-- ============================================
\echo '2. INDEXES';
\echo '---------';

SELECT 
    indexname,
    tablename,
    '✅' as status
FROM pg_indexes 
WHERE schemaname = 'public' 
    AND indexname IN (
        'idx_fish_catches_user_date',
        'idx_fish_catches_species',
        'idx_fish_catches_setup',
        'idx_fishing_setups_user',
        'idx_rods_user',
        'idx_reels_user',
        'idx_lines_user',
        'idx_lures_user',
        'idx_leaders_user',
        'idx_hooks_user',
        'idx_fish_species_common_name',
        'idx_fishing_brands_category_name',
        'idx_user_statistics_last_updated'
    )
ORDER BY indexname;

\echo '';

-- ============================================
-- 3. VÉRIFIER LES COLONNES AJOUTÉES
-- ============================================
\echo '3. NOUVELLES COLONNES';
\echo '--------------------';

SELECT 
    table_name,
    column_name,
    data_type,
    '✅' as status
FROM information_schema.columns
WHERE table_schema = 'public'
    AND (
        (table_name = 'fish_catches' AND column_name = 'photo_thumbnail_url')
        OR (table_name = 'user_profiles' AND column_name = 'avatar_thumbnail_url')
    );

\echo '';

-- ============================================
-- 4. VÉRIFIER LES TRIGGERS
-- ============================================
\echo '4. TRIGGERS';
\echo '----------';

SELECT 
    trigger_name,
    event_object_table,
    '✅' as status
FROM information_schema.triggers
WHERE trigger_schema = 'public'
    AND trigger_name IN (
        'update_fish_catches_updated_at',
        'update_fishing_setups_updated_at',
        'update_user_profiles_updated_at',
        'update_statistics_on_catch'
    )
ORDER BY trigger_name;

\echo '';

-- ============================================
-- 5. VÉRIFIER LES FONCTIONS
-- ============================================
\echo '5. FONCTIONS';
\echo '------------';

SELECT 
    routine_name,
    '✅' as status
FROM information_schema.routines
WHERE routine_schema = 'public'
    AND routine_name IN (
        'update_updated_at_column',
        'update_user_statistics',
        'cleanup_old_data'
    )
ORDER BY routine_name;

\echo '';

-- ============================================
-- 6. VÉRIFIER LES VUES
-- ============================================
\echo '6. VUES';
\echo '-------';

SELECT 
    table_name,
    '✅' as status
FROM information_schema.views
WHERE table_schema = 'public'
    AND table_name = 'catches_with_details';

\echo '';

-- ============================================
-- 7. VÉRIFIER RLS
-- ============================================
\echo '7. ROW LEVEL SECURITY';
\echo '---------------------';

SELECT 
    schemaname,
    tablename,
    CASE WHEN rowsecurity THEN '✅ ENABLED' ELSE '❌ DISABLED' END as rls_status
FROM pg_tables
WHERE schemaname = 'public'
    AND tablename IN (
        'fish_catches',
        'rods',
        'reels',
        'lines',
        'lures',
        'leaders',
        'hooks',
        'fishing_setups',
        'user_profiles',
        'user_statistics',
        'fish_species',
        'fishing_brands'
    )
ORDER BY tablename;

\echo '';

-- ============================================
-- 8. VÉRIFIER LES POLICIES RLS
-- ============================================
\echo '8. RLS POLICIES';
\echo '---------------';

SELECT 
    schemaname,
    tablename,
    policyname,
    '✅' as status
FROM pg_policies
WHERE schemaname = 'public'
ORDER BY tablename, policyname;

\echo '';

-- ============================================
-- 9. VÉRIFIER LES DOUBLONS DE PRISES
-- ============================================
\echo '9. DOUBLONS DE PRISES';
\echo '--------------------';

-- Détecter les prises potentiellement dupliquées (même utilisateur, poisson, date, heure, lieu)
WITH duplicates AS (
    SELECT 
        user_id,
        fish_name,
        catch_date,
        catch_time,
        location_name,
        latitude,
        longitude,
        COUNT(*) as nombre_doublons,
        string_agg(id::text, ', ' ORDER BY created_at) as ids,
        MIN(created_at) as premiere_creation,
        MAX(created_at) as derniere_creation
    FROM fish_catches
    GROUP BY user_id, fish_name, catch_date, catch_time, location_name, latitude, longitude
    HAVING COUNT(*) > 1
)
SELECT 
    fish_name,
    catch_date,
    catch_time,
    nombre_doublons,
    ids,
    premiere_creation,
    derniere_creation,
    CASE 
        WHEN derniere_creation - premiere_creation < INTERVAL '5 seconds' THEN '🔴 SUSPECT (< 5s)'
        WHEN derniere_creation - premiere_creation < INTERVAL '1 minute' THEN '🟠 PROBABLE (< 1min)'
        ELSE '🟢 NORMAL'
    END as statut
FROM duplicates
ORDER BY derniere_creation DESC;

\echo '';
\echo '💡 Doublons suspects = créés à moins de 5 secondes d''intervalle';
\echo '💡 Si des doublons sont détectés, utilisez le script cleanup_duplicates.sql';
\echo '';

-- ============================================
-- 10. STATISTIQUES DES TABLES
-- ============================================
\echo '10. TAILLE DES TABLES';
\echo '--------------------';

SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as size
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC
LIMIT 10;

\echo '';

-- ============================================
-- 11. RÉSUMÉ
-- ============================================
\echo '11. RÉSUMÉ';
\echo '----------';

SELECT 
    'Tables' as category,
    COUNT(*) as count
FROM pg_tables
WHERE schemaname = 'public'
UNION ALL
SELECT 
    'Indexes',
    COUNT(*)
FROM pg_indexes
WHERE schemaname = 'public'
UNION ALL
SELECT 
    'Triggers',
    COUNT(*)
FROM information_schema.triggers
WHERE trigger_schema = 'public'
UNION ALL
SELECT 
    'Functions',
    COUNT(*)
FROM information_schema.routines
WHERE routine_schema = 'public'
UNION ALL
SELECT 
    'Views',
    COUNT(*)
FROM information_schema.views
WHERE table_schema = 'public'
UNION ALL
SELECT 
    'RLS Policies',
    COUNT(*)
FROM pg_policies
WHERE schemaname = 'public';

\echo '';
\echo '===================================================';
\echo 'DIAGNOSTIC TERMINÉ';
\echo '===================================================';
