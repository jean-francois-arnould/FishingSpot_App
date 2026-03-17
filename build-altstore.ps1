# Script de build et publication pour AltStore
# Nécessite: 
# - Visual Studio 2022+ avec workload .NET MAUI
# - Certificat de développement Apple installé
# - Provisioning Profile configuré

param(
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0"
)

Write-Host "🎣 FishingSpot - Build pour AltStore" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Chemin du projet
$ProjectPath = "FishingSpot\FishingSpot.csproj"
$OutputPath = "publish\ios"

# Créer le dossier de sortie
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

Write-Host "📦 Nettoyage..." -ForegroundColor Yellow
dotnet clean $ProjectPath -c $Configuration

Write-Host "🔧 Restauration des packages..." -ForegroundColor Yellow
dotnet restore $ProjectPath

Write-Host "🏗️  Build iOS..." -ForegroundColor Yellow
# Build pour iOS
dotnet build $ProjectPath `
    -c $Configuration `
    -f net10.0-ios `
    -p:RuntimeIdentifier=ios-arm64 `
    -p:ArchiveOnBuild=true `
    -p:CreatePackage=true

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erreur lors du build" -ForegroundColor Red
    exit 1
}

# Rechercher le fichier .ipa généré
$IpaPath = Get-ChildItem -Path "FishingSpot\bin\$Configuration\net10.0-ios\ios-arm64" -Filter "*.ipa" -Recurse | Select-Object -First 1

if ($IpaPath) {
    $DestinationPath = "$OutputPath\FishingSpot-$Version.ipa"
    Copy-Item -Path $IpaPath.FullName -Destination $DestinationPath -Force

    Write-Host "✅ Fichier .ipa créé avec succès!" -ForegroundColor Green
    Write-Host "📍 Emplacement: $DestinationPath" -ForegroundColor Green

    # Afficher la taille du fichier
    $FileSize = (Get-Item $DestinationPath).Length
    $FileSizeMB = [math]::Round($FileSize / 1MB, 2)
    Write-Host "📊 Taille: $FileSizeMB MB" -ForegroundColor Cyan

    Write-Host ""
    Write-Host "📝 Prochaines étapes:" -ForegroundColor Yellow
    Write-Host "1. Mettez à jour altstore-source.json avec:"
    Write-Host "   - La taille réelle: $FileSize"
    Write-Host "   - L'URL de téléchargement du fichier .ipa"
    Write-Host "2. Hébergez les fichiers sur un serveur accessible (GitHub Pages, etc.)"
    Write-Host "3. Partagez l'URL de altstore-source.json"

} else {
    Write-Host "❌ Fichier .ipa non trouvé" -ForegroundColor Red
    Write-Host "Vérifiez que votre provisioning profile est correctement configuré" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "✨ Terminé!" -ForegroundColor Green
