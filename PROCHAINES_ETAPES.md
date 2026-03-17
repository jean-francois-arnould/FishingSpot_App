# PROCHAINES ÉTAPES - FishingSpot

## ✅ Ce qui est fait

### Architecture de base
- ✅ Structure MAUI complète (iOS + Android)
- ✅ Navigation par onglets (TabBar)
- ✅ Injection de dépendances configurée
- ✅ Modèles de données (Fish, FishCatch)
- ✅ Service de base de données en mémoire

### Pages implémentées
- ✅ MapPage (placeholder pour Google Maps)
- ✅ FishDocumentationPage (7 espèces documentées)
- ✅ MyCatchesPage (liste des captures)
- ✅ AddCatchPage (formulaire complet)

### Fonctionnalités actives
- ✅ Capture photo avec MediaPicker
- ✅ Géolocalisation GPS
- ✅ Enregistrement des captures
- ✅ Suppression par swipe
- ✅ Validation des formulaires
- ✅ Permissions Android & iOS

---

## 🔄 Améliorations Recommandées

### 1. **Stockage Persistant avec SQLite**

**Priorité : HAUTE** 🔴

#### Installation
```bash
dotnet add package sqlite-net-pcl
dotnet add package SQLitePCLRaw.bundle_green
```

#### Implémentation

**Créer `Services/SqliteService.cs` :**

```csharp
using SQLite;
using FishingSpot.Models;

public class SqliteService
{
    private readonly SQLiteAsyncConnection _database;

    public SqliteService()
    {
        var dbPath = Path.Combine(
            FileSystem.AppDataDirectory, 
            "fishingspot.db3"
        );
        _database = new SQLiteAsyncConnection(dbPath);

        _database.CreateTableAsync<FishCatch>().Wait();
    }

    public Task<List<FishCatch>> GetCatchesAsync() =>
        _database.Table<FishCatch>().ToListAsync();

    public Task<int> SaveCatchAsync(FishCatch fishCatch) =>
        _database.InsertAsync(fishCatch);

    public Task<int> DeleteCatchAsync(FishCatch fishCatch) =>
        _database.DeleteAsync(fishCatch);
}
```

**Mettre à jour `Models/FishCatch.cs` :**

```csharp
using SQLite;

public class FishCatch
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    // ... reste des propriétés
}
```

**Enregistrer dans `MauiProgram.cs` :**

```csharp
builder.Services.AddSingleton<SqliteService>();
```

---

### 2. **Intégration Google Maps Complète**

**Priorité : HAUTE** 🔴

#### Installation

```bash
dotnet add package Microsoft.Maui.Controls.Maps
```

OU pour une solution plus avancée :

```bash
dotnet add package Maui.GoogleMaps
```

#### Configuration

**Android - `Platforms/Android/AndroidManifest.xml` :**

```xml
<application>
    <meta-data 
        android:name="com.google.android.geo.API_KEY" 
        android:value="VOTRE_CLE_API_GOOGLE" />
</application>
```

**iOS - Créer `Platforms/iOS/AppDelegate.cs` :**

```csharp
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Google.Maps;

namespace FishingSpot
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            MapServices.ProvideAPIKey("VOTRE_CLE_API_IOS");
            return MauiProgram.CreateMauiApp();
        }
    }
}
```

**Mettre à jour `MauiProgram.cs` :**

```csharp
builder
    .UseMauiApp<App>()
    .UseMauiMaps() // Ajouter cette ligne
    .ConfigureFonts(...)
```

**Remplacer `Views/MapPage.xaml` :**

```xml
<ContentPage ...
    xmlns:maps="clr-namespace:Microsoft.Maui.Controls.Maps;assembly=Microsoft.Maui.Controls.Maps">

    <Grid>
        <maps:Map x:Name="map"
                  IsShowingUser="True"
                  MapType="Hybrid">
            <!-- Les pins seront ajoutés dynamiquement -->
        </maps:Map>

        <Button Text="+" 
                VerticalOptions="End"
                HorizontalOptions="End"
                Margin="20"
                Clicked="OnAddPinClicked"
                WidthRequest="60"
                HeightRequest="60"
                CornerRadius="30"
                BackgroundColor="#4CAF50"/>
    </Grid>
</ContentPage>
```

