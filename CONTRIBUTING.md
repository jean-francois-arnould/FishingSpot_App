# 🤝 Guide de Contribution - FishingSpot

Merci de votre intérêt pour contribuer à FishingSpot ! Ce guide vous aidera à soumettre des contributions de qualité.

---

## 📋 Table des Matières

1. [Code de Conduite](#code-de-conduite)
2. [Comment Contribuer](#comment-contribuer)
3. [Standards de Code](#standards-de-code)
4. [Processus de Pull Request](#processus-de-pull-request)
5. [Tests](#tests)
6. [Documentation](#documentation)

---

## 📜 Code de Conduite

### Nos Engagements

- 🤝 Être respectueux et inclusif
- 💬 Communiquer de manière constructive
- 🎯 Se concentrer sur ce qui est meilleur pour la communauté
- 🙏 Faire preuve d'empathie envers les autres

### Comportements Inacceptables

- ❌ Langage offensant ou dégradant
- ❌ Harcèlement sous toute forme
- ❌ Trolling ou commentaires insultants
- ❌ Publication d'informations privées sans consentement

---

## 🚀 Comment Contribuer

### 1. Signaler un Bug

**Avant de créer une issue :**
- Vérifiez que le bug n'est pas déjà signalé
- Testez avec la dernière version

**Template d'issue :**
```markdown
**Description du bug**
Une description claire du problème

**Étapes pour reproduire**
1. Aller sur '...'
2. Cliquer sur '....'
3. Voir l'erreur

**Comportement attendu**
Ce qui devrait se passer

**Screenshots**
Si applicable

**Environnement**
- OS: [e.g. Windows 11]
- Navigateur: [e.g. Chrome 120]
- Version: [e.g. 2.0]
```

### 2. Proposer une Fonctionnalité

**Template :**
```markdown
**Problème à résoudre**
Quel besoin cette fonctionnalité répond-elle ?

**Solution proposée**
Description de votre idée

**Alternatives considérées**
Autres approches envisagées

**Mockups/Exemples**
Si applicable
```

### 3. Soumettre du Code

#### Fork & Clone
```bash
# Fork le repo via GitHub UI, puis :
git clone https://github.com/VOTRE_USERNAME/FishingSpot_App.git
cd FishingSpot_App
git remote add upstream https://github.com/jean-francois-arnould/FishingSpot_App.git
```

#### Créer une branche
```bash
# Pour une fonctionnalité
git checkout -b feature/nom-fonctionnalite

# Pour un bug
git checkout -b fix/nom-bug

# Pour la documentation
git checkout -b docs/amelioration-doc
```

#### Faire vos changements
```bash
# Éditer les fichiers...

# Ajouter les tests
dotnet test

# Vérifier le build
dotnet build
```

---

## 🎨 Standards de Code

### C# / .NET

#### Conventions de Nommage

```csharp
// ✅ BON
public class FishCatchService { }
private readonly ILogger _logger;
public async Task<FishCatch> GetCatchAsync(int id) { }

// ❌ MAUVAIS
public class fishcatchservice { }
private readonly ILogger logger;
public async Task<FishCatch> getcatch(int id) { }
```

#### Style

```csharp
// ✅ BON - Async/Await
public async Task<List<FishCatch>> GetCatchesAsync()
{
    try
    {
        _logger.LogInformation("Fetching catches");
        var result = await _httpClient.GetAsync(...);
        return await result.Content.ReadFromJsonAsync<List<FishCatch>>();
    }
    catch (Exception ex)
    {
        _logger.LogError("Error fetching catches", ex);
        throw;
    }
}

// ❌ MAUVAIS - Synchrone, pas de logging
public List<FishCatch> GetCatches()
{
    return _httpClient.GetAsync(...).Result.Content
        .ReadFromJsonAsync<List<FishCatch>>().Result;
}
```

#### Injection de Dépendances

```csharp
// ✅ BON
public class MyService
{
    private readonly ILogger _logger;
    private readonly ISupabaseService _supabase;

    public MyService(ILogger logger, ISupabaseService supabase)
    {
        _logger = logger;
        _supabase = supabase;
    }
}

// ❌ MAUVAIS - new direct
public class MyService
{
    private Logger _logger = new Logger();
}
```

### Blazor / Razor

#### Composants

```razor
@* ✅ BON *@
@page "/catches"
@using FishingSpot.PWA.Models
@inject ISupabaseService Supabase
@inject ILogger Logger

<PageTitle>Mes Prises</PageTitle>

@if (isLoading)
{
    <LoadingSpinner />
}
else if (catches.Any())
{
    <CatchList Items="@catches" />
}
else
{
    <EmptyState Message="Aucune prise" />
}

@code {
    private List<FishCatch> catches = new();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadCatchesAsync();
    }

    private async Task LoadCatchesAsync()
    {
        try
        {
            isLoading = true;
            catches = await Supabase.GetAllCatchesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error loading catches", ex);
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

### CSS

```css
/* ✅ BON - BEM-like */
.catch-card {
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.catch-card__image {
    width: 100%;
    aspect-ratio: 16/9;
}

.catch-card__title {
    font-size: 18px;
    font-weight: 600;
}

/* ❌ MAUVAIS - Noms génériques */
.card { }
.img { }
.title { }
```

---

## 🔄 Processus de Pull Request

### 1. Avant de Soumettre

**Checklist :**
- [ ] Code compile sans erreurs
- [ ] Tous les tests passent
- [ ] Nouveaux tests ajoutés si nécessaire
- [ ] Documentation mise à jour
- [ ] Pas de console.log ou code de debug
- [ ] Commits bien nommés

### 2. Commits

**Format :**
```
type(scope): description courte

Description détaillée si nécessaire

Fixes #123
```

**Types :**
- `feat`: Nouvelle fonctionnalité
- `fix`: Correction de bug
- `docs`: Documentation
- `style`: Formatage (pas de changement de code)
- `refactor`: Refactoring
- `test`: Tests
- `chore`: Maintenance

**Exemples :**
```bash
git commit -m "feat(catches): add export to CSV functionality"
git commit -m "fix(auth): resolve token refresh issue #42"
git commit -m "docs(readme): update installation instructions"
```

### 3. Créer la Pull Request

**Template :**
```markdown
## Description
Brève description des changements

## Type de changement
- [ ] Bug fix
- [ ] Nouvelle fonctionnalité
- [ ] Breaking change
- [ ] Documentation

## Tests
- [ ] Tests unitaires ajoutés/mis à jour
- [ ] Tests manuels effectués

## Checklist
- [ ] Code suit les standards du projet
- [ ] Tests passent localement
- [ ] Documentation mise à jour
- [ ] Pas de warnings

## Screenshots
Si applicable
```

### 4. Revue de Code

**Attentes :**
- Réponse dans les 48h
- Feedback constructif des reviewers
- Corrections demandées à faire rapidement

**Si des changements sont demandés :**
```bash
# Faire les corrections
git add .
git commit -m "fix: address review comments"
git push origin feature/ma-fonctionnalite
```

---

## ✅ Tests

### Écrire des Tests

**Structure :**
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Préparer
    var service = new MyService(mockDependency);
    var input = new TestData();

    // Act - Agir
    var result = await service.DoSomethingAsync(input);

    // Assert - Vérifier
    result.Should().NotBeNull();
    result.Value.Should().Be(expected);
}
```

**Coverage Minimum : 70%**

```bash
# Vérifier coverage
dotnet test /p:CollectCoverage=true

# Générer rapport
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report
```

### Types de Tests

#### Tests Unitaires
```csharp
// Tester une méthode isolée
[Fact]
public void Calculate_WithValidInput_ReturnsCorrectResult()
{
    var calculator = new Calculator();
    var result = calculator.Add(2, 3);
    result.Should().Be(5);
}
```

#### Tests d'Intégration
```csharp
// Tester plusieurs composants ensemble
[Fact]
public async Task GetCatches_WithAuthentication_ReturnsUserCatches()
{
    var authService = new AuthService(...);
    var supabaseService = new SupabaseService(..., authService);

    await authService.SignInAsync(...);
    var catches = await supabaseService.GetAllCatchesAsync();

    catches.Should().NotBeEmpty();
}
```

#### Tests de Composants Blazor (bUnit)
```csharp
[Fact]
public void CatchCard_WithValidData_RendersCorrectly()
{
    using var ctx = new TestContext();
    var catch = new FishCatch { FishName = "Brochet" };

    var cut = ctx.RenderComponent<CatchCard>(parameters => parameters
        .Add(p => p.Catch, catch)
    );

    cut.Find(".catch-name").TextContent.Should().Be("Brochet");
}
```

---

## 📚 Documentation

### Code Documentation

```csharp
/// <summary>
/// Récupère toutes les prises de l'utilisateur connecté
/// </summary>
/// <returns>Liste des prises triées par date décroissante</returns>
/// <exception cref="AuthenticationException">Si l'utilisateur n'est pas authentifié</exception>
public async Task<List<FishCatch>> GetAllCatchesAsync()
{
    // Implementation...
}
```

### README / Markdown

- Utiliser des emojis pour la clarté : 🎣 ✅ ❌ 📊
- Inclure des exemples de code
- Ajouter des screenshots si pertinent
- Structurer avec des titres clairs

---

## 🎯 Priorités du Projet

### Haute Priorité
- 🐛 Corrections de bugs critiques
- 🔒 Problèmes de sécurité
- ⚡ Améliorations de performance

### Priorité Moyenne
- ✨ Nouvelles fonctionnalités demandées
- 📱 Améliorations UX
- ♿ Accessibilité

### Priorité Basse
- 🎨 Améliorations visuelles mineures
- 📝 Refactoring non-urgent
- 📚 Documentation supplémentaire

---

## ❓ Questions ?

### Obtenir de l'Aide

- **GitHub Discussions** : Pour questions générales
- **Issues** : Pour bugs et fonctionnalités
- **Email** : jean-francois.arnould@example.com

### Ressources

- [Documentation Blazor](https://learn.microsoft.com/blazor)
- [Supabase Docs](https://supabase.com/docs)
- [.NET Guidelines](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)

---

## 🏆 Contributeurs

Merci à tous ceux qui contribuent à FishingSpot !

<!-- readme: contributors -start -->
<!-- readme: contributors -end -->

---

## 📄 Licence

En contribuant, vous acceptez que vos contributions soient sous la même licence que le projet.

---

**Merci de contribuer à FishingSpot ! 🎣**

Votre aide améliore l'expérience de tous les pêcheurs utilisant l'application.
