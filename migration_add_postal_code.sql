-- Migration: Ajouter colonne postal_code à user_profiles
-- Date: 2026-03-18

-- Ajouter la colonne postal_code
ALTER TABLE user_profiles 
ADD COLUMN IF NOT EXISTS postal_code VARCHAR(10);

-- Créer un index sur postal_code pour les recherches
CREATE INDEX IF NOT EXISTS idx_user_profiles_postal_code 
ON user_profiles(postal_code);

-- Commentaire sur la colonne
COMMENT ON COLUMN user_profiles.postal_code IS 'Code postal de l''utilisateur';
