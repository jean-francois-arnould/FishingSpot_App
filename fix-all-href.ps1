# Script pour corriger tous les href HTML sans le préfixe FishingSpot_App

$patterns = @(
    @('href="/materiel"', 'href="/FishingSpot_App/materiel"'),
    @('href="/catches"', 'href="/FishingSpot_App/catches"'),
    @('href="/montages"', 'href="/FishingSpot_App/montages"'),
    @('href="/profile"', 'href="/FishingSpot_App/profile"'),
    @('href="/materiel/cannes"', 'href="/FishingSpot_App/materiel/cannes"'),
    @('href="/materiel/moulinets"', 'href="/FishingSpot_App/materiel/moulinets"'),
    @('href="/materiel/fils"', 'href="/FishingSpot_App/materiel/fils"'),
    @('href="/materiel/leurres"', 'href="/FishingSpot_App/materiel/leurres"'),
    @('href="/materiel/hamecons"', 'href="/FishingSpot_App/materiel/hamecons"'),
    @('href="/materiel/bas-de-ligne"', 'href="/FishingSpot_App/materiel/bas-de-ligne"'),
    @('href="/materiel/cannes/ajouter"', 'href="/FishingSpot_App/materiel/cannes/ajouter"'),
    @('href="/materiel/moulinets/ajouter"', 'href="/FishingSpot_App/materiel/moulinets/ajouter"'),
    @('href="/materiel/fils/ajouter"', 'href="/FishingSpot_App/materiel/fils/ajouter"'),
    @('href="/materiel/leurres/ajouter"', 'href="/FishingSpot_App/materiel/leurres/ajouter"'),
    @('href="/materiel/hamecons/ajouter"', 'href="/FishingSpot_App/materiel/hamecons/ajouter"'),
    @('href="/materiel/bas-de-ligne/ajouter"', 'href="/FishingSpot_App/materiel/bas-de-ligne/ajouter"'),
    @('href="/materiel/cannes/modifier/@', 'href="/FishingSpot_App/materiel/cannes/modifier/@'),
    @('href="/materiel/moulinets/modifier/@', 'href="/FishingSpot_App/materiel/moulinets/modifier/@'),
    @('href="/materiel/fils/modifier/@', 'href="/FishingSpot_App/materiel/fils/modifier/@'),
    @('href="/materiel/leurres/modifier/@', 'href="/FishingSpot_App/materiel/leurres/modifier/@'),
    @('href="/materiel/hamecons/modifier/@', 'href="/FishingSpot_App/materiel/hamecons/modifier/@'),
    @('href="/materiel/bas-de-ligne/modifier/@', 'href="/FishingSpot_App/materiel/bas-de-ligne/modifier/@')
)

$files = Get-ChildItem -Path "." -Filter "*.razor" -Recurse
$totalModified = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content

    foreach ($pattern in $patterns) {
        $content = $content.Replace($pattern[0], $pattern[1])
    }

    if ($content -ne $originalContent) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "✅ Modified: $($file.Name)"
        $totalModified++
    }
}

Write-Host "`n🎉 Total files modified: $totalModified"
Write-Host "✅ All HTML href now use /FishingSpot_App/ prefix"
