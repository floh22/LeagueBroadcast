using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Utils;

namespace LeagueBroadcast.Common.Data.Pregame
{
    public class PreGameDisplayData
    {
        public bool ScoreEnabled { get; set; }

        public bool SpellsEnabled { get; set; }

        public bool CoachesEnabled { get; set; }

        public bool ChampionStatsEnabled { get; set; }

        public bool LoadingScreenEnabled { get; set; }

        public StringVersion Patch { get; set; }


        private static ComponentConfig _componentConfig = ConfigController.Get<ComponentConfig>();

        public PreGameDisplayData()
        {
            _componentConfig.PickBan.PropertyChanged += OnConfigPropertyChanged;
            _componentConfig.PickBan.PickBanPickData.PropertyChanged += OnPickDataPropertyChanged;
            _componentConfig.PickBan.TeamInfo.PropertyChanged += OnTeamInfoPropertyChanged;
            _componentConfig.PickBan.LoadingScreen.PropertyChanged += OnLoadingScreenPropertyChanged;


            ScoreEnabled = _componentConfig.PickBan.TeamInfo.ShowScores;
            SpellsEnabled = _componentConfig.PickBan.TeamInfo.ShowSummonerSpells;
            CoachesEnabled = _componentConfig.PickBan.TeamInfo.ShowCoaches;
            ChampionStatsEnabled = _componentConfig.PickBan.PickBanPickData.IsActive && _componentConfig.PickBan.PickBanPickData.AutoSwap;
            LoadingScreenEnabled = _componentConfig.PickBan.LoadingScreen.IsActive && _componentConfig.PickBan.LoadingScreen.AutoSwap;
            Patch = StringVersion.LCUVersion;
        }

        private void OnConfigPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ScoreEnabled = _componentConfig.PickBan.TeamInfo.ShowScores;
            SpellsEnabled = _componentConfig.PickBan.TeamInfo.ShowSummonerSpells;
            CoachesEnabled = _componentConfig.PickBan.TeamInfo.ShowCoaches;
            ChampionStatsEnabled = _componentConfig.PickBan.PickBanPickData.IsActive && _componentConfig.PickBan.PickBanPickData.AutoSwap;
            LoadingScreenEnabled = _componentConfig.PickBan.LoadingScreen.IsActive && _componentConfig.PickBan.LoadingScreen.AutoSwap;
            Patch = StringVersion.LCUVersion;
        }

        private void OnPickDataPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ChampionStatsEnabled = _componentConfig.PickBan.PickBanPickData.IsActive && _componentConfig.PickBan.PickBanPickData.AutoSwap;
        }


        private void OnTeamInfoPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ScoreEnabled = _componentConfig.PickBan.TeamInfo.ShowScores;
            SpellsEnabled = _componentConfig.PickBan.TeamInfo.ShowSummonerSpells;
            CoachesEnabled = _componentConfig.PickBan.TeamInfo.ShowCoaches;
        }

        private void OnLoadingScreenPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            LoadingScreenEnabled = _componentConfig.PickBan.LoadingScreen.IsActive && _componentConfig.PickBan.LoadingScreen.AutoSwap;
        }
    }
}