**Mettre à jour `Views/MapPage.xaml.cs` :**

```csharp
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class MapPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public MapPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        LoadCatchPins();
    }

    private void LoadCatchPins()
    {
        map.Pins.Clear();

        foreach (var fishCatch in _databaseService.GetAllCatches())
        {
            if (fishCatch.Latitude != 0 && fishCatch.Longitude != 0)
            {
                var pin = new Pin
                {
                    Label = fishCatch.FishName,
                    Location = new Location(fishCatch.Latitude, fishCatch.Longitude),
                    Type = PinType.Place
                };
                map.Pins.Add(pin);
            }
        }

        // Centrer sur la première capture
        if (map.Pins.Count > 0)
        {
            var firstPin = map.Pins[0];
            map.MoveToRegion(MapSpan.FromCenterAndRadius(
                firstPin.Location, 
                Distance.FromKilometers(5)
            ));
        }
    }

    private async void OnAddPinClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("AddCatchPage");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadCatchPins();
    }
}
```

---

### 3. **Affichage des Photos Enregistrées**

**Priorité : MOYENNE** 🟠

**Problème actuel :** Les photos ne s'affichent pas dans la liste des captures.

**Solution dans `Views/MyCatchesPage.xaml` :**

```xml
<Frame Grid.Row="0" Grid.RowSpan="4"
       Grid.Column="0"
       Padding="0"
       CornerRadius="8"
       IsClippedToBounds="True"
       HasShadow="False"
       WidthRequest="70"
       HeightRequest="70"
       VerticalOptions="Center">

    <!-- Remplacer le Label emoji par : -->
    <Image Source="{Binding PhotoPath}"
           Aspect="AspectFill">
        <Image.Triggers>
            <DataTrigger TargetType="Image" 
                        Binding="{Binding PhotoPath}" 
                        Value="">
                <Setter Property="IsVisible" Value="False"/>
            </DataTrigger>
        </Image.Triggers>
    </Image>

    <!-- Placeholder si pas de photo -->
    <Label Text="🐟"
           FontSize="40"
           HorizontalOptions="Center"
           VerticalOptions="Center">
        <Label.Triggers>
            <DataTrigger TargetType="Label" 
                        Binding="{Binding PhotoPath}" 
                        Value="">
                <Setter Property="IsVisible" Value="True"/>
            </DataTrigger>
        </Label.Triggers>
    </Label>
</Frame>
```

---

### 4. **Statistiques de Pêche**

**Priorité : MOYENNE** 🟠

**Créer `Views/StatisticsPage.xaml` :**

```xml
<ContentPage Title="Statistiques">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="15">

            <!-- Résumé -->
            <Frame BackgroundColor="#4CAF50" CornerRadius="10">
                <VerticalStackLayout Spacing="5">
                    <Label Text="Total Captures" 
                           TextColor="White" 
                           FontSize="16"/>
                    <Label Text="{Binding TotalCatches}" 
                           TextColor="White" 
                           FontSize="32" 
                           FontAttributes="Bold"/>
                </VerticalStackLayout>
            </Frame>

            <!-- Plus gros poisson -->
            <Frame CornerRadius="10">
                <VerticalStackLayout Spacing="5">
                    <Label Text="Plus Gros Poisson" 
                           FontSize="18" 
                           FontAttributes="Bold"/>
                    <Label Text="{Binding BiggestFish.FishName}"/>
                    <Label Text="{Binding BiggestFish.Weight, StringFormat='{0:F2} kg'}"/>
                </VerticalStackLayout>
            </Frame>

            <!-- Poisson le plus capturé -->
            <Frame CornerRadius="10">
                <VerticalStackLayout Spacing="5">
                    <Label Text="Espèce la Plus Capturée" 
                           FontSize="18" 
                           FontAttributes="Bold"/>
                    <Label Text="{Binding MostCaughtSpecies}"/>
                </VerticalStackLayout>
            </Frame>

        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

**Ajouter l'onglet dans `AppShell.xaml` :**

```xml
<ShellContent
    Title="Statistiques"
    Icon="chart.png"
    ContentTemplate="{DataTemplate views:StatisticsPage}"
    Route="StatisticsPage" />
