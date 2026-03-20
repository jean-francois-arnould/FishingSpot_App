-- ===================================================================
-- Script de migration de la base de données FishingSpot
-- Ce script supprime et recrée toutes les tables avec la structure correcte
-- ===================================================================

-- ATTENTION : Ce script supprime toutes les données existantes !
-- Assurez-vous d'avoir une sauvegarde avant de l'exécuter

-- ===================================================================
-- 1. SUPPRESSION DES TABLES EXISTANTES
-- ===================================================================

DROP TABLE IF EXISTS public.fish_catches CASCADE;
DROP TABLE IF EXISTS public.fishing_setups CASCADE;
DROP TABLE IF EXISTS public.user_profiles CASCADE;

-- ===================================================================
-- 2. CRÉATION DE LA TABLE user_profiles
-- ===================================================================

CREATE TABLE public.user_profiles (
    id UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
    email TEXT NOT NULL UNIQUE,
    display_name TEXT,
    avatar_url TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Activer RLS (Row Level Security)
ALTER TABLE public.user_profiles ENABLE ROW LEVEL SECURITY;

-- Politique : Les utilisateurs peuvent voir et modifier uniquement leur profil
CREATE POLICY "Users can view own profile" ON public.user_profiles
    FOR SELECT USING (auth.uid() = id);

CREATE POLICY "Users can update own profile" ON public.user_profiles
    FOR UPDATE USING (auth.uid() = id);

CREATE POLICY "Users can insert own profile" ON public.user_profiles
    FOR INSERT WITH CHECK (auth.uid() = id);

-- ===================================================================
-- 3. CRÉATION DE LA TABLE fishing_setups
-- ===================================================================

CREATE TABLE public.fishing_setups (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    rod_brand TEXT,
    rod_model TEXT,
    rod_length DOUBLE PRECISION,
    rod_power TEXT,
    reel_brand TEXT,
    reel_model TEXT,
    reel_type TEXT,
    line_type TEXT,
    line_diameter DOUBLE PRECISION,
    line_breaking_strength DOUBLE PRECISION,
    hook_size TEXT,
    bait_type TEXT,
    description TEXT,
    is_current BOOLEAN DEFAULT FALSE,
    is_favorite BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Activer RLS
ALTER TABLE public.fishing_setups ENABLE ROW LEVEL SECURITY;

-- Politiques RLS
CREATE POLICY "Users can view own setups" ON public.fishing_setups
    FOR SELECT USING (auth.uid() = user_id);

CREATE POLICY "Users can insert own setups" ON public.fishing_setups
    FOR INSERT WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update own setups" ON public.fishing_setups
    FOR UPDATE USING (auth.uid() = user_id);

CREATE POLICY "Users can delete own setups" ON public.fishing_setups
    FOR DELETE USING (auth.uid() = user_id);

-- ===================================================================
-- 4. CRÉATION DE LA TABLE fish_catches
-- ===================================================================

CREATE TABLE public.fish_catches (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    fish_name TEXT NOT NULL,
    photo_url TEXT,
    latitude TEXT,
    longitude TEXT,
    location_name TEXT,
    catch_date DATE NOT NULL DEFAULT CURRENT_DATE,
    catch_time TIME,
    length DOUBLE PRECISION DEFAULT 0,  -- Stocké en cm (100cm = 1m)
    weight DOUBLE PRECISION DEFAULT 0,   -- Stocké en grammes (1000g = 1kg)
    notes TEXT,
    setup_id INTEGER REFERENCES public.fishing_setups(id) ON DELETE SET NULL,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Activer RLS
ALTER TABLE public.fish_catches ENABLE ROW LEVEL SECURITY;

-- Politiques RLS
CREATE POLICY "Users can view own catches" ON public.fish_catches
    FOR SELECT USING (auth.uid() = user_id);

CREATE POLICY "Users can insert own catches" ON public.fish_catches
    FOR INSERT WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update own catches" ON public.fish_catches
    FOR UPDATE USING (auth.uid() = user_id);

CREATE POLICY "Users can delete own catches" ON public.fish_catches
    FOR DELETE USING (auth.uid() = user_id);

-- ===================================================================
-- 5. CRÉATION DES INDEX POUR LES PERFORMANCES
-- ===================================================================

CREATE INDEX idx_fish_catches_user_id ON public.fish_catches(user_id);
CREATE INDEX idx_fish_catches_catch_date ON public.fish_catches(catch_date DESC);
CREATE INDEX idx_fish_catches_fish_name ON public.fish_catches(fish_name);

CREATE INDEX idx_fishing_setups_user_id ON public.fishing_setups(user_id);
CREATE INDEX idx_fishing_setups_is_current ON public.fishing_setups(is_current) WHERE is_current = TRUE;
CREATE INDEX idx_fishing_setups_is_favorite ON public.fishing_setups(is_favorite) WHERE is_favorite = TRUE;

CREATE INDEX idx_user_profiles_email ON public.user_profiles(email);

-- ===================================================================
-- 6. CRÉATION DES FONCTIONS DE MISE À JOUR AUTOMATIQUE
-- ===================================================================

-- Fonction pour mettre à jour updated_at automatiquement
CREATE OR REPLACE FUNCTION public.update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Triggers pour updated_at
CREATE TRIGGER update_user_profiles_updated_at
    BEFORE UPDATE ON public.user_profiles
    FOR EACH ROW
    EXECUTE FUNCTION public.update_updated_at_column();

CREATE TRIGGER update_fishing_setups_updated_at
    BEFORE UPDATE ON public.fishing_setups
    FOR EACH ROW
    EXECUTE FUNCTION public.update_updated_at_column();

CREATE TRIGGER update_fish_catches_updated_at
    BEFORE UPDATE ON public.fish_catches
    FOR EACH ROW
    EXECUTE FUNCTION public.update_updated_at_column();

-- ===================================================================
-- 7. COMMENTAIRES SUR LES TABLES ET COLONNES
-- ===================================================================

COMMENT ON TABLE public.fish_catches IS 'Table des prises de pêche des utilisateurs';
COMMENT ON COLUMN public.fish_catches.length IS 'Longueur en centimètres (cm). 100cm = 1m';
COMMENT ON COLUMN public.fish_catches.weight IS 'Poids en grammes (g). 1000g = 1kg';
COMMENT ON COLUMN public.fish_catches.latitude IS 'Latitude GPS (format string pour précision)';
COMMENT ON COLUMN public.fish_catches.longitude IS 'Longitude GPS (format string pour précision)';

COMMENT ON TABLE public.fishing_setups IS 'Table des montages de pêche des utilisateurs';
COMMENT ON TABLE public.user_profiles IS 'Profils utilisateurs étendus';

-- ===================================================================
-- MIGRATION TERMINÉE
-- ===================================================================

SELECT 'Migration terminée avec succès ! 🎣' AS status;
