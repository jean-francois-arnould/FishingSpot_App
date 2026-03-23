-- ===================================================
-- FISHINGSPOT DATABASE IMPROVEMENTS
-- ===================================================
-- Ce script contient toutes les améliorations recommandées
-- pour la base de données FishingSpot sur Supabase
-- ===================================================

-- ============================================
-- 1. INDEXES POUR PERFORMANCE
-- ============================================
-- Ces indexes améliorent significativement les performances des requêtes

CREATE INDEX IF NOT EXISTS idx_fish_catches_user_date 
ON fish_catches(user_id, catch_date DESC);

CREATE INDEX IF NOT EXISTS idx_fish_catches_species 
ON fish_catches(species_id) WHERE species_id IS NOT NULL;

CREATE INDEX IF NOT EXISTS idx_fish_catches_setup 
ON fish_catches(setup_id) WHERE setup_id IS NOT NULL;

CREATE INDEX IF NOT EXISTS idx_fishing_setups_user 
ON fishing_setups(user_id);

CREATE INDEX IF NOT EXISTS idx_rods_user 
ON rods(user_id);

CREATE INDEX IF NOT EXISTS idx_reels_user 
ON reels(user_id);

CREATE INDEX IF NOT EXISTS idx_lines_user 
ON lines(user_id);

CREATE INDEX IF NOT EXISTS idx_lures_user 
ON lures(user_id);

CREATE INDEX IF NOT EXISTS idx_leaders_user 
ON leaders(user_id);

CREATE INDEX IF NOT EXISTS idx_hooks_user 
ON hooks(user_id);

-- Index pour recherche par nom d'espèce (insensible à la casse)
CREATE INDEX IF NOT EXISTS idx_fish_species_common_name 
ON fish_species(LOWER(common_name));

-- Index pour recherche par marque d'équipement
CREATE INDEX IF NOT EXISTS idx_fishing_brands_category_name 
ON fishing_brands(category, LOWER(name));

-- ============================================
-- 2. COLONNES POUR IMAGES OPTIMISÉES
-- ============================================
-- Ajouter colonnes pour thumbnails (miniatures)

ALTER TABLE fish_catches 
ADD COLUMN IF NOT EXISTS photo_thumbnail_url text;

ALTER TABLE user_profiles 
ADD COLUMN IF NOT EXISTS avatar_thumbnail_url text;

-- ============================================
-- 3. TABLE DE STATISTIQUES (CACHE)
-- ============================================
-- Cache des statistiques utilisateur pour améliorer les performances

-- Créer la table si elle n'existe pas (sans la contrainte en double)
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'user_statistics') THEN
        CREATE TABLE user_statistics (
            user_id uuid PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
            total_catches integer DEFAULT 0,
            total_species integer DEFAULT 0,
            biggest_catch_id integer REFERENCES fish_catches(id) ON DELETE SET NULL,
            heaviest_catch_id integer REFERENCES fish_catches(id) ON DELETE SET NULL,
            favorite_location text,
            favorite_species text,
            last_catch_date date,
            last_updated timestamp with time zone DEFAULT now()
        );
    END IF;
END $$;

-- Index sur user_statistics
CREATE INDEX IF NOT EXISTS idx_user_statistics_last_updated 
ON user_statistics(last_updated);

-- ============================================
-- 4. TRIGGERS POUR updated_at AUTOMATIQUE
-- ============================================
-- Fonction pour mettre à jour automatiquement updated_at

CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Appliquer le trigger sur les tables appropriées
DROP TRIGGER IF EXISTS update_fish_catches_updated_at ON fish_catches;
CREATE TRIGGER update_fish_catches_updated_at 
    BEFORE UPDATE ON fish_catches
    FOR EACH ROW 
    EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_fishing_setups_updated_at ON fishing_setups;
CREATE TRIGGER update_fishing_setups_updated_at 
    BEFORE UPDATE ON fishing_setups
    FOR EACH ROW 
    EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_user_profiles_updated_at ON user_profiles;
CREATE TRIGGER update_user_profiles_updated_at 
    BEFORE UPDATE ON user_profiles
    FOR EACH ROW 
    EXECUTE FUNCTION update_updated_at_column();

-- ============================================
-- 5. FONCTION DE MISE À JOUR DES STATISTIQUES
-- ============================================
-- Fonction appelée automatiquement après insertion/suppression de prise

