using FishingSpot.Models;
using FishingSpot.Services;

namespace FishingSpot.Views
{
    [QueryProperty(nameof(FishCatchId), "id")]
    public partial class AddCatchPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly SetupService _setupService;
        private readonly WeatherService _weatherService;
        private string _photoPath = string.Empty;
        private double _latitude;
        private double _longitude;
        private int _fishCatchId = 0;
        private FishCatch? _existingFishCatch;
        private WeatherData? _currentWeather;

        public int FishCatchId
        {
            get => _fishCatchId;
            set
            {
                _fishCatchId = value;
                if (_fishCatchId > 0)
                {
                    LoadExistingFishCatch();
                }
            }
        }

        public AddCatchPage(DatabaseService databaseService, SetupService setupService, WeatherService weatherService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _setupService = setupService;
            _weatherService = weatherService;

            // Initialiser les dates
            CatchDatePicker.Date = DateTime.Now;
            CatchTimePicker.Time = DateTime.Now.TimeOfDay;

            // Charger les setups
            LoadSetupPicker();
        }

        private void LoadSetupPicker()
        {
            var setups = _setupService.GetAllSetups();
            var setupsList = new List<SetupPickerItem> { new SetupPickerItem { Id = null, Name = "Aucun" } };
            setupsList.AddRange(setups.Select(s => new SetupPickerItem { Id = s.Id, Name = s.Name }));
            SetupPicker.ItemsSource = setupsList;
            SetupPicker.ItemDisplayBinding = new Binding("Name");
            SetupPicker.SelectedIndex = 0;

            // Événement pour afficher les détails du setup
            SetupPicker.SelectedIndexChanged += (s, e) =>
            {
                var selectedSetup = SetupPicker.SelectedItem as SetupPickerItem;
                if (selectedSetup != null && selectedSetup.Id.HasValue)
                {
                    var setup = _setupService.GetSetupById(selectedSetup.Id.Value);
                    if (setup != null)
                    {
                        SetupDetailsLabel.Text = _setupService.GetSetupSummary(setup);
                        SetupDetailsLabel.IsVisible = true;
                        return;
                    }
                }
                SetupDetailsLabel.IsVisible = false;
            };

            // Sélectionner le setup actif par défaut
            var activeSetup = _setupService.GetActiveSetup();
            if (activeSetup != null)
            {
                var activeItem = setupsList.FirstOrDefault(s => s.Id == activeSetup.Id);
                if (activeItem != null)
                {
                    SetupPicker.SelectedItem = activeItem;
                }
            }
        }

        private class SetupPickerItem
        {
            public int? Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private void LoadExistingFishCatch()
        {
            _existingFishCatch = _databaseService.GetCatchById(_fishCatchId);
            if (_existingFishCatch != null)
            {
                // Mettre à jour le titre
                Title = "Modifier la Capture";

                // Remplir les champs
                FishNameEntry.Text = _existingFishCatch.FishName;

                // Calculer M et CM depuis la longueur totale en cm
                int meters = (int)(_existingFishCatch.Length / 100);
                int centimeters = (int)(_existingFishCatch.Length % 100);
                LengthMetersEntry.Text = meters > 0 ? meters.ToString() : "";
                LengthCentimetersEntry.Text = centimeters > 0 ? centimeters.ToString() : "";

                // Calculer KG et GR depuis le poids total en kg
                int kilograms = (int)_existingFishCatch.Weight;
                int grams = (int)((_existingFishCatch.Weight - kilograms) * 1000);
                WeightKilogramsEntry.Text = kilograms > 0 ? kilograms.ToString() : "";
                WeightGramsEntry.Text = grams > 0 ? grams.ToString() : "";

                LocationEntry.Text = _existingFishCatch.LocationName;
                _latitude = _existingFishCatch.Latitude;
                _longitude = _existingFishCatch.Longitude;

                if (_latitude != 0 && _longitude != 0)
                {
                    CoordinatesLabel.Text = $"GPS: {_latitude:F6}, {_longitude:F6}";
                    CoordinatesLabel.TextColor = Colors.Green;
                }

                CatchDatePicker.Date = _existingFishCatch.CatchDate;
                CatchTimePicker.Time = _existingFishCatch.CatchTime;
                NotesEditor.Text = _existingFishCatch.Notes;

                _photoPath = _existingFishCatch.PhotoPath;
                if (!string.IsNullOrWhiteSpace(_photoPath))
                {
                    PhotoImage.Source = ImageSource.FromFile(_photoPath);
                    PhotoImage.IsVisible = true;
                    PhotoPlaceholder.IsVisible = false;
                }

                // Charger la sélection de setup
                if (_existingFishCatch.SetupId.HasValue)
                {
                    var setupsList = (List<SetupPickerItem>)SetupPicker.ItemsSource;
                    var setupItem = setupsList.FirstOrDefault(s => s.Id == _existingFishCatch.SetupId);
                    if (setupItem != null)
                    {
                        SetupPicker.SelectedItem = setupItem;
                    }
                }
            }
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                        using var sourceStream = await photo.OpenReadAsync();
                        using var localFileStream = File.OpenWrite(localFilePath);
                        await sourceStream.CopyToAsync(localFileStream);

                        _photoPath = localFilePath;
                        PhotoImage.Source = ImageSource.FromFile(localFilePath);
                        PhotoImage.IsVisible = true;
                        PhotoPlaceholder.IsVisible = false;
                    }
                }
                else
                {
                    await DisplayAlertAsync("Non supporté", "La capture de photo n'est pas supportée sur cet appareil", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erreur", $"Impossible de prendre la photo: {ex.Message}", "OK");
            }
        }

