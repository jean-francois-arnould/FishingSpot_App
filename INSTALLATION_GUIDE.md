# 🚀 Installation de la fonctionnalité météo - Guide pas à pas

## Étape 1 : Mise à jour de la base de données Supabase ⚠️ OBLIGATOIRE

### 1.1 Ouvrir Supabase
1. Allez sur https://supabase.com
2. Connectez-vous à votre compte
3. Ouvrez votre projet FishingSpot

### 1.2 Exécuter le script SQL
1. Dans le menu de gauche, cliquez sur **"SQL Editor"**
2. Cliquez sur **"New query"**
3. Copiez-collez le script suivant :

```sql
ALTER TABLE fish_catches 
ADD COLUMN IF NOT EXISTS weather_temperature DOUBLE PRECISION,
ADD COLUMN IF NOT EXISTS weather_condition TEXT,
ADD COLUMN IF NOT EXISTS weather_code INTEGER,
ADD COLUMN IF NOT EXISTS wind_speed DOUBLE PRECISION,
ADD COLUMN IF NOT EXISTS humidity INTEGER;
```

4. Cliquez sur **"Run"** (ou appuyez sur Ctrl+Entrée)
5. Vous devriez voir : **"Success. No rows returned"**

### 1.3 Vérifier l'installation
Exécutez cette requête de vérification :

```sql
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'fish_catches' 
AND column_name IN ('weather_temperature', 'weather_condition', 'weather_code', 'wind_speed', 'humidity')
ORDER BY column_name;
```

Vous devriez voir 5 lignes avec les colonnes météo.

---

## Étape 2 : Tester l'application

### 2.1 Lancer l'application
```powershell
dotnet run
```

Ou appuyez sur **F5** dans Visual Studio.

### 2.2 Tester la fonctionnalité météo

#### Test 1 : Ajouter une prise avec météo
1. **Ouvrez l'application** dans votre navigateur
2. **Connectez-vous** à votre compte
3. **Cliquez sur "Ajouter une prise"**
4. **Autorisez la géolocalisation** quand le navigateur le demande
5. **Attendez quelques secondes** :
   - Les coordonnées GPS devraient apparaître
   - La météo devrait se charger automatiquement
6. **Vérifiez l'affichage** :
   - ☀️ Icône météo
   - 🌡️ Température
   - 💨 Vitesse du vent
   - 💧 Taux d'humidité

#### Test 2 : Si la météo ne s'affiche pas automatiquement
1. **Vérifiez que le GPS est actif** (coordonnées visibles)
2. **Cliquez sur le bouton "🌤️ Récupérer la météo"**
3. **Attendez le chargement**

#### Test 3 : Enregistrer et visualiser
1. **Remplissez le formulaire** (espèce, taille, poids...)
2. **Cliquez sur "✅ Enregistrer"**
3. **Allez dans la liste des prises**
4. **Cliquez sur la prise que vous venez de créer**
5. **Vérifiez que la météo s'affiche** dans la section dédiée

---

## Étape 3 : Résolution des problèmes

### Problème 1 : La météo ne s'affiche pas

**Causes possibles :**
- ❌ GPS non autorisé
- ❌ Pas de connexion internet
- ❌ Problème avec l'API Open-Meteo

**Solutions :**
1. **Vérifiez le GPS** :
   - Cliquez sur l'icône 🔒 dans la barre d'adresse
   - Autorisez la localisation
   - Rechargez la page

2. **Vérifiez la connexion internet** :
   - Ouvrez un autre site web
   - Vérifiez votre connexion

3. **Consultez la console** :
   - Appuyez sur **F12**
   - Onglet **Console**
   - Recherchez les erreurs en rouge

### Problème 2 : Erreur lors de l'enregistrement

**Message d'erreur** : "Column 'weather_temperature' does not exist"

**Solution** : Vous n'avez pas exécuté le script SQL dans Supabase
- Retournez à l'**Étape 1**
- Exécutez le script SQL

### Problème 3 : La météo affichée semble incorrecte

**Cause** : C'est la météo au moment de la prise, pas la météo actuelle

**C'est normal** : L'API récupère les conditions météo actuelles au moment où vous ajoutez la prise. Si vous consultez la prise plus tard, c'est la météo de ce moment-là qui est affichée, pas la météo actuelle.

### Problème 4 : Build error

**Si vous avez une erreur de compilation** :

```powershell
dotnet clean
dotnet restore
dotnet build
```

---

## Étape 4 : Vérification complète

### Checklist ✅

- [ ] Script SQL exécuté dans Supabase
- [ ] 5 colonnes météo créées (vérification OK)
- [ ] Application compilée sans erreur
- [ ] GPS autorisé dans le navigateur
- [ ] Météo s'affiche dans le formulaire
- [ ] Prise de test enregistrée avec succès
- [ ] Météo visible dans les détails de la prise

---

## 🎉 Félicitations !

Si tous les tests sont passés, votre application FishingSpot dispose maintenant d'un système météo complet !

### Utilisation quotidienne

Désormais, à chaque nouvelle prise :
1. Le GPS se lance automatiquement
2. La météo est récupérée automatiquement
3. Tout est enregistré avec la prise
4. Vous pouvez analyser l'impact de la météo sur vos prises

### Fonctionnalités disponibles

- ✅ Récupération automatique de la météo
- ✅ Affichage dans le formulaire avec design moderne
- ✅ Sauvegarde avec chaque prise
- ✅ Visualisation dans les détails
- ✅ Données précises (API Open-Meteo)

### Données enregistrées

Pour chaque prise :
- 🌡️ Température (°C)
- 🌤️ Conditions météo (texte + emoji)
- 💨 Vitesse du vent (km/h)
- 💧 Taux d'humidité (%)

---

## 📞 Besoin d'aide ?

### Documentation disponible
- **`RESUME_MODIFICATIONS.md`** : Résumé de toutes les modifications
- **`README_METEO.md`** : Guide utilisateur complet
- **`WEATHER_INTEGRATION.md`** : Documentation technique
- **`sql/add_weather_columns.sql`** : Script SQL (déjà utilisé)

### Débogage
1. Ouvrez la console du navigateur (**F12**)
2. Vérifiez les messages dans l'onglet **Console**
3. Recherchez les erreurs en rouge

### Support API
- Documentation Open-Meteo : https://open-meteo.com/en/docs
- Status de l'API : https://status.open-meteo.com

---

**Développé pour FishingSpot**  
**Version : 1.0**  
**Date : 2025**  
**Status : ✅ Production ready**
