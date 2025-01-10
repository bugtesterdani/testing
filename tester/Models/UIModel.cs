using System.ComponentModel;

namespace tester.Models
{
    public class UIElementModel : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private string _elementType;
        private string _contents;
        private List<string> _contentsList;
        public double x
        {
            get => _x;
            set { if (_x != value) { _x = value; OnPropertyChanged(nameof(x)); } }
        }
        public double y
        {
            get => _y;
            set { if (_y != value) { _y = value; OnPropertyChanged(nameof(y)); } }
        }
        public double width
        {
            get => _width;
            set { if (_width != value) { _width = value; OnPropertyChanged(nameof(width)); } }
        }
        public double height
        {
            get => _height;
            set { if (_height != value) { _height = value; OnPropertyChanged(nameof(height)); } }
        }
        public string ElementType
        {
            get => _elementType;
            set { if (_elementType != value) { _elementType = value; OnPropertyChanged(nameof(ElementType)); } }
        }
        public string Contents
        {
            get => _contents;
            set { if (_contents != value) { _contents = value; OnPropertyChanged(nameof(Contents)); } }
        }
        public List<string> ContentsList
        {
            get => _contentsList;
            set { if (_contentsList != value) { _contentsList = value; OnPropertyChanged(nameof(ContentsList)); } }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
