using System.ComponentModel;

namespace tester
{
    public class Teil : INotifyPropertyChanged
    {
        private double _positionX;
        private double _positionY;
        private double _breite;
        private double _höhe;
        private string _farbe;
        private List<Knotenpunkt> _knoten;

        public double PositionX
        {
            get => _positionX;
            set { _positionX = value; OnPropertyChanged(nameof(PositionX)); }
        }

        public double PositionY
        {
            get => _positionY;
            set { _positionY = value; OnPropertyChanged(nameof(PositionY)); }
        }

        public double Breite
        {
            get => _breite;
            set { _breite = value; OnPropertyChanged(nameof(Breite)); }
        }

        public double Höhe
        {
            get => _höhe;
            set { _höhe = value; OnPropertyChanged(nameof(Höhe)); }
        }

        public string Farbe
        {
            get => _farbe;
            set { _farbe = value; OnPropertyChanged(nameof(Farbe)); }
        }

        public List<Knotenpunkt> Knoten
        {
            get => _knoten;
            set { _knoten = value; OnPropertyChanged(nameof(Knoten)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Knotenpunkt
    {
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public bool IsConnected { get; set; } = false;
        public Knotenpunkt ConnectedKnoten { get; set; }

        public void Dock(Knotenpunkt otherKnoten)
        {
            if (!IsConnected && !otherKnoten.IsConnected)
            {
                PositionX = otherKnoten.PositionX;
                PositionY = otherKnoten.PositionY;
                IsConnected = true;
                otherKnoten.IsConnected = true;
                ConnectedKnoten = otherKnoten;
                otherKnoten.ConnectedKnoten = this;
            }
        }
    }
}
