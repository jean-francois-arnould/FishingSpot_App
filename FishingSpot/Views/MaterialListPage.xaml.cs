using FishingSpot.Models;
using FishingSpot.Services;

namespace FishingSpot.Views
{
    public partial class MaterialListPage : ContentPage
    {
        private readonly MaterialService _materialService;

        public MaterialListPage(MaterialService materialService)
        {
            InitializeComponent();
            _materialService = materialService;
            LoadMaterials();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            MaterialCollectionView.ItemsSource = _materialService.GetAllMaterials();
        }

        private void OnFilterAll(object sender, EventArgs e)
        {
            MaterialCollectionView.ItemsSource = _materialService.GetAllMaterials();
        }

        private void OnFilterCanne(object sender, EventArgs e)
        {
            MaterialCollectionView.ItemsSource = _materialService.GetMaterialsByType(MaterialType.Canne);
        }

        private void OnFilterFil(object sender, EventArgs e)
        {
            MaterialCollectionView.ItemsSource = _materialService.GetMaterialsByType(MaterialType.Fil);
        }

        private void OnFilterBasDeLigne(object sender, EventArgs e)
        {
            MaterialCollectionView.ItemsSource = _materialService.GetMaterialsByType(MaterialType.BasDeLigne);
        }

        private void OnFilterAppAt(object sender, EventArgs e)
        {
            MaterialCollectionView.ItemsSource = _materialService.GetMaterialsByType(MaterialType.AppAtOuLeurre);
        }

        private async void OnMaterialSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is FishingMaterial selectedMaterial)
            {
                ((CollectionView)sender).SelectedItem = null;
                await Shell.Current.GoToAsync($"AddMaterialPage?id={selectedMaterial.Id}");
            }
        }

        private async void OnAddMaterialClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddMaterialPage");
        }
    }
}
