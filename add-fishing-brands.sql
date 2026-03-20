-- =====================================================
-- Script pour créer la table fishing_brands
-- Marques de matériel de pêche pré-enregistrées
-- =====================================================

-- Créer la table fishing_brands
CREATE TABLE IF NOT EXISTS public.fishing_brands (
    id SERIAL PRIMARY KEY,
    category TEXT NOT NULL CHECK (category IN ('rod', 'reel', 'line', 'lure')),
    name TEXT NOT NULL,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT unique_brand_per_category UNIQUE (category, name)
);

-- Activer RLS (Row Level Security)
ALTER TABLE public.fishing_brands ENABLE ROW LEVEL SECURITY;

-- Policy: Tout le monde peut lire les marques
CREATE POLICY "Everyone can view fishing brands"
    ON public.fishing_brands
    FOR SELECT
    USING (true);

-- Policy: Seuls les utilisateurs authentifiés peuvent ajouter des marques
CREATE POLICY "Authenticated users can insert fishing brands"
    ON public.fishing_brands
    FOR INSERT
    TO authenticated
    WITH CHECK (true);

-- =====================================================
-- Insertion des marques pré-enregistrées
-- =====================================================

-- CANNES À PÊCHE (rod)
INSERT INTO public.fishing_brands (category, name) VALUES
('rod', 'Shimano'),
('rod', 'Daiwa'),
('rod', 'Abu Garcia'),
('rod', 'Penn'),
('rod', 'Mitchell'),
('rod', 'Berkley'),
('rod', 'Savage Gear'),
('rod', 'Fox Rage'),
('rod', 'St. Croix'),
('rod', 'G. Loomis'),
('rod', 'Major Craft'),
('rod', 'Tenryu'),
('rod', 'Illex'),
('rod', 'Sakura'),
('rod', 'Gunki')
ON CONFLICT (category, name) DO NOTHING;

-- MOULINETS (reel)
INSERT INTO public.fishing_brands (category, name) VALUES
('reel', 'Shimano'),
('reel', 'Daiwa'),
('reel', 'Abu Garcia'),
('reel', 'Penn'),
('reel', 'Okuma'),
('reel', 'Mitchell'),
('reel', 'Lew''s'),
('reel', 'Quantum'),
('reel', 'Ryobi'),
('reel', 'Pflueger')
ON CONFLICT (category, name) DO NOTHING;

-- FILS DE PÊCHE (line)
INSERT INTO public.fishing_brands (category, name) VALUES
('line', 'Berkley'),
('line', 'PowerPro'),
('line', 'SpiderWire'),
('line', 'Sufix'),
('line', 'Daiwa'),
('line', 'Shimano'),
('line', 'Sunline'),
('line', 'Yo-Zuri'),
('line', 'Seaguar'),
('line', 'Varivas')
ON CONFLICT (category, name) DO NOTHING;

-- LEURRES (lure)
INSERT INTO public.fishing_brands (category, name) VALUES
('lure', 'Rapala'),
('lure', 'Savage Gear'),
('lure', 'Illex'),
('lure', 'Megabass'),
('lure', 'Strike King'),
('lure', 'Lucky Craft'),
('lure', 'Yo-Zuri'),
('lure', 'Storm'),
('lure', 'Gunki'),
('lure', 'Westin'),
('lure', 'Z-Man'),
('lure', 'Keitech'),
('lure', 'Molix'),
('lure', 'Salmo')
ON CONFLICT (category, name) DO NOTHING;

-- =====================================================
-- Vérification
-- =====================================================

-- Compter les marques par catégorie
SELECT 
    category,
    COUNT(*) as total_brands,
    CASE 
        WHEN category = 'rod' THEN '🎣 Cannes à pêche'
        WHEN category = 'reel' THEN '🎡 Moulinets'
        WHEN category = 'line' THEN '🧵 Fils de pêche'
        WHEN category = 'lure' THEN '🐟 Leurres'
    END as category_name
FROM public.fishing_brands
WHERE is_active = true
GROUP BY category
ORDER BY category;

-- Liste complète
SELECT * FROM public.fishing_brands ORDER BY category, name;
