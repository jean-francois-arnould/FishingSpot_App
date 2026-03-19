-- ========================================
-- Script de correction pour is_current
-- Exécutez ce script AVANT supabase_v2_migration.sql
-- ========================================

-- Vérifier et ajouter la colonne is_current si elle n'existe pas
DO $$
BEGIN
  -- Ajouter is_current à fishing_setups si elle n'existe pas
  IF NOT EXISTS (
    SELECT 1 
    FROM information_schema.columns 
    WHERE table_schema = 'public'
      AND table_name = 'fishing_setups'
      AND column_name = 'is_current'
  ) THEN
    ALTER TABLE public.fishing_setups
    ADD COLUMN is_current BOOLEAN DEFAULT FALSE;

    RAISE NOTICE 'Colonne is_current ajoutée à fishing_setups';
  ELSE
    RAISE NOTICE 'Colonne is_current existe déjà dans fishing_setups';
  END IF;

  -- Ajouter description si elle n'existe pas
  IF NOT EXISTS (
    SELECT 1 
    FROM information_schema.columns 
    WHERE table_schema = 'public'
      AND table_name = 'fishing_setups'
      AND column_name = 'description'
  ) THEN
    ALTER TABLE public.fishing_setups
    ADD COLUMN description TEXT;

    RAISE NOTICE 'Colonne description ajoutée à fishing_setups';
  ELSE
    RAISE NOTICE 'Colonne description existe déjà dans fishing_setups';
  END IF;
END $$;

-- Supprimer l'ancien index s'il existe
DROP INDEX IF EXISTS idx_one_current_setup_per_user;

-- Créer le nouvel index
CREATE UNIQUE INDEX IF NOT EXISTS idx_one_current_setup_per_user 
  ON fishing_setups(user_id, is_current) 
  WHERE is_current = TRUE;

SELECT 'Correction terminée !' AS status;
