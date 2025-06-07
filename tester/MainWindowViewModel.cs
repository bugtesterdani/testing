using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using tester.Models;

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
        public List<Lines> Line => [.. _lines.Values];

        public ObservableCollection<Teil> Teile { get; set; } = [];
        public ObservableCollection<ButtonItem> Buttons { get; set; }

        public MainWindowViewModel()
        {
            Buttons =
            [
                new()
                {
                    Name = "Texteingabe",
                    Command = new RelayCommand<string>(param => OnButtonClick("0"))
                },
                new()
                {
                    Name = "Klone",
                    Command = new RelayCommand<string>(param => OnButtonClick("1"))
                }
            ];
            Teile.Add(new()
            {
                PositionX = 100,
                PositionY = 100,
                Breite = 50,
                Höhe = 40,
                Farbe = "Yellow",
                Knoten =
                [
                    new() { PositionX = 40, PositionY = 15, selectedpos = Positionselected.End }
                ]
            });
            ParseJsonMain();
        }

        private void ParseJsonMain()
        {
            string json = File.ReadAllText("Softwares.json");
            List<SoftwaresModel>? softwares = JsonConvert.DeserializeObject<List<SoftwaresModel>>(json);
            if (softwares == null)
                return;

            foreach (var soft in softwares)
                foreach (var sprache in soft.inhalt)
                {
                    string json2 = File.ReadAllText(soft.sprache + "//" + sprache.path + "//" + sprache.name + ".json");
                    ProgModel? progs = JsonConvert.DeserializeObject<ProgModel>(json2);
                    if (progs == null)
                        continue;

                    Buttons.Add(new()
                    {
                        Name = sprache.path + "-" + sprache.name,
                        Command = new RelayCommand<string>(param =>
                        {
                            List<Knotenpunkt> k = [];
                            foreach (var knot in progs.inputs)
                                k.Add(new() { PositionX = knot.position.x, PositionY = knot.position.y, selectedpos = Positionselected.Start });
                            foreach (var knot in progs.output)
                                k.Add(new() { PositionX = knot.position.x, PositionY = knot.position.y, selectedpos = Positionselected.End });
                            k.Add(new() { PositionX = (progs.width / 2) - 10, PositionY = 0, selectedpos = Positionselected.Start });
                            Teile.Add(new()
                            {
                                Breite = progs.width,
                                Höhe = progs.height,
                                Farbe = progs.color,
                                PositionX = 50,
                                PositionY = 50,
                                Knoten = k
                            });
                        })
                    });
                }
        }

        private void OnButtonClick(string buttonLabel)
        {
            switch (buttonLabel)
            {
                case "0":
                    Teile.Add(new()
                    {
                        PositionX = 100,
                        PositionY = 100,
                        Breite = 100,
                        Höhe = 40,
                        Farbe = "Yellow",
                        Knoten =
                        [
                            new() { PositionX = 90, PositionY = 15, selectedpos = Positionselected.End }
                        ]
                    });
                    break;
                case "1":
                    Teile.Add(new()
                    {
                        PositionX = 100,
                        PositionY = 100,
                        Breite = 100,
                        Höhe = 60,
                        Farbe = "Yellow",
                        Knoten =
                        [
                            new() { PositionX = 0, PositionY = 25, selectedpos = Positionselected.Start },
                            new() { PositionX = 90, PositionY = 15, selectedpos = Positionselected.End },
                            new() { PositionX = 90, PositionY = 35, selectedpos = Positionselected.End }
                        ]
                    });
                    break;
                default:
                    break;
            }
        }

        public void UpdatePosition(Teil teil, double newX, double newY)
        {
            teil.PositionX = newX;
            teil.PositionY = newY;
            OnPropertyChanged(nameof(Teile));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
