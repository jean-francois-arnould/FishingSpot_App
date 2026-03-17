using Microsoft.Extensions.DependencyInjection;

namespace FishingSpot
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(MainPage!);
        }
    }
}