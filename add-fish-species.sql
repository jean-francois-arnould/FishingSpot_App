-- ===================================================================
-- AJOUT DE LA TABLE fish_species (Espèces de poissons)
-- À exécuter APRÈS database-migration.sql
-- ===================================================================

-- ===================================================================
-- 1. CRÉATION DE LA TABLE fish_species
-- ===================================================================

CREATE TABLE IF NOT EXISTS public.fish_species (
    id SERIAL PRIMARY KEY,
    common_name TEXT NOT NULL UNIQUE,
    scientific_name TEXT,
    family TEXT,
    category TEXT, -- 'carnassier', 'cyprinidé', 'salmonidé', 'migrateur', 'autre'
    description TEXT,
    min_legal_size INTEGER, -- Taille légale minimale en cm (si applicable)
    icon_emoji TEXT DEFAULT '🐟',
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Index pour les recherches
CREATE INDEX idx_fish_species_common_name ON public.fish_species(common_name);
CREATE INDEX idx_fish_species_category ON public.fish_species(category);
CREATE INDEX idx_fish_species_is_active ON public.fish_species(is_active) WHERE is_active = TRUE;

-- RLS (tout le monde peut lire, seuls les admins peuvent modifier)
ALTER TABLE public.fish_species ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Everyone can view fish species" ON public.fish_species
    FOR SELECT USING (TRUE);

-- Commentaires
COMMENT ON TABLE public.fish_species IS 'Liste des espèces de poissons d''eau douce disponibles';
COMMENT ON COLUMN public.fish_species.min_legal_size IS 'Taille légale minimale de capture en France (en cm)';

-- ===================================================================
-- 2. INSERTION DES POISSONS DE RIVIÈRE FRANÇAIS
-- ===================================================================

-- CARNASSIERS (Prédateurs)
INSERT INTO public.fish_species (common_name, scientific_name, family, category, min_legal_size, icon_emoji, description) VALUES
('Brochet', 'Esox lucius', 'Ésocidés', 'carnassier', 50, '🦈', 'Grand prédateur d''eau douce, reconnaissable à son corps allongé'),
('Sandre', 'Sander lucioperca', 'Percidés', 'carnassier', 40, '🐟', 'Carnassier crépusculaire aux yeux vitreux caractéristiques'),
('Perche commune', 'Perca fluviatilis', 'Percidés', 'carnassier', NULL, '🐠', 'Poisson rayé orange et noir, grégaire'),
('Black-bass (Achigan)', 'Micropterus salmoides', 'Centrarchidés', 'carnassier', 30, '🐟', 'Espèce introduite d''Amérique du Nord, combattif'),
('Silure glane', 'Silurus glanis', 'Siluridés', 'carnassier', NULL, '🐋', 'Plus gros poisson d''eau douce d''Europe, peut dépasser 2m'),
('Truite fario', 'Salmo trutta fario', 'Salmonidés', 'carnassier', 20, '🐟', 'Truite autochtone des rivières françaises'),
('Truite arc-en-ciel', 'Oncorhynchus mykiss', 'Salmonidés', 'carnassier', 20, '🌈', 'Truite d''élevage introduite, bande rosée caractéristique');

-- SALMONIDÉS (Suite)
INSERT INTO public.fish_species (common_name, scientific_name, family, category, min_legal_size, icon_emoji, description) VALUES
('Omble chevalier', 'Salvelinus alpinus', 'Salmonidés', 'salmonidé', 20, '❄️', 'Salmonidé des lacs de montagne, chair rose délicate'),
('Cristivomer (Truite grise)', 'Salvelinus namaycush', 'Salmonidés', 'salmonidé', 40, '🐟', 'Grande truite des lacs profonds'),
('Ombre commun', 'Thymallus thymallus', 'Salmonidés', 'salmonidé', 30, '✨', 'Poisson élégant à la grande nageoire dorsale, odeur de thym');

-- MIGRATEURS
INSERT INTO public.fish_species (common_name, scientific_name, family, category, min_legal_size, icon_emoji, description) VALUES
('Saumon atlantique', 'Salmo salar', 'Salmonidés', 'migrateur', 50, '🦈', 'Poisson migrateur prestigieux, remonte les rivières pour se reproduire'),
('Truite de mer', 'Salmo trutta trutta', 'Salmonidés', 'migrateur', 35, '🌊', 'Forme migratrice de la truite fario'),
('Anguille européenne', 'Anguilla anguilla', 'Anguillidés', 'migrateur', NULL, '🐍', 'Poisson serpentiforme migrateur, en danger critique'),
('Grande alose', 'Alosa alosa', 'Clupéidés', 'migrateur', 30, '🐟', 'Poisson migrateur remontant les fleuves au printemps'),
('Alose feinte', 'Alosa fallax', 'Clupéidés', 'migrateur', 30, '🐟', 'Cousin de la grande alose, plus petit'),
('Lamproie marine', 'Petromyzon marinus', 'Pétromyzontidés', 'migrateur', NULL, '🦠', 'Poisson primitif sans mâchoires, parasite en mer'),
('Lamproie de rivière', 'Lampetra fluviatilis', 'Pétromyzontidés', 'migrateur', NULL, '🦠', 'Petite lamproie des cours d''eau');

-- CYPRINIDÉS (Poissons blancs)
INSERT INTO public.fish_species (common_name, scientific_name, family, category, min_legal_size, icon_emoji, description) VALUES
('Carpe commune', 'Cyprinus carpio', 'Cyprinidés', 'cyprinidé', NULL, '🐟', 'Poisson emblématique de la pêche au coup et à la carpe'),
('Carpe miroir', 'Cyprinus carpio var. mirror', 'Cyprinidés', 'cyprinidé', NULL, '🪞', 'Variété de carpe avec peu d''écailles'),
('Carpe cuir', 'Cyprinus carpio var. leather', 'Cyprinidés', 'cyprinidé', NULL, '🐟', 'Variété de carpe sans écailles'),
('Carpe koï', 'Cyprinus carpio var. koi', 'Cyprinidés', 'cyprinidé', NULL, '🎨', 'Carpe ornementale aux couleurs vives'),
('Gardon', 'Rutilus rutilus', 'Cyprinidés', 'cyprinidé', NULL, '🐠', 'Poisson blanc très commun, nageoires rouges'),
('Rotengle', 'Scardinius erythrophthalmus', 'Cyprinidés', 'cyprinidé', NULL, '🔴', 'Ressemble au gardon, nageoires plus rouges'),
('Brème commune', 'Abramis brama', 'Cyprinidés', 'cyprinidé', 30, '🐟', 'Grand poisson plat argenté des eaux calmes'),
('Brème bordelière', 'Blicca bjoerkna', 'Cyprinidés', 'cyprinidé', NULL, '🐟', 'Petite brème, plus ronde que la commune'),
('Tanche', 'Tinca tinca', 'Cyprinidés', 'cyprinidé', NULL, '🟢', 'Poisson vert-olive épais, peau visqueuse'),
('Chevesne (Chevaine)', 'Squalius cephalus', 'Cyprinidés', 'cyprinidé', NULL, '🐟', 'Poisson robuste des rivières courantes'),
('Barbeau fluviatile', 'Barbus barbus', 'Cyprinidés', 'cyprinidé', 30, '🧔', 'Grand poisson à barbillons, fond de rivière'),
('Ablette', 'Alburnus alburnus', 'Cyprinidés', 'cyprinidé', NULL, '✨', 'Petit poisson argenté de surface, grégaire'),
('Vandoise', 'Leuciscus leuciscus', 'Cyprinidés', 'cyprinidé', NULL, '🐟', 'Poisson des eaux vives et oxygénées'),
('Hotu', 'Chondrostoma nasus', 'Cyprinidés', 'cyprinidé', NULL, '👃', 'Nez proéminent, se nourrit d''algues'),
('Goujon', 'Gobio gobio', 'Cyprinidés', 'cyprinidé', NULL, '🐟', 'Petit poisson de fond à barbillons'),
('Vairon', 'Phoxinus phoxinus', 'Cyprinidés', 'cyprinidé', NULL, '🔴', 'Très petit poisson de tête de bassin'),
('Ide mélanote', 'Leuciscus idus', 'Cyprinidés', 'cyprinidé', NULL, '🐟', 'Grand cyprinidé, forme dorée ornementale'),
('Carassin', 'Carassius carassius', 'Cyprinidés', 'cyprinidé', NULL, '🟡', 'Proche de la carpe, très résistant'),
('Aspe', 'Aspius aspius', 'Cyprinidés', 'cyprinidé', 40, '⚡', 'Grand prédateur cyprinidé, chasseur de surface'),
('Toxostome', 'Parachondrostoma toxostoma', 'Cyprinidés', 'cyprinidé', NULL, '🐟', 'Cyprinidé endémique du bassin du Rhône'),
('Bouvière', 'Rhodeus amarus', 'Cyprinidés', 'cyprinidé', NULL, '💎', 'Très petit, reproduction dans les moules'),
('Spirlin', 'Alburnoides bipunctatus', 'Cyprinidés', 'cyprinidé', NULL, '✨', 'Petit poisson à bande latérale sombre');

-- AUTRES ESPÈCES
INSERT INTO public.fish_species (common_name, scientific_name, family, category, min_legal_size, icon_emoji, description) VALUES
('Loche franche', 'Barbatula barbatula', 'Balitoridés', 'autre', NULL, '🪱', 'Petit poisson de fond serpentiforme'),
('Lotte de rivière (Mustèle)', 'Lota lota', 'Lotidés', 'autre', NULL, '🐟', 'Seul gadidé d''eau douce, actif la nuit'),
('Poisson-chat', 'Ameiurus melas', 'Ictaluridés', 'autre', NULL, '😾', 'Espèce invasive nord-américaine, nuisible'),
('Perche-soleil', 'Lepomis gibbosus', 'Centrarchidés', 'autre', NULL, '☀️', 'Petit poisson coloré, espèce invasive'),
('Grémille', 'Gymnocephalus cernua', 'Percidés', 'autre', NULL, '🐟', 'Petit percidé tacheté'),
('Apron du Rhône', 'Zingel asper', 'Percidés', 'autre', NULL, '🔒', 'Espèce endémique et protégée, en danger critique'),
('Chabot commun', 'Cottus gobio', 'Cottidés', 'autre', NULL, '🪨', 'Petit poisson de fond à grosse tête'),
('Épinoche', 'Gasterosteus aculeatus', 'Gastérostéidés', 'autre', NULL, '🔱', 'Petit poisson à épines dorsales'),
('Épinochette', 'Pungitius pungitius', 'Gastérostéidés', 'autre', NULL, '🔱', 'Proche de l''épinoche, plus petite');

-- ===================================================================
-- 3. MODIFICATION DE fish_catches POUR RÉFÉRENCER fish_species
-- ===================================================================

-- Ajouter une colonne optionnelle pour référencer l'espèce
ALTER TABLE public.fish_catches 
ADD COLUMN IF NOT EXISTS species_id INTEGER REFERENCES public.fish_species(id) ON DELETE SET NULL;

-- Index pour les recherches
CREATE INDEX IF NOT EXISTS idx_fish_catches_species_id ON public.fish_catches(species_id);

-- Commentaire
COMMENT ON COLUMN public.fish_catches.species_id IS 'Référence vers l''espèce de poisson (optionnel, si l''utilisateur choisit dans la liste)';

-- ===================================================================
-- 4. MESSAGE DE CONFIRMATION
-- ===================================================================

DO $$
BEGIN
    RAISE NOTICE '🐟 ============================================';
    RAISE NOTICE '✅ Table fish_species créée avec succès !';
    RAISE NOTICE '🐟 ============================================';
    RAISE NOTICE '';
    RAISE NOTICE '📊 Statistiques :';
    RAISE NOTICE '  • % espèces de poissons insérées', (SELECT COUNT(*) FROM public.fish_species);
    RAISE NOTICE '  • % carnassiers', (SELECT COUNT(*) FROM public.fish_species WHERE category = 'carnassier');
    RAISE NOTICE '  • % salmonidés', (SELECT COUNT(*) FROM public.fish_species WHERE category = 'salmonidé');
    RAISE NOTICE '  • % migrateurs', (SELECT COUNT(*) FROM public.fish_species WHERE category = 'migrateur');
    RAISE NOTICE '  • % cyprinidés', (SELECT COUNT(*) FROM public.fish_species WHERE category = 'cyprinidé');
    RAISE NOTICE '  • % autres', (SELECT COUNT(*) FROM public.fish_species WHERE category = 'autre');
    RAISE NOTICE '';
    RAISE NOTICE '🎣 Les poissons sont prêts à être pêchés !';
END $$;

-- Afficher un aperçu
SELECT 
    'Aperçu des espèces' as section,
    category,
    COUNT(*) as nb_especes,
    STRING_AGG(common_name, ', ' ORDER BY common_name) as especes
FROM public.fish_species
GROUP BY category
ORDER BY 
    CASE category
        WHEN 'carnassier' THEN 1
        WHEN 'salmonidé' THEN 2
        WHEN 'migrateur' THEN 3
        WHEN 'cyprinidé' THEN 4
        WHEN 'autre' THEN 5
    END;
