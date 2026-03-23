-- Migration pour ajouter les champs météo à la table fish_catches
-- À exécuter dans l'éditeur SQL de Supabase

-- Ajout des colonnes météo
ALTER TABLE fish_catches 
ADD COLUMN IF NOT EXISTS weather_temperature DOUBLE PRECISION,
ADD COLUMN IF NOT EXISTS weather_condition TEXT,
ADD COLUMN IF NOT EXISTS weather_code INTEGER,
ADD COLUMN IF NOT EXISTS wind_speed DOUBLE PRECISION,
ADD COLUMN IF NOT EXISTS humidity INTEGER;

-- Commentaires pour documenter les colonnes
COMMENT ON COLUMN fish_catches.weather_temperature IS 'Température en degrés Celsius au moment de la prise';
COMMENT ON COLUMN fish_catches.weather_condition IS 'Description des conditions météo (ex: "Pluie", "Ensoleillé")';
COMMENT ON COLUMN fish_catches.weather_code IS 'Code WMO de la condition météo (0-99)';
COMMENT ON COLUMN fish_catches.wind_speed IS 'Vitesse du vent en km/h';
COMMENT ON COLUMN fish_catches.humidity IS 'Taux d''humidité en pourcentage (0-100)';

-- Vérification
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'fish_catches' 
AND column_name IN ('weather_temperature', 'weather_condition', 'weather_code', 'wind_speed', 'humidity')
ORDER BY column_name;
