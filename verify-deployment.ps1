# Script de vérification pré-déploiement
Write-Host "🔍 Vérification de la configuration PWA pour GitHub Pages..." -ForegroundColor Cyan
Write-Host ""

$errors = @()
$warnings = @()
$success = @()

# 1. Vérifier qu'il n'y a pas de routes en dur
Write-Host "1️⃣ Vérification des routes en dur..." -ForegroundColor Yellow
$hardcodedRoutes = Select-String -Path "*.razor" -Pattern '@page.*FishingSpot_App' -ErrorAction SilentlyContinue
if ($hardcodedRoutes) {
    $errors += "❌ Routes en dur trouvées avec /FishingSpot_App/"
    $hardcodedRoutes | ForEach-Object { Write-Host "   $($_.Filename): $($_.Line)" -ForegroundColor Red }
} else {
    $success += "✅ Aucune route en dur trouvée"
    Write-Host "   ✅ OK" -ForegroundColor Green
}

# 2. Vérifier le base href dans index.html
Write-Host "2️⃣ Vérification du base href..." -ForegroundColor Yellow
$indexContent = Get-Content "wwwroot/index.html" -Raw
if ($indexContent -match '<base href="/" />') {
    $success += "✅ base href='/' dans index.html (correct pour local)"
    Write-Host "   ✅ OK - base href='/'" -ForegroundColor Green
} elseif ($indexContent -match '<base href="/FishingSpot_App/" />') {
    $warnings += "⚠️ base href='/FishingSpot_App/' dans index.html (sera corrigé par le workflow)"
    Write-Host "   ⚠️ WARNING - base href='/FishingSpot_App/' (workflow le corrigera)" -ForegroundColor DarkYellow
} else {
    $errors += "❌ base href non trouvé ou incorrect dans index.html"
    Write-Host "   ❌ ERROR - base href non trouvé" -ForegroundColor Red
}

# 3. Vérifier le manifest.webmanifest
Write-Host "3️⃣ Vérification du manifest..." -ForegroundColor Yellow
if (Test-Path "wwwroot/manifest.webmanifest") {
    $manifestContent = Get-Content "wwwroot/manifest.webmanifest" -Raw
    if ($manifestContent -match '"id": "\.\/"' -and $manifestContent -match '"start_url": "\.\/"') {
        $success += "✅ manifest.webmanifest avec chemins relatifs"
        Write-Host "   ✅ OK - chemins relatifs (./) trouvés" -ForegroundColor Green
    } else {
        $warnings += "⚠️ manifest.webmanifest pourrait avoir des chemins incorrects"
        Write-Host "   ⚠️ WARNING - vérifier les chemins dans manifest" -ForegroundColor DarkYellow
    }
} else {
    $errors += "❌ manifest.webmanifest non trouvé"
    Write-Host "   ❌ ERROR - fichier manquant" -ForegroundColor Red
}

# 4. Vérifier le service-worker.published.js
Write-Host "4️⃣ Vérification du service worker..." -ForegroundColor Yellow
if (Test-Path "wwwroot/service-worker.published.js") {
    $swContent = Get-Content "wwwroot/service-worker.published.js" -Raw
    if ($swContent -match 'const base = "/";') {
        $success += "✅ service-worker.published.js avec base='/'"
        Write-Host "   ✅ OK - base='/' trouvé" -ForegroundColor Green
    } else {
        $warnings += "⚠️ service-worker.published.js pourrait avoir une base incorrecte"
        Write-Host "   ⚠️ WARNING - vérifier la variable 'base'" -ForegroundColor DarkYellow
    }
} else {
    $errors += "❌ service-worker.published.js non trouvé"
    Write-Host "   ❌ ERROR - fichier manquant" -ForegroundColor Red
}

# 5. Vérifier le 404.html
Write-Host "5️⃣ Vérification du 404.html..." -ForegroundColor Yellow
if (Test-Path "404.html") {
    $notFoundContent = Get-Content "404.html" -Raw
    if ($notFoundContent -match 'pathSegmentsToKeep = 1') {
        $success += "✅ 404.html avec pathSegmentsToKeep=1"
        Write-Host "   ✅ OK - pathSegmentsToKeep=1 trouvé" -ForegroundColor Green
    } else {
        $errors += "❌ 404.html sans pathSegmentsToKeep=1"
        Write-Host "   ❌ ERROR - pathSegmentsToKeep devrait être 1" -ForegroundColor Red
    }
} else {
    $errors += "❌ 404.html non trouvé"
    Write-Host "   ❌ ERROR - fichier manquant" -ForegroundColor Red
}

