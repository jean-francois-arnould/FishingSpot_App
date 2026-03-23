# FishingSpot Setup Script
# Ce script automatise la configuration initiale du projet

Write-Host "🎣 FishingSpot - Configuration Automatique" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Vérifier .NET 10
Write-Host "📦 Vérification de .NET 10..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ .NET SDK n'est pas installé!" -ForegroundColor Red
    Write-Host "Téléchargez-le depuis: https://dotnet.microsoft.com/download" -ForegroundColor Red
    exit 1
}
Write-Host "✅ .NET $dotnetVersion détecté" -ForegroundColor Green
Write-Host ""

# Restaurer les dépendances
Write-Host "📦 Restauration des dépendances..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Échec de la restauration" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Dépendances restaurées" -ForegroundColor Green
Write-Host ""

# Créer appsettings.json si nécessaire
$appSettingsPath = "wwwroot/appsettings.json"
$appSettingsTemplatePath = "wwwroot/appsettings.template.json"

if (-not (Test-Path $appSettingsPath)) {
    Write-Host "📝 Création du fichier de configuration..." -ForegroundColor Yellow

    if (Test-Path $appSettingsTemplatePath) {
        Copy-Item $appSettingsTemplatePath $appSettingsPath
        Write-Host "✅ appsettings.json créé à partir du template" -ForegroundColor Green
        Write-Host "⚠️  N'oubliez pas de configurer vos identifiants Supabase!" -ForegroundColor Yellow
    } else {
        Write-Host "❌ Template appsettings.template.json introuvable" -ForegroundColor Red
    }
} else {
    Write-Host "✅ appsettings.json existe déjà" -ForegroundColor Green
}
Write-Host ""

# Build du projet
Write-Host "🔨 Compilation du projet..." -ForegroundColor Yellow
dotnet build --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Échec de la compilation" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Compilation réussie" -ForegroundColor Green
Write-Host ""

# Lancer les tests
Write-Host "🧪 Lancement des tests..." -ForegroundColor Yellow
dotnet test --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Certains tests ont échoué" -ForegroundColor Yellow
} else {
    Write-Host "✅ Tous les tests passent" -ForegroundColor Green
}
Write-Host ""

# Résumé
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "✅ Configuration terminée!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Prochaines étapes:" -ForegroundColor Cyan
Write-Host "1. Configurer wwwroot/appsettings.json avec vos identifiants Supabase"
Write-Host "2. Exécuter database/improvements.sql dans Supabase SQL Editor"
Write-Host "3. Lancer l'application: dotnet run"
Write-Host "4. Ouvrir https://localhost:5001 dans votre navigateur"
Write-Host ""
Write-Host "📚 Documentation:" -ForegroundColor Cyan
Write-Host "- README.md          : Guide principal"
Write-Host "- IMPROVEMENTS.md    : Détails des améliorations"
Write-Host "- CONTRIBUTING.md    : Guide de contribution"
Write-Host "- database/README.md : Documentation base de données"
Write-Host ""
Write-Host "🎣 Bonne pêche avec FishingSpot!" -ForegroundColor Green
