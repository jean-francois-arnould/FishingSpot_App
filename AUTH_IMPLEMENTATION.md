# 🔐 SYSTÈME D'AUTHENTIFICATION - FishingSpot PWA

## ✅ CE QUI A ÉTÉ AJOUTÉ

### 1. Authentification complète
- ✅ Service d'authentification (`AuthService.cs`)
- ✅ Gestion des sessions avec localStorage
- ✅ Login / Register / Logout
- ✅ Protection des routes par utilisateur

### 2. Pages créées
- ✅ `/login` - Page de connexion
- ✅ `/register` - Page d'inscription
- ✅ Composant `AuthorizeView` pour protéger les routes

### 3. Sécurité des données
- ✅ Chaque utilisateur a ses propres données
- ✅ RLS (Row Level Security) Supabase configurée
- ✅ Les prises sont liées à `user_id`
- ✅ Isolation complète entre utilisateurs

### 4. Interface utilisateur
- ✅ Menu dynamique selon l'état de connexion
- ✅ Affichage de l'email utilisateur
- ✅ Bouton de déconnexion
- ✅ Messages d'erreur/succès pour les formulaires

## 📋 FICHIERS MODIFIÉS/CRÉÉS

### Nouveaux fichiers
```
FishingSpot.PWA/
├── Models/
│   └── User.cs                      # Modèle utilisateur + Auth
├── Services/
│   ├── IAuthService.cs              # Interface auth
│   └── AuthService.cs               # Service d'authentification
├── Components/
│   └── AuthorizeView.razor          # Composant de protection
└── Pages/
    ├── Login.razor                  # Page de connexion
    └── Register.razor               # Page d'inscription
```

### Fichiers modifiés
```
FishingSpot.PWA/
├── Program.cs                       # Ajout AuthService
├── Layout/NavMenu.razor             # Menu dynamique + logout
├── Pages/Catches.razor              # Protection + affichage user
├── Models/FishCatch.cs              # Ajout user_id
├── Services/SupabaseService.cs      # Utilisation du token auth
└── supabase_setup.sql               # Nouvelles policies RLS
```

## 🚀 CONFIGURATION REQUISE

### 1. Activer l'authentification Email dans Supabase

1. Aller dans votre projet Supabase
2. **Authentication** > **Providers**
3. **Email** > **Enable**
4. **IMPORTANT:** Désactiver "Confirm email" pour les tests
   - Settings > **Disable email confirmations**
   - (Sinon les utilisateurs doivent confirmer leur email avant de se connecter)

### 2. Exécuter le nouveau script SQL

Copier tout le contenu de `FishingSpot.PWA/supabase_setup.sql` dans le **SQL Editor** de Supabase et exécuter.

Ce script va:
- Créer la table `fish_catches` avec `user_id`
- Configurer les RLS policies par utilisateur
- Chaque user ne verra que ses propres données

### 3. Si vous avez déjà une table fish_catches

Si la table existe déjà, supprimer l'ancienne avant:
```sql
DROP TABLE IF EXISTS fish_catches CASCADE;
```
Puis exécuter le nouveau script.

## 🔐 COMMENT ÇA FONCTIONNE

### Flow d'authentification

1. **Inscription (`/register`)**
   ```
   User entre email + password
   → AuthService.SignUpAsync()
   → Appel API Supabase /auth/v1/signup
   → Compte créé dans auth.users
   → Redirect vers /login
   ```

2. **Connexion (`/login`)**
   ```
   User entre email + password
   → AuthService.SignInAsync()
   → Appel API Supabase /auth/v1/token
   → Retour: access_token + user info
   → Sauvegarde dans localStorage
   → Redirect vers /catches
   ```

3. **Session persistante**
   ```
   App démarre
   → AuthService.InitializeAsync()
   → Lit le token depuis localStorage
   → User reste connecté
   ```

4. **Appels API sécurisés**
   ```
   User fait une action (ex: voir ses prises)
   → SupabaseService.GetAllCatchesAsync()
   → SetAuthHeaders() ajoute: Bearer {access_token}
   → Supabase vérifie le token
   → RLS policies filtrent par user_id
   → Retourne uniquement les données du user
   ```

### RLS Policies (Sécurité PostgreSQL)

