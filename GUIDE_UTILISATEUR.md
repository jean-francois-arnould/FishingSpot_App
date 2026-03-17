# Guide d'Utilisation - FishingSpot

## 🎣 Bienvenue dans FishingSpot !

FishingSpot est votre compagnon de pêche numérique, compatible iOS et Android, développé avec .NET MAUI.

---

# Guide d'Utilisation - FishingSpot

## 🎣 Bienvenue dans FishingSpot !

FishingSpot est votre compagnon de pêche numérique, compatible iOS et Android, développé avec .NET MAUI.

---

## 📱 Navigation de l'Application

L'application est organisée en **2 onglets principaux** accessibles depuis la barre de navigation en bas de l'écran :

### 1. 📚 **Documentation**
*Guide complet des espèces de poissons*

**Contenu disponible :**
- **7 espèces de poissons** détaillées :
  1. Truite Arc-en-ciel
  2. Brochet
  3. Sandre
  4. Carpe Commune
  5. Perche
  6. Silure
  7. Black Bass

**Informations affichées pour chaque poisson :**
- ✅ Nom commun et nom scientifique
- ✅ Description détaillée
- ✅ Habitat préféré
- ✅ Meilleurs appâts recommandés
- ✅ Taille moyenne et maximale

**Utilisation :**
- Parcourez la liste pour découvrir les espèces
- Consultez les informations pour améliorer vos techniques de pêche

---

### 2. 🐟 **Mes Poissons**
*Journal de bord de vos captures*

**Fonctionnalités :**
- Affichage de toutes vos prises enregistrées
- Détails affichés :
  - Nom du poisson
  - Photo (si disponible)
  - Longueur et poids
  - Date et heure de capture
  - Lieu de la prise avec coordonnées GPS

**Actions disponibles :**
- ➕ Bouton "Ajouter une Capture" en bas de l'écran
- 🗑️ Glissez vers la gauche sur une capture pour la supprimer

**Écran vide :**
Quand aucune capture n'est enregistrée, un message d'accueil vous invite à ajouter votre première prise !

---

## ➕ **Page d'Ajout de Capture**

Accessible depuis l'onglet "Mes Poissons" en appuyant sur le bouton vert "➕ Ajouter une Capture".

### Formulaire de saisie :

#### 📸 **Photo du Poisson**
- Appuyez sur la zone de photo pour ouvrir l'appareil photo
- Prenez une photo de votre prise
- La photo est automatiquement enregistrée

#### 🐟 **Informations du Poisson**
- **Nom** : Entrez le nom du poisson (Truite, Brochet, etc.)
  - ⚠️ Champ obligatoire

#### 📏 **Mesures**
- **Longueur** : En centimètres (ex: 45)
  - ⚠️ Obligatoire, doit être un nombre positif
- **Poids** : En kilogrammes (ex: 2.5)
  - ⚠️ Obligatoire, doit être un nombre positif

#### 📍 **Localisation GPS**
- **Nom du lieu** : Entrez manuellement (ex: "Lac de Sainte-Croix")
- **Bouton GPS 📍** : Appuyez pour capturer votre position actuelle
  - **Utilise automatiquement la géolocalisation du téléphone**
  - Affiche les coordonnées GPS précises (latitude/longitude)
  - Remplit automatiquement le nom du lieu si vide
  - ⚠️ **Important** : Assurez-vous que le GPS est activé sur votre téléphone

**Fonctionnement de la géolocalisation :**
1. Appuyez sur "📍 Utiliser ma position actuelle"
2. Autorisez l'accès à la localisation si demandé
3. L'application capture automatiquement vos coordonnées GPS
4. Les coordonnées s'affichent à l'écran (ex: 📍 GPS: 43.296482, 5.369780)

