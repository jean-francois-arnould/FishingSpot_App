# 🔐 Configuration des Certificats iOS sans Mac

## Option 1 : Compte Apple Developer GRATUIT (Recommandé)

Vous pouvez créer un certificat de développement **sans Mac** en utilisant **Transporter** ou **Xcode Cloud**.

### Méthode : Utiliser Xcode via le navigateur (NOUVEAU)

Apple propose maintenant Xcode Cloud qui permet de signer sans Mac local.

## Option 2 : Générer les certificats avec OpenSSL (Windows)

### Étape 1 : Créer une clé privée

```powershell
# Installer OpenSSL (si pas déjà installé)
# Via Chocolatey: choco install openssl
# Ou télécharger depuis: https://slproweb.com/products/Win32OpenSSL.html

# Générer la clé privée
openssl genrsa -out private.key 2048

# Créer une demande de signature de certificat (CSR)
openssl req -new -key private.key -out CertificateSigningRequest.certSigningRequest -subj "/CN=FishingSpot Distribution/C=FR"
```

### Étape 2 : Sur Apple Developer Portal (depuis Windows)

1. **Connectez-vous** à https://developer.apple.com/account
2. **Certificats, Identifiants & Profils**
3. **Certificats** > **+** (Nouveau certificat)
4. Sélectionnez **iOS App Development** (compte gratuit) ou **iOS Distribution** (compte payant)
5. **Uploadez** `CertificateSigningRequest.certSigningRequest`
6. **Téléchargez** le certificat (`ios_development.cer` ou `ios_distribution.cer`)

### Étape 3 : Convertir en .p12

```powershell
# Convertir le certificat .cer en .pem
openssl x509 -inform DER -in ios_development.cer -out certificate.pem

# Créer le fichier .p12
openssl pkcs12 -export -out certificate.p12 -inkey private.key -in certificate.pem
# Entrez un mot de passe quand demandé (vous en aurez besoin pour GitHub)
```

### Étape 4 : Créer un Provisioning Profile

1. Retournez sur **Apple Developer Portal**
2. **Profils** > **+** (Nouveau profil)
3. Sélectionnez **iOS App Development**
4. Choisissez votre **App ID** : `com.companyname.fishingspot`
5. Sélectionnez votre **certificat**
6. Sélectionnez vos **appareils de test** (si compte gratuit)
7. **Générez** et **téléchargez** le profil (`.mobileprovision`)

### Étape 5 : Encoder en Base64 pour GitHub Secrets

```powershell
# Encoder le certificat .p12
$certBytes = [System.IO.File]::ReadAllBytes("certificate.p12")
$certBase64 = [System.Convert]::ToBase64String($certBytes)
$certBase64 | Out-File -FilePath "certificate_base64.txt"

# Encoder le provisioning profile
$profileBytes = [System.IO.File]::ReadAllBytes("FishingSpot_Distribution.mobileprovision")
$profileBase64 = [System.Convert]::ToBase64String($profileBytes)
$profileBase64 | Out-File -FilePath "profile_base64.txt"
```

### Étape 6 : Configurer les GitHub Secrets

1. Allez sur votre repo GitHub : https://github.com/jean-francois-arnould/FishingSpot_App
2. **Settings** > **Secrets and variables** > **Actions**
3. **New repository secret** - Créez les 3 secrets suivants :

| Nom du Secret | Valeur | Source |
|---------------|--------|--------|
| `IOS_CERTIFICATE_BASE64` | Contenu de `certificate_base64.txt` | Certificat .p12 encodé |
| `IOS_CERTIFICATE_PASSWORD` | Le mot de passe utilisé lors de la création du .p12 | Votre mot de passe |
| `IOS_PROVISIONING_PROFILE_BASE64` | Contenu de `profile_base64.txt` | Profil encodé |

## Option 3 : Utiliser un service tiers (Plus simple mais payant)

### Codemagic (Recommandé - 500 minutes gratuites/mois)

1. **Inscription** : https://codemagic.io
2. **Connectez votre repo GitHub**
3. **Configuration automatique** du signing
4. **Build automatique** à chaque push

Configuration `codemagic.yaml` :

```yaml
workflows:
  ios-workflow:
    name: iOS Build
    instance_type: mac_mini_m1
    environment:
      groups:
        - app_store_credentials
    scripts:
      - name: Install dependencies
        script: dotnet workload install maui
      - name: Build iOS
        script: |
          dotnet publish FishingSpot/FishingSpot.csproj \
            -c Release \
            -f net10.0-ios \
            -p:ArchiveOnBuild=true
    artifacts:
      - FishingSpot/bin/**/*.ipa
    publishing:
      scripts:
        - name: Upload to GitHub Release
```

### App Center (Microsoft)

1. **Inscription** : https://appcenter.ms
2. **Nouvelle app** > iOS
3. **Build** > Connectez GitHub
4. **Configurez** le signing
5. **Build automatique**

## Option 4 : Compte Apple Developer Payant ($99/an)

Si vous prévoyez de distribuer sérieusement :

**Avantages** :
- ✅ Pas de limite d'appareils
- ✅ Distribution TestFlight
- ✅ Certificats de distribution valables 1 an
- ✅ Support Apple officiel

**Lien** : https://developer.apple.com/programs/

## 🚀 Test du Workflow GitHub Actions

Une fois les secrets configurés :

```bash
# Pousser un tag pour déclencher le build
git tag v1.0.0
git push origin v1.0.0

# OU déclencher manuellement via l'interface GitHub
# Actions > Build iOS pour AltStore > Run workflow
```

## 📊 Vérification

1. Allez dans **Actions** sur GitHub
2. Observez le workflow **Build iOS pour AltStore**
3. Une fois terminé, téléchargez l'artifact `.ipa`
4. Le workflow met à jour automatiquement `altstore-source.json`

## ⚠️ Limitations du Compte Gratuit

- **7 jours de validité** : Les apps doivent être re-signées toutes les semaines
- **3 apps maximum** simultanément
- **10 appareils maximum** par app

**Solution** : Utilisez AltStore qui re-signe automatiquement en arrière-plan !

## 🆘 Besoin d'aide ?

Si vous êtes bloqué, je peux vous aider avec une autre approche :
- Build Android (fonctionne nativement sur Windows)
- Service de build en ligne pré-configuré
- Accès temporaire à un Mac virtuel

---

**Note** : Pour un usage personnel/test, le compte Apple Developer GRATUIT + GitHub Actions est largement suffisant ! 🎣
