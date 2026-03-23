# Intégration de la météo dans FishingSpot

## Modifications apportées

### 1. Nouveaux fichiers créés

#### Models/WeatherData.cs
- Modèle de données pour stocker les informations météorologiques
- Propriétés : température, code météo, vitesse du vent, humidité, pression
- Méthodes helper pour convertir les codes météo en descriptions et emojis
- Support pour l'API Open-Meteo

#### Services/IWeatherService.cs
- Interface pour le service météo
- Méthode : `GetCurrentWeatherAsync(latitude, longitude)`

#### Services/WeatherService.cs
- Implémentation du service météo utilisant l'API gratuite Open-Meteo
- URL de l'API : `https://api.open-meteo.com/v1/forecast`
- Aucune clé API requise
- Récupère : température, conditions météo, vent, humidité, pression

### 2. Fichiers modifiés

#### Models/FishCatch.cs
Ajout des propriétés pour stocker les données météo :
- `WeatherTemperature` : Température en °C
- `WeatherCondition` : Description textuelle (ex: "Pluie", "Ensoleillé")
- `WeatherCode` : Code WMO de la condition météo
- `WindSpeed` : Vitesse du vent en km/h
- `Humidity` : Humidité en %

#### AddCatch.razor
- Injection du service `IWeatherService`
- Ajout d'une section météo dans le formulaire, sous les coordonnées GPS
- Affichage des données météo avec emojis et styling
- Récupération automatique de la météo après obtention de la position GPS
- Bouton manuel pour rafraîchir la météo si nécessaire
- Variables d'état : `weatherData`, `isGettingWeather`
- Nouvelle méthode : `GetWeatherData()` pour récupérer les données météo

#### Program.cs
- Enregistrement du service `IWeatherService` dans le conteneur DI
- Configuration avec un HttpClient dédié pour les appels API météo

#### wwwroot/css/app.css
Ajout des styles CSS pour l'affichage de la météo :
- `.weather-info` : Card avec gradient coloré
- `.weather-main` : Layout pour icône, température et description
- `.weather-details` : Détails supplémentaires (vent, humidité)
- `.weather-loading` : État de chargement
- Design responsive et moderne

## Fonctionnalités

### Flux utilisateur
1. L'utilisateur ouvre le formulaire "Ajouter une prise"
2. Le GPS se lance automatiquement pour obtenir la position
3. Une fois la position obtenue, la météo est récupérée automatiquement
4. Les données météo s'affichent avec :
   - Emoji représentant les conditions (☀️, 🌧️, ⛈️, etc.)
   - Température actuelle en °C
   - Description textuelle (ex: "Pluie", "Ciel dégagé")
   - Vitesse du vent en km/h
   - Taux d'humidité en %
5. Les données météo sont enregistrées avec la prise dans la base de données

### Options pour l'utilisateur
- Si le GPS n'est pas activé : message invitant à activer la position GPS
- Si la météo n'est pas récupérée automatiquement : bouton "🌤️ Récupérer la météo"
- Possibilité de modifier manuellement les coordonnées GPS puis récupérer la météo

## API utilisée : Open-Meteo

### Pourquoi Open-Meteo ?
- ✅ **Gratuit** : Aucune clé API requise
- ✅ **Pas de limite** : Utilisation raisonnable sans restriction
- ✅ **Pas d'inscription** : Pas besoin de créer un compte
- ✅ **Données précises** : Sources météo officielles (NOAA, DWD, etc.)
- ✅ **CORS activé** : Fonctionne directement depuis Blazor WebAssembly
- ✅ **HTTPS** : Sécurisé par défaut

### Données récupérées
- Température à 2 mètres (°C)
- Code météo WMO (pour déterminer les conditions)
- Vitesse du vent à 10 mètres (km/h)
- Humidité relative (%)
- Pression atmosphérique (hPa)

### Codes météo WMO
Le service traduit automatiquement les codes en descriptions et emojis :
- 0 : Ciel dégagé ☀️
- 1-3 : Nuageux 🌤️⛅☁️
- 45-48 : Brouillard 🌫️
- 51-55 : Bruine 🌦️
- 61-65 : Pluie 🌧️
- 71-77 : Neige/Grésil 🌨️
- 80-82 : Averses 🌧️
- 95-99 : Orage ⛈️

## Base de données

### Colonnes à créer dans Supabase

Pour stocker les données météo, il faut ajouter ces colonnes à la table `catches` :

```sql
ALTER TABLE catches 
ADD COLUMN weather_temperature DOUBLE PRECISION,
ADD COLUMN weather_condition TEXT,
ADD COLUMN weather_code INTEGER,
ADD COLUMN wind_speed DOUBLE PRECISION,
ADD COLUMN humidity INTEGER;
```

## Alternative : OpenWeatherMap

Si vous préférez utiliser OpenWeatherMap (nécessite une clé API gratuite) :

1. Inscription sur https://openweathermap.org/api
2. Récupération de la clé API gratuite (60 appels/minute)
3. Modification du `WeatherService.cs` pour utiliser :
   - URL : `https://api.openweathermap.org/data/2.5/weather`
   - Paramètres : `?lat={lat}&lon={lon}&appid={API_KEY}&units=metric&lang=fr`

## Tests

Pour tester la fonctionnalité :
1. Ouvrir l'application
2. Aller dans "Ajouter une prise"
3. Autoriser la géolocalisation
4. Vérifier que les données météo s'affichent automatiquement
5. Créer une prise et vérifier que la météo est sauvegardée

## Améliorations futures possibles

- Affichage de la météo dans la liste des prises
- Affichage de la météo sur la page de détail d'une prise
- Graphiques de corrélation entre météo et prises
- Prévisions météo pour planifier les sorties de pêche
- Historique météo pour analyser les meilleures conditions de pêche
