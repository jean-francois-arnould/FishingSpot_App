-- ========================================
-- MIGRATION: fishing_setups vers Foreign Keys
-- ========================================
-- Ce script migre fishing_setups pour utiliser des FK
-- au lieu de champs texte (rod_brand, reel_brand, etc.)
-- ========================================

-- ÉTAPE 1: Sauvegarder les données existantes (optionnel)
-- Vous pouvez créer une table de backup si nécessaire
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
ADD COLUMN rod_id bigint,
ADD COLUMN reel_id bigint,
ADD COLUMN line_id bigint,
ADD COLUMN lure_id bigint,
ADD COLUMN leader_id bigint,
ADD COLUMN hook_id bigint;

-- ÉTAPE 4: Ajouter les contraintes de clés étrangères
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
CREATE INDEX IF NOT EXISTS idx_fishing_setups_rod_id ON public.fishing_setups(rod_id);
CREATE INDEX IF NOT EXISTS idx_fishing_setups_reel_id ON public.fishing_setups(reel_id);
CREATE INDEX IF NOT EXISTS idx_fishing_setups_line_id ON public.fishing_setups(line_id);
CREATE INDEX IF NOT EXISTS idx_fishing_setups_lure_id ON public.fishing_setups(lure_id);
CREATE INDEX IF NOT EXISTS idx_fishing_setups_leader_id ON public.fishing_setups(leader_id);
CREATE INDEX IF NOT EXISTS idx_fishing_setups_hook_id ON public.fishing_setups(hook_id);

-- ÉTAPE 6: Vérification
SELECT 
  column_name, 
  data_type, 
  is_nullable
FROM information_schema.columns
WHERE table_name = 'fishing_setups'
ORDER BY ordinal_position;

-- ========================================
-- FIN DE LA MIGRATION
-- ========================================
-- Exécutez ce script dans le SQL Editor de Supabase
-- ⚠️ ATTENTION: Cela supprimera les données de montages existants
-- Si vous avez des montages, sauvegardez-les d'abord !
-- ========================================
