# Script de build Android pour FishingSpot
# Fonctionne nativement sur Windows !

param(
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0"
)

Write-Host "🎣 FishingSpot - Build Android" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Cyan
Write-Host ""

$ProjectPath = "FishingSpot\FishingSpot.csproj"
$OutputPath = "publish\android"

# Créer le dossier de sortie
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

Write-Host "📦 Nettoyage..." -ForegroundColor Yellow
dotnet clean $ProjectPath -c $Configuration

Write-Host "🔧 Restauration des packages..." -ForegroundColor Yellow
dotnet restore $ProjectPath

Write-Host "🤖 Build Android APK..." -ForegroundColor Yellow
# Build pour Android
dotnet publish $ProjectPath `
    -c $Configuration `
    -f net10.0-android `
    -p:AndroidPackageFormat=apk `
    -p:AndroidKeyStore=true `
    -p:AndroidSigningKeyStore="fishingspot.keystore" `
    -p:AndroidSigningStorePass="fishingspot123" `
    -p:AndroidSigningKeyAlias="fishingspot" `
    -p:AndroidSigningKeyPass="fishingspot123"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erreur lors du build" -ForegroundColor Red
    Write-Host ""
    Write-Host "💡 Si vous n'avez pas de keystore, créez-en un avec:" -ForegroundColor Yellow
    Write-Host "keytool -genkey -v -keystore fishingspot.keystore -alias fishingspot -keyalg RSA -keysize 2048 -validity 10000"
    Write-Host ""
    Write-Host "OU utilisez le build sans signing:" -ForegroundColor Yellow
    Write-Host ".\build-android.ps1 -NoSigning"
    exit 1
}

# Rechercher le fichier APK généré
$ApkPath = Get-ChildItem -Path "FishingSpot\bin\$Configuration\net10.0-android" -Filter "*-Signed.apk" -Recurse | Select-Object -First 1

if (-not $ApkPath) {
    $ApkPath = Get-ChildItem -Path "FishingSpot\bin\$Configuration\net10.0-android" -Filter "*.apk" -Recurse | Select-Object -First 1
}

if ($ApkPath) {
    $DestinationPath = "$OutputPath\FishingSpot-$Version.apk"
    Copy-Item -Path $ApkPath.FullName -Destination $DestinationPath -Force

    Write-Host ""
    Write-Host "✅ Fichier APK créé avec succès!" -ForegroundColor Green
    Write-Host "📍 Emplacement: $DestinationPath" -ForegroundColor Green

    # Afficher la taille du fichier
    $FileSize = (Get-Item $DestinationPath).Length
    $FileSizeMB = [math]::Round($FileSize / 1MB, 2)
    Write-Host "📊 Taille: $FileSizeMB MB" -ForegroundColor Cyan

    Write-Host ""
    Write-Host "📱 Distribution Android:" -ForegroundColor Yellow
    Write-Host "1. Google Play Store (via Play Console)"
    Write-Host "2. Distribution directe (téléchargement APK)"
    Write-Host "3. F-Droid (pour open source)"
    Write-Host "4. Amazon Appstore"
    Write-Host "5. GitHub Releases"

    Write-Host ""
    Write-Host "🔗 Test rapide:" -ForegroundColor Green
    Write-Host "Copiez le fichier APK sur votre téléphone Android et installez-le"
    Write-Host "(Activez 'Sources inconnues' dans les paramètres si nécessaire)"

} else {
    Write-Host "❌ Fichier APK non trouvé" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✨ Terminé!" -ForegroundColor Green
