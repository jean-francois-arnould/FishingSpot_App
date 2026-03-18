# 📋 CHANGELOG - FISHINGSPOT PWA

## Version 2.0 - Authentification ajoutée (2026-03-18)

### 🔐 Nouvelles fonctionnalités
- ✅ Système d'authentification complet (login/register/logout)
- ✅ Gestion de session avec persistance (localStorage)
- ✅ Isolation des données par utilisateur (RLS Supabase)
- ✅ Protection des routes authentifiées
- ✅ Menu dynamique selon l'état de connexion
- ✅ Affichage de l'email utilisateur dans le menu

### 📄 Nouveaux fichiers créés
```
FishingSpot.PWA/
├── Models/
│   └── User.cs
├── Services/
│   ├── IAuthService.cs
│   └── AuthService.cs
├── Components/
│   └── AuthorizeView.razor
├── Pages/
│   ├── Login.razor
│   └── Register.razor
└── supabase_setup.sql (mis à jour avec RLS)
```

### 📝 Fichiers modifiés
- `FishingSpot.PWA/Program.cs` - Ajout AuthService
- `FishingSpot.PWA/Layout/NavMenu.razor` - Menu dynamique avec logout
- `FishingSpot.PWA/Pages/Catches.razor` - Protection + affichage user
- `FishingSpot.PWA/Models/FishCatch.cs` - Ajout champ user_id
- `FishingSpot.PWA/Services/SupabaseService.cs` - Utilisation du token d'auth

### 📖 Documentation ajoutée
- `AUTH_IMPLEMENTATION.md` - Guide complet de l'authentification
- `GUIDE_COMPLET.md` - Guide de démarrage rapide
- `SUMMARY.md` - Mis à jour avec authentification

### 🔒 Sécurité
- Row Level Security (RLS) configurée dans PostgreSQL
- Chaque utilisateur ne peut voir/modifier QUE ses propres données
- Token JWT stocké en localStorage
- Authentification requise pour toutes les opérations sur les données

### ⚠️ Breaking Changes
**IMPORTANT:** Le schéma de la base de données a changé!

Si vous aviez déjà une table `fish_catches`:
1. Sauvegarder vos données (optionnel)
2. Supprimer l'ancienne table: `DROP TABLE IF EXISTS fish_catches CASCADE;`
3. Exécuter le nouveau script `supabase_setup.sql`

**Nouvelle structure:**
- Ajout colonne `user_id UUID NOT NULL` avec référence à `auth.users`
- RLS policies configurées pour isolation par user
- Authentification Email activée dans Supabase

---

## Version 1.0 - PWA initiale (2026-03-18)

### ✨ Fonctionnalités initiales
- ✅ Transformation de l'app MAUI en PWA
- ✅ Blazor WebAssembly configuré
- ✅ Service Worker pour mode offline
- ✅ Manifest pour installation PWA
- ✅ Connexion à Supabase (PostgreSQL)
- ✅ Modèle de données FishCatch
- ✅ Page d'affichage des prises
- ✅ GitHub Actions pour déploiement automatique

### 📄 Fichiers créés
```
FishingSpot.PWA/
├── Models/
│   └── FishCatch.cs
├── Services/
│   ├── ISupabaseService.cs
│   └── SupabaseService.cs
├── Pages/
│   ├── Catches.razor
│   ├── Home.razor
│   ├── Counter.razor
│   └── Weather.razor
├── Layout/
│   ├── MainLayout.razor
│   └── NavMenu.razor
├── wwwroot/
│   ├── appsettings.json
│   ├── manifest.webmanifest
│   └── service-worker.js
├── Program.cs
└── supabase_setup.sql

.github/workflows/
└── deploy-pwa.yml
```

### 📖 Documentation créée
- `README_PWA.md` - Guide complet de la PWA
- `SUMMARY.md` - Résumé de la transformation

### 💰 Coûts
- Supabase: Gratuit (Free Tier)
- GitHub Pages: Gratuit
- **Total: 0€**

---

## Roadmap / À venir

### Version 3.0 (Futures fonctionnalités)
- [ ] Formulaire d'ajout de prises
- [ ] Upload de photos vers Supabase Storage
- [ ] Carte interactive avec localisation des prises
- [ ] Statistiques et graphiques par utilisateur
- [ ] Export PDF des données
- [ ] Profil utilisateur avec paramètres
- [ ] "Forgot password" / Reset password
- [ ] OAuth (Google, GitHub, Facebook)
- [ ] Mode offline complet avec synchronisation
- [ ] Partage de prises sur réseaux sociaux
- [ ] 2FA (Two-Factor Authentication)
- [ ] Notifications push
- [ ] Mode sombre / clair

---

## Notes de migration

### De version 1.0 à 2.0

**Base de données:**
```sql
-- 1. Sauvegarder les données existantes (si nécessaire)
-- 2. Supprimer l'ancienne table
DROP TABLE IF EXISTS fish_catches CASCADE;

-- 3. Exécuter le nouveau script
-- Copier-coller supabase_setup.sql dans SQL Editor
```

**Configuration Supabase:**
1. Activer l'authentification Email
2. Désactiver email confirmations (pour tests)
3. Les anciennes données ne seront plus accessibles car pas de user_id

**Code:**
- Aucun changement requis dans votre code local
- Pull les derniers changements de la branche WAP
- Rebuild: `dotnet build FishingSpot.PWA/FishingSpot.PWA.csproj`

---

## Support

Pour toute question ou problème:
1. Consulter `GUIDE_COMPLET.md` (troubleshooting)
2. Consulter `AUTH_IMPLEMENTATION.md` (détails authentification)
3. Vérifier la console navigateur (F12) pour erreurs
4. Vérifier les logs Supabase dans le dashboard

---

**Dernière mise à jour:** 2026-03-18
**Version actuelle:** 2.0 (avec authentification)
