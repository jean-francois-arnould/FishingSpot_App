# 🎨 Guide du Nouveau Design Mobile-First

## ✨ Caractéristiques principales

### 1. Navigation Bottom (Mobile)
- **Fixed bottom bar** avec 5 onglets principaux
- **Icônes emojis** pour une meilleure lisibilité
- **Active state** avec ligne bleue en haut
- Disparaît sur desktop (768px+)

### 2. Top Bar
- Gradient bleu moderne
- Logo + Titre à gauche
- Bouton déconnexion à droite
- Sticky en haut

### 3. Composants Cards
```html
<div class="card">
    <div class="card-header">Titre</div>
    <div class="card-body">Contenu</div>
</div>
```

### 4. Liste Items
```html
<div class="list-item">
    <div class="list-item-icon">🐟</div>
    <div class="list-item-content">
        <div class="list-item-title">Titre</div>
        <div class="list-item-subtitle">Sous-titre</div>
    </div>
    <div class="list-item-action">→</div>
</div>
```

### 5. Empty State
```html
<div class="empty-state">
    <div class="empty-state-icon">🐟</div>
    <div class="empty-state-title">Aucune prise</div>
    <div class="empty-state-text">Commencez à pêcher !</div>
    <button class="btn btn-primary">Ajouter</button>
</div>
```

### 6. Floating Action Button (FAB)
```html
<button class="fab" @onclick="NavigateToAdd">
    ➕
</button>
```

### 7. Accordéons (pour Matériel)
```html
<div class="accordion-item" @onclick="ToggleAccordion">
    <div class="accordion-header">
        <span>🎣 Cannes</span>
        <span class="accordion-icon">▼</span>
    </div>
    <div class="accordion-content">
        <div class="accordion-body">
            <!-- Contenu -->
        </div>
    </div>
</div>
```

## 🎨 Palette de Couleurs

- **Primary**: `#0066cc` - Bleu principal
- **Secondary**: `#00b4d8` - Bleu clair
- **Success**: `#06d6a0` - Vert
- **Danger**: `#ef476f` - Rouge
- **Dark**: `#023047` - Foncé
- **Light**: `#f8f9fa` - Clair
- **Grey**: `#6c757d` - Gris

## 📐 Spacing

- `gap-1`: 0.5rem
- `gap-2`: 1rem
- `gap-3`: 1.5rem
- `mt-1` à `mt-4`: marges top
- `mb-1` à `mb-4`: marges bottom

## 🔘 Boutons

- `btn-primary`: Bleu avec gradient
- `btn-success`: Vert
- `btn-danger`: Rouge
- `btn-outline-*`: Version outline
- `btn-sm`: Petit
- `btn-lg`: Grand
- `btn-icon`: Bouton circulaire

## 📱 Responsive

- **< 768px**: Navigation bottom visible, design mobile
- **≥ 768px**: Navigation bottom cachée, design desktop
- Content max-width adaptatif

## ✅ À Faire pour Adapter une Page

1. ✅ Supprimer l'ancien navbar
2. ✅ Utiliser `.card` pour les conteneurs
3. ✅ Utiliser `.list-item` pour les listes
4. ✅ Ajouter `.fab` pour ajouter du contenu
5. ✅ Utiliser `.empty-state` quand vide
6. ✅ Tester sur mobile (responsive mode)

## 🎯 Prochaines Étapes

Je vais maintenant adapter toutes les pages principales :
1. Catches.razor ← En cours
2. AddCatch.razor
3. EditCatch.razor
4. Home.razor
5. Profile.razor
6. Matériel/Index.razor (avec accordéons)
7. Montages/Index.razor

Chaque page aura :
- Design cohérent
- Mobile-first
- Animations fluides
- Empty states
- FAB si nécessaire
