# Configuration Supabase pour FishingSpot PWA 🎣

## Étape 1 : Créer un projet Supabase (si pas déjà fait)

1. Allez sur https://supabase.com
2. Créez un compte ou connectez-vous
3. Cliquez sur **"New Project"**
4. Donnez un nom à votre projet (ex: "FishingSpot")
5. Créez un mot de passe sécurisé pour la base de données
6. Choisissez une région proche de vous
7. Cliquez sur **"Create new project"**

## Étape 2 : Récupérer vos identifiants

Une fois le projet créé :

1. Allez dans **Settings** (⚙️) dans la barre latérale
2. Cliquez sur **API** 
3. Vous verrez deux informations importantes :

### URL du projet
- Cherchez **"Project URL"**
- Format : `https://xxxxxxxxxxxxx.supabase.co`
- Copiez cette URL complète

### Clé API publique (anon key)
- Cherchez **"anon public"** dans la section "Project API keys"
- C'est une longue chaîne de caractères
- Copiez cette clé

## Étape 3 : Configurer votre application

Ouvrez le fichier `wwwroot/appsettings.json` et remplacez les valeurs :

```json
{
  "Supabase": {
    "Url": "https://votre-projet-id.supabase.co",
    "Key": "votre-cle-anon-publique-ici"
  }
}
```

⚠️ **Important** : 
- Ne committez JAMAIS ces valeurs dans Git si votre repo est public
- Ajoutez `appsettings.json` à votre `.gitignore`
- Utilisez des variables d'environnement pour la production

## Étape 4 : Créer les tables dans Supabase

1. Allez dans **SQL Editor** dans Supabase
2. Exécutez ce script SQL :

```sql
-- Table des utilisateurs (profils)
CREATE TABLE user_profiles (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
  display_name TEXT,
  avatar_url TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Table des prises de poisson
CREATE TABLE fish_catches (
  id SERIAL PRIMARY KEY,
  user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
  species TEXT NOT NULL,
  weight_kg DECIMAL(5,2),
  length_cm DECIMAL(5,2),
  location TEXT,
  latitude DECIMAL(10,8),
  longitude DECIMAL(11,8),
  catch_date TIMESTAMP WITH TIME ZONE NOT NULL,
  photo_url TEXT,
  notes TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Active Row Level Security (RLS)
ALTER TABLE user_profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE fish_catches ENABLE ROW LEVEL SECURITY;

-- Politiques RLS pour user_profiles
CREATE POLICY "Users can view their own profile" 
  ON user_profiles FOR SELECT 
  USING (auth.uid() = user_id);

CREATE POLICY "Users can update their own profile" 
  ON user_profiles FOR UPDATE 
  USING (auth.uid() = user_id);

CREATE POLICY "Users can insert their own profile" 
  ON user_profiles FOR INSERT 
  WITH CHECK (auth.uid() = user_id);

-- Politiques RLS pour fish_catches
CREATE POLICY "Users can view their own catches" 
  ON fish_catches FOR SELECT 
  USING (auth.uid() = user_id);

CREATE POLICY "Users can insert their own catches" 
  ON fish_catches FOR INSERT 
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update their own catches" 
  ON fish_catches FOR UPDATE 
  USING (auth.uid() = user_id);

CREATE POLICY "Users can delete their own catches" 
  ON fish_catches FOR DELETE 
  USING (auth.uid() = user_id);
```

## Étape 5 : Configurer l'authentification

1. Allez dans **Authentication** > **Settings**
2. Dans **Email Auth**, assurez-vous que :
   - "Enable email confirmations" est activé ou désactivé selon vos besoins
   - Pour le développement, vous pouvez désactiver la confirmation par email

## Étape 6 : Tester votre application

1. Relancez votre application Blazor
2. Allez sur `/register`
3. Créez un compte de test
4. Vous devriez maintenant voir un message de succès au lieu de "failed to fetch"

## Dépannage

### "Failed to fetch"
- Vérifiez que votre URL Supabase est correcte
- Vérifiez que votre clé API est correcte
- Vérifiez votre connexion internet

### "Invalid API key"
- Assurez-vous d'utiliser la clé **anon public** et non la service_role key

### "Email not confirmed"
- Allez dans Authentication > Users dans Supabase
- Cliquez sur l'utilisateur et confirmez manuellement l'email pour les tests

## Prochaines étapes

Une fois configuré, vous pourrez :
- ✅ Créer des comptes utilisateurs
- ✅ Se connecter / Se déconnecter
- ✅ Enregistrer des prises de poisson
- ✅ Voir vos statistiques de pêche