#### 📅 **Date et Heure**
- **Date** : Sélecteur de date (par défaut : aujourd'hui)
- **Heure** : Sélecteur d'heure (par défaut : heure actuelle)

#### 📝 **Notes** (optionnel)
- Zone de texte libre pour noter :
  - Conditions météo
  - Appâts utilisés
  - Techniques employées
  - Observations diverses

#### 💾 **Enregistrement**
- Bouton vert "Enregistrer la Capture" en bas
- Validation automatique des champs obligatoires
- Message de confirmation après sauvegarde
- Retour automatique à la liste "Mes Poissons"

---

## 🔐 **Permissions Requises**

L'application demande les permissions suivantes :

### Android
- 📷 **Caméra** : Pour prendre des photos des poissons
- 📍 **Localisation GPS** : Pour enregistrer automatiquement le lieu de capture
- 💾 **Stockage** : Pour sauvegarder les photos

### iOS
- 📷 **Appareil photo** : "L'application a besoin d'accéder à votre appareil photo..."
- 📍 **Localisation** : "L'application a besoin d'accéder à votre localisation..."
- 🖼️ **Photos** : "L'application a besoin d'accéder à vos photos..."

*Toutes ces permissions sont nécessaires pour le bon fonctionnement de l'application.*

---

## 💡 **Conseils d'Utilisation**

### Pour une meilleure expérience :

1. **Autorisez toutes les permissions** lors du premier lancement
2. **Activez votre GPS** avant d'ajouter une capture pour un positionnement précis
3. **Utilisez la géolocalisation automatique** au lieu de saisir manuellement les coordonnées
4. **Prenez des photos nettes** en bon éclairage
5. **Mesurez avec précision** pour des statistiques fiables
6. **Ajoutez des notes** pour vous souvenir des détails importants

### Géolocalisation GPS :

✅ **Avantages** :
- Capture instantanée et précise de votre position
- Coordonnées exactes pour retrouver vos spots
- Pas besoin de saisir manuellement l'adresse

⚠️ **Important** :
- Le GPS doit être activé sur votre téléphone
- Une connexion réseau peut améliorer la précision
- La première localisation peut prendre quelques secondes

### Stockage des données :

⚠️ **Actuellement** : Les données sont stockées en mémoire et seront perdues à la fermeture de l'application.

✅ **Prochaine version** : Intégration de SQLite pour un stockage persistant.

---

## 🚀 **Fonctionnalités à Venir**

- ✅ Stockage persistant avec SQLite
- ✅ Vue carte avec tous vos spots de pêche (optionnel)
- ✅ Statistiques de pêche (graphiques, totaux)
- ✅ Export des données (CSV, PDF)
- ✅ Partage sur les réseaux sociaux
- ✅ Prévisions météo en temps réel
- ✅ Carnets de pêche avec calendrier
- ✅ Mode hors ligne complet

---

## 📞 **Support**

Pour toute question ou suggestion :
- Consultez le fichier README.md
- Vérifiez que vous avez la dernière version
- Assurez-vous que toutes les permissions sont accordées
- Vérifiez que le GPS est activé sur votre téléphone

---

## 📄 **Version**

**FishingSpot v1.0**  
Compatible : iOS 15+ / Android 5.0+  
Framework : .NET MAUI (.NET 10)

---

**Bonne pêche ! 🎣🐟**
*Guide complet des espèces de poissons*

**Contenu disponible :**
- **7 espèces de poissons** détaillées :
  1. Truite Arc-en-ciel
  2. Brochet
  3. Sandre
  4. Carpe Commune
  5. Perche
  6. Silure
  7. Black Bass

**Informations affichées pour chaque poisson :**
- ✅ Nom commun et nom scientifique
- ✅ Description détaillée
- ✅ Habitat préféré
- ✅ Meilleurs appâts recommandés
- ✅ Taille moyenne et maximale

**Utilisation :**
- Parcourez la liste pour découvrir les espèces
- Consultez les informations pour améliorer vos techniques de pêche

---

### 3. 🐟 **Mes Poissons**
*Journal de bord de vos captures*

**Fonctionnalités :**
- Affichage de toutes vos prises enregistrées
- Détails affichés :
  - Nom du poisson
  - Photo (si disponible)
  - Longueur et poids
  - Date et heure de capture
  - Lieu de la prise

**Actions disponibles :**
- ➕ Bouton "Ajouter une Capture" en bas de l'écran
- 🗑️ Glissez vers la gauche sur une capture pour la supprimer

**Écran vide :**
Quand aucune capture n'est enregistrée, un message d'accueil vous invite à ajouter votre première prise !

---

## ➕ **Page d'Ajout de Capture**

Accessible depuis l'onglet "Mes Poissons" en appuyant sur le bouton vert "➕ Ajouter une Capture".

### Formulaire de saisie :

#### 📸 **Photo du Poisson**
- Appuyez sur la zone de photo pour ouvrir l'appareil photo
- Prenez une photo de votre prise
- La photo est automatiquement enregistrée

#### 🐟 **Informations du Poisson**
- **Nom** : Entrez le nom du poisson (Truite, Brochet, etc.)
  - ⚠️ Champ obligatoire

#### 📏 **Mesures**
- **Longueur** : En centimètres (ex: 45)
  - ⚠️ Obligatoire, doit être un nombre positif
- **Poids** : En kilogrammes (ex: 2.5)
  - ⚠️ Obligatoire, doit être un nombre positif

#### 📍 **Localisation**
- **Nom du lieu** : Entrez manuellement (ex: "Lac de Sainte-Croix")
- **Bouton GPS** : Appuyez pour obtenir votre position actuelle
  - Affiche les coordonnées GPS précises
  - Remplit automatiquement le nom du lieu si vide

#### 📅 **Date et Heure**
- **Date** : Sélecteur de date (par défaut : aujourd'hui)
- **Heure** : Sélecteur d'heure (par défaut : heure actuelle)

#### 📝 **Notes** (optionnel)
- Zone de texte libre pour noter :
  - Conditions météo
  - Appâts utilisés
  - Techniques employées
  - Observations diverses

#### 💾 **Enregistrement**
- Bouton vert "Enregistrer la Capture" en bas
- Validation automatique des champs obligatoires
- Message de confirmation après sauvegarde
- Retour automatique à la liste "Mes Poissons"

---

## 🔐 **Permissions Requises**

L'application demande les permissions suivantes :

### Android
- 📷 **Caméra** : Pour prendre des photos des poissons
- 📍 **Localisation** : Pour enregistrer le lieu GPS
- 💾 **Stockage** : Pour sauvegarder les photos

### iOS
- 📷 **Appareil photo** : "L'application a besoin d'accéder à votre appareil photo..."
- 📍 **Localisation** : "L'application a besoin d'accéder à votre localisation..."
- 🖼️ **Photos** : "L'application a besoin d'accéder à vos photos..."

*Toutes ces permissions sont nécessaires pour le bon fonctionnement de l'application.*

---

## 💡 **Conseils d'Utilisation**

### Pour une meilleure expérience :

1. **Autorisez toutes les permissions** lors du premier lancement
2. **Activez votre GPS** avant d'ajouter une capture pour un positionnement précis
3. **Prenez des photos nettes** en bon éclairage
4. **Mesurez avec précision** pour des statistiques fiables
5. **Ajoutez des notes** pour vous souvenir des détails importants

### Stockage des données :

⚠️ **Actuellement** : Les données sont stockées en mémoire et seront perdues à la fermeture de l'application.

✅ **Prochaine version** : Intégration de SQLite pour un stockage persistant.

---

## 🚀 **Fonctionnalités à Venir**

- ✅ Stockage persistant avec SQLite
- ✅ Intégration complète de Google Maps
- ✅ Statistiques de pêche (graphiques, totaux)
- ✅ Export des données (CSV, PDF)
- ✅ Partage sur les réseaux sociaux
- ✅ Prévisions météo en temps réel
- ✅ Ajout de spots personnalisés sur la carte
- ✅ Carnets de pêche avec calendrier
- ✅ Mode hors ligne complet

---

## 📞 **Support**

Pour toute question ou suggestion :
- Consultez le fichier README.md
- Vérifiez que vous avez la dernière version
- Assurez-vous que toutes les permissions sont accordées

---

## 📄 **Version**

**FishingSpot v1.0**  
Compatible : iOS 15+ / Android 5.0+  
Framework : .NET MAUI (.NET 10)

---

**Bonne pêche ! 🎣🐟**
