-- Script SQL pour FishingSpot PWA (Version Sécurisée)
-- Ce script peut être exécuté plusieurs fois sans erreur
-- Exécutez ce script dans SQL Editor de Supabase

-- ============================================
-- ÉTAPE 1: Créer les tables de base
-- ============================================

-- Table des utilisateurs (profils)
CREATE TABLE IF NOT EXISTS user_profiles (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE UNIQUE,
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

-- Table des setups de pêche
CREATE TABLE IF NOT EXISTS fishing_setups (
  id SERIAL PRIMARY KEY,
  user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  rod_brand TEXT,
  rod_model TEXT,
  rod_length TEXT,
  reel_brand TEXT,
  reel_model TEXT,
  line_type TEXT,
  line_strength TEXT,
  lure_bait TEXT,
  hook_size TEXT,
  notes TEXT,
  is_favorite BOOLEAN DEFAULT FALSE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Table des prises de poisson (sans setup_id d'abord)
CREATE TABLE IF NOT EXISTS fish_catches (
  id SERIAL PRIMARY KEY,
  user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
  fish_name TEXT NOT NULL,
  photo_path TEXT,
  latitude DOUBLE PRECISION,
  longitude DOUBLE PRECISION,
  location_name TEXT NOT NULL,
  catch_date DATE NOT NULL,
  catch_time TEXT,
  length DOUBLE PRECISION,
  weight DOUBLE PRECISION,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ============================================
-- ÉTAPE 2: Ajouter la colonne setup_id si elle n'existe pas
-- ============================================

DO $$ 
BEGIN
  -- Vérifier si la colonne setup_id existe
  IF NOT EXISTS (
    SELECT 1 
    FROM information_schema.columns 
    WHERE table_name = 'fish_catches' 
    AND column_name = 'setup_id'
  ) THEN
    -- Ajouter la colonne
    ALTER TABLE fish_catches 
    ADD COLUMN setup_id INTEGER;

    -- Ajouter la contrainte de clé étrangère
    ALTER TABLE fish_catches 
    ADD CONSTRAINT fk_fish_catches_setup 
    FOREIGN KEY (setup_id) 
    REFERENCES fishing_setups(id) 
    ON DELETE SET NULL;

    RAISE NOTICE 'Colonne setup_id ajoutée à fish_catches';
  ELSE
    RAISE NOTICE 'Colonne setup_id existe déjà dans fish_catches';
  END IF;
END $$;

-- ============================================
-- ÉTAPE 3: Activer Row Level Security (RLS)
-- ============================================

ALTER TABLE user_profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE fishing_setups ENABLE ROW LEVEL SECURITY;
ALTER TABLE fish_catches ENABLE ROW LEVEL SECURITY;

-- ============================================
-- ÉTAPE 4: Créer les politiques RLS
-- ============================================

-- Politiques pour user_profiles
DO $$ 
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'user_profiles' 
    AND policyname = 'Users can view their own profile'
  ) THEN
    CREATE POLICY "Users can view their own profile" 
      ON user_profiles FOR SELECT 
      USING (auth.uid() = user_id);
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'user_profiles' 
    AND policyname = 'Users can update their own profile'
  ) THEN
    CREATE POLICY "Users can update their own profile" 
      ON user_profiles FOR UPDATE 
      USING (auth.uid() = user_id);
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'user_profiles' 
    AND policyname = 'Users can insert their own profile'
  ) THEN
    CREATE POLICY "Users can insert their own profile" 
      ON user_profiles FOR INSERT 
      WITH CHECK (auth.uid() = user_id);
  END IF;
END $$;

-- Politiques pour fishing_setups
DO $$ 
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'fishing_setups' 
    AND policyname = 'Users can view their own setups'
  ) THEN
    CREATE POLICY "Users can view their own setups" 
      ON fishing_setups FOR SELECT 
      USING (auth.uid() = user_id);
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'fishing_setups' 
    AND policyname = 'Users can insert their own setups'
  ) THEN
    CREATE POLICY "Users can insert their own setups" 
      ON fishing_setups FOR INSERT 
      WITH CHECK (auth.uid() = user_id);
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'fishing_setups' 
    AND policyname = 'Users can update their own setups'
  ) THEN
    CREATE POLICY "Users can update their own setups" 
      ON fishing_setups FOR UPDATE 
      USING (auth.uid() = user_id);
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'fishing_setups' 
    AND policyname = 'Users can delete their own setups'
  ) THEN
    CREATE POLICY "Users can delete their own setups" 
      ON fishing_setups FOR DELETE 
      USING (auth.uid() = user_id);
  END IF;
END $$;

-- Politiques pour fish_catches
DO $$ 
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'fish_catches' 
    AND policyname = 'Users can view their own catches'
  ) THEN
    CREATE POLICY "Users can view their own catches" 
      ON fish_catches FOR SELECT 
      USING (auth.uid() = user_id);
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'fish_catches' 
    AND policyname = 'Users can insert their own catches'
  ) THEN
    CREATE POLICY "Users can insert their own catches" 
      ON fish_catches FOR INSERT 
      WITH CHECK (auth.uid() = user_id);
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'fish_catches' 
    AND policyname = 'Users can update their own catches'
  ) THEN
    CREATE POLICY "Users can update their own catches" 
      ON fish_catches FOR UPDATE 
      USING (auth.uid() = user_id);
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM pg_policies 
    WHERE tablename = 'fish_catches' 
    AND policyname = 'Users can delete their own catches'
  ) THEN
    CREATE POLICY "Users can delete their own catches" 
      ON fish_catches FOR DELETE 
      USING (auth.uid() = user_id);
  END IF;
END $$;

-- ============================================
-- ÉTAPE 5: Message de confirmation
-- ============================================

SELECT 
  '✅ Setup terminé avec succès !' AS status,
  'Tables créées/mises à jour :' AS info,
  (SELECT COUNT(*) FROM user_profiles) AS user_profiles_count,
  (SELECT COUNT(*) FROM fishing_setups) AS fishing_setups_count,
  (SELECT COUNT(*) FROM fish_catches) AS fish_catches_count;