Les policies dans Supabase garantissent que:
- **SELECT**: L'utilisateur ne voit QUE ses prises (`auth.uid() = user_id`)
- **INSERT**: L'utilisateur ne peut créer QUE ses prises
- **UPDATE**: L'utilisateur ne peut modifier QUE ses prises
- **DELETE**: L'utilisateur ne peut supprimer QUE ses prises

Même si quelqu'un essaie de modifier l'API ou le code, **PostgreSQL bloque au niveau DB**.

## 📱 UTILISATION

### Première utilisation

1. Ouvrir l'app: `https://votresite.com`
2. Cliquer sur "Register" ou aller sur `/register`
3. Entrer un email et mot de passe (min 6 caractères)
4. Se connecter avec `/login`
5. L'app redirige vers `/catches` (vide au début)

### Navigation

- **Connecté:**
  - Menu affiche: Home | My Catches | [Email] | Déconnexion
  - Accès à toutes les fonctionnalités

- **Non connecté:**
  - Menu affiche: Home | Login | Register
  - Accès uniquement à la home page

### Session

- La session reste active même après fermeture du navigateur
- Token stocké dans localStorage
- Cliquer sur "Déconnexion" pour se déconnecter

## 🔒 SÉCURITÉ

### Ce qui est sécurisé

✅ **Données isolées** - Chaque user ne voit que ses données
✅ **Token JWT** - Authentification par token Bearer
✅ **RLS PostgreSQL** - Sécurité au niveau DB (impossible à bypass)
✅ **HTTPS** - Communications chiffrées (GitHub Pages = HTTPS automatique)

### Ce qui n'est PAS crypté

⚠️ **Passwords** - Hashés par Supabase (bcrypt), jamais stockés en clair
⚠️ **Token** - Stocké en localStorage (visible dans DevTools)
  - C'est normal pour une PWA
  - Le token expire automatiquement
  - Refresh token pour renouveler

### Améliorations possibles (optionnel)

- [ ] Ajouter 2FA (Two-Factor Authentication)
- [ ] Ajouter OAuth (Google, GitHub, etc.)
- [ ] Ajouter "Forgot password"
- [ ] Limiter les tentatives de connexion
- [ ] Ajouter CAPTCHA pour éviter les bots

## 🐛 DÉBOGAGE

### "Email already exists"
→ Normal si vous essayez de créer un compte déjà existant
→ Utiliser `/login` à la place

### "Invalid login credentials"
→ Email ou mot de passe incorrect
→ Vérifier la casse et les espaces

### "User not found" ou données vides
→ Vérifier que l'authentification Supabase est activée
→ Vérifier que le script SQL a été exécuté
→ Check console navigateur (F12) pour erreurs

### Session perdue après refresh
→ Vérifier que `AuthService.InitializeAsync()` est appelé dans Program.cs
→ Vérifier la console pour erreurs localStorage

## 📊 LIMITES GRATUITES SUPABASE

### Authentification (Free Tier)
- ✅ 50,000 utilisateurs actifs mensuels (MAU)
- ✅ Illimité d'inscriptions
- ✅ Social OAuth inclus
- ✅ Email auth inclus

**Pour votre usage (<100 connexions/mois):** Très largement suffisant!

## 🎯 PROCHAINES ÉTAPES

### Fonctionnalités à ajouter
- [ ] Formulaire d'ajout de prises de pêche
- [ ] Upload de photos
- [ ] Profil utilisateur
- [ ] Statistiques personnelles
- [ ] Export des données
- [ ] Partage de prises (optionnel)

### Tests recommandés
1. Créer 2 comptes différents
2. Ajouter des prises sur chaque compte
3. Vérifier que chaque user ne voit que ses données
4. Tester déconnexion/reconnexion
5. Tester refresh de la page (session persistante)

## 📖 DOCUMENTATION SUPABASE

- Auth Guide: https://supabase.com/docs/guides/auth
- RLS Policies: https://supabase.com/docs/guides/auth/row-level-security
- JavaScript Client: https://supabase.com/docs/reference/javascript/auth-signup

---

**Status:** ✅ Système d'authentification complet et fonctionnel
**Sécurité:** 🔒 Isolation complète des données par utilisateur
**Coût:** 💰 0€ (inclus dans le plan gratuit Supabase)
