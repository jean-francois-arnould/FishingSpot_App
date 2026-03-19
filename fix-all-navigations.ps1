# Script pour ajouter le préfixe /FishingSpot_App/ à toutes les navigations

$patterns = @(
    @('NavigateTo\("/login"\)', 'NavigateTo("/FishingSpot_App/login")'),
    @('NavigateTo\("/register"\)', 'NavigateTo("/FishingSpot_App/register")'),
    @('NavigateTo\("/catches"\)', 'NavigateTo("/FishingSpot_App/catches")'),
    @('NavigateTo\("/catches/add"\)', 'NavigateTo("/FishingSpot_App/catches/add")'),
    @('NavigateTo\("/profile"\)', 'NavigateTo("/FishingSpot_App/profile")'),
    @('NavigateTo\("/materiel"\)', 'NavigateTo("/FishingSpot_App/materiel")'),
    @('NavigateTo\("/montages"\)', 'NavigateTo("/FishingSpot_App/montages")'),
    @('NavigateTo\("/"\)', 'NavigateTo("/FishingSpot_App/")'),
    @('NavigateTo\(\$"/catches/edit/\{catchId\}"\)', 'NavigateTo($"/FishingSpot_App/catches/edit/{catchId}")'),
    @('NavigateTo\(\$"/materiel/cannes/modifier/\{', 'NavigateTo($"/FishingSpot_App/materiel/cannes/modifier/{'),
    @('NavigateTo\(\$"/materiel/moulinets/modifier/\{', 'NavigateTo($"/FishingSpot_App/materiel/moulinets/modifier/{'),
    @('NavigateTo\(\$"/materiel/fils/modifier/\{', 'NavigateTo($"/FishingSpot_App/materiel/fils/modifier/{'),
    @('NavigateTo\(\$"/materiel/leurres/modifier/\{', 'NavigateTo($"/FishingSpot_App/materiel/leurres/modifier/{'),
    @('NavigateTo\(\$"/materiel/hamecons/modifier/\{', 'NavigateTo($"/FishingSpot_App/materiel/hamecons/modifier/{'),
    @('NavigateTo\(\$"/materiel/bas-de-ligne/modifier/\{', 'NavigateTo($"/FishingSpot_App/materiel/bas-de-ligne/modifier/{'),
    @('NavigateTo\(\$"/montages/modifier/\{', 'NavigateTo($"/FishingSpot_App/montages/modifier/{'),
    @('NavigateTo\("/materiel/cannes"\)', 'NavigateTo("/FishingSpot_App/materiel/cannes")'),
    @('NavigateTo\("/materiel/moulinets"\)', 'NavigateTo("/FishingSpot_App/materiel/moulinets")'),
    @('NavigateTo\("/materiel/fils"\)', 'NavigateTo("/FishingSpot_App/materiel/fils")'),
    @('NavigateTo\("/materiel/leurres"\)', 'NavigateTo("/FishingSpot_App/materiel/leurres")'),
    @('NavigateTo\("/materiel/hamecons"\)', 'NavigateTo("/FishingSpot_App/materiel/hamecons")'),
    @('NavigateTo\("/materiel/bas-de-ligne"\)', 'NavigateTo("/FishingSpot_App/materiel/bas-de-ligne")'),
    @('NavigateTo\("/materiel/cannes/ajouter"\)', 'NavigateTo("/FishingSpot_App/materiel/cannes/ajouter")'),
    @('NavigateTo\("/materiel/moulinets/ajouter"\)', 'NavigateTo("/FishingSpot_App/materiel/moulinets/ajouter")'),
    @('NavigateTo\("/materiel/fils/ajouter"\)', 'NavigateTo("/FishingSpot_App/materiel/fils/ajouter")'),
    @('NavigateTo\("/materiel/leurres/ajouter"\)', 'NavigateTo("/FishingSpot_App/materiel/leurres/ajouter")'),
    @('NavigateTo\("/materiel/hamecons/ajouter"\)', 'NavigateTo("/FishingSpot_App/materiel/hamecons/ajouter")'),
    @('NavigateTo\("/materiel/bas-de-ligne/ajouter"\)', 'NavigateTo("/FishingSpot_App/materiel/bas-de-ligne/ajouter")'),
    @('NavigateTo\("/montages/ajouter"\)', 'NavigateTo("/FishingSpot_App/montages/ajouter")')
)

$files = Get-ChildItem -Path "." -Filter "*.razor" -Recurse
$totalReplacements = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content

    foreach ($pattern in $patterns) {
        $content = $content -replace $pattern[0], $pattern[1]
    }

    if ($content -ne $originalContent) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "✅ Modified: $($file.Name)"
        $totalReplacements++
    }
}

Write-Host "`n🎉 Total files modified: $totalReplacements"
Write-Host "✅ All NavigateTo calls now use /FishingSpot_App/ prefix"
