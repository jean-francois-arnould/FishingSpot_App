# 🧪 Guide de test - Nouvelle UX Sélection de Poissons

## 📋 Liste de vérification mobile

### Test 1: Interface de base (champ lecture seule)
1. ✅ Ouvrir l'app sur mobile
2. ✅ Aller sur "Ajouter une prise"
3. ✅ Observer le champ "Espèce"
4. ✅ **VÉRIFIER**: Le champ est en lecture seule (grisé)
5. ✅ **VÉRIFIER**: Placeholder: "Sélectionner un poisson..."
6. ✅ **VÉRIFIER**: Deux boutons visibles:
   - "🔍 Chercher dans la liste"
   - "➕ Ajouter une nouvelle espèce"
7. ✅ Cliquer sur le champ lecture seule
8. ✅ **VÉRIFIER**: Le clavier NE s'ouvre PAS (champ readonly)

**Résultat attendu**: Plus de problème de clavier qui apparaît/disparaît

---

### Test 2: Ouverture de la liste (bouton "🔍 Chercher dans la liste")
1. ✅ Ouvrir "Ajouter une prise"
2. ✅ Cliquer sur le bouton "🔍 Chercher dans la liste"
3. ✅ **VÉRIFIER**: Un modal s'affiche avec la liste complète
4. ✅ **VÉRIFIER**: Les poissons sont organisés par catégories:
   - 🦈 Carnassiers
   - 🌊 Salmonidés
   - ➡️ Migrateurs
   - 🐠 Cyprinidés
   - 🐟 Autres espèces
5. ✅ Scroller dans la liste
6. ✅ Cliquer sur "Brochet"
7. ✅ **VÉRIFIER**: Le modal se ferme
8. ✅ **VÉRIFIER**: "🦈 Brochet" apparaît dans le champ "Espèce"
9. ✅ **VÉRIFIER**: Les infos du brochet s'affichent en dessous (description, taille légale)

**Résultat attendu**: Sélection fluide en 2 clics (bouton puis le poisson)

---

### Test 3: Fermeture du modal de liste
1. ✅ Ouvrir le modal avec "🔍 Chercher dans la liste"
2. ✅ Cliquer sur le fond sombre (backdrop)
3. ✅ **VÉRIFIER**: Le modal se ferme
4. ✅ Rouvrir le modal avec "🔍 Chercher dans la liste"
5. ✅ Cliquer sur le bouton ✕ en haut à droite
6. ✅ **VÉRIFIER**: Le modal se ferme

**Résultat attendu**: 3 façons de fermer (sélection, backdrop, bouton ✕)

---

### Test 4: Ajouter un poisson qui N'existe PAS
1. ✅ Ouvrir "Ajouter une prise"
2. ✅ Cliquer sur le bouton "➕ Ajouter une nouvelle espèce"
3. ✅ **VÉRIFIER**: Un formulaire s'ouvre avec 7 champs
4. ✅ Remplir:
   - Nom commun: "Poisson Test Mobile"
   - Nom scientifique: "Testus mobileus"
   - Famille: "Testidae"
   - Catégorie: Sélectionner "🐟 Autre"
   - Taille légale: 30
   - Emoji: 🧪
   - Description: "Poisson de test pour validation mobile"
5. ✅ Cliquer sur "✅ Ajouter"
6. ✅ **VÉRIFIER**: Message de succès "✅ Le poisson 'Poisson Test Mobile' a été ajouté avec succès !"
7. ✅ **VÉRIFIER**: Le modal se ferme après 2 secondes
8. ✅ **VÉRIFIER**: "🧪 Poisson Test Mobile" est affiché dans le champ "Espèce"
9. ✅ Rouvrir le modal "🔍 Chercher dans la liste"
10. ✅ **VÉRIFIER**: "Poisson Test Mobile" apparaît dans la catégorie "🐟 Autres espèces"

**Résultat attendu**: Nouveau poisson ajouté et sélectionné automatiquement

---

### Test 5: Tentative d'ajouter un DOUBLON
1. ✅ Ouvrir "Ajouter une prise"
2. ✅ Cliquer sur ➕
3. ✅ Remplir avec un poisson existant:
   - Nom commun: "brochet" (minuscules)
   - Catégorie: "🦈 Carnassier"
   - Emoji: 🐟
4. ✅ Cliquer sur "✅ Ajouter"
5. ✅ **VÉRIFIER**: Message d'erreur "❌ Le poisson 'brochet' existe déjà dans la base de données."
6. ✅ **VÉRIFIER**: Le formulaire reste ouvert
7. ✅ **VÉRIFIER**: Aucun poisson n'a été créé

**Résultat attendu**: Détection des doublons (insensible à la casse)

---

