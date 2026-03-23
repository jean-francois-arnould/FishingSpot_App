# Script pour exécuter tous les tests et générer un rapport

Write-Host "🧪 FishingSpot - Suite de Tests Complète" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Nettoyer les anciens rapports
Write-Host "🧹 Nettoyage des anciens rapports..." -ForegroundColor Yellow
if (Test-Path "TestResults") {
    Remove-Item -Recurse -Force "TestResults"
}
if (Test-Path "coverage-report") {
    Remove-Item -Recurse -Force "coverage-report"
}
Write-Host "✅ Nettoyage terminé" -ForegroundColor Green
Write-Host ""

# 2. Restaurer les dépendances
Write-Host "📦 Restauration des dépendances de tests..." -ForegroundColor Yellow
dotnet restore FishingSpot.Tests/FishingSpot.Tests.csproj
Write-Host "✅ Dépendances restaurées" -ForegroundColor Green
Write-Host ""

# 3. Build du projet de tests
Write-Host "🔨 Compilation des tests..." -ForegroundColor Yellow
dotnet build FishingSpot.Tests/FishingSpot.Tests.csproj --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Échec de la compilation" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Compilation réussie" -ForegroundColor Green
Write-Host ""

# 4. Exécuter les tests avec coverage
Write-Host "🧪 Exécution des tests avec coverage..." -ForegroundColor Yellow
dotnet test FishingSpot.Tests/FishingSpot.Tests.csproj `
    --configuration Debug `
    --no-build `
    --verbosity normal `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput=./TestResults/

$testExitCode = $LASTEXITCODE

if ($testExitCode -eq 0) {
    Write-Host "✅ Tous les tests passent!" -ForegroundColor Green
} else {
    Write-Host "⚠️  Certains tests ont échoué" -ForegroundColor Yellow
}
Write-Host ""

# 5. Vérifier si reportgenerator est installé
Write-Host "📊 Génération du rapport de coverage..." -ForegroundColor Yellow
$reportGeneratorInstalled = Get-Command reportgenerator -ErrorAction SilentlyContinue

if (-not $reportGeneratorInstalled) {
    Write-Host "📦 Installation de ReportGenerator..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-reportgenerator-globaltool
    $env:PATH += ";$env:USERPROFILE\.dotnet\tools"
}

# 6. Générer le rapport HTML
$coverageFile = "FishingSpot.Tests/TestResults/coverage.cobertura.xml"

if (Test-Path $coverageFile) {
    reportgenerator `
        "-reports:$coverageFile" `
        "-targetdir:coverage-report" `
        "-reporttypes:Html;HtmlSummary" `
        "-title:FishingSpot Test Coverage"

    Write-Host "✅ Rapport généré dans: coverage-report/index.html" -ForegroundColor Green

    # Afficher le résumé du coverage
    Write-Host ""
    Write-Host "📊 Résumé du Coverage:" -ForegroundColor Cyan

    $summaryFile = "coverage-report/Summary.txt"
    if (Test-Path $summaryFile) {
        Get-Content $summaryFile | Select-Object -First 20
    }
} else {
    Write-Host "⚠️  Fichier de coverage introuvable: $coverageFile" -ForegroundColor Yellow
}
Write-Host ""

# 7. Statistiques des tests
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "📈 Statistiques:" -ForegroundColor Cyan
Write-Host ""

$testResultFiles = Get-ChildItem -Path "FishingSpot.Tests/TestResults" -Filter "*.trx" -Recurse -ErrorAction SilentlyContinue
if ($testResultFiles) {
    $latestResult = $testResultFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    Write-Host "Dernier résultat: $($latestResult.FullName)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "✅ Analyse terminée!" -ForegroundColor Green
Write-Host ""
Write-Host "📂 Fichiers générés:" -ForegroundColor Cyan
Write-Host "- TestResults/        : Résultats bruts des tests"
Write-Host "- coverage-report/    : Rapport HTML de coverage"
Write-Host ""

# 8. Ouvrir le rapport dans le navigateur
Write-Host "🌐 Voulez-vous ouvrir le rapport dans le navigateur? (O/N)" -ForegroundColor Yellow
$response = Read-Host
if ($response -eq "O" -or $response -eq "o") {
    $reportPath = Resolve-Path "coverage-report/index.html"
    Start-Process $reportPath
    Write-Host "✅ Rapport ouvert dans le navigateur" -ForegroundColor Green
}

Write-Host ""
Write-Host "🎯 Objectif Coverage: 70%" -ForegroundColor Cyan
Write-Host "🎉 Pour améliorer le coverage, ajoutez plus de tests dans FishingSpot.Tests/" -ForegroundColor Yellow
Write-Host ""
Write-Host "🎣 Happy Testing!" -ForegroundColor Green

exit $testExitCode
