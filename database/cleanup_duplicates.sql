-- ===================================================
-- SCRIPT DE NETTOYAGE DES DOUBLONS
-- ===================================================
-- Ce script supprime les prises en double en conservant
-- la première occurrence (basée sur created_at)
-- ===================================================

\echo '===================================================';
\echo 'NETTOYAGE DES DOUBLONS';
\echo '===================================================';
\echo '';

-- ============================================
-- 1. AFFICHER LES DOUBLONS AVANT SUPPRESSION
-- ============================================
\echo '1. DOUBLONS DÉTECTÉS';
\echo '-------------------';

WITH duplicates AS (
    SELECT 
        user_id,
        fish_name,
        catch_date,
        catch_time,
        location_name,
        latitude,
        longitude,
        COUNT(*) as nombre_doublons,
        string_agg(id::text, ', ' ORDER BY created_at) as ids
    FROM fish_catches
    GROUP BY user_id, fish_name, catch_date, catch_time, location_name, latitude, longitude
    HAVING COUNT(*) > 1
)
SELECT 
    fish_name,
    catch_date,
    nombre_doublons,
    ids as "IDs (premier sera conservé)"
FROM duplicates
ORDER BY catch_date DESC;

\echo '';
\echo 'Appuyez sur Ctrl+C pour annuler, ou attendez 5 secondes...';
SELECT pg_sleep(5);

-- ============================================
-- 2. SUPPRIMER LES DOUBLONS
-- ============================================
\echo '';
\echo '2. SUPPRESSION EN COURS...';
\echo '--------------------------';

-- Créer une table temporaire avec les IDs à conserver (le plus ancien de chaque groupe)
CREATE TEMP TABLE ids_to_keep AS
SELECT DISTINCT ON (user_id, fish_name, catch_date, catch_time, location_name, latitude, longitude)
    id
FROM fish_catches
ORDER BY user_id, fish_name, catch_date, catch_time, location_name, latitude, longitude, created_at ASC;

-- Supprimer les doublons (tout sauf les IDs à conserver)
WITH deleted AS (
    DELETE FROM fish_catches
    WHERE id NOT IN (SELECT id FROM ids_to_keep)
    AND id IN (
        -- Ne supprimer que les lignes qui sont effectivement des doublons
        SELECT fc.id
        FROM fish_catches fc
        INNER JOIN (
            SELECT user_id, fish_name, catch_date, catch_time, location_name, latitude, longitude
            FROM fish_catches
            GROUP BY user_id, fish_name, catch_date, catch_time, location_name, latitude, longitude
            HAVING COUNT(*) > 1
        ) dups ON 
            fc.user_id = dups.user_id
            AND fc.fish_name = dups.fish_name
            AND fc.catch_date = dups.catch_date
            AND COALESCE(fc.catch_time, '00:00'::time) = COALESCE(dups.catch_time, '00:00'::time)
            AND COALESCE(fc.location_name, '') = COALESCE(dups.location_name, '')
            AND COALESCE(fc.latitude, '') = COALESCE(dups.latitude, '')
            AND COALESCE(fc.longitude, '') = COALESCE(dups.longitude, '')
    )
    RETURNING *
)
SELECT COUNT(*) as "Prises supprimées" FROM deleted;

\echo '';
\echo '✅ Nettoyage terminé';
\echo '';

-- ============================================
-- 3. VÉRIFIER QU'IL N'Y A PLUS DE DOUBLONS
-- ============================================
\echo '3. VÉRIFICATION FINALE';
\echo '----------------------';

WITH duplicates AS (
    SELECT 
        COUNT(*) as nombre_doublons
    FROM fish_catches
    GROUP BY user_id, fish_name, catch_date, catch_time, location_name, latitude, longitude
    HAVING COUNT(*) > 1
)
SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '✅ Aucun doublon restant'
        ELSE '⚠️ ' || COUNT(*)::text || ' groupes de doublons restants'
    END as resultat
FROM duplicates;

\echo '';
\echo '===================================================';
\echo 'FIN DU NETTOYAGE';
\echo '===================================================';
