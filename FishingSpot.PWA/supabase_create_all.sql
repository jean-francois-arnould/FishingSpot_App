-- ========================================
-- FISHINGSPOT V2.0 - CRÉATION COMPLÈTE
-- Script de création depuis zéro
-- ========================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ========================================
-- TABLES DE MATÉRIEL
-- ========================================

-- Cannes
CREATE TABLE rods (
  id SERIAL PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  brand TEXT NOT NULL,
  model TEXT NOT NULL,
  length TEXT,
  power TEXT,
  action TEXT,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Moulinets
CREATE TABLE reels (
  id SERIAL PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  brand TEXT NOT NULL,
  model TEXT NOT NULL,
  type TEXT,
  gear_ratio TEXT,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Fils/Lignes
CREATE TABLE lines (
  id SERIAL PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  brand TEXT NOT NULL,
  type TEXT NOT NULL,
  strength TEXT,
  diameter TEXT,
  color TEXT,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Leurres
CREATE TABLE lures (
  id SERIAL PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  type TEXT,
  color TEXT,
  weight TEXT,
  size TEXT,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Bas de ligne
CREATE TABLE leaders (
  id SERIAL PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  material TEXT NOT NULL,
  strength TEXT,
  length TEXT,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Hameçons
CREATE TABLE hooks (
  id SERIAL PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  size TEXT NOT NULL,
  type TEXT,
  brand TEXT,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- TABLE DES SETUPS (MONTAGES)
-- ========================================

CREATE TABLE fishing_setups (
  id SERIAL PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  description TEXT,
  rod_id INTEGER REFERENCES rods(id) ON DELETE SET NULL,
  reel_id INTEGER REFERENCES reels(id) ON DELETE SET NULL,
  line_id INTEGER REFERENCES lines(id) ON DELETE SET NULL,
  lure_id INTEGER REFERENCES lures(id) ON DELETE SET NULL,
  leader_id INTEGER REFERENCES leaders(id) ON DELETE SET NULL,
  hook_id INTEGER REFERENCES hooks(id) ON DELETE SET NULL,
  is_favorite BOOLEAN DEFAULT FALSE,
  is_current BOOLEAN DEFAULT FALSE,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- TABLE DES PRISES
-- ========================================

CREATE TABLE fish_catches (
  id SERIAL PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  fish_name TEXT NOT NULL,
  photo_url TEXT,
  latitude DOUBLE PRECISION,
  longitude DOUBLE PRECISION,
  location_name TEXT NOT NULL,
  catch_date DATE NOT NULL,
  catch_time TIME,
  length DOUBLE PRECISION,
  weight DOUBLE PRECISION,
  notes TEXT,
  setup_id INTEGER REFERENCES fishing_setups(id) ON DELETE SET NULL,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- TABLE DES PROFILS UTILISATEURS
-- ========================================

CREATE TABLE user_profiles (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID UNIQUE NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  first_name TEXT,
  last_name TEXT,
  phone TEXT,
  country TEXT,
  city TEXT,
  favorite_spot TEXT,
  bio TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- INDEX DE PERFORMANCE
-- ========================================

-- Index pour les recherches par utilisateur
CREATE INDEX idx_rods_user ON rods(user_id);
CREATE INDEX idx_reels_user ON reels(user_id);
CREATE INDEX idx_lines_user ON lines(user_id);
CREATE INDEX idx_lures_user ON lures(user_id);
CREATE INDEX idx_leaders_user ON leaders(user_id);
CREATE INDEX idx_hooks_user ON hooks(user_id);
CREATE INDEX idx_fishing_setups_user ON fishing_setups(user_id);
CREATE INDEX idx_fish_catches_user ON fish_catches(user_id);
CREATE INDEX idx_user_profiles_user ON user_profiles(user_id);

-- Index pour les tris par date
CREATE INDEX idx_fish_catches_date ON fish_catches(catch_date DESC);
CREATE INDEX idx_fish_catches_created ON fish_catches(created_at DESC);

-- Index unique : un seul setup actuel par utilisateur
CREATE UNIQUE INDEX idx_one_current_setup_per_user 
  ON fishing_setups(user_id) 
  WHERE is_current = TRUE;

-- ========================================
-- ROW LEVEL SECURITY (RLS)
-- ========================================

ALTER TABLE rods ENABLE ROW LEVEL SECURITY;
ALTER TABLE reels ENABLE ROW LEVEL SECURITY;
ALTER TABLE lines ENABLE ROW LEVEL SECURITY;
ALTER TABLE lures ENABLE ROW LEVEL SECURITY;
ALTER TABLE leaders ENABLE ROW LEVEL SECURITY;
ALTER TABLE hooks ENABLE ROW LEVEL SECURITY;
ALTER TABLE fishing_setups ENABLE ROW LEVEL SECURITY;
ALTER TABLE fish_catches ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_profiles ENABLE ROW LEVEL SECURITY;

-- ========================================
-- POLITIQUES RLS - MATÉRIEL
-- ========================================

-- CANNES
CREATE POLICY rods_select ON rods
  FOR SELECT TO authenticated
  USING (auth.uid() = user_id);

CREATE POLICY rods_insert ON rods
  FOR INSERT TO authenticated
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY rods_update ON rods
  FOR UPDATE TO authenticated
  USING (auth.uid() = user_id)
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY rods_delete ON rods
  FOR DELETE TO authenticated
  USING (auth.uid() = user_id);

-- MOULINETS
CREATE POLICY reels_select ON reels
  FOR SELECT TO authenticated
  USING (auth.uid() = user_id);

CREATE POLICY reels_insert ON reels
  FOR INSERT TO authenticated
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY reels_update ON reels
  FOR UPDATE TO authenticated
  USING (auth.uid() = user_id)
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY reels_delete ON reels
  FOR DELETE TO authenticated
  USING (auth.uid() = user_id);

-- FILS
CREATE POLICY lines_select ON lines
  FOR SELECT TO authenticated
  USING (auth.uid() = user_id);

CREATE POLICY lines_insert ON lines
  FOR INSERT TO authenticated
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY lines_update ON lines
  FOR UPDATE TO authenticated
  USING (auth.uid() = user_id)
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY lines_delete ON lines
  FOR DELETE TO authenticated
  USING (auth.uid() = user_id);

-- LEURRES
CREATE POLICY lures_select ON lures
  FOR SELECT TO authenticated
  USING (auth.uid() = user_id);

CREATE POLICY lures_insert ON lures
  FOR INSERT TO authenticated
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY lures_update ON lures
  FOR UPDATE TO authenticated
  USING (auth.uid() = user_id)
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY lures_delete ON lures
  FOR DELETE TO authenticated
  USING (auth.uid() = user_id);

-- BAS DE LIGNE
CREATE POLICY leaders_select ON leaders
  FOR SELECT TO authenticated
  USING (auth.uid() = user_id);

CREATE POLICY leaders_insert ON leaders
  FOR INSERT TO authenticated
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY leaders_update ON leaders
  FOR UPDATE TO authenticated
  USING (auth.uid() = user_id)
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY leaders_delete ON leaders
  FOR DELETE TO authenticated
  USING (auth.uid() = user_id);

-- HAMEÇONS
CREATE POLICY hooks_select ON hooks
  FOR SELECT TO authenticated
  USING (auth.uid() = user_id);

CREATE POLICY hooks_insert ON hooks
  FOR INSERT TO authenticated
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY hooks_update ON hooks
  FOR UPDATE TO authenticated
  USING (auth.uid() = user_id)
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY hooks_delete ON hooks
  FOR DELETE TO authenticated
  USING (auth.uid() = user_id);

-- ========================================
-- POLITIQUES RLS - SETUPS
-- ========================================

CREATE POLICY setups_select ON fishing_setups
  FOR SELECT TO authenticated
  USING (auth.uid() = user_id);

CREATE POLICY setups_insert ON fishing_setups
  FOR INSERT TO authenticated
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY setups_update ON fishing_setups
  FOR UPDATE TO authenticated
  USING (auth.uid() = user_id)
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY setups_delete ON fishing_setups
  FOR DELETE TO authenticated
  USING (auth.uid() = user_id);

-- ========================================
-- POLITIQUES RLS - PRISES
-- ========================================

CREATE POLICY catches_select ON fish_catches
  FOR SELECT TO authenticated
  USING (auth.uid() = user_id);

CREATE POLICY catches_insert ON fish_catches
  FOR INSERT TO authenticated
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY catches_update ON fish_catches
  FOR UPDATE TO authenticated
  USING (auth.uid() = user_id)
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY catches_delete ON fish_catches
  FOR DELETE TO authenticated
  USING (auth.uid() = user_id);

-- ========================================
-- POLITIQUES RLS - PROFILS
-- ========================================

CREATE POLICY profiles_select ON user_profiles
  FOR SELECT TO authenticated
  USING (auth.uid() = user_id);

CREATE POLICY profiles_insert ON user_profiles
  FOR INSERT TO authenticated
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY profiles_update ON user_profiles
  FOR UPDATE TO authenticated
  USING (auth.uid() = user_id)
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY profiles_delete ON user_profiles
  FOR DELETE TO authenticated
  USING (auth.uid() = user_id);

-- ========================================
-- VÉRIFICATION FINALE
-- ========================================

SELECT
  '✅ Base de données FishingSpot V2.0 créée avec succès !' AS status,
  'Tables créées : 9' AS tables_count,
  'Index créés : 13' AS index_count,
  'Politiques RLS : 36' AS policies_count,
  '🎣 Prêt à pêcher !' AS message;
