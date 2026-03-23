-- ===================================================
-- FISHINGSPOT DATABASE ROLLBACK SCRIPT
-- ===================================================
-- Ce script permet de nettoyer/supprimer les améliorations
-- appliquées par improvements.sql
-- ⚠️ ATTENTION : Exécutez ceci uniquement si vous voulez
-- revenir en arrière ou résoudre des problèmes
-- ===================================================

-- ============================================
-- 1. SUPPRIMER LA TABLE user_statistics
-- ============================================
-- Ceci supprimera aussi le cache des statistiques

DROP TABLE IF EXISTS user_statistics CASCADE;

-- ============================================
-- 2. SUPPRIMER LES TRIGGERS
-- ============================================

DROP TRIGGER IF EXISTS update_fish_catches_updated_at ON fish_catches;
DROP TRIGGER IF EXISTS update_fishing_setups_updated_at ON fishing_setups;
DROP TRIGGER IF EXISTS update_user_profiles_updated_at ON user_profiles;
DROP TRIGGER IF EXISTS update_statistics_on_catch ON fish_catches;

-- ============================================
-- 3. SUPPRIMER LES FONCTIONS
-- ============================================

DROP FUNCTION IF EXISTS update_updated_at_column() CASCADE;
DROP FUNCTION IF EXISTS update_user_statistics() CASCADE;
DROP FUNCTION IF EXISTS cleanup_old_data() CASCADE;

-- ============================================
-- 4. SUPPRIMER LES VUES
-- ============================================

DROP VIEW IF EXISTS catches_with_details CASCADE;

-- ============================================
-- 5. SUPPRIMER LES INDEXES
-- ============================================

DROP INDEX IF EXISTS idx_fish_catches_user_date;
DROP INDEX IF EXISTS idx_fish_catches_species;
DROP INDEX IF EXISTS idx_fish_catches_setup;
DROP INDEX IF EXISTS idx_fishing_setups_user;
DROP INDEX IF EXISTS idx_rods_user;
DROP INDEX IF EXISTS idx_reels_user;
DROP INDEX IF EXISTS idx_lines_user;
DROP INDEX IF EXISTS idx_lures_user;
DROP INDEX IF EXISTS idx_leaders_user;
DROP INDEX IF EXISTS idx_hooks_user;
DROP INDEX IF EXISTS idx_fish_species_common_name;
DROP INDEX IF EXISTS idx_fishing_brands_category_name;
DROP INDEX IF EXISTS idx_user_statistics_last_updated;

-- ============================================
-- 6. SUPPRIMER LES COLONNES AJOUTÉES
-- ============================================

ALTER TABLE fish_catches DROP COLUMN IF EXISTS photo_thumbnail_url;
ALTER TABLE user_profiles DROP COLUMN IF EXISTS avatar_thumbnail_url;

-- ============================================
-- 7. DÉSACTIVER RLS (si nécessaire)
-- ============================================
-- Décommenter si vous voulez désactiver RLS

-- ALTER TABLE fish_catches DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE rods DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE reels DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE lines DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE lures DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE leaders DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE hooks DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE fishing_setups DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE user_profiles DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE fish_species DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE fishing_brands DISABLE ROW LEVEL SECURITY;

-- ============================================
-- 8. SUPPRIMER LES POLICIES RLS
-- ============================================

DROP POLICY IF EXISTS "Users see own catches" ON fish_catches;
DROP POLICY IF EXISTS "Users insert own catches" ON fish_catches;
DROP POLICY IF EXISTS "Users update own catches" ON fish_catches;
DROP POLICY IF EXISTS "Users delete own catches" ON fish_catches;
DROP POLICY IF EXISTS "Users manage own rods" ON rods;
DROP POLICY IF EXISTS "Users manage own reels" ON reels;
DROP POLICY IF EXISTS "Users manage own lines" ON lines;
DROP POLICY IF EXISTS "Users manage own lures" ON lures;
DROP POLICY IF EXISTS "Users manage own leaders" ON leaders;
DROP POLICY IF EXISTS "Users manage own hooks" ON hooks;
DROP POLICY IF EXISTS "Users manage own setups" ON fishing_setups;
DROP POLICY IF EXISTS "Users manage own profile" ON user_profiles;
DROP POLICY IF EXISTS "Users see own statistics" ON user_statistics;
DROP POLICY IF EXISTS "Anyone can view fish species" ON fish_species;
DROP POLICY IF EXISTS "Anyone can view fishing brands" ON fishing_brands;

-- ===================================================
-- FIN DU ROLLBACK
-- ===================================================

SELECT 'Rollback completed successfully!' as message;
