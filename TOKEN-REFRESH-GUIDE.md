# 🔄 Rafraîchissement automatique du token JWT

## ✅ Problème résolu

**AVANT** :
- ❌ Token expire après 1 heure
- ❌ Erreur "JWT expired" lors de la sauvegarde
- ❌ Obligation de se reconnecter manuellement
- ❌ Perte du contexte utilisateur

**APRÈS** :
- ✅ Token rafraîchi automatiquement toutes les heures
- ✅ Rafraîchissement transparent (en arrière-plan)
- ✅ Plus besoin de se reconnecter
- ✅ Session persistante tant que l'app est ouverte

---

## 🔧 Comment ça fonctionne

### 1. Sauvegarde du refresh token

Lors de la connexion, l'application sauvegarde maintenant **4 informations** :
```
localStorage:
  - supabase_token          → Token d'accès (valide 1h)
  - supabase_refresh_token  → Token de rafraîchissement (valide 30 jours)
  - supabase_user           → Infos utilisateur
  - supabase_token_expires_at → Date d'expiration du token
```

### 2. Rafraîchissement automatique

Un **timer** est programmé pour rafraîchir le token **5 minutes avant son expiration** :

```
Connexion à 10:00
  ↓
Token expire à 11:00
  ↓
Rafraîchissement automatique à 10:55 ✅
  ↓
Nouveau token valide jusqu'à 11:55
  ↓
Prochain rafraîchissement à 11:50
```

### 3. Rafraîchissement au démarrage

Si vous fermez et rouvrez l'app :
- ✅ L'app vérifie si le token a expiré
- ✅ Si oui, rafraîchissement automatique au démarrage
- ✅ Sinon, planification du prochain rafraîchissement

---

## 📊 Logs dans la console

Vous verrez maintenant des messages dans la console (F12) :

### À la connexion :
```
✅ Connexion réussie ! Token expire à 11:00:00
🔄 Rafraîchissement automatique planifié dans 55.0 minutes
```

### Au démarrage de l'app :
```
🔄 Token expiré ou proche de l'expiration, rafraîchissement...
✅ Token rafraîchi avec succès ! Expire à 11:55:00
🔄 Rafraîchissement automatique planifié dans 55.0 minutes
```

### Toutes les heures :
```
✅ Token rafraîchi avec succès ! Expire à 12:00:00
🔄 Rafraîchissement automatique planifié dans 55.0 minutes
```

---

## ⏱️ Durées de validité

| Type | Durée | Renouvellement |
|------|-------|----------------|
| **Access Token** | 1 heure | Automatique à 55 min |
| **Refresh Token** | 30 jours | Lors de chaque connexion |

---

## 🧪 Tests

### Test 1 : Connexion et attente
1. Connectez-vous
2. Ouvrez la console (F12)
3. Notez l'heure d'expiration
4. Attendez ~55 minutes
5. Vérifiez que le token se rafraîchit automatiquement

### Test 2 : Fermeture/Ouverture
1. Connectez-vous
2. Fermez l'onglet/navigateur
3. Attendez 1 heure (pour que le token expire)
4. Rouvrez l'app
5. ✅ Vous devriez rester connecté (rafraîchissement auto)

### Test 3 : Utilisation prolongée
1. Connectez-vous
2. Laissez l'app ouverte pendant plusieurs heures
3. Utilisez l'app normalement (ajouter des prises, etc.)
4. ✅ Aucune erreur "JWT expired"
5. ✅ Pas besoin de se reconnecter

---

## 🔐 Sécurité

- ✅ **Refresh token** stocké en localStorage (sécurisé navigateur)
- ✅ **HTTPS obligatoire** (GitHub Pages)
- ✅ Token refresh via API Supabase sécurisée
- ✅ Refresh token expire après 30 jours d'inactivité
- ✅ Déconnexion nettoie tous les tokens

---

## 🐛 Dépannage

### Le token ne se rafraîchit pas
- Vérifiez la console pour les erreurs
- Vérifiez que le refresh_token est présent :
  ```javascript
  console.log(localStorage.getItem('supabase_refresh_token'));
  ```

### Erreur "No refresh token available"
- Déconnectez-vous
- Reconnectez-vous (génère un nouveau refresh token)

### Toujours des erreurs 401
- Vérifiez les logs Supabase (Dashboard → Logs)
- Vérifiez que l'API key est correcte
- Assurez-vous que RLS est bien configuré

---

## 💡 Avantages

1. **Expérience utilisateur fluide** : Plus de déconnexions intempestives
2. **Productivité** : Pas d'interruption pendant l'utilisation
3. **Sécurité** : Tokens à courte durée de vie (1h)
4. **Transparence** : L'utilisateur ne voit rien, tout se passe en arrière-plan
5. **Fiabilité** : Gestion intelligente avec fallback

---

## 📱 Compatibilité

- ✅ Desktop (Chrome, Firefox, Edge, Safari)
- ✅ Mobile iOS (Safari)
- ✅ Mobile Android (Chrome)
- ✅ PWA installée

---

**Vous n'aurez plus besoin de vous reconnecter ! 🎣**

Le token sera automatiquement rafraîchi en arrière-plan toutes les heures.