# 6. Vérifier les icônes PWA
Write-Host "6️⃣ Vérification des icônes PWA..." -ForegroundColor Yellow
$icons = @("wwwroot/icon-192.png", "wwwroot/icon-512.png", "wwwroot/favicon.png")
$missingIcons = $icons | Where-Object { -not (Test-Path $_) }
if ($missingIcons.Count -eq 0) {
    $success += "✅ Toutes les icônes PWA présentes"
    Write-Host "   ✅ OK - toutes les icônes trouvées" -ForegroundColor Green
} else {
    $errors += "❌ Icônes PWA manquantes: $($missingIcons -join ', ')"
    Write-Host "   ❌ ERROR - icônes manquantes" -ForegroundColor Red
    $missingIcons | ForEach-Object { Write-Host "      - $_" -ForegroundColor Red }
}

# 7. Vérifier le workflow
Write-Host "7️⃣ Vérification du workflow GitHub Actions..." -ForegroundColor Yellow
if (Test-Path ".github/workflows/blazor-deploy.yml") {
    $workflowContent = Get-Content ".github/workflows/blazor-deploy.yml" -Raw
    $checks = @(
        ($workflowContent -match 'sed.*base href.*FishingSpot_App', "Modification base href"),
        ($workflowContent -match 'sed.*manifest.*FishingSpot_App', "Modification manifest"),
        ($workflowContent -match 'sed.*service-worker.*FishingSpot_App', "Modification service worker"),
        ($workflowContent -match 'cp 404.html', "Copie 404.html"),
        ($workflowContent -match 'touch.*\.nojekyll', "Création .nojekyll")
    )

    $allOk = $true
    foreach ($check in $checks) {
        if ($check[0]) {
            Write-Host "   ✅ $($check[1])" -ForegroundColor Green
        } else {
            Write-Host "   ❌ $($check[1]) - manquant" -ForegroundColor Red
            $errors += "❌ Workflow: $($check[1]) manquant"
            $allOk = $false
        }
    }

    if ($allOk) {
        $success += "✅ Workflow correctement configuré"
    }
} else {
    $errors += "❌ Workflow GitHub Actions non trouvé"
    Write-Host "   ❌ ERROR - fichier manquant" -ForegroundColor Red
}

# 8. Vérifier le build
Write-Host "8️⃣ Test de compilation..." -ForegroundColor Yellow
Write-Host "   Compilation en cours..." -ForegroundColor Gray
$buildResult = dotnet build --nologo 2>&1
if ($LASTEXITCODE -eq 0) {
    $success += "✅ Projet compile sans erreur"
    Write-Host "   ✅ OK - compilation réussie" -ForegroundColor Green
} else {
    $errors += "❌ Erreurs de compilation"
    Write-Host "   ❌ ERROR - échec de compilation" -ForegroundColor Red
    Write-Host $buildResult -ForegroundColor Red
}

# Résumé
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "📊 RÉSUMÉ DE LA VÉRIFICATION" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""

if ($success.Count -gt 0) {
    Write-Host "✅ SUCCÈS ($($success.Count)):" -ForegroundColor Green
    $success | ForEach-Object { Write-Host "   $_" -ForegroundColor Green }
    Write-Host ""
}

if ($warnings.Count -gt 0) {
    Write-Host "⚠️ AVERTISSEMENTS ($($warnings.Count)):" -ForegroundColor DarkYellow
    $warnings | ForEach-Object { Write-Host "   $_" -ForegroundColor DarkYellow }
    Write-Host ""
}

if ($errors.Count -gt 0) {
    Write-Host "❌ ERREURS ($($errors.Count)):" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host "   $_" -ForegroundColor Red }
    Write-Host ""
    Write-Host "❌ DÉPLOIEMENT DÉCONSEILLÉ - Corrigez les erreurs d'abord!" -ForegroundColor Red
    Write-Host ""
    exit 1
} else {
    Write-Host "🎉 TOUT EST PRÊT POUR LE DÉPLOIEMENT!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Commandes pour déployer:" -ForegroundColor Cyan
    Write-Host "  git add ." -ForegroundColor White
    Write-Host "  git commit -m 'Fix routes and prepare PWA deployment'" -ForegroundColor White
    Write-Host "  git push origin main" -ForegroundColor White
    Write-Host ""
    Write-Host "Le déploiement se fera automatiquement via GitHub Actions." -ForegroundColor Gray
    Write-Host "Surveillez: https://github.com/jean-francois-arnould/FishingSpot_App/actions" -ForegroundColor Gray
    Write-Host ""
    exit 0
}