### Test 6: Validation des champs requis
1. ✅ Ouvrir le formulaire d'ajout (➕)
2. ✅ Laisser "Nom commun" vide
3. ✅ Cliquer sur "✅ Ajouter"
4. ✅ **VÉRIFIER**: Validation HTML5 empêche la soumission
5. ✅ Remplir "Nom commun": "Test Validation"
6. ✅ Laisser "Catégorie" vide
7. ✅ Cliquer sur "✅ Ajouter"
8. ✅ **VÉRIFIER**: Validation HTML5 empêche la soumission
9. ✅ Sélectionner une catégorie
10. ✅ Laisser "Emoji" vide
11. ✅ Cliquer sur "✅ Ajouter"
12. ✅ **VÉRIFIER**: Validation HTML5 empêche la soumission

**Résultat attendu**: Impossible de soumettre sans les champs requis (nom, catégorie, emoji)

---

### Test 7: Responsive mobile
1. ✅ Ouvrir sur iPhone / Android
2. ✅ Ouvrir le modal de liste 🔍
3. ✅ **VÉRIFIER**: Le modal prend 95% de la largeur
4. ✅ **VÉRIFIER**: Le scroll fonctionne bien
5. ✅ **VÉRIFIER**: Les boutons sont facilement cliquables
6. ✅ Ouvrir le formulaire d'ajout ➕
7. ✅ **VÉRIFIER**: Les champs sont accessibles avec le clavier tactile
8. ✅ **VÉRIFIER**: Les boutons "Annuler" et "Ajouter" sont empilés verticalement

**Résultat attendu**: Interface parfaitement utilisable sur petit écran

---

### Test 8: Workflow complet
1. ✅ Ouvrir "Ajouter une prise"
2. ✅ Cliquer sur "🔍 Chercher dans la liste"
3. ✅ Ne pas trouver le poisson souhaité
4. ✅ Fermer le modal
5. ✅ Cliquer sur "➕ Ajouter une nouvelle espèce"
6. ✅ Remplir le formulaire avec "Silure Albinos"
7. ✅ Ajouter le poisson
8. ✅ **VÉRIFIER**: "Silure Albinos" est affiché dans le champ "Espèce"
9. ✅ Compléter la prise (taille, poids, lieu, etc.)
10. ✅ Sauvegarder
11. ✅ Créer une nouvelle prise
12. ✅ Ouvrir la liste "🔍 Chercher dans la liste"
13. ✅ **VÉRIFIER**: "Silure Albinos" est dans la liste

**Résultat attendu**: Workflow fluide de bout en bout

---

## 🐛 Problèmes connus possibles

### Si le bouton 🔍 n'apparaît pas:
- Vérifier que la liste des poissons s'est bien chargée
- Regarder la console pour des erreurs
- Vérifier la connexion à Supabase

### Si l'ajout d'un poisson échoue:
- Vérifier que l'utilisateur est authentifié
- Vérifier les permissions RLS sur la table `fish_species`
- Regarder la console pour le code d'erreur

### Si les modals ne s'affichent pas correctement:
- Vérifier que le CSS `app.css` a bien été déployé
- Forcer le refresh du cache (Ctrl+F5)
- Vérifier qu'il n'y a pas de conflits CSS

---

## 📸 Screenshots à prendre

Pour valider visuellement:
1. 📸 Champ "Espèce" en lecture seule avec les 2 boutons
2. 📸 Modal de liste ouvert avec les catégories
3. 📸 Un poisson sélectionné avec ses infos affichées
4. 📸 Formulaire d'ajout de poisson ouvert
5. 📸 Message de succès après ajout
6. 📸 Message d'erreur pour un doublon

---

## ✅ Checklist finale

- [ ] Le champ "Espèce" est en lecture seule (pas de clavier)
- [ ] Le bouton "🔍 Chercher dans la liste" affiche le modal
- [ ] Le bouton "➕ Ajouter une nouvelle espèce" ouvre le formulaire
- [ ] La sélection depuis la liste fonctionne
- [ ] L'ajout d'un nouveau poisson fonctionne
- [ ] La détection de doublon fonctionne
- [ ] Les validations fonctionnent
- [ ] Le design est responsive
- [ ] Les animations sont fluides
- [ ] Tout fonctionne en mode avion puis reconnexion

---

## 🆘 En cas de problème

1. Vérifier la console navigateur (F12) pour des erreurs
2. Vérifier les logs Supabase dans le SQL Editor
3. Tester la connexion réseau
4. Vérifier que le token JWT est valide
5. Redémarrer l'application si nécessaire

Si un bug persiste, noter:
- Le device (iPhone, Android, navigateur)
- L'action effectuée
- Le message d'erreur exact
- Les logs de la console
