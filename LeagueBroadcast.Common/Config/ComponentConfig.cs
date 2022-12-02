using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;

#pragma warning disable CS8618
namespace LeagueBroadcast.Common.Config
{
    public class ComponentConfig : JsonConfig
    {
        [JsonIgnore]
        public override string Name => "Component.json";
        [JsonIgnore]
        public override StringVersion CurrentVersion => new(2, 0, 0);

        public CommunityDragonConfig CommunityDragon { get; set; }
        public PickBanComponentConfig PickBan { get; set; }
        public IngameComponentConfig Ingame { get; set; }

        private List<string> _leagueInstallLocations = new();

        public List<string> LeagueInstallLocations
        {
            get { return _leagueInstallLocations; }
            set { _leagueInstallLocations = value; OnPropertyChanged(); }
        }


        public override void CheckForUpdate()
        {
            if (FileVersion != CurrentVersion)
            {
                $"{Name} update detected".Info();
            }

            FileVersion = CurrentVersion;
        }

        public override void RevertToDefault()
        {
            LeagueInstallLocations = new List<string>() { "C:/Riot Games/League of Legends" };
            CommunityDragon = new CommunityDragonConfig()
            {
                MinimumItemGoldCost = 2000,
                Region = "global",
                Locale = "en_US",
                Patch = "latest",
                CDragonRaw = "https://raw.communitydragon.org/"
            };
            Ingame = new()
            {
                IsActive = true,
                UseFarsightAPI = true,
                UseLiveEventAPI = true,
                JglProximityDistance = 1200
            };
            PickBan = new()
            {
                IsActive = true,
                Delay = new()
                {
                    UseDelay = false,
                    DelayAmount = 180
                },
                TeamInfo = new()
                {
                    ShowTeamNames = true,
                    ShowTeamTagsInsteadOfNames = true,
                    ShowCoaches = false,
                    ShowScores = true,
                    ShowSummonerSpells = false,
                    UseTeamColors = false
                },
                SideColors = new()
                {
                    BlueColorPrimary = "rgb(0,0,0)",
                    BlueColorSecondary = "rgb(0,0,0)",
                    RedColorPrimary = "rgb(0,0,0)",
                    RedColorSecondary = "rgb(0,0,0)"
                },
                PickBanPickData = new()
                {
                    IsActive = false,
                    AutoSwap = true,
                    UseOnlineData = false,
                },
                LoadingScreen = new()
                {
                    IsActive = true,
                    AutoSwap = true,
                    UseSkins = false
                },
                PickBanPickOrder = new()
                {
                    IsActive = false,
                    AutoSwap = false,
                    AutoSwapIfDelayed = false,
                    FillEntireLoadScreenTime = true,
                    Size = PickBanPickOrderComponent.DisplaySize.Small
                }
            };
        }
    }

    public class CommunityDragonConfig : ObservableObject
    {
        private string _patch = "";
        public string Patch
        {
            get { return _patch; }
            set { _patch = value; OnPropertyChanged(); }
        }

        private string _cDragonRaw = "";
        public string CDragonRaw
        {
            get { return _cDragonRaw; }
            set { _cDragonRaw = value; OnPropertyChanged(); }
        }

        private string _region = "";

        public string Region
        {
            get { return _region; }
            set { _region = value; OnPropertyChanged(); }
        }

        private string _locale = "";

        public string Locale
        {
            get { return _locale; }
            set { _locale = value; OnPropertyChanged(); }
        }

        private int _minimumItemGoldCost;

        public int MinimumItemGoldCost
        {
            get { return _minimumItemGoldCost; }
            set { _minimumItemGoldCost = value; OnPropertyChanged(); }
        }

    }

    #region Ingame
    public class IngameComponentConfig : INotifyPropertyChanged
    {
        public bool IsActive { get; set; }
        public bool UseFarsightAPI { get; set; }
        public bool UseLiveEventAPI { get; set; }
        public float JglProximityDistance { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    #endregion

    #region PickBan
    public class PickBanComponentConfig : INotifyPropertyChanged
    {
        public bool IsActive { get; set; }

        public PickBanTeamInfoComponent TeamInfo { get; set; }

        public PickBanColorComponent SideColors { get; set; }

        public PickBanDelayComponent Delay { get; set; }

        public PickBanLoadingScreenComponent LoadingScreen { get; set; }

        public PickBanPickDataComponent PickBanPickData { get; set; }

        public PickBanPickOrderComponent PickBanPickOrder { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

    }

    public class PickBanTeamInfoComponent : INotifyPropertyChanged
    {
        public bool ShowTeamNames { get; set; }
        public bool ShowTeamTagsInsteadOfNames { get; set; }
        public bool ShowScores { get; set; }
        public bool ShowCoaches { get; set; }
        public bool ShowSummonerSpells { get; set; }
        public bool UseTeamColors { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class PickBanColorComponent : INotifyPropertyChanged
    {
        public string BlueColorPrimary { get; set; }
        public string BlueColorSecondary { get; set; }

        public string RedColorPrimary { get; set; }
        public string RedColorSecondary { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class PickBanDelayComponent : INotifyPropertyChanged
    {
        public bool UseDelay { get; set; }

        public int DelayAmount { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class PickBanLoadingScreenComponent : INotifyPropertyChanged
    {
        public bool IsActive { get; set; }

        public bool UseSkins { get; set; }

        public bool AutoSwap { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class PickBanPickOrderComponent : INotifyPropertyChanged
    {
        public bool IsActive { get; set; }

        public DisplaySize Size { get; set; }

        public bool AutoSwap { get; set; }

        public bool AutoSwapIfDelayed { get; set; }

        public bool FillEntireLoadScreenTime { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public enum DisplaySize
        {
            Small,
            Medium,
            Large
        }
    }

    public class PickBanPickDataComponent : INotifyPropertyChanged
    {
        public bool IsActive { get; set; }

        public bool AutoSwap { get; set; }

        public bool UseOnlineData { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    #endregion
}