```

---

### 5. **Export des Données**

**Priorité : BASSE** 🟢

**Ajouter un bouton dans `MyCatchesPage.xaml` :**

```xml
<Button Text="Exporter en CSV"
        Clicked="OnExportClicked"
        BackgroundColor="#2196F3"
        TextColor="White"
        Margin="20,0"
        CornerRadius="8"/>
```

**Implémentation dans `MyCatchesPage.xaml.cs` :**

```csharp
private async void OnExportClicked(object sender, EventArgs e)
{
    var catches = _databaseService.GetAllCatches();

    var csv = new StringBuilder();
    csv.AppendLine("Date,Heure,Poisson,Longueur(cm),Poids(kg),Lieu");

    foreach (var catch in catches)
    {
        csv.AppendLine($"{catch.CatchDate:dd/MM/yyyy}," +
                      $"{catch.CatchTime:hh\\:mm}," +
                      $"{catch.FishName}," +
                      $"{catch.Length}," +
                      $"{catch.Weight}," +
                      $"\"{catch.LocationName}\"");
    }

    var filePath = Path.Combine(FileSystem.AppDataDirectory, "mes_captures.csv");
    await File.WriteAllTextAsync(filePath, csv.ToString());

    await Share.Default.RequestAsync(new ShareFileRequest
    {
        Title = "Exporter mes captures",
        File = new ShareFile(filePath)
    });
}
```

---

### 6. **Amélioration de l'UX**

**Priorité : BASSE** 🟢

#### Pull-to-Refresh

Dans `MyCatchesPage.xaml` :

```xml
<RefreshView IsRefreshing="{Binding IsRefreshing}"
             Command="{Binding RefreshCommand}">
    <CollectionView ... />
</RefreshView>
```

#### Loading Indicators

```xml
<ActivityIndicator IsRunning="{Binding IsBusy}"
                   IsVisible="{Binding IsBusy}"
                   Color="#4CAF50"/>
```

#### Animations

```csharp
await myElement.FadeTo(0, 250);
await myElement.FadeTo(1, 250);
```

---

### 7. **Tests et Optimisations**

**Priorité : BASSE** 🟢

- Tests unitaires pour les modèles
- Tests d'intégration pour les services
- Optimisation des images (compression)
- Gestion du cache des photos
- Mode hors ligne complet

---

## 📋 Checklist de Déploiement

### Android

- [ ] Signer l'APK avec une clé de déploiement
- [ ] Mettre à jour `ApplicationVersion` dans .csproj
- [ ] Créer des captures d'écran pour le Play Store
- [ ] Rédiger la description de l'application
- [ ] Préparer l'icône haute résolution
- [ ] Tester sur plusieurs appareils Android

### iOS

- [ ] Configurer le provisioning profile
- [ ] Créer les certificats de distribution
- [ ] Mettre à jour `ApplicationVersion` dans .csproj
- [ ] Créer des captures d'écran pour l'App Store
- [ ] Rédiger la description de l'application
- [ ] Tester sur plusieurs appareils iOS
- [ ] Soumettre pour review Apple

---

## 🔒 Sécurité et Confidentialité

- [ ] Ajouter une politique de confidentialité
- [ ] Implémenter le chiffrement des données sensibles
- [ ] Gérer les permissions de manière granulaire
- [ ] Ajouter une authentification utilisateur (optionnel)

---

## 📚 Ressources Utiles

### Documentation
- [.NET MAUI Docs](https://learn.microsoft.com/dotnet/maui/)
- [Google Maps Platform](https://developers.google.com/maps)
- [SQLite.NET](https://github.com/praeclarum/sqlite-net)

### Tutoriels
- [MAUI GPS Location](https://learn.microsoft.com/dotnet/maui/platform-integration/device/geolocation)
- [MAUI Media Picker](https://learn.microsoft.com/dotnet/maui/platform-integration/storage/media-picker)
- [MAUI Maps](https://learn.microsoft.com/dotnet/maui/user-interface/controls/map)

---

**Bon développement ! 🚀**
