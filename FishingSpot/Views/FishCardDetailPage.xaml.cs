using FishingSpot.Models;
using FishingSpot.Services;

namespace FishingSpot.Views
{
    [QueryProperty(nameof(FishCatchId), "id")]
    public partial class FishCardDetailPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly MaterialService _materialService;
        private readonly SocialShareService _socialShareService;
        private readonly WeatherService _weatherService;
        private int _fishCatchId;
        private FishCatch? _currentFishCatch;

        public int FishCatchId
        {
            get => _fishCatchId;
            set
            {
                _fishCatchId = value;
                LoadFishDetails();
            }
        }

        public FishCardDetailPage(DatabaseService databaseService, MaterialService materialService, 
            SocialShareService socialShareService, WeatherService weatherService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _materialService = materialService;
            _socialShareService = socialShareService;
            _weatherService = weatherService;
        }

        private void LoadFishDetails()
        {
            _currentFishCatch = _databaseService.GetCatchById(_fishCatchId);

            if (_currentFishCatch != null)
            {
                FishNameLabel.Text = _currentFishCatch.FishName;
                LengthLabel.Text = $"{_currentFishCatch.Length:F1} cm";
                WeightLabel.Text = $"{_currentFishCatch.Weight:F2} kg";
                LocationLabel.Text = _currentFishCatch.LocationName;
                DateLabel.Text = $"{_currentFishCatch.CatchDate:dd/MM/yyyy} a {_currentFishCatch.CatchTime:hh\\:mm}";

                if (_currentFishCatch.Latitude != 0 && _currentFishCatch.Longitude != 0)
                {
                    GPSLabel.Text = $"{_currentFishCatch.Latitude:F4}, {_currentFishCatch.Longitude:F4}";
                }
                else
                {
                    GPSLabel.Text = "Non disponible";
                }

                if (!string.IsNullOrWhiteSpace(_currentFishCatch.Notes))
                {
                    NotesLabel.Text = _currentFishCatch.Notes;
                    NotesSection.IsVisible = true;
                }
                else
                {
                    NotesSection.IsVisible = false;
                }

                if (!string.IsNullOrWhiteSpace(_currentFishCatch.PhotoPath))
                {
                    PhotoImage.Source = ImageSource.FromFile(_currentFishCatch.PhotoPath);
                    PhotoImage.IsVisible = true;
                    PhotoPlaceholder.IsVisible = false;
                }

                // Charger et afficher la météo si disponible
                LoadWeatherInfo();

                // Afficher le matériel utilisé
                // LoadMaterialInfo(); // Temporairement commenté jusqu'à la résolution des erreurs XAML
            }
        }

        private async void LoadWeatherInfo()
        {
            if (_currentFishCatch == null) return;

            var weather = await _weatherService.GetWeatherForCatchAsync(_currentFishCatch.Id);
            if (weather != null)
            {
                // Afficher les informations météo (vous pouvez créer des labels dans le XAML pour cela)
                System.Diagnostics.Debug.WriteLine($"Weather for catch: {weather.Temperature}°C, {weather.Description}");
            }
        }

        private void LoadMaterialInfo()
        {
            /* Temporairement commenté jusqu'à la résolution des erreurs XAML
            if (_currentFishCatch == null) return;

            bool hasMaterial = false;

            // Canne
            if (_currentFishCatch.CanneId.HasValue)
            {
                var canne = _materialService.GetMaterialById(_currentFishCatch.CanneId.Value);
                if (canne != null)
                {
                    CanneLabel.Text = $"{canne.Name} ({canne.Brand})";
                    CanneFrame.IsVisible = true;
                    hasMaterial = true;
                }
            }

            // Fil
            if (_currentFishCatch.FilId.HasValue)
            {
                var fil = _materialService.GetMaterialById(_currentFishCatch.FilId.Value);
                if (fil != null)
                {
                    FilLabel.Text = $"{fil.Name} ({fil.Brand})";
                    FilFrame.IsVisible = true;
                    hasMaterial = true;
                }
            }

            // Bas de ligne
            if (_currentFishCatch.BasDeLigneId.HasValue)
            {
                var basLigne = _materialService.GetMaterialById(_currentFishCatch.BasDeLigneId.Value);
                if (basLigne != null)
                {
                    BasDeLigneLabel.Text = $"{basLigne.Name} ({basLigne.Brand})";
                    BasDeLigneFrame.IsVisible = true;
                    hasMaterial = true;
                }
            }

            // Appât/Leurre
            if (_currentFishCatch.AppAtId.HasValue)
            {
                var appat = _materialService.GetMaterialById(_currentFishCatch.AppAtId.Value);
                if (appat != null)
                {
                    AppAtLabel.Text = $"{appat.Name} ({appat.Brand})";
                    AppAtFrame.IsVisible = true;
                    hasMaterial = true;
                }
            }

            // Afficher ou masquer la section matériel
            MaterialSection.IsVisible = hasMaterial;
            */
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (_currentFishCatch != null)
            {
                // Navigation vers la page de modification avec l'ID
                await Shell.Current.GoToAsync($"AddCatchPage?id={_currentFishCatch.Id}");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert(
                "Confirmer", 
                "Voulez-vous vraiment supprimer cette capture ?", 
                "Oui", "Non");

            if (answer && _currentFishCatch != null)
            {
                _databaseService.DeleteCatch(_currentFishCatch);
                await DisplayAlert("Supprime", "La capture a ete supprimee", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnShareClicked(object sender, EventArgs e)
        {
            if (_currentFishCatch != null)
            {
                var success = await _socialShareService.ShareCatchAsync(_currentFishCatch, _currentFishCatch.PhotoPath);
                if (!success)
                {
                    await DisplayAlert("Erreur", "Impossible de partager la capture", "OK");
                }
            }
        }
    }
}