CREATE OR REPLACE FUNCTION update_user_statistics()
RETURNS TRIGGER AS $$
BEGIN
    -- Recalculer les statistiques pour l'utilisateur concerné
    INSERT INTO user_statistics (
        user_id, 
        total_catches, 
        total_species,
        biggest_catch_id,
        heaviest_catch_id,
        favorite_species,
        last_catch_date,
        last_updated
    )
    SELECT 
        user_id,
        COUNT(*) as total_catches,
        COUNT(DISTINCT fish_name) as total_species,
        (SELECT id FROM fish_catches WHERE user_id = NEW.user_id ORDER BY length DESC LIMIT 1) as biggest_catch_id,
        (SELECT id FROM fish_catches WHERE user_id = NEW.user_id ORDER BY weight DESC LIMIT 1) as heaviest_catch_id,
        (SELECT fish_name FROM fish_catches WHERE user_id = NEW.user_id GROUP BY fish_name ORDER BY COUNT(*) DESC LIMIT 1) as favorite_species,
        MAX(catch_date) as last_catch_date,
        now() as last_updated
    FROM fish_catches
    WHERE user_id = NEW.user_id
    GROUP BY user_id
    ON CONFLICT (user_id) 
    DO UPDATE SET
        total_catches = EXCLUDED.total_catches,
        total_species = EXCLUDED.total_species,
        biggest_catch_id = EXCLUDED.biggest_catch_id,
        heaviest_catch_id = EXCLUDED.heaviest_catch_id,
        favorite_species = EXCLUDED.favorite_species,
        last_catch_date = EXCLUDED.last_catch_date,
        last_updated = now();

    RETURN NEW;
END;
$$ language 'plpgsql';

-- Trigger pour mettre à jour les statistiques
DROP TRIGGER IF EXISTS update_statistics_on_catch ON fish_catches;
CREATE TRIGGER update_statistics_on_catch
    AFTER INSERT OR UPDATE OR DELETE ON fish_catches
    FOR EACH ROW
    EXECUTE FUNCTION update_user_statistics();

-- ============================================
-- 6. ROW LEVEL SECURITY (RLS)
-- ============================================
-- Assurer que chaque utilisateur ne voit que ses propres données

-- Activer RLS sur toutes les tables utilisateur
ALTER TABLE fish_catches ENABLE ROW LEVEL SECURITY;
ALTER TABLE rods ENABLE ROW LEVEL SECURITY;
ALTER TABLE reels ENABLE ROW LEVEL SECURITY;
ALTER TABLE lines ENABLE ROW LEVEL SECURITY;
ALTER TABLE lures ENABLE ROW LEVEL SECURITY;
ALTER TABLE leaders ENABLE ROW LEVEL SECURITY;
ALTER TABLE hooks ENABLE ROW LEVEL SECURITY;
ALTER TABLE fishing_setups ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_statistics ENABLE ROW LEVEL SECURITY;

-- Policies pour fish_catches
DROP POLICY IF EXISTS "Users see own catches" ON fish_catches;
CREATE POLICY "Users see own catches" ON fish_catches 
    FOR SELECT USING (auth.uid() = user_id);

DROP POLICY IF EXISTS "Users insert own catches" ON fish_catches;
CREATE POLICY "Users insert own catches" ON fish_catches 
    FOR INSERT WITH CHECK (auth.uid() = user_id);

DROP POLICY IF EXISTS "Users update own catches" ON fish_catches;
CREATE POLICY "Users update own catches" ON fish_catches 
    FOR UPDATE USING (auth.uid() = user_id);

DROP POLICY IF EXISTS "Users delete own catches" ON fish_catches;
CREATE POLICY "Users delete own catches" ON fish_catches 
    FOR DELETE USING (auth.uid() = user_id);

-- Policies pour equipment (appliquer le même pattern pour toutes les tables)
-- Rods
DROP POLICY IF EXISTS "Users manage own rods" ON rods;
CREATE POLICY "Users manage own rods" ON rods 
    FOR ALL USING (auth.uid() = user_id);

-- Reels
DROP POLICY IF EXISTS "Users manage own reels" ON reels;
CREATE POLICY "Users manage own reels" ON reels 
    FOR ALL USING (auth.uid() = user_id);

