using FishingSpot.Models;
using FishingSpot.Services;

namespace FishingSpot.Views
{
    [QueryProperty(nameof(SetupId), "id")]
    public partial class AddSetupPage : ContentPage
    {
        private readonly SetupService _setupService;
        private readonly MaterialService _materialService;
        private int _setupId = 0;
        private FishingSetup? _existingSetup;

        public int SetupId
        {
            get => _setupId;
            set
            {
                _setupId = value;
                if (_setupId > 0)
                {
                    LoadExistingSetup();
                }
            }
        }

        public AddSetupPage(SetupService setupService, MaterialService materialService)
        {
            InitializeComponent();
            _setupService = setupService;
            _materialService = materialService;

            LoadMaterialPickers();
        }

        private void LoadMaterialPickers()
        {
            // Charger les cannes
            var cannes = _materialService.GetMaterialsByType(MaterialType.Canne);
            var cannesList = new List<MaterialPickerItem> { new MaterialPickerItem { Id = null, Name = "Aucune" } };
            cannesList.AddRange(cannes.Select(m => new MaterialPickerItem { Id = m.Id, Name = m.Name }));
            CannePicker.ItemsSource = cannesList;
            CannePicker.ItemDisplayBinding = new Binding("Name");
            CannePicker.SelectedIndex = 0;

            // Charger les fils
            var fils = _materialService.GetMaterialsByType(MaterialType.Fil);
            var filsList = new List<MaterialPickerItem> { new MaterialPickerItem { Id = null, Name = "Aucun" } };
            filsList.AddRange(fils.Select(m => new MaterialPickerItem { Id = m.Id, Name = m.Name }));
            FilPicker.ItemsSource = filsList;
            FilPicker.ItemDisplayBinding = new Binding("Name");
            FilPicker.SelectedIndex = 0;

            // Charger les bas de ligne
            var basLignes = _materialService.GetMaterialsByType(MaterialType.BasDeLigne);
            var basLignesList = new List<MaterialPickerItem> { new MaterialPickerItem { Id = null, Name = "Aucun" } };
            basLignesList.AddRange(basLignes.Select(m => new MaterialPickerItem { Id = m.Id, Name = m.Name }));
            BasDeLignePicker.ItemsSource = basLignesList;
            BasDeLignePicker.ItemDisplayBinding = new Binding("Name");
            BasDeLignePicker.SelectedIndex = 0;

            // Charger les appâts/leurres
            var appats = _materialService.GetMaterialsByType(MaterialType.AppAtOuLeurre);
            var appatsList = new List<MaterialPickerItem> { new MaterialPickerItem { Id = null, Name = "Aucun" } };
            appatsList.AddRange(appats.Select(m => new MaterialPickerItem { Id = m.Id, Name = m.Name }));
            AppAtPicker.ItemsSource = appatsList;
            AppAtPicker.ItemDisplayBinding = new Binding("Name");
            AppAtPicker.SelectedIndex = 0;
        }

        private class MaterialPickerItem
        {
            public int? Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private void LoadExistingSetup()
        {
            _existingSetup = _setupService.GetSetupById(_setupId);
            if (_existingSetup != null)
            {
                Title = "Modifier le Setup";
                NameEntry.Text = _existingSetup.Name;
                DescriptionEditor.Text = _existingSetup.Description;
                NotesEditor.Text = _existingSetup.Notes;
                IsActiveCheckBox.IsChecked = _existingSetup.IsActive;

                // Sélectionner les matériels
                if (_existingSetup.CanneId.HasValue)
                {
                    var cannesList = (List<MaterialPickerItem>)CannePicker.ItemsSource;
                    var canneItem = cannesList.FirstOrDefault(m => m.Id == _existingSetup.CanneId);
                    if (canneItem != null) CannePicker.SelectedItem = canneItem;
                }

                if (_existingSetup.FilId.HasValue)
                {
                    var filsList = (List<MaterialPickerItem>)FilPicker.ItemsSource;
                    var filItem = filsList.FirstOrDefault(m => m.Id == _existingSetup.FilId);
                    if (filItem != null) FilPicker.SelectedItem = filItem;
                }

                if (_existingSetup.BasDeLigneId.HasValue)
                {
                    var basLignesList = (List<MaterialPickerItem>)BasDeLignePicker.ItemsSource;
                    var basLigneItem = basLignesList.FirstOrDefault(m => m.Id == _existingSetup.BasDeLigneId);
                    if (basLigneItem != null) BasDeLignePicker.SelectedItem = basLigneItem;
                }

                if (_existingSetup.AppAtId.HasValue)
                {
                    var appatsList = (List<MaterialPickerItem>)AppAtPicker.ItemsSource;
                    var appatItem = appatsList.FirstOrDefault(m => m.Id == _existingSetup.AppAtId);
                    if (appatItem != null) AppAtPicker.SelectedItem = appatItem;
                }
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                await DisplayAlert("Erreur", "Veuillez entrer un nom pour le setup", "OK");
                return;
            }

            // Récupérer les IDs du matériel sélectionné
            int? canneId = (CannePicker.SelectedItem as MaterialPickerItem)?.Id;
            int? filId = (FilPicker.SelectedItem as MaterialPickerItem)?.Id;
            int? basLigneId = (BasDeLignePicker.SelectedItem as MaterialPickerItem)?.Id;
            int? appatId = (AppAtPicker.SelectedItem as MaterialPickerItem)?.Id;

            if (_existingSetup != null)
            {
                // Mode modification
                _existingSetup.Name = NameEntry.Text;
                _existingSetup.Description = DescriptionEditor.Text ?? string.Empty;
                _existingSetup.CanneId = canneId;
                _existingSetup.FilId = filId;
                _existingSetup.BasDeLigneId = basLigneId;
                _existingSetup.AppAtId = appatId;
                _existingSetup.Notes = NotesEditor.Text ?? string.Empty;
                _existingSetup.IsActive = IsActiveCheckBox.IsChecked;

                _setupService.UpdateSetup(_existingSetup);

                if (IsActiveCheckBox.IsChecked)
                {
                    _setupService.SetActiveSetup(_existingSetup.Id);
                }

                await DisplayAlert("Succès", "Setup modifié avec succès !", "OK");
            }
            else
            {
                // Mode création
                var setup = new FishingSetup
                {
                    Name = NameEntry.Text,
                    Description = DescriptionEditor.Text ?? string.Empty,
                    CanneId = canneId,
                    FilId = filId,
                    BasDeLigneId = basLigneId,
                    AppAtId = appatId,
                    CreatedDate = DateTime.Now,
                    Notes = NotesEditor.Text ?? string.Empty,
                    IsActive = IsActiveCheckBox.IsChecked
                };

                _setupService.AddSetup(setup);

                if (IsActiveCheckBox.IsChecked)
                {
                    _setupService.SetActiveSetup(setup.Id);
                }

                await DisplayAlert("Succès", "Setup créé avec succès !", "OK");
            }

            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
