using FishingSpot.Views;

namespace FishingSpot
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Enregistrement des routes de navigation
            Routing.RegisterRoute("AddCatchPage", typeof(AddCatchPage));
            Routing.RegisterRoute("FishCardDetailPage", typeof(FishCardDetailPage));
            Routing.RegisterRoute("AddMaterialPage", typeof(AddMaterialPage));
            Routing.RegisterRoute("AddSetupPage", typeof(AddSetupPage));
            Routing.RegisterRoute("SelectSetupPage", typeof(SelectSetupPage));
        }
    }
}
