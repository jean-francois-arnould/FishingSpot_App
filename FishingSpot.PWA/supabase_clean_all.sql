-- ========================================
-- NETTOYAGE COMPLET DE LA BASE DE DONNÉES
-- ⚠️ ATTENTION : Supprime TOUTES les données !
-- ========================================

-- Désactiver temporairement les contraintes de clés étrangères
SET session_replication_role = 'replica';

-- Supprimer TOUTES les politiques RLS
DO $$
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT schemaname, tablename, policyname 
              FROM pg_policies 
              WHERE schemaname = 'public') 
    LOOP
        EXECUTE format('DROP POLICY IF EXISTS %I ON %I.%I', 
                      r.policyname, r.schemaname, r.tablename);
        RAISE NOTICE 'Politique supprimée: %.%', r.tablename, r.policyname;
    END LOOP;
END $$;

-- Supprimer toutes les tables dans le bon ordre (contraintes FK)
DROP TABLE IF EXISTS fish_catches CASCADE;
DROP TABLE IF EXISTS fishing_setups CASCADE;
DROP TABLE IF EXISTS rods CASCADE;
DROP TABLE IF EXISTS reels CASCADE;
DROP TABLE IF EXISTS lines CASCADE;
DROP TABLE IF EXISTS lures CASCADE;
DROP TABLE IF EXISTS leaders CASCADE;
DROP TABLE IF EXISTS hooks CASCADE;
DROP TABLE IF EXISTS user_profiles CASCADE;

-- Réactiver les contraintes
SET session_replication_role = 'origin';

-- Vérification
SELECT 
    '✅ Nettoyage complet terminé !' AS status,
    'Toutes les tables et politiques ont été supprimées' AS message,
    'Vous pouvez maintenant exécuter le script de création' AS next_step;
