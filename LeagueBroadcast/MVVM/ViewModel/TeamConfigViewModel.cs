using LeagueBroadcast.ChampSelect.Data.Config;
using LeagueBroadcast.MVVM.Core;
using System.Windows.Media;

namespace LeagueBroadcast.MVVM.ViewModel
{
    public class TeamConfigViewModel : ObservableObject
    {
        public TeamConfig ConfigReference;

        private string _name;
        private int _score;
        private string _coach;
        private Color _color;
        private SolidColorBrush _colorBrush;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public int Score { get { return _score; } set { _score = value; OnPropertyChanged(); } }
        public string Coach { get { return _coach; } set { _coach = value; OnPropertyChanged(); } }
        public Color Color { get { return _color; } set { _color = value; OnPropertyChanged(); UpdateColorConfig(); } }
        public SolidColorBrush ColorBrush { get { return _colorBrush; } set { _colorBrush = value; OnPropertyChanged(); } }

        public static TeamConfigViewModel BlueTeam = new ();
        public static TeamConfigViewModel RedTeam = new ();

        public TeamConfigViewModel(TeamConfig config)
        {
            this.ConfigReference = config;
        }

        public TeamConfigViewModel()
        {

        }

        public void Init(TeamConfig config)
        {
            this.ConfigReference = config;
            this.Name = ConfigReference.name;
            this.Score = ConfigReference.score;
            this.Coach = ConfigReference.coach;
            var cleanedColors = ConfigReference.color.Replace("rgb(", "").Replace(")", "").Split(",");
            this.Color = Color.FromRgb(
                byte.Parse(cleanedColors[0]), 
                byte.Parse(cleanedColors[1]), 
                byte.Parse(cleanedColors[2])
                );
        }

        private void UpdateColorConfig()
        {
            ConfigReference.color = $"rgb({Color.R},{Color.G},{Color.B})";
            ColorBrush = new SolidColorBrush(Color);
        }
    }
}
