#!/bin/bash

# FishingSpot Setup Script (Linux/macOS)
# Ce script automatise la configuration initiale du projet

echo "🎣 FishingSpot - Configuration Automatique"
echo "=========================================="
echo ""

# Vérifier .NET 10
echo "📦 Vérification de .NET 10..."
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK n'est pas installé!"
    echo "Téléchargez-le depuis: https://dotnet.microsoft.com/download"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "✅ .NET $DOTNET_VERSION détecté"
echo ""

# Restaurer les dépendances
echo "📦 Restauration des dépendances..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "❌ Échec de la restauration"
    exit 1
fi
echo "✅ Dépendances restaurées"
echo ""

# Créer appsettings.json si nécessaire
APP_SETTINGS="wwwroot/appsettings.json"
APP_SETTINGS_TEMPLATE="wwwroot/appsettings.template.json"

if [ ! -f "$APP_SETTINGS" ]; then
    echo "📝 Création du fichier de configuration..."

    if [ -f "$APP_SETTINGS_TEMPLATE" ]; then
        cp "$APP_SETTINGS_TEMPLATE" "$APP_SETTINGS"
        echo "✅ appsettings.json créé à partir du template"
        echo "⚠️  N'oubliez pas de configurer vos identifiants Supabase!"
    else
        echo "❌ Template appsettings.template.json introuvable"
    fi
else
    echo "✅ appsettings.json existe déjà"
fi
echo ""

# Build du projet
echo "🔨 Compilation du projet..."
dotnet build --configuration Debug
if [ $? -ne 0 ]; then
    echo "❌ Échec de la compilation"
    exit 1
fi
echo "✅ Compilation réussie"
echo ""

# Lancer les tests
echo "🧪 Lancement des tests..."
dotnet test --no-build
if [ $? -ne 0 ]; then
    echo "⚠️  Certains tests ont échoué"
else
    echo "✅ Tous les tests passent"
fi
echo ""

# Résumé
echo "=========================================="
echo "✅ Configuration terminée!"
echo ""
echo "📋 Prochaines étapes:"
echo "1. Configurer wwwroot/appsettings.json avec vos identifiants Supabase"
echo "2. Exécuter database/improvements.sql dans Supabase SQL Editor"
echo "3. Lancer l'application: dotnet run"
echo "4. Ouvrir https://localhost:5001 dans votre navigateur"
echo ""
echo "📚 Documentation:"
echo "- README.md          : Guide principal"
echo "- IMPROVEMENTS.md    : Détails des améliorations"
echo "- CONTRIBUTING.md    : Guide de contribution"
echo "- database/README.md : Documentation base de données"
echo ""
echo "🎣 Bonne pêche avec FishingSpot!"
