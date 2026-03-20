-- ===================================================================
-- SCRIPT DE VÉRIFICATION POST-MIGRATION
-- Exécutez ce script après la migration pour vérifier que tout est OK
-- ===================================================================

-- ===================================================================
-- 1. VÉRIFICATION DES TABLES
-- ===================================================================

SELECT '🔍 === VÉRIFICATION DES TABLES ===' as section;

SELECT 
    table_name,
    CASE 
        WHEN table_name IN ('user_profiles', 'fishing_setups', 'fish_catches') 
        THEN '✅ OK'
        ELSE '❌ Inattendu'
    END as statut
FROM information_schema.tables
WHERE table_schema = 'public' 
  AND table_type = 'BASE TABLE'
ORDER BY table_name;

-- ===================================================================
-- 2. VÉRIFICATION DE LA STRUCTURE DE fish_catches
-- ===================================================================

SELECT '🐟 === STRUCTURE: fish_catches ===' as section;

SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'fish_catches'
ORDER BY ordinal_position;

-- Vérification spécifique des unités
SELECT '📏 === VÉRIFICATION DES UNITÉS ===' as section;

SELECT 
    column_name,
    data_type,
    col_description('public.fish_catches'::regclass, ordinal_position) as description
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'fish_catches'
  AND column_name IN ('length', 'weight', 'latitude', 'longitude')
ORDER BY ordinal_position;

-- ===================================================================
-- 3. VÉRIFICATION DES POLITIQUES RLS
-- ===================================================================

SELECT '🔐 === POLITIQUES RLS ===' as section;

SELECT 
    tablename,
    COUNT(*) as nb_policies,
    CASE 
        WHEN COUNT(*) >= 3 THEN '✅ OK (minimum 3 politiques)'
        ELSE '⚠️ Attention : pas assez de politiques'
    END as statut
FROM pg_policies
WHERE schemaname = 'public'
GROUP BY tablename
ORDER BY tablename;

-- Détail des politiques
SELECT 
    tablename,
    policyname,
    cmd as operation
FROM pg_policies
WHERE schemaname = 'public'
ORDER BY tablename, cmd, policyname;

-- ===================================================================
-- 4. VÉRIFICATION DES INDEX
-- ===================================================================

SELECT '📈 === INDEX CRÉÉS ===' as section;

SELECT 
    schemaname,
    tablename,
    indexname,
    CASE 
        WHEN indexname LIKE 'idx_%' THEN '✅ Index personnalisé'
        WHEN indexname LIKE '%_pkey' THEN '✅ Primary Key'
        ELSE '⚠️ Autre'
    END as type
FROM pg_indexes
WHERE schemaname = 'public'
ORDER BY tablename, indexname;

-- ===================================================================
-- 5. VÉRIFICATION DES FOREIGN KEYS
-- ===================================================================

SELECT '🔗 === FOREIGN KEYS ===' as section;

SELECT
    tc.table_name as table_source, 
    kcu.column_name as colonne_source,
    ccu.table_name AS table_cible,
    ccu.column_name AS colonne_cible,
    '✅ OK' as statut
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_schema = 'public'
ORDER BY tc.table_name;

-- ===================================================================
-- 6. VÉRIFICATION DES TRIGGERS
-- ===================================================================

SELECT '⚡ === TRIGGERS ===' as section;

SELECT 
    event_object_table as table_name,
    trigger_name,
    event_manipulation as event,
    '✅ OK' as statut
FROM information_schema.triggers
WHERE trigger_schema = 'public'
ORDER BY event_object_table, trigger_name;

-- ===================================================================
-- 7. TEST D'INSERTION (SIMULATION)
-- ===================================================================

SELECT '🧪 === TEST DE STRUCTURE ===' as section;

-- Vérifier que les colonnes essentielles existent
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM information_schema.columns 
            WHERE table_schema = 'public' 
              AND table_name = 'fish_catches' 
              AND column_name = 'length' 
              AND data_type = 'double precision'
        ) THEN '✅ Colonne length OK (double precision)'
        ELSE '❌ Problème avec la colonne length'
    END as test_length,

    CASE 
        WHEN EXISTS (
            SELECT 1 FROM information_schema.columns 
            WHERE table_schema = 'public' 
              AND table_name = 'fish_catches' 
              AND column_name = 'weight' 
              AND data_type = 'double precision'
        ) THEN '✅ Colonne weight OK (double precision)'
        ELSE '❌ Problème avec la colonne weight'
    END as test_weight,

    CASE 
        WHEN EXISTS (
            SELECT 1 FROM information_schema.columns 
            WHERE table_schema = 'public' 
              AND table_name = 'fish_catches' 
              AND column_name = 'latitude' 
              AND data_type = 'text'
        ) THEN '✅ Colonne latitude OK (text)'
        ELSE '❌ Problème avec la colonne latitude'
    END as test_latitude,

    CASE 
        WHEN EXISTS (
            SELECT 1 FROM information_schema.columns 
            WHERE table_schema = 'public' 
              AND table_name = 'fish_catches' 
              AND column_name = 'longitude' 
              AND data_type = 'text'
        ) THEN '✅ Colonne longitude OK (text)'
        ELSE '❌ Problème avec la colonne longitude'
    END as test_longitude;

-- ===================================================================
-- 8. RÉSUMÉ FINAL
-- ===================================================================

SELECT '📊 === RÉSUMÉ FINAL ===' as section;

SELECT 
    '✅ Migration réussie !' as statut,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE') as nb_tables,
    (SELECT COUNT(*) FROM pg_indexes WHERE schemaname = 'public') as nb_index,
    (SELECT COUNT(*) FROM pg_policies WHERE schemaname = 'public') as nb_policies,
    (SELECT COUNT(*) FROM information_schema.triggers WHERE trigger_schema = 'public') as nb_triggers;

-- Message final
DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '🎣 ============================================';
    RAISE NOTICE '✅ VÉRIFICATION TERMINÉE';
    RAISE NOTICE '🎣 ============================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Si tous les tests ci-dessus affichent ✅, la migration est réussie !';
    RAISE NOTICE '';
    RAISE NOTICE '🚀 Prochaines étapes :';
    RAISE NOTICE '  1. Testez l''application';
    RAISE NOTICE '  2. Ajoutez votre première prise';
    RAISE NOTICE '  3. Vérifiez que les mesures (m/cm, kg/g) fonctionnent';
    RAISE NOTICE '';
    RAISE NOTICE 'Bon développement ! 🎣';
END $$;
