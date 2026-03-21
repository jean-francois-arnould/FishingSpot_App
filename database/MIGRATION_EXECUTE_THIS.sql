-- ========================================
-- MIGRATION: fishing_setups vers Foreign Keys
-- ========================================
-- Ce script migre la table fishing_setups pour utiliser
-- des foreign keys au lieu de champs texte
-- 
-- ⚠️ ATTENTION: Cela supprimera les montages existants !
-- ⚠️ Sauvegardez vos données avant d'exécuter ce script
-- ========================================

-- ÉTAPE 1: Sauvegarder les données existantes (optionnel)
-- Décommentez si vous voulez garder une copie
-- CREATE TABLE fishing_setups_backup AS SELECT * FROM fishing_setups;

-- ÉTAPE 2: Supprimer les anciens champs texte
ALTER TABLE public.fishing_setups
DROP COLUMN IF EXISTS rod_brand,
DROP COLUMN IF EXISTS rod_model,
DROP COLUMN IF EXISTS rod_length,
DROP COLUMN IF EXISTS rod_power,
DROP COLUMN IF EXISTS reel_brand,
DROP COLUMN IF EXISTS reel_model,
DROP COLUMN IF EXISTS reel_type,
DROP COLUMN IF EXISTS line_type,
DROP COLUMN IF EXISTS line_diameter,
DROP COLUMN IF EXISTS line_breaking_strength,
DROP COLUMN IF EXISTS hook_size,
DROP COLUMN IF EXISTS bait_type;

-- ÉTAPE 3: Ajouter les nouvelles colonnes FK
ALTER TABLE public.fishing_setups
ADD COLUMN IF NOT EXISTS rod_id bigint,
ADD COLUMN IF NOT EXISTS reel_id bigint,
ADD COLUMN IF NOT EXISTS line_id bigint,
ADD COLUMN IF NOT EXISTS lure_id bigint,
ADD COLUMN IF NOT EXISTS leader_id bigint,
ADD COLUMN IF NOT EXISTS hook_id bigint;

-- ÉTAPE 4: Ajouter les contraintes de clés étrangères
ALTER TABLE public.fishing_setups
DROP CONSTRAINT IF EXISTS fishing_setups_rod_id_fkey,
DROP CONSTRAINT IF EXISTS fishing_setups_reel_id_fkey,
DROP CONSTRAINT IF EXISTS fishing_setups_line_id_fkey,
DROP CONSTRAINT IF EXISTS fishing_setups_lure_id_fkey,
DROP CONSTRAINT IF EXISTS fishing_setups_leader_id_fkey,
DROP CONSTRAINT IF EXISTS fishing_setups_hook_id_fkey;

ALTER TABLE public.fishing_setups
ADD CONSTRAINT fishing_setups_rod_id_fkey 
  FOREIGN KEY (rod_id) REFERENCES public.rods(id) ON DELETE SET NULL,
ADD CONSTRAINT fishing_setups_reel_id_fkey 
  FOREIGN KEY (reel_id) REFERENCES public.reels(id) ON DELETE SET NULL,
ADD CONSTRAINT fishing_setups_line_id_fkey 
  FOREIGN KEY (line_id) REFERENCES public.lines(id) ON DELETE SET NULL,
ADD CONSTRAINT fishing_setups_lure_id_fkey 
  FOREIGN KEY (lure_id) REFERENCES public.lures(id) ON DELETE SET NULL,
ADD CONSTRAINT fishing_setups_leader_id_fkey 
  FOREIGN KEY (leader_id) REFERENCES public.leaders(id) ON DELETE SET NULL,
ADD CONSTRAINT fishing_setups_hook_id_fkey 
  FOREIGN KEY (hook_id) REFERENCES public.hooks(id) ON DELETE SET NULL;

-- ÉTAPE 5: Créer des index pour les performances
DROP INDEX IF EXISTS idx_fishing_setups_rod_id;
DROP INDEX IF EXISTS idx_fishing_setups_reel_id;
DROP INDEX IF EXISTS idx_fishing_setups_line_id;
DROP INDEX IF EXISTS idx_fishing_setups_lure_id;
DROP INDEX IF EXISTS idx_fishing_setups_leader_id;
DROP INDEX IF EXISTS idx_fishing_setups_hook_id;

CREATE INDEX idx_fishing_setups_rod_id ON public.fishing_setups(rod_id);
CREATE INDEX idx_fishing_setups_reel_id ON public.fishing_setups(reel_id);
CREATE INDEX idx_fishing_setups_line_id ON public.fishing_setups(line_id);
CREATE INDEX idx_fishing_setups_lure_id ON public.fishing_setups(lure_id);
CREATE INDEX idx_fishing_setups_leader_id ON public.fishing_setups(leader_id);
CREATE INDEX idx_fishing_setups_hook_id ON public.fishing_setups(hook_id);

-- ÉTAPE 6: Vérification de la structure
SELECT 
  column_name, 
  data_type, 
  is_nullable,
  column_default
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'fishing_setups'
ORDER BY ordinal_position;

-- ÉTAPE 7: Vérification des contraintes
SELECT
  conname AS constraint_name,
  contype AS constraint_type,
  pg_get_constraintdef(oid) AS constraint_definition
FROM pg_constraint
WHERE conrelid = 'public.fishing_setups'::regclass;

-- ========================================
-- FIN DE LA MIGRATION
-- ========================================
-- ✅ La table fishing_setups utilise maintenant des FK
-- ✅ Les montages référenceront le matériel existant
-- 
-- Structure finale:
-- - id (PK)
-- - user_id (FK vers auth.users)
-- - rod_id (FK vers rods)
-- - reel_id (FK vers reels)
-- - line_id (FK vers lines)
-- - lure_id (FK vers lures)
-- - leader_id (FK vers leaders)
-- - hook_id (FK vers hooks)
-- - description
-- - is_current
-- - is_favorite
-- - created_at
-- - updated_at
-- ========================================
