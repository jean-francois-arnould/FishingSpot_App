using FishingSpot.Services;
using FishingSpot.Models;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace FishingSpot.Views
{
    public partial class MyCatchesPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<FishCatch> _allCatches;
        private ObservableCollection<FishCatch> _filteredCatches;
        private string _searchText = string.Empty;
        private double _minLength = 0;
        private double _maxLength = 1000;
        private double _minWeight = 0;
        private double _maxWeight = 1000;

        public ICommand DeleteCommand { get; }

        public MyCatchesPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;

            DeleteCommand = new Command<FishCatch>(OnDeleteCatch);
            BindingContext = this;

            _allCatches = new ObservableCollection<FishCatch>();
            _filteredCatches = new ObservableCollection<FishCatch>();

            SortPicker.SelectedIndex = 0; // Par défaut : Date (récent)
            LoadCatches();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCatches();
        }

        private void LoadCatches()
        {
            _allCatches.Clear();
            var catches = _databaseService.GetAllCatches();
            foreach (var catchItem in catches)
            {
                _allCatches.Add(catchItem);
            }
            ApplyFiltersAndSort();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = e.NewTextValue ?? string.Empty;
            ApplyFiltersAndSort();
        }

        private void OnSortChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private async void OnFiltersClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet(
                "Filtres avancés", 
                "Annuler", 
                null, 
                "Filtrer par taille", 
                "Filtrer par poids",
                "Réinitialiser les filtres");

            switch (action)
            {
                case "Filtrer par taille":
                    await ShowLengthFilter();
                    break;
                case "Filtrer par poids":
                    await ShowWeightFilter();
                    break;
                case "Réinitialiser les filtres":
                    ResetFilters();
                    break;
            }
        }

        private async Task ShowLengthFilter()
        {
            string result = await DisplayPromptAsync(
                "Filtrer par taille",
                "Entrez la taille minimale (en cm):",
                placeholder: "ex: 20",
                keyboard: Keyboard.Numeric);

            if (!string.IsNullOrEmpty(result) && double.TryParse(result, out double minLength))
            {
                _minLength = minLength;

                result = await DisplayPromptAsync(
                    "Filtrer par taille",
                    "Entrez la taille maximale (en cm):",
                    placeholder: "ex: 100",
                    keyboard: Keyboard.Numeric);

                if (!string.IsNullOrEmpty(result) && double.TryParse(result, out double maxLength))
                {
                    _maxLength = maxLength;
                    ApplyFiltersAndSort();
                    await DisplayAlert("Filtre appliqué", $"Affichage des poissons entre {_minLength} et {_maxLength} cm", "OK");
                }
            }
        }

        private async Task ShowWeightFilter()
        {
            string result = await DisplayPromptAsync(
                "Filtrer par poids",
                "Entrez le poids minimum (en kg):",
                placeholder: "ex: 0.5",
                keyboard: Keyboard.Numeric);

            if (!string.IsNullOrEmpty(result) && double.TryParse(result, out double minWeight))
            {
                _minWeight = minWeight;

                result = await DisplayPromptAsync(
                    "Filtrer par poids",
                    "Entrez le poids maximum (en kg):",
                    placeholder: "ex: 10",
                    keyboard: Keyboard.Numeric);

                if (!string.IsNullOrEmpty(result) && double.TryParse(result, out double maxWeight))
                {
                    _maxWeight = maxWeight;
                    ApplyFiltersAndSort();
                    await DisplayAlert("Filtre appliqué", $"Affichage des poissons entre {_minWeight} et {_maxWeight} kg", "OK");
                }
            }
        }

        private void ResetFilters()
        {
            _minLength = 0;
            _maxLength = 1000;
            _minWeight = 0;
            _maxWeight = 1000;
            _searchText = string.Empty;
            SearchBar.Text = string.Empty;
            ApplyFiltersAndSort();
        }

        private void ApplyFiltersAndSort()
        {
            // Filtrage
            var filtered = _allCatches.AsEnumerable();

            // Filtre par recherche de nom
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                filtered = filtered.Where(c => 
                    c.FishName.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
            }

            // Filtre par taille
            filtered = filtered.Where(c => c.Length >= _minLength && c.Length <= _maxLength);

            // Filtre par poids
            filtered = filtered.Where(c => c.Weight >= _minWeight && c.Weight <= _maxWeight);

            // Tri selon la sélection
            var sorted = SortPicker.SelectedIndex switch
            {
                0 => filtered.OrderByDescending(c => c.CatchDate).ThenByDescending(c => c.CatchTime), // Date (récent)
                1 => filtered.OrderBy(c => c.CatchDate).ThenBy(c => c.CatchTime), // Date (ancien)
                2 => filtered.OrderBy(c => c.FishName), // Nom (A-Z)
                3 => filtered.OrderByDescending(c => c.FishName), // Nom (Z-A)
                4 => filtered.OrderBy(c => c.Length), // Taille (croissant)
                5 => filtered.OrderByDescending(c => c.Length), // Taille (décroissant)
                6 => filtered.OrderBy(c => c.Weight), // Poids (croissant)
                7 => filtered.OrderByDescending(c => c.Weight), // Poids (décroissant)
                _ => filtered.OrderByDescending(c => c.CatchDate).ThenByDescending(c => c.CatchTime)
            };

            _filteredCatches.Clear();
            foreach (var item in sorted)
            {
                _filteredCatches.Add(item);
            }

            CatchesCollectionView.ItemsSource = _filteredCatches;
        }

        private void OnDeleteCatch(FishCatch fishCatch)
        {
            _databaseService.DeleteCatch(fishCatch);
            _allCatches.Remove(fishCatch);
            ApplyFiltersAndSort();
        }

        private async void OnCatchSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is FishCatch selectedCatch)
            {
                // Désélectionner immédiatement
                ((CollectionView)sender).SelectedItem = null;

                // Naviguer vers la page de détail
                await Shell.Current.GoToAsync($"FishCardDetailPage?id={selectedCatch.Id}");
            }
        }

        private async void OnAddCatchClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddCatchPage");
        }

        private async void OnDeleteSwipe(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is FishCatch fishCatch)
            {
                bool confirm = await DisplayAlert(
                    "Confirmer",
                    $"Voulez-vous vraiment supprimer {fishCatch.FishName} ?",
                    "Oui",
                    "Non");

                if (confirm)
                {
                    _databaseService.DeleteCatch(fishCatch);
                    LoadCatches();
                }
            }
        }
    }
}
