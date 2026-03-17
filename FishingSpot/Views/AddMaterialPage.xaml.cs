using FishingSpot.Models;
using FishingSpot.Services;

namespace FishingSpot.Views
{
    [QueryProperty(nameof(MaterialId), "id")]
    public partial class AddMaterialPage : ContentPage
    {
        private readonly MaterialService _materialService;
        private int _materialId = 0;
        private FishingMaterial? _existingMaterial;

        public int MaterialId
        {
            get => _materialId;
            set
            {
                _materialId = value;
                if (_materialId > 0)
                {
                    LoadExistingMaterial();
                }
            }
        }

        public AddMaterialPage(MaterialService materialService)
        {
            InitializeComponent();
            _materialService = materialService;

            PurchaseDatePicker.Date = DateTime.Now;
        }

        private void LoadExistingMaterial()
        {
            _existingMaterial = _materialService.GetMaterialById(_materialId);
            if (_existingMaterial != null)
            {
                Title = "Modifier le Materiel";

                TypePicker.SelectedIndex = (int)_existingMaterial.Type;
                NameEntry.Text = _existingMaterial.Name;
                BrandEntry.Text = _existingMaterial.Brand;
                DescriptionEditor.Text = _existingMaterial.Description;
                PurchaseDatePicker.Date = _existingMaterial.PurchaseDate;
                NotesEditor.Text = _existingMaterial.Notes;

                // Charger les champs spécifiques
                LengthEntry.Text = _existingMaterial.Length;
                StrengthEntry.Text = _existingMaterial.Strength;
                StrengthEntry2.Text = _existingMaterial.Strength;
                ColorEntry.Text = _existingMaterial.Color;
            }
        }

        private void OnTypeChanged(object sender, EventArgs e)
        {
            // Masquer tous les champs spécifiques
            CanneFields.IsVisible = false;
            FilFields.IsVisible = false;
            BasDeLigneFields.IsVisible = false;
            AppAtFields.IsVisible = false;

            // Afficher les champs selon le type sélectionné
            switch (TypePicker.SelectedIndex)
            {
                case 0: // Canne
                    CanneFields.IsVisible = true;
                    break;
                case 1: // Fil
                    FilFields.IsVisible = true;
                    break;
                case 2: // Bas de ligne
                    BasDeLigneFields.IsVisible = true;
                    break;
                case 3: // Appât ou leurre
                    AppAtFields.IsVisible = true;
                    break;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                await DisplayAlert("Erreur", "Veuillez entrer un nom", "OK");
                return;
            }

            if (TypePicker.SelectedIndex < 0)
            {
                await DisplayAlert("Erreur", "Veuillez choisir un type", "OK");
                return;
            }

            var material = _existingMaterial ?? new FishingMaterial();

            material.Type = (MaterialType)TypePicker.SelectedIndex;
            material.Name = NameEntry.Text;
            material.Brand = BrandEntry.Text ?? string.Empty;
            material.Description = DescriptionEditor.Text ?? string.Empty;
            material.PurchaseDate = PurchaseDatePicker.Date ?? DateTime.Now;
            material.Notes = NotesEditor.Text ?? string.Empty;

            // Champs spécifiques selon le type
            material.Length = TypePicker.SelectedIndex == 0 ? LengthEntry.Text ?? string.Empty : string.Empty;
            material.Strength = TypePicker.SelectedIndex == 1 ? StrengthEntry.Text ?? string.Empty :
                               TypePicker.SelectedIndex == 2 ? StrengthEntry2.Text ?? string.Empty : string.Empty;
            material.Color = TypePicker.SelectedIndex == 3 ? ColorEntry.Text ?? string.Empty : string.Empty;

            if (_existingMaterial != null)
            {
                _materialService.UpdateMaterial(material);
                await DisplayAlert("Succes", "Materiel modifie !", "OK");
            }
            else
            {
                _materialService.AddMaterial(material);
                await DisplayAlert("Succes", "Materiel ajoute !", "OK");
            }

            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
