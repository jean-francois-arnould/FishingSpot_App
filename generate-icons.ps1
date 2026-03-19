# Script pour convertir AppIcon.svg en PNG pour PWA
# Ce script nécessite Inkscape ou utilise un fallback avec ImageMagick

$svgPath = "wwwroot/AppIcon.svg"
$outputDir = "wwwroot"

Write-Host "🎨 Conversion de AppIcon.svg en icônes PNG..." -ForegroundColor Cyan

# Vérifier si Inkscape est installé
$inkscapePath = $null
$possiblePaths = @(
    "C:\Program Files\Inkscape\bin\inkscape.exe",
    "C:\Program Files (x86)\Inkscape\bin\inkscape.exe",
    "$env:LOCALAPPDATA\Programs\Inkscape\bin\inkscape.exe"
)

foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $inkscapePath = $path
        break
    }
}

if ($inkscapePath) {
    Write-Host "✅ Inkscape trouvé: $inkscapePath" -ForegroundColor Green

    # Générer icon-512.png
    Write-Host "📦 Génération de icon-512.png..." -ForegroundColor Yellow
    & $inkscapePath --export-type=png --export-filename="$outputDir/icon-512.png" --export-width=512 --export-height=512 $svgPath

    # Générer icon-192.png
    Write-Host "📦 Génération de icon-192.png..." -ForegroundColor Yellow
    & $inkscapePath --export-type=png --export-filename="$outputDir/icon-192.png" --export-width=192 --export-height=192 $svgPath

    Write-Host "✅ Icônes PNG générées avec succès!" -ForegroundColor Green
} else {
    Write-Host "⚠️  Inkscape n'est pas installé." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "📥 Options pour générer les icônes:" -ForegroundColor Cyan
    Write-Host "1. Installer Inkscape depuis: https://inkscape.org/release/" -ForegroundColor White
    Write-Host "2. Utiliser un convertisseur en ligne:" -ForegroundColor White
    Write-Host "   - https://cloudconvert.com/svg-to-png" -ForegroundColor Gray
    Write-Host "   - https://convertio.co/fr/svg-png/" -ForegroundColor Gray
    Write-Host ""
    Write-Host "📋 Instructions manuelles:" -ForegroundColor Cyan
    Write-Host "   - Convertissez AppIcon.svg en PNG 512x512 → enregistrez comme icon-512.png" -ForegroundColor White
    Write-Host "   - Convertissez AppIcon.svg en PNG 192x192 → enregistrez comme icon-192.png" -ForegroundColor White
    Write-Host "   - Placez les deux fichiers dans le dossier wwwroot/" -ForegroundColor White
    Write-Host ""
    Write-Host "🔄 Alternative: Si vous avez PowerShell 7+, vous pouvez utiliser Playwright" -ForegroundColor Cyan
    Write-Host "   Install-Module -Name Playwright" -ForegroundColor Gray
}

Write-Host ""
Write-Host "📝 Les fichiers suivants doivent exister dans wwwroot/:" -ForegroundColor Cyan
Write-Host "   - icon-512.png (512x512)" -ForegroundColor White
Write-Host "   - icon-192.png (192x192)" -ForegroundColor White
Write-Host "   - AppIcon.svg (déjà présent)" -ForegroundColor Green
