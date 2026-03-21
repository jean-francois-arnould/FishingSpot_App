-- ================================================
-- Migration: Ajouter le champ 'name' à fishing_setups
-- Date: 2026
-- Description: Ajoute un champ obligatoire 'name' pour le titre des montages
-- ================================================

-- 1. Ajouter la colonne name (nullable temporairement)
ALTER TABLE public.fishing_setups
ADD COLUMN IF NOT EXISTS name TEXT NULL;

-- 2. Mettre à jour les montages existants avec un nom par défaut
-- Utilise la description si disponible, sinon génère un nom
UPDATE public.fishing_setups
SET name = CASE 
    WHEN description IS NOT NULL AND description != '' THEN description
    ELSE 'Montage #' || id::text
END
WHERE name IS NULL;

-- 3. Rendre la colonne obligatoire
ALTER TABLE public.fishing_setups
ALTER COLUMN name SET NOT NULL;

-- 4. Créer un index pour les recherches par nom
CREATE INDEX IF NOT EXISTS idx_fishing_setups_name 
ON public.fishing_setups USING btree (name) 
TABLESPACE pg_default;

-- 5. Vérification
SELECT 
    id, 
    name, 
    description, 
    is_current, 
    is_favorite,
    created_at
FROM public.fishing_setups
ORDER BY created_at DESC
LIMIT 10;