-- Lines
DROP POLICY IF EXISTS "Users manage own lines" ON lines;
CREATE POLICY "Users manage own lines" ON lines 
    FOR ALL USING (auth.uid() = user_id);

-- Lures
DROP POLICY IF EXISTS "Users manage own lures" ON lures;
CREATE POLICY "Users manage own lures" ON lures 
    FOR ALL USING (auth.uid() = user_id);

-- Leaders
DROP POLICY IF EXISTS "Users manage own leaders" ON leaders;
CREATE POLICY "Users manage own leaders" ON leaders 
    FOR ALL USING (auth.uid() = user_id);

-- Hooks
DROP POLICY IF EXISTS "Users manage own hooks" ON hooks;
CREATE POLICY "Users manage own hooks" ON hooks 
    FOR ALL USING (auth.uid() = user_id);

-- Fishing Setups
DROP POLICY IF EXISTS "Users manage own setups" ON fishing_setups;
CREATE POLICY "Users manage own setups" ON fishing_setups 
    FOR ALL USING (auth.uid() = user_id);

-- User Profiles
DROP POLICY IF EXISTS "Users manage own profile" ON user_profiles;
CREATE POLICY "Users manage own profile" ON user_profiles 
    FOR ALL USING (auth.uid() = id);

-- User Statistics
DROP POLICY IF EXISTS "Users see own statistics" ON user_statistics;
CREATE POLICY "Users see own statistics" ON user_statistics 
    FOR SELECT USING (auth.uid() = user_id);

DROP POLICY IF EXISTS "Users insert own statistics" ON user_statistics;
CREATE POLICY "Users insert own statistics" ON user_statistics 
    FOR INSERT WITH CHECK (auth.uid() = user_id);

DROP POLICY IF EXISTS "Users update own statistics" ON user_statistics;
CREATE POLICY "Users update own statistics" ON user_statistics 
    FOR UPDATE USING (auth.uid() = user_id);

-- Policies publiques pour fish_species et fishing_brands (lecture seule)
ALTER TABLE fish_species ENABLE ROW LEVEL SECURITY;
ALTER TABLE fishing_brands ENABLE ROW LEVEL SECURITY;

DROP POLICY IF EXISTS "Anyone can view fish species" ON fish_species;
CREATE POLICY "Anyone can view fish species" ON fish_species 
    FOR SELECT USING (true);

DROP POLICY IF EXISTS "Anyone can view fishing brands" ON fishing_brands;
CREATE POLICY "Anyone can view fishing brands" ON fishing_brands 
    FOR SELECT USING (true);

-- ============================================
-- 7. VUES UTILES
-- ============================================
-- Vue pour obtenir les prises avec toutes les infos liées

CREATE OR REPLACE VIEW catches_with_details AS
SELECT 
    fc.*,
    fs.common_name as species_common_name,
    fs.scientific_name as species_scientific_name,
    fs.icon_emoji as species_icon,
    fsetup.name as setup_name,
    fsetup.description as setup_description
FROM fish_catches fc
LEFT JOIN fish_species fs ON fc.species_id = fs.id
LEFT JOIN fishing_setups fsetup ON fc.setup_id = fsetup.id;

-- ============================================
-- 8. FONCTIONS UTILITAIRES
-- ============================================
-- Fonction pour nettoyer les anciennes données (optionnel)

CREATE OR REPLACE FUNCTION cleanup_old_data()
RETURNS void AS $$
BEGIN
    -- Supprimer les prises sans photos de plus de 2 ans
    DELETE FROM fish_catches 
    WHERE photo_url IS NULL 
        AND catch_date < CURRENT_DATE - INTERVAL '2 years';

    -- Nettoyer les équipements orphelins (non utilisés dans les montages)
    -- À implémenter selon les besoins

    RAISE NOTICE 'Cleanup completed';
END;
$$ language 'plpgsql';

-- ===================================================
-- FIN DES AMÉLIORATIONS
-- ===================================================
-- Pour exécuter ce script:
-- 1. Connectez-vous à votre projet Supabase
-- 2. Allez dans SQL Editor
-- 3. Copiez-collez ce script
-- 4. Exécutez-le
-- 
-- Note: Ce script est idempotent (peut être exécuté plusieurs fois)
-- ===================================================
