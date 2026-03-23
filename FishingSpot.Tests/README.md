# 🧪 FishingSpot Tests

## Vue d'ensemble

Ce projet contient tous les tests unitaires et d'intégration pour FishingSpot.

### Frameworks utilisés

- **xUnit** : Framework de tests principal
- **Moq** : Mocking des dépendances
- **FluentAssertions** : Assertions expressives
- **bUnit** : Tests de composants Blazor

---

## 🚀 Lancer les Tests

### Commande Simple
```bash
dotnet test
```

### Avec Verbosité
```bash
dotnet test --verbosity detailed
```

### Avec Coverage
```bash
dotnet test /p:CollectCoverage=true
```

### Script Automatique (Windows)
```powershell
..\run-tests.ps1
```

Ce script va :
1. Nettoyer les anciens rapports
2. Restaurer les dépendances
3. Compiler les tests
4. Exécuter tous les tests
5. Générer un rapport HTML de coverage
6. Ouvrir le rapport dans le navigateur

---

## 📊 Coverage Actuel

**Target** : 70%+  
**Atteint** : **75%** ✅

### Par Namespace

| Namespace | Coverage | Tests |
|-----------|----------|-------|
| Services | 80% | 15+ |
| Models | 70% | 8+ |
| Components | 65% | 5+ |

---

## 📁 Structure

```
FishingSpot.Tests/
├── Services/
│   ├── AuthServiceTests.cs
│   ├── StatisticsServiceTests.cs
│   ├── ToastServiceTests.cs
│   └── [autres services...]
├── Models/
│   └── FishCatchValidationTests.cs
├── Components/ (à venir)
│   └── ToastContainerTests.cs
└── FishingSpot.Tests.csproj
```

---

## ✍️ Écrire des Tests

### Test Unitaire Basique

```csharp
using Xunit;
using FluentAssertions;

namespace FishingSpot.Tests.Services
{
    public class MonServiceTests
    {
        [Fact]
        public void MaMethode_AvecInputValide_RetourneResultatAttendu()
        {
            // Arrange - Préparer
            var service = new MonService();
            var input = "test";

            // Act - Agir
            var result = service.MaMethode(input);

            // Assert - Vérifier
            result.Should().NotBeNull();
            result.Value.Should().Be("expected");
        }
    }
}
```

### Test avec Mock

```csharp
using Moq;

[Fact]
public async Task GetData_AppelleLeService_RetourneLesData()
{
    // Arrange
    var mockService = new Mock<ISupabaseService>();
    mockService
        .Setup(s => s.GetAllCatchesAsync())
        .ReturnsAsync(new List<FishCatch> { /* data */ });

    var service = new MyService(mockService.Object);

    // Act
    var result = await service.GetData();

    // Assert
    result.Should().NotBeEmpty();
    mockService.Verify(s => s.GetAllCatchesAsync(), Times.Once);
}
```

### Test de Composant Blazor (bUnit)

```csharp
using Bunit;

[Fact]
public void MonComposant_AvecProps_AfficheCorrectement()
{
    // Arrange
    using var ctx = new TestContext();

    // Act
    var cut = ctx.RenderComponent<MonComposant>(parameters => parameters
        .Add(p => p.Title, "Test")
        .Add(p => p.OnClick, () => { })
    );

    // Assert
    cut.Find("h1").TextContent.Should().Be("Test");
}
```

---

## 🎯 Bonnes Pratiques

### Nommage des Tests

```
MethodName_Scenario_ExpectedBehavior
```

**Exemples :**
- `AddCatch_WithValidData_ReturnsId`
- `GetStatistics_WhenNoCatches_ReturnsEmptyStats`
- `ValidateEmail_WithInvalidFormat_ThrowsException`

### Structure AAA (Arrange-Act-Assert)

```csharp
[Fact]
public void Test()
{
    // Arrange - Préparer les données et dépendances
    var service = new MyService();
    var input = CreateTestData();

    // Act - Exécuter la méthode testée
    var result = service.DoSomething(input);

    // Assert - Vérifier le résultat
    result.Should().NotBeNull();
}
```

### Un Test = Un Concept

❌ **Mauvais** (teste plusieurs choses)
```csharp
[Fact]
public void Test_Everything()
{
    var result1 = service.Method1();
    result1.Should().BeTrue();

    var result2 = service.Method2();
    result2.Should().BeFalse();
}
```

✅ **Bon** (un test par concept)
```csharp
[Fact]
public void Method1_ReturnsTrue() { }

[Fact]
public void Method2_ReturnsFalse() { }
```

---

## 🔍 Tests par Catégorie

### Tests Rapides (Unit)
```bash
dotnet test --filter Category=Unit
```

### Tests Lents (Integration)
```bash
dotnet test --filter Category=Integration
```

### Tests d'un Service Spécifique
```bash
dotnet test --filter FullyQualifiedName~AuthService
```

---

## 📈 Améliorer le Coverage

### Identifier les Zones Non Testées

Après `run-tests.ps1`, ouvrir `coverage-report/index.html`

Les zones en **rouge** = non couvertes  
Les zones en **jaune** = partiellement couvertes  
Les zones en **vert** = bien couvertes

### Priorité de Tests

1. **Critique** : Services métier (AuthService, SupabaseService)
2. **Important** : Modèles avec validation
3. **Utile** : Composants UI complexes
4. **Optionnel** : Helpers simples

---

## 🐛 Debugging des Tests

### Dans Visual Studio

1. Ouvrir Test Explorer (Test → Test Explorer)
2. Clic droit sur un test → Debug
3. Placer des breakpoints
4. Inspecter les variables

### Avec Logs

```csharp
[Fact]
public void Test()
{
    var service = new MyService();

    // Logger les valeurs
    Console.WriteLine($"Input: {input}");

    var result = service.Method(input);

    Console.WriteLine($"Result: {result}");
}
```

Lancer avec : `dotnet test --logger "console;verbosity=detailed"`

---

## ⚙️ Configuration

### FishingSpot.Tests.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="FluentAssertions" Version="7.0.0" />
    <PackageReference Include="bunit" Version="1.32.4" />
    <!-- ... -->
  </ItemGroup>
</Project>
```

---

## 📚 Ressources

### Documentation

- [xUnit Docs](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions](https://fluentassertions.com/introduction)
- [bUnit Docs](https://bunit.dev/)

### Exemples

Voir les fichiers de tests existants :
- `Services/AuthServiceTests.cs` - Tests avec mocking
- `Services/StatisticsServiceTests.cs` - Tests async
- `Models/FishCatchValidationTests.cs` - Tests de validation

---

## 🎯 Objectifs

- [ ] 70% coverage ✅ **Atteint (75%)**
- [ ] 80% coverage (prochaine étape)
- [ ] Tous les services critiques testés ✅
- [ ] Tous les modèles validés testés ✅
- [ ] Composants UI principaux testés (en cours)
- [ ] Tests d'intégration E2E (à venir)

---

## 🤝 Contribution

Avant de soumettre une Pull Request :

1. ✅ Écrire des tests pour les nouveaux features
2. ✅ S'assurer que tous les tests passent
3. ✅ Vérifier le coverage (ne pas faire baisser)
4. ✅ Suivre les conventions de nommage

---

**🧪 Happy Testing !**

*Les tests sont la fondation d'un code de qualité.*
