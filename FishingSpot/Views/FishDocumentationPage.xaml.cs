using FishingSpot.Services;

namespace FishingSpot.Views
{
    public partial class FishDocumentationPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public FishDocumentationPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            LoadFishSpecies();
        }

        private void LoadFishSpecies()
        {
            FishCollectionView.ItemsSource = _databaseService.GetAllFishSpecies();
        }
    }
}
