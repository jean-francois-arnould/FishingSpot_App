# 🔧 Guide de Dépannage - Base de Données

## Problèmes Courants et Solutions

### ❌ Erreur : "constraint already exists"

**Symptôme :**
```
ERROR: 42710: constraint "user_statistics_user_id_fkey" for relation "user_statistics" already exists
```

**Cause :** Le script a été exécuté partiellement ou plusieurs fois.

**Solution 1 : Nettoyer et réexécuter**
```sql
-- 1. Exécuter le rollback
-- Copier-coller database/rollback.sql dans Supabase SQL Editor

-- 2. Réexécuter improvements.sql
-- Copier-coller database/improvements.sql dans Supabase SQL Editor
```

**Solution 2 : Exécution manuelle par sections**
```sql
-- Exécuter improvements.sql section par section
-- Commenter les sections déjà appliquées avec --
```

---

### ❌ Erreur : "relation does not exist"

**Symptôme :**
```
ERROR: relation "user_statistics" does not exist
```

**Cause :** L'ordre d'exécution des commandes SQL est incorrect.

**Solution :**
```sql
-- Vérifier que toutes les tables parent existent
SELECT * FROM auth.users LIMIT 1;
SELECT * FROM fish_catches LIMIT 1;

-- Si erreur, créer d'abord les tables de base
-- puis exécuter improvements.sql
```

---

### ❌ Erreur : "permission denied"

**Symptôme :**
```
ERROR: permission denied for schema public
```

**Cause :** Vous n'avez pas les permissions suffisantes.

**Solution :**
```sql
-- Vérifier vos permissions
SELECT * FROM information_schema.role_table_grants 
WHERE grantee = current_user;

-- Dans Supabase, assurez-vous d'utiliser le SQL Editor
-- avec les droits postgres (pas le service role)
```

---

### ❌ Erreur : "syntax error"

**Symptôme :**
```
ERROR: syntax error at or near "..."
```

**Cause :** Copier-coller incorrect ou encodage de caractères.

**Solution :**
1. Ouvrir `improvements.sql` dans un éditeur texte simple
2. Copier tout le contenu (Ctrl+A, Ctrl+C)
3. Coller dans Supabase SQL Editor
4. S'assurer qu'il n'y a pas de caractères spéciaux
5. Exécuter

---

### ⚠️ Warning : "trigger already exists"

**Symptôme :**
```
NOTICE: trigger "..." already exists, skipping
```

**Cause :** Le trigger existe déjà (normal si réexécution).

**Solution :** Aucune action nécessaire, c'est normal avec `CREATE OR REPLACE`.

---

## 🔍 Diagnostic

### Vérifier l'état de la BDD

```sql
-- Exécuter database/diagnostic.sql
-- Cela affichera l'état complet de votre base de données
```

### Vérifier les indexes

```sql
SELECT 
    indexname,
    tablename
FROM pg_indexes 
WHERE schemaname = 'public' 
    AND indexname LIKE 'idx_%'
ORDER BY tablename, indexname;
```

### Vérifier les triggers

```sql
SELECT 
    trigger_name,
    event_object_table
FROM information_schema.triggers
WHERE trigger_schema = 'public'
ORDER BY event_object_table, trigger_name;
```

### Vérifier RLS

```sql
SELECT 
    tablename,
    CASE WHEN rowsecurity THEN 'ENABLED' ELSE 'DISABLED' END as rls_status
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY tablename;
```

---

## 🔄 Réinitialisation Complète

Si tout est cassé et que vous voulez repartir de zéro :

### Étape 1 : Sauvegarder vos données
```sql
-- Export des données importantes
COPY (SELECT * FROM fish_catches) TO '/tmp/catches_backup.csv' CSV HEADER;
COPY (SELECT * FROM user_profiles) TO '/tmp/profiles_backup.csv' CSV HEADER;
```

### Étape 2 : Rollback
```sql
-- Exécuter database/rollback.sql
```

### Étape 3 : Réappliquer
```sql
-- Exécuter database/improvements.sql
```

### Étape 4 : Restaurer les données
```sql
-- Si nécessaire, réimporter les données sauvegardées
```

---

## 🧪 Test des Fonctionnalités

### Tester les triggers

```sql
-- Test trigger updated_at
UPDATE fish_catches 
SET notes = 'test' 
WHERE id = 1;

-- Vérifier que updated_at a changé
SELECT id, notes, updated_at 
FROM fish_catches 
WHERE id = 1;
```

### Tester les statistiques

```sql
-- Vérifier la table user_statistics
SELECT * FROM user_statistics LIMIT 5;

-- Forcer une mise à jour
INSERT INTO fish_catches (user_id, fish_name, catch_date)
VALUES ('your-user-id', 'Test', CURRENT_DATE);

-- Vérifier que les stats ont été mises à jour
SELECT * FROM user_statistics WHERE user_id = 'your-user-id';
```

### Tester RLS

```sql
-- En tant que super admin (devrait tout voir)
SELECT COUNT(*) FROM fish_catches;

-- En tant qu'utilisateur (devrait voir seulement ses données)
-- Se connecter avec un compte utilisateur normal
SELECT COUNT(*) FROM fish_catches;
```

---

## 📊 Performance

### Vérifier l'utilisation des indexes

```sql
-- Indexes utilisés récemment
SELECT 
    schemaname,
    tablename,
    indexname,
    idx_scan as index_scans,
    idx_tup_read as tuples_read
FROM pg_stat_user_indexes
WHERE schemaname = 'public'
ORDER BY idx_scan DESC;
```

### Requêtes lentes

```sql
-- Activer le tracking des requêtes
CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

-- Voir les requêtes les plus lentes
SELECT 
    query,
    calls,
    total_exec_time,
    mean_exec_time,
    max_exec_time
FROM pg_stat_statements
ORDER BY mean_exec_time DESC
LIMIT 10;
```

---

## 🆘 Aide Supplémentaire

### Logs Supabase

1. Aller dans Supabase Dashboard
2. Menu "Logs" → "Postgres Logs"
3. Chercher les erreurs récentes

### Vérifier la connexion

```sql
-- Tester la connexion basique
SELECT version();
SELECT current_database();
SELECT current_user;
```

### Support

Si les problèmes persistent :

1. Consulter `database/README.md`
2. Vérifier les logs Supabase
3. Exécuter `database/diagnostic.sql`
4. Créer une issue GitHub avec :
   - Le message d'erreur complet
   - Le résultat du diagnostic
   - La version de PostgreSQL (`SELECT version();`)

---

## ✅ Checklist de Vérification

Après avoir appliqué les améliorations :

- [ ] Table `user_statistics` créée
- [ ] 13 indexes créés
- [ ] 2 colonnes thumbnails ajoutées
- [ ] 4 triggers fonctionnent
- [ ] 3 fonctions créées
- [ ] 1 vue créée
- [ ] RLS activé sur toutes les tables
- [ ] 15 policies RLS créées
- [ ] Aucune erreur dans les logs
- [ ] Performance améliorée (test avec des requêtes)

---

## 🎯 Scripts Disponibles

| Script | Usage |
|--------|-------|
| `improvements.sql` | Application des améliorations |
| `rollback.sql` | Annulation des améliorations |
| `diagnostic.sql` | Vérification de l'état |

---

**💡 Conseil :** Toujours sauvegarder avant d'exécuter des scripts SQL en production !

**🔍 Debug :** En cas de doute, exécutez `diagnostic.sql` pour voir l'état exact de votre BDD.
