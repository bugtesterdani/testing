using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace tester
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Dictionary<int, Lines> _lines { get; set; } = [];
        public Dictionary<int, Lines> LineDic
        {
            get { return _lines; }
            set
            {
                _lines = value;
                OnPropertyChanged(nameof(Line));
            }
        }
        public List<Lines> Line
        {
            get
            {
                return [.. _lines.Values];
            }
        }

        public ObservableCollection<Teil> Teile { get; set; }

        public MainWindowViewModel()
        {
            Teile = new ObservableCollection<Teil>
        {
            new Teil { PositionX = 50, PositionY = 50, Breite = 100, Höhe = 50, Farbe = "Red", Knoten = new List<Knotenpunkt>
            {
                new Knotenpunkt() { PositionX = 0, PositionY = 12 },
                new Knotenpunkt() { PositionX = 0, PositionY = 28 }
            } },
            new Teil { PositionX = 200, PositionY = 100, Breite = 70, Höhe = 50, Farbe = "Blue", Knoten = new List<Knotenpunkt> {
                new Knotenpunkt() { PositionX = 60, PositionY = 12 },
                new Knotenpunkt() { PositionX = 60, PositionY = 28 }
            } }
        };
            _lines.Add(0, new Lines { StartPoint = new(10,10), EndPoint = new(100,100) });
        }
        public void UpdatePosition(Teil teil, double newX, double newY) { teil.PositionX = newX; teil.PositionY = newY; OnPropertyChanged(nameof(Teile)); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
