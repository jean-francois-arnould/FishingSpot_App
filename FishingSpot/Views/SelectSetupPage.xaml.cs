using FishingSpot.Models;
using FishingSpot.Services;

namespace FishingSpot.Views
{
    public partial class SelectSetupPage : ContentPage
    {
        private readonly SetupService _setupService;
        private readonly MaterialService _materialService;

        public SelectSetupPage(SetupService setupService, MaterialService materialService)
        {
            InitializeComponent();
            _setupService = setupService;
            _materialService = materialService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSetups();
        }

        private void LoadSetups()
        {
            var setups = _setupService.GetAllSetups();

            SetupsContainer.Children.Clear();

            foreach (var setup in setups)
            {
                var setupSection = CreateSetupSection(setup);
                SetupsContainer.Children.Add(setupSection);
            }
        }

        private Frame CreateSetupSection(FishingSetup setup)
        {
            var mainFrame = new Frame
            {
                Padding = 0,
                CornerRadius = 15,
                HasShadow = true,
                BackgroundColor = Colors.White,
                BorderColor = setup.IsActive ? Color.FromArgb("#4CAF50") : Color.FromArgb("#E0E0E0")
            };

            var mainGrid = new Grid
            {
                Padding = 15,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };

            // En-tête avec nom et badge
            var headerGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var nameLabel = new Label
            {
                Text = setup.Name,
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2C3E50"),
                VerticalOptions = LayoutOptions.Center
            };
            headerGrid.Add(nameLabel, 0, 0);

            if (setup.IsActive)
            {
                var badgeFrame = new Frame
                {
                    Padding = new Thickness(12, 6),
                    CornerRadius = 12,
                    BackgroundColor = Color.FromArgb("#4CAF50"),
                    HasShadow = false
                };

                var badgeLabel = new Label
                {
                    Text = "✓ ACTIF",
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White
                };

                badgeFrame.Content = badgeLabel;
                headerGrid.Add(badgeFrame, 1, 0);
            }

            mainGrid.Add(headerGrid, 0, 0);

            // Description
            if (!string.IsNullOrWhiteSpace(setup.Description))
            {
                var descLabel = new Label
                {
                    Text = setup.Description,
                    FontSize = 14,
                    TextColor = Colors.Gray,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                mainGrid.Add(descLabel, 0, 1);
            }

            // Composition du setup
            var compositionStack = new VerticalStackLayout
            {
                Spacing = 8,
                Margin = new Thickness(0, 15, 0, 0)
            };

            // Canne
            var canneName = GetMaterialName(setup.CanneId);
            if (!string.IsNullOrEmpty(canneName))
            {
                compositionStack.Children.Add(CreateMaterialItem("🎣", canneName, "#FFF3E0", "#E65100"));
            }

            // Fil
            var filName = GetMaterialName(setup.FilId);
            if (!string.IsNullOrEmpty(filName))
            {
                compositionStack.Children.Add(CreateMaterialItem("🧵", filName, "#E8F5E9", "#2E7D32"));
            }

            // Bas de ligne
            var basLigneName = GetMaterialName(setup.BasDeLigneId);
            if (!string.IsNullOrEmpty(basLigneName))
            {
                compositionStack.Children.Add(CreateMaterialItem("🔗", basLigneName, "#E3F2FD", "#1976D2"));
            }

            // Appât
            var appatName = GetMaterialName(setup.AppAtId);
            if (!string.IsNullOrEmpty(appatName))
            {
                compositionStack.Children.Add(CreateMaterialItem("🐟", appatName, "#F3E5F5", "#7B1FA2"));
            }

            mainGrid.Add(compositionStack, 0, 2);

            // Bouton Choisir
            if (!setup.IsActive)
            {
                var chooseButton = new Button
                {
                    Text = "Choisir ce setup",
                    BackgroundColor = Color.FromArgb("#4CAF50"),
                    TextColor = Colors.White,
                    CornerRadius = 10,
                    Margin = new Thickness(0, 15, 0, 0),
                    HeightRequest = 45,
                    FontAttributes = FontAttributes.Bold
                };

                chooseButton.Clicked += async (s, e) =>
                {
                    bool confirm = await DisplayAlert(
                        "Changer de setup",
                        $"Voulez-vous activer le setup '{setup.Name}' ?",
                        "Oui",
                        "Non");

                    if (confirm)
                    {
                        _setupService.SetActiveSetup(setup.Id);
                        await DisplayAlert("Succès", $"Le setup '{setup.Name}' est maintenant actif", "OK");
                        LoadSetups(); // Rafraîchir l'affichage
                    }
                };

                mainGrid.Add(chooseButton, 0, 3);
            }
            else
            {
                var activeLabel = new Label
                {
                    Text = "Ce setup est actuellement actif",
                    FontSize = 13,
                    TextColor = Color.FromArgb("#4CAF50"),
                    FontAttributes = FontAttributes.Italic,
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 15, 0, 0)
                };
                mainGrid.Add(activeLabel, 0, 3);
            }

            mainFrame.Content = mainGrid;
            return mainFrame;
        }

        private Frame CreateMaterialItem(string emoji, string name, string bgColor, string textColor)
        {
            var frame = new Frame
            {
                Padding = new Thickness(12, 10),
                CornerRadius = 8,
                BackgroundColor = Color.FromArgb(bgColor),
                HasShadow = false
            };

            var stack = new HorizontalStackLayout
            {
                Spacing = 10
            };

            var emojiLabel = new Label
            {
                Text = emoji,
                FontSize = 18,
                VerticalOptions = LayoutOptions.Center
            };

            var nameLabel = new Label
            {
                Text = name,
                FontSize = 14,
                TextColor = Color.FromArgb(textColor),
                VerticalOptions = LayoutOptions.Center
            };

            stack.Children.Add(emojiLabel);
            stack.Children.Add(nameLabel);
            frame.Content = stack;

            return frame;
        }

        private string? GetMaterialName(int? materialId)
        {
            if (!materialId.HasValue) return null;

            var material = _materialService.GetMaterialById(materialId.Value);
            return material != null ? $"{material.Name} ({material.Brand})" : null;
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnCreateNewSetupClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddSetupPage");
        }
    }
}
