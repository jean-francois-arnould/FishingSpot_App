using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FishingSpot.Models
{
    [Table("FishingSetups")]
    public class FishingSetup : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private int? _canneId;
        private int? _filId;
        private int? _basDeLigneId;
        private int? _appAtId;
        private DateTime _createdDate;
        private string _notes = string.Empty;
        private bool _isActive;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        // IDs du matériel composant le setup
        public int? CanneId
        {
            get => _canneId;
            set => SetProperty(ref _canneId, value);
        }

        public int? FilId
        {
            get => _filId;
            set => SetProperty(ref _filId, value);
        }

        public int? BasDeLigneId
        {
            get => _basDeLigneId;
            set => SetProperty(ref _basDeLigneId, value);
        }

        public int? AppAtId
        {
            get => _appAtId;
            set => SetProperty(ref _appAtId, value);
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        // Indicateur si c'est le setup actuel
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
