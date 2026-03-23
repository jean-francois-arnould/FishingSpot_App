-- ===================================================
-- TEST DE DIAGNOSTIC - Problème ID non retourné
-- ===================================================
-- Exécutez ce script dans Supabase SQL Editor
-- pour diagnostiquer le problème
-- ===================================================

-- 1. Vérifier que RLS est activé
SELECT 
    schemaname,
    tablename,
    CASE WHEN rowsecurity THEN '✅ ENABLED' ELSE '❌ DISABLED' END as rls_status
FROM pg_tables
WHERE schemaname = 'public' 
    AND tablename = 'fish_catches';

-- 2. Vérifier les policies existantes
SELECT 
    policyname,
    cmd,
    qual,
    with_check
FROM pg_policies
WHERE schemaname = 'public' 
    AND tablename = 'fish_catches'
ORDER BY cmd;

-- 3. Vérifier la structure de la table
SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
    AND table_name = 'fish_catches'
ORDER BY ordinal_position;

-- 4. Test d'insertion AVEC return=representation
-- Note: Remplacez 'YOUR_USER_ID' par votre vrai user_id
DO $$
DECLARE
    test_user_id uuid;
    new_catch_id integer;
BEGIN
    -- Récupérer le premier user_id disponible
    SELECT id INTO test_user_id FROM auth.users LIMIT 1;

    IF test_user_id IS NULL THEN
        RAISE NOTICE '❌ Aucun utilisateur trouvé dans auth.users';
        RETURN;
    END IF;

    RAISE NOTICE '📝 Test avec user_id: %', test_user_id;

    -- Tester l'insertion
    INSERT INTO fish_catches (
        user_id,
        fish_name,
        catch_date,
        length,
        weight
    ) VALUES (
        test_user_id,
        'Test Diagnostic',
        CURRENT_DATE,
        50,
        1.5
    )
    RETURNING id INTO new_catch_id;

    IF new_catch_id IS NOT NULL AND new_catch_id > 0 THEN
        RAISE NOTICE '✅ Insertion réussie avec ID: %', new_catch_id;

        -- Nettoyer le test
        DELETE FROM fish_catches WHERE id = new_catch_id;
        RAISE NOTICE '🧹 Test nettoyé';
    ELSE
        RAISE NOTICE '❌ Insertion a retourné un ID invalide';
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        RAISE NOTICE '❌ Erreur lors du test: %', SQLERRM;
END $$;

-- 5. Vérifier que la séquence fonctionne
SELECT 
    'fish_catches_id_seq' as sequence_name,
    last_value,
    is_called
FROM fish_catches_id_seq;

-- 6. Vérifier les triggers sur la table
SELECT 
    trigger_name,
    event_manipulation,
    action_timing,
    action_statement
FROM information_schema.triggers
WHERE event_object_table = 'fish_catches'
ORDER BY trigger_name;

-- ===================================================
-- RÉSULTATS ATTENDUS
-- ===================================================
-- 1. RLS: ✅ ENABLED
-- 2. Policies: 4 policies (SELECT, INSERT, UPDATE, DELETE)
-- 3. Structure: id (integer, PK), user_id (uuid, NOT NULL)
-- 4. Test insertion: ✅ Insertion réussie avec ID: XXX
-- 5. Séquence: last_value > 0, is_called = true
-- 6. Triggers: update_fish_catches_updated_at, update_statistics_on_catch
-- ===================================================
