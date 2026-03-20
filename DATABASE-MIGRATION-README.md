# Migration de la base de données FishingSpot

## ⚠️ ATTENTION
Ce script supprime et recrée toutes les tables. **Toutes les données existantes seront perdues !**

## 📋 Changements principaux

### 1. **Unités de mesure**
- **Longueur** : Stockée en centimètres (cm)
  - Interface utilisateur : Sélection séparée Mètres + Centimètres
  - Base de données : Conversion automatique (1m 50cm = 150cm)

- **Poids** : Stocké en grammes (g)
  - Interface utilisateur : Sélection séparée Kilogrammes + Grammes
  - Base de données : Conversion automatique (2kg 500g = 2500g)

### 2. **Coordonnées GPS**
- Changement de `double precision` vers `text`
- Permet une meilleure précision et évite les erreurs de validation

### 3. **Liste de poissons pré-définie**
- 34 espèces de poissons de rivière françaises
- Possibilité d'ajouter une espèce personnalisée

## 🚀 Comment appliquer la migration

### Méthode 1 : Via l'interface Supabase (Recommandée)

1. Connectez-vous à votre projet Supabase : https://app.supabase.com
2. Allez dans **SQL Editor** (menu latéral)
3. Créez une nouvelle requête
4. Copiez-collez tout le contenu du fichier `database-migration.sql`
5. Cliquez sur **RUN** pour exécuter le script

### Méthode 2 : Via psql (ligne de commande)

```bash
psql -h [your-supabase-host] -U postgres -d postgres -f database-migration.sql
```

## 📊 Structure des tables après migration

### fish_catches
| Colonne | Type | Description |
|---------|------|-------------|
| id | SERIAL | Identifiant unique |
| user_id | UUID | Référence vers l'utilisateur |
| fish_name | TEXT | Nom de l'espèce |
| photo_url | TEXT | URL de la photo |
| latitude | TEXT | Latitude GPS (string) |
| longitude | TEXT | Longitude GPS (string) |
| location_name | TEXT | Nom du lieu |
| catch_date | DATE | Date de la prise |
| catch_time | TIME | Heure de la prise |
| **length** | DOUBLE PRECISION | **Longueur en CM** |
| **weight** | DOUBLE PRECISION | **Poids en GRAMMES** |
| notes | TEXT | Notes personnelles |
| setup_id | INTEGER | Référence vers le montage |
| created_at | TIMESTAMPTZ | Date de création |
| updated_at | TIMESTAMPTZ | Date de modification |

### fishing_setups
*(Inchangée)*

### user_profiles
*(Inchangée)*

## ✅ Vérification post-migration

Après avoir exécuté le script, vérifiez que :

1. Les 3 tables existent : `fish_catches`, `fishing_setups`, `user_profiles`
2. Les politiques RLS sont actives (Security > Policies)
3. Les index sont créés (Database > Indexes)

## 🔄 Rafraîchissement automatique

L'application rafraîchit maintenant automatiquement les données :
- Au chargement de la page
- Lors du changement d'onglet (focus/blur)
- Après chaque sauvegarde

## 📱 Nouvelles fonctionnalités UI

1. **Sélection d'espèce** : Liste déroulante avec 34 poissons + option personnalisée
2. **Mesures précises** : Sélecteurs séparés (m/cm et kg/g)
3. **Montage en évidence** : Card dédiée avec style distinct
4. **Copie GPS** : Bouton pour partager les coordonnées

## 🐛 En cas de problème

Si vous rencontrez des erreurs :

1. Vérifiez que vous êtes connecté avec les droits `postgres`
2. Assurez-vous que les extensions nécessaires sont activées
3. Consultez les logs dans Supabase : Logs > Postgres Logs

## 📞 Support

Pour toute question, vérifiez :
- La console du navigateur (F12)
- Les logs Supabase
- La documentation Supabase : https://supabase.com/docs
