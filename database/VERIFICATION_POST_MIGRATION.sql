-- ========================================
-- VÉRIFICATION POST-MIGRATION
-- ========================================
-- Exécutez ce script APRÈS la migration
-- pour vérifier que tout est OK
-- ========================================

-- 1. Vérifier la structure de fishing_setups
SELECT 
  column_name, 
  data_type, 
  is_nullable,
  column_default
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'fishing_setups'
ORDER BY ordinal_position;

-- 2. Vérifier les contraintes FK
SELECT
  tc.constraint_name,
  tc.table_name,
  kcu.column_name,
  ccu.table_name AS foreign_table_name,
  ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
  AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
  AND ccu.table_schema = tc.table_schema
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_name = 'fishing_setups'
ORDER BY tc.constraint_name;

-- 3. Vérifier les index
SELECT
  indexname,
  indexdef
FROM pg_indexes
WHERE schemaname = 'public'
  AND tablename = 'fishing_setups'
ORDER BY indexname;

-- 4. Compter les enregistrements dans chaque table
SELECT 'rods' AS table_name, COUNT(*) AS count FROM public.rods
UNION ALL
SELECT 'reels', COUNT(*) FROM public.reels
UNION ALL
SELECT 'lines', COUNT(*) FROM public.lines
UNION ALL
SELECT 'lures', COUNT(*) FROM public.lures
UNION ALL
SELECT 'leaders', COUNT(*) FROM public.leaders
UNION ALL
SELECT 'hooks', COUNT(*) FROM public.hooks
UNION ALL
SELECT 'fishing_setups', COUNT(*) FROM public.fishing_setups;

-- 5. Tester la création d'un montage (exemple)
-- Remplacez YOUR_USER_ID par votre UUID utilisateur
-- Remplacez les IDs par des IDs valides de votre matériel
/*
INSERT INTO public.fishing_setups (
  user_id,
  rod_id,
  reel_id,
  description,
  is_favorite,
  is_current
) VALUES (
  'YOUR_USER_ID'::uuid,
  1,  -- ID d'une canne existante
  1,  -- ID d'un moulinet existant
  'Test montage après migration',
  false,
  false
);
*/

-- ========================================
-- RÉSULTATS ATTENDUS
-- ========================================
-- 1. fishing_setups doit avoir les colonnes:
--    - id, user_id, rod_id, reel_id, line_id, 
--      lure_id, leader_id, hook_id, description,
--      is_current, is_favorite, created_at, updated_at
--
-- 2. 6 contraintes FK doivent exister:
--    - fishing_setups_rod_id_fkey
--    - fishing_setups_reel_id_fkey
--    - fishing_setups_line_id_fkey
--    - fishing_setups_lure_id_fkey
--    - fishing_setups_leader_id_fkey
--    - fishing_setups_hook_id_fkey
--
-- 3. 6 index doivent exister sur les FK
--
-- 4. Le comptage montrera combien de matériel vous avez
-- ========================================
