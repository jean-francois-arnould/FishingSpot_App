# 🧪 COMPTE DE TEST

## Compte de test pré-configuré

Une fois que vous avez configuré Supabase, vous pouvez créer un compte de test directement via l'interface web:

### Option 1: Créer via l'application

1. Ouvrir votre app PWA
2. Cliquer sur "Créer un compte" 
3. Utiliser ces identifiants:
   - **Email:** `test@fishingspot.com`
   - **Password:** `Test123!`
4. Se connecter

### Option 2: Créer via Supabase Dashboard

1. Aller dans votre projet Supabase
2. **Authentication** > **Users**
3. Cliquer "Add user" > "Create new user"
4. Remplir:
   - **Email:** `test@fishingspot.com`
   - **Password:** `Test123!`
   - Décocher "Auto Confirm User" si vous avez désactivé la confirmation email
5. Cliquer "Create user"

## Test de l'authentification

### 1. Page d'accueil
- Ouvrir `/`
- Vous devriez voir:
  - Titre "FishingSpot"
  - Boutons "Créer un compte" et "Se connecter"
  - 3 cartes de fonctionnalités

### 2. Inscription
- Cliquer "Créer un compte" ou aller sur `/register`
- Remplir le formulaire:
  - Email: `test@fishingspot.com`
  - Password: `Test123!`
  - Confirmer password: `Test123!`
- Cliquer "Créer mon compte"
- Message de succès devrait s'afficher
- Redirection vers `/login`

### 3. Connexion
- Sur `/login`, entrer:
  - Email: `test@fishingspot.com`
  - Password: `Test123!`
- Cliquer "Se connecter"
- Vous devriez être redirigé vers `/catches`

### 4. Navigation une fois connecté
Le menu devrait maintenant afficher:
- **Home** - Page d'accueil
- **Mes Prises** - Liste des prises (vide initialement)
- **Mon Compte** - Page de profil
- **[votre email]** - Email affiché
- **Déconnexion** - Bouton pour se déconnecter

### 5. Page "Mon Compte" (`/profile`)

Aller sur `/profile`, vous devriez voir:

**Formulaire de profil:**
- Prénom
- Nom
- Téléphone
- Pays
- Ville
- Spot favori
- Bio

**Zone dangereuse (en bas):**
- Bouton "Se déconnecter"
- Bouton "Supprimer mon compte"
  - Cliquer dessus affiche une confirmation
  - "Oui, supprimer définitivement" supprime:
    - Le profil utilisateur
    - Toutes les prises de pêche
    - Se déconnecte automatiquement

### 6. Test du profil

1. Remplir le formulaire de profil:
   - Prénom: `Jean`
   - Nom: `Pêcheur`
   - Téléphone: `+33 6 12 34 56 78`
   - Pays: `France`
   - Ville: `Paris`
   - Spot favori: `Lac de Sainte-Croix`
   - Bio: `Passionné de pêche depuis 10 ans`

2. Cliquer "Enregistrer"
3. Message "Profil enregistré avec succès!" devrait s'afficher
4. Actualiser la page (F5)
5. Les données devraient être toujours là (persistées dans Supabase)

### 7. Test de déconnexion

1. Cliquer sur "Déconnexion" dans le menu
2. Vous devriez être redirigé vers la page d'accueil
3. Le menu devrait maintenant afficher "Se connecter" et "Créer un compte"
4. Essayer d'aller sur `/catches` ou `/profile`
   - Vous devriez être redirigé vers `/login`

### 8. Test de suppression de compte

⚠️ **ATTENTION:** Cette action est irréversible!

1. Se connecter avec le compte test
2. Aller sur `/profile`
3. Scroller en bas jusqu'à la "Zone dangereuse"
4. Cliquer "Supprimer mon compte"
5. Une alerte de confirmation apparaît avec:
   - Liste des données qui seront supprimées
   - Bouton "Oui, supprimer définitivement"
   - Bouton "Annuler"
6. Cliquer "Oui, supprimer définitivement"
7. Toutes les données sont supprimées
8. Vous êtes déconnecté et redirigé vers la page d'accueil

## Vérification dans Supabase

Après avoir créé/modifié le compte test:

### Vérifier l'authentification
1. **Authentication** > **Users**
2. Vous devriez voir `test@fishingspot.com`
3. Status: "Confirmed"

### Vérifier le profil
1. **Table Editor** > `user_profiles`
2. Vous devriez voir une ligne avec:
   - `user_id`: UUID du user
   - `first_name`, `last_name`, etc. remplis

### Vérifier la sécurité RLS
1. Créer un 2ème compte: `test2@fishingspot.com`
2. Se connecter avec ce compte
3. Essayer d'accéder aux données de `test@fishingspot.com`
4. Vous ne devriez voir AUCUNE donnée
5. ✅ La sécurité RLS fonctionne!

## Scénarios de test complets

### Scénario 1: Nouveau utilisateur
1. Ouvrir l'app en mode navigation privée
2. Cliquer "Créer un compte"
3. S'inscrire
4. Se connecter
5. Aller sur "Mon Compte"
6. Remplir le profil
7. Sauvegarder
8. Aller sur "Mes Prises" (vide)
9. Se déconnecter
10. Se reconnecter
11. Vérifier que le profil est toujours là

### Scénario 2: Multi-utilisateurs
1. Créer compte A: `alice@test.com`
2. Remplir profil A
3. Se déconnecter
4. Créer compte B: `bob@test.com`
5. Remplir profil B (différent de A)
6. Vérifier que B ne voit pas les données de A
7. Se déconnecter
8. Se reconnecter en tant que A
9. Vérifier que A ne voit pas les données de B

### Scénario 3: Persistance
1. Se connecter
2. Remplir le profil
3. Fermer le navigateur complètement
4. Rouvrir le navigateur
5. Aller sur l'app
6. ✅ Vous devriez être toujours connecté
7. Le profil devrait être toujours là

### Scénario 4: Suppression
1. Se connecter avec un compte de test
2. Créer quelques données (profil, éventuellement prises)
3. Aller sur "Mon Compte"
4. Supprimer le compte
5. ✅ Redirection vers page d'accueil
6. Vérifier dans Supabase:
   - User supprimé de auth.users
   - Profil supprimé de user_profiles
   - Prises supprimées de fish_catches (cascade)

## Problèmes courants

### "Invalid login credentials"
→ Vérifier que:
- L'email est correct
- Le password est correct (min 6 caractères)
- Le compte existe dans Supabase

### "Email already exists"
→ Le compte existe déjà
→ Utiliser "Se connecter" au lieu de "Créer un compte"

### Profil ne se sauvegarde pas
→ Vérifier:
- Que vous êtes bien connecté
- La console navigateur (F12) pour erreurs
- Que la table `user_profiles` existe dans Supabase
- Que les RLS policies sont configurées

### Déconnexion ne fonctionne pas
→ Vérifier la console pour erreurs
→ Vérifier que `AuthService.SignOutAsync()` est appelé
→ Clear le localStorage du navigateur

---

## Identifiants de test recommandés

Pour vos tests, utilisez:

```
Email: test@fishingspot.com
Password: Test123!

OU

Email: demo@fishingspot.com
Password: Demo2024!

OU

Email: [votre-email]@test.com
Password: [min 6 caractères]
```

**Note:** Les emails n'ont pas besoin d'être réels pour les tests (si la confirmation email est désactivée).

---

**Status:** ✅ Système d'authentification complet avec profil utilisateur
**Fonctionnalités:** Login, Register, Profil, Déconnexion, Suppression compte
**Sécurité:** RLS activée, données isolées par utilisateur
