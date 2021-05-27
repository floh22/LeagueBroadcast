using LeagueBroadcast.ChampSelect.Data.Config;
using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.MVVM.Core;
using LeagueBroadcast.MVVM.View;
using LeagueBroadcast.OperatingSystem;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace LeagueBroadcast.MVVM.ViewModel
{
    public class TeamConfigViewModel : ObservableObject
    {
        public TeamConfig ConfigReference;

        private string _name;
        private int _score;
        private string _coach;
        private Color _colorBlue, _colorRed;
        private SolidColorBrush _colorBrushBlue, _colorBrushRed;

        private string _iconName;
        private string _nameTag;

        public string MapSide;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public int Score { get { return _score; } set { _score = value; OnPropertyChanged(); } }
        public string Coach { get { return _coach; } set { _coach = value; OnPropertyChanged(); } }
        public Color ColorBlue { get { return _colorBlue; } set { _colorBlue = value; OnPropertyChanged(); UpdateColorConfig(); } }
        public Color ColorRed { get { return _colorRed; } set { _colorRed = value; OnPropertyChanged(); UpdateColorConfig(); } }
        public SolidColorBrush ColorBrushBlue { get { return _colorBrushBlue; } set { _colorBrushBlue = value; OnPropertyChanged(); } }
        public SolidColorBrush ColorBrushRed { get { return _colorBrushRed; } set { _colorBrushRed = value; OnPropertyChanged(); } }
        public string IconName
        {
            get
            {
                return _iconName;
            }
            set
            {
                _iconName = value;
                OnPropertyChanged();
                OnPropertyChanged("IconNameFull");
                OnPropertyChanged("ShowIconReset");
            }
        }
        [JsonIgnore]
        public string IconNameFull { get { return Path.Combine(Directory.GetCurrentDirectory(), IconName); } }
        public bool ShowIconReset { get { return IconName != DefaultIconPath; } }
        public string NameTag { get { return _nameTag; } set { _nameTag = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Teams { get { return JSONConfigProvider.Instance.TeamConfigs; } }

        #region Static
        public static TeamConfigViewModel BlueTeam = new ();
        public static TeamConfigViewModel RedTeam = new ();
        public static string DefaultIconPath = Path.Combine("Cache", "TeamIcons", "Default.png");
        #endregion
        public TeamConfigViewModel(TeamConfig config)
        {
            this.ConfigReference = config;
        }

        public TeamConfigViewModel() { }

        public void Init(TeamConfig config, string mapSide)
        {
            this.ConfigReference = config;
            this.Name = ConfigReference.name;

            this.MapSide = mapSide;

            var cfgPath = Teams.SingleOrDefault(path => path.Equals(this.Name));
            if(cfgPath == null)
            {
                this.NameTag = ConfigReference.nameTag;
                this.Score = ConfigReference.score;
                this.Coach = ConfigReference.coach;
                this.IconName = DefaultIconPath;
                this.ColorBlue = ConfigController.Component.PickBan.DefaultBlueColor.ToColor();
                this.ColorRed = ConfigController.Component.PickBan.DefaultRedColor.ToColor();

            } else
            {
                var cfg = new ExtendedTeamConfig(this.Name);
                var res = JSONConfigProvider.Instance.ReadTeam(cfg);
                this.NameTag = cfg.Config.nameTag;
                this.Score = cfg.Config.score;
                this.Coach = cfg.Config.coach;
                this.IconName = cfg.IconLocation;
                this.ColorBlue = cfg.BlueColor.ToColor();
                this.ColorRed = cfg.RedColor.ToColor();
            }
        }

        private void UpdateColorConfig()
        {
            if (ConfigReference == null)
                return;

            ColorBrushBlue = new SolidColorBrush(ColorBlue);
            ColorBrushRed = new SolidColorBrush(ColorRed);

            Color c = MapSide == "blue" ? ColorBlue : ColorRed;
            ConfigReference.color = c.ToSerializedString();

        }

    }
}
