-- Script de nettoyage pour FishingSpot PWA
-- Exécutez ce script UNIQUEMENT si vous voulez tout réinitialiser
-- ⚠️ ATTENTION : Cela supprimera TOUTES vos données !

-- Supprimer les politiques RLS existantes
DROP POLICY IF EXISTS "Users can delete their own catches" ON fish_catches;
DROP POLICY IF EXISTS "Users can update their own catches" ON fish_catches;
DROP POLICY IF EXISTS "Users can insert their own catches" ON fish_catches;
DROP POLICY IF EXISTS "Users can view their own catches" ON fish_catches;

DROP POLICY IF EXISTS "Users can delete their own setups" ON fishing_setups;
DROP POLICY IF EXISTS "Users can update their own setups" ON fishing_setups;
DROP POLICY IF EXISTS "Users can insert their own setups" ON fishing_setups;
DROP POLICY IF EXISTS "Users can view their own setups" ON fishing_setups;

DROP POLICY IF EXISTS "Users can insert their own profile" ON user_profiles;
DROP POLICY IF EXISTS "Users can update their own profile" ON user_profiles;
DROP POLICY IF EXISTS "Users can view their own profile" ON user_profiles;

-- Supprimer les tables (dans l'ordre à cause des foreign keys)
DROP TABLE IF EXISTS fish_catches CASCADE;
DROP TABLE IF EXISTS fishing_setups CASCADE;
DROP TABLE IF EXISTS user_profiles CASCADE;

-- Message de confirmation
SELECT 'Toutes les tables et politiques ont été supprimées. Vous pouvez maintenant exécuter supabase_setup.sql' AS message;