        private async void OnGetLocationClicked(object sender, EventArgs e)
        {
            try
            {
                // Vérifier et demander les permissions de localisation
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlertAsync("Permission refusée", 
                        "L'accès à la localisation est nécessaire pour enregistrer le lieu de pêche. " +
                        "Veuillez autoriser l'accès dans les paramètres de l'application.", 
                        "OK");
                    return;
                }

                // Maintenant on peut accéder à la localisation
                var location = await Geolocation.Default.GetLastKnownLocationAsync();

                if (location == null)
                {
                    location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                }

                if (location != null)
                {
                    _latitude = location.Latitude;
                    _longitude = location.Longitude;
                    CoordinatesLabel.Text = $"📍 GPS: {_latitude:F6}, {_longitude:F6}";
                    CoordinatesLabel.TextColor = Colors.Green;

                    if (string.IsNullOrWhiteSpace(LocationEntry.Text))
                    {
                        LocationEntry.Text = $"Position GPS ({_latitude:F4}, {_longitude:F4})";
                    }

                    // Récupérer la météo actuelle
                    _currentWeather = await _weatherService.GetCurrentWeatherAsync(_latitude, _longitude);
                    if (_currentWeather != null)
                    {
                        await DisplayAlertAsync("Météo", 
                            $"Température: {_currentWeather.Temperature:F1}°C\n" +
                            $"Conditions: {_currentWeather.Description}\n" +
                            $"Vent: {_currentWeather.WindSpeed} m/s\n" +
                            $"Humidité: {_currentWeather.Humidity}%", 
                            "OK");
                    }
                }
                else
                {
                    await DisplayAlertAsync("Localisation non disponible", 
                        "Impossible d'obtenir votre position. Assurez-vous que le GPS est activé.", 
                        "OK");
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlertAsync("Non supporté", 
                    "La géolocalisation n'est pas supportée sur cet appareil.", 
                    "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlertAsync("Permission refusée", 
                    "L'accès à la localisation a été refusé.", 
                    "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erreur", $"Impossible d'obtenir la localisation: {ex.Message}", "OK");
            }
        }

        private async void OnSaveCatchClicked(object sender, EventArgs e)
        {
            // Seul le nom est obligatoire
            if (string.IsNullOrWhiteSpace(FishNameEntry.Text))
            {
                await DisplayAlertAsync("Erreur", "Veuillez entrer le nom du poisson", "OK");
                return;
            }

            // Calculer la longueur totale en cm (M * 100 + CM)
            double lengthMeters = 0;
            double lengthCentimeters = 0;

            if (!string.IsNullOrWhiteSpace(LengthMetersEntry.Text))
            {
                double.TryParse(LengthMetersEntry.Text, out lengthMeters);
            }

            if (!string.IsNullOrWhiteSpace(LengthCentimetersEntry.Text))
            {
                double.TryParse(LengthCentimetersEntry.Text, out lengthCentimeters);
            }

            double totalLengthInCm = (lengthMeters * 100) + lengthCentimeters;

            // Calculer le poids total en kg (KG + GR/1000)
            double weightKilograms = 0;
            double weightGrams = 0;

            if (!string.IsNullOrWhiteSpace(WeightKilogramsEntry.Text))
            {
                double.TryParse(WeightKilogramsEntry.Text, out weightKilograms);
            }

            if (!string.IsNullOrWhiteSpace(WeightGramsEntry.Text))
            {
                double.TryParse(WeightGramsEntry.Text, out weightGrams);
            }

            double totalWeightInKg = weightKilograms + (weightGrams / 1000.0);

            // Récupérer l'ID du setup sélectionné
            int? setupId = (SetupPicker.SelectedItem as SetupPickerItem)?.Id;

            if (_existingFishCatch != null)
            {
                // Mode modification
                _existingFishCatch.FishName = FishNameEntry.Text;
                _existingFishCatch.PhotoPath = _photoPath;
                _existingFishCatch.Latitude = _latitude;
                _existingFishCatch.Longitude = _longitude;
                _existingFishCatch.LocationName = string.IsNullOrWhiteSpace(LocationEntry.Text) ? "Non renseigné" : LocationEntry.Text;
                _existingFishCatch.CatchDate = CatchDatePicker.Date ?? DateTime.Now;
                _existingFishCatch.CatchTime = CatchTimePicker.Time ?? DateTime.Now.TimeOfDay;
                _existingFishCatch.Length = totalLengthInCm;
                _existingFishCatch.Weight = totalWeightInKg;
                _existingFishCatch.Notes = NotesEditor.Text ?? string.Empty;
                _existingFishCatch.SetupId = setupId;

                _databaseService.UpdateCatch(_existingFishCatch);
                await DisplayAlertAsync("Succès", "Votre capture a été modifiée !", "OK");
            }
            else
            {
                // Mode création
                var fishCatch = new FishCatch
                {
                    FishName = FishNameEntry.Text,
                    PhotoPath = _photoPath,
                    Latitude = _latitude,
                    Longitude = _longitude,
                    LocationName = string.IsNullOrWhiteSpace(LocationEntry.Text) ? "Non renseigné" : LocationEntry.Text,
                    CatchDate = CatchDatePicker.Date ?? DateTime.Now,
                    CatchTime = CatchTimePicker.Time ?? DateTime.Now.TimeOfDay,
                    Length = totalLengthInCm,
                    Weight = totalWeightInKg,
                    Notes = NotesEditor.Text ?? string.Empty,
                    SetupId = setupId
                };

                var catchId = _databaseService.AddCatch(fishCatch);

                // Associer la météo à la capture
                if (_currentWeather != null)
                {
                    _currentWeather.CatchId = catchId;
                    await _weatherService.UpdateWeatherForCatchAsync(_currentWeather);
                }

                await DisplayAlertAsync("Succès", "Votre capture a été enregistrée !", "OK");
            }

            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
