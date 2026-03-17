using FishingSpot.Models;
using FishingSpot.Services;

namespace FishingSpot.Views
{
    public partial class MaterialManagementPage : ContentPage
    {
        private readonly MaterialService _materialService;
        private readonly SetupService _setupService;

        public MaterialManagementPage(MaterialService materialService, SetupService setupService)
        {
            InitializeComponent();
            _materialService = materialService;
            _setupService = setupService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadAllData();
        }

        private void LoadAllData()
        {
            // Charger le setup actuel
            var activeSetup = _setupService.GetActiveSetup();
            if (activeSetup != null)
            {
                ActiveSetupName.Text = activeSetup.Name;
                ActiveSetupSummary.Text = _setupService.GetSetupSummary(activeSetup);
            }
            else
            {
                ActiveSetupName.Text = "Aucun setup actif";
                ActiveSetupSummary.Text = "Créez un setup pour commencer";
            }

            // Charger tous les setups
            SetupsCollectionView.ItemsSource = _setupService.GetAllSetups();

            // Charger le matériel par type
            CannesCollectionView.ItemsSource = _materialService.GetMaterialsByType(MaterialType.Canne);
            FilsCollectionView.ItemsSource = _materialService.GetMaterialsByType(MaterialType.Fil);
            BasDeLigneCollectionView.ItemsSource = _materialService.GetMaterialsByType(MaterialType.BasDeLigne);
            AppatsCollectionView.ItemsSource = _materialService.GetMaterialsByType(MaterialType.AppAtOuLeurre);
        }

        private async void OnAddSetupClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddSetupPage");
        }

        private async void OnEditActiveSetupClicked(object sender, EventArgs e)
        {
            var activeSetup = _setupService.GetActiveSetup();
            if (activeSetup != null)
            {
                await Shell.Current.GoToAsync($"AddSetupPage?id={activeSetup.Id}");
            }
        }

        private async void OnChangeSetupClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SelectSetupPage");
        }

        private async void OnSetupTapped(object sender, TappedEventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is FishingSetup setup)
            {
                await Shell.Current.GoToAsync($"AddSetupPage?id={setup.Id}");
            }
        }

        private async void OnAddCanneClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddMaterialPage?type=0"); // Canne = 0
        }

        private async void OnAddFilClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddMaterialPage?type=1"); // Fil = 1
        }

        private async void OnAddBasDeLigneClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddMaterialPage?type=2"); // BasDeLigne = 2
        }

        private async void OnAddAppAtClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddMaterialPage?type=3"); // AppAtOuLeurre = 3
        }

        private async void OnMaterialTapped(object sender, TappedEventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is FishingMaterial material)
            {
                await Shell.Current.GoToAsync($"AddMaterialPage?id={material.Id}");
            }
        }

        private void OnCannesHeaderTapped(object sender, EventArgs e)
        {
            CannesCollectionView.IsVisible = !CannesCollectionView.IsVisible;
        }

        private void OnSetupsHeaderTapped(object sender, EventArgs e)
        {
            SetupsCollectionView.IsVisible = !SetupsCollectionView.IsVisible;
        }

        private void OnFilsHeaderTapped(object sender, EventArgs e)
        {
            FilsCollectionView.IsVisible = !FilsCollectionView.IsVisible;
        }

        private void OnBasDeLigneHeaderTapped(object sender, EventArgs e)
        {
            BasDeLigneCollectionView.IsVisible = !BasDeLigneCollectionView.IsVisible;
        }

        private void OnAppatsHeaderTapped(object sender, EventArgs e)
        {
            AppatsCollectionView.IsVisible = !AppatsCollectionView.IsVisible;
        }
    }
}
