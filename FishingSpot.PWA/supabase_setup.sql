-- ========================================
-- CONFIGURATION SUPABASE AVEC AUTHENTIFICATION
-- ========================================

-- 1. Activer l'authentification Email dans Supabase Dashboard:
--    Authentication > Providers > Email > Enable
--    Désactiver "Confirm email" pour les tests (optionnel)

-- ========================================
-- TABLE: fish_catches
-- ========================================

CREATE TABLE IF NOT EXISTS fish_catches (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    fish_name TEXT NOT NULL,
    photo_path TEXT,
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    location_name TEXT,
    catch_date TIMESTAMP NOT NULL,
    catch_time TEXT,
    length DOUBLE PRECISION,
    weight DOUBLE PRECISION,
    notes TEXT,
    setup_id INTEGER,
    created_at TIMESTAMP DEFAULT NOW()
);

-- Index pour améliorer les performances
CREATE INDEX IF NOT EXISTS idx_fish_catches_user ON fish_catches(user_id);
CREATE INDEX IF NOT EXISTS idx_fish_catches_date ON fish_catches(catch_date DESC);

-- Activer Row Level Security (RLS)
ALTER TABLE fish_catches ENABLE ROW LEVEL SECURITY;

-- Supprimer les anciennes policies si elles existent
DROP POLICY IF EXISTS "enable_read_access_for_all_users" ON fish_catches;
DROP POLICY IF EXISTS "enable_insert_for_all_users" ON fish_catches;
DROP POLICY IF EXISTS "enable_update_for_all_users" ON fish_catches;
DROP POLICY IF EXISTS "enable_delete_for_all_users" ON fish_catches;

-- Policies de sécurité par utilisateur
CREATE POLICY "Users can view their own catches" ON fish_catches
    FOR SELECT 
    USING (auth.uid() = user_id);

CREATE POLICY "Users can insert their own catches" ON fish_catches
    FOR INSERT 
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update their own catches" ON fish_catches
    FOR UPDATE 
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can delete their own catches" ON fish_catches
    FOR DELETE 
    USING (auth.uid() = user_id);

-- ========================================
-- TABLE: user_profiles
-- ========================================

CREATE TABLE IF NOT EXISTS user_profiles (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL UNIQUE REFERENCES auth.users(id) ON DELETE CASCADE,
    first_name TEXT,
    last_name TEXT,
    phone TEXT,
    country TEXT,
    city TEXT,
    favorite_spot TEXT,
    bio TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

-- Index pour améliorer les performances
CREATE INDEX IF NOT EXISTS idx_user_profiles_user ON user_profiles(user_id);

-- Activer Row Level Security (RLS)
ALTER TABLE user_profiles ENABLE ROW LEVEL SECURITY;

-- Policies de sécurité par utilisateur
CREATE POLICY "Users can view their own profile" ON user_profiles
    FOR SELECT 
    USING (auth.uid() = user_id);

CREATE POLICY "Users can insert their own profile" ON user_profiles
    FOR INSERT 
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update their own profile" ON user_profiles
    FOR UPDATE 
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can delete their own profile" ON user_profiles
    FOR DELETE 
    USING (auth.uid() = user_id);

-- ========================================
-- CONFIGURATION TERMINÉE
-- ========================================
-- Chaque utilisateur a maintenant:
-- 1. Ses propres prises de pêche (fish_catches)
-- 2. Son profil personnel (user_profiles)
-- Toutes les données sont isolées et sécurisées par RLS
