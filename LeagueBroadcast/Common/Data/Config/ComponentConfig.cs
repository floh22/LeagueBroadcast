using LeagueBroadcast.Common.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static LeagueBroadcast.Common.Log;

namespace LeagueBroadcast.Common.Data.Config
{
    class ComponentConfig : JSONConfig
    {
        [JsonIgnore]
        public override string Name => "Component";

        public override string FileVersion { get => _fileVersion; set => _fileVersion = value; }

        [JsonIgnore]
        public static new string CurrentVersion => "1.6";

        public DataDragonConfig DataDragon;

        public PickBanConfig PickBan;

        public IngameComponentConfig Ingame;

        public ReplayConfig Replay;

        public PostGameConfig PostGame;

        public AppConfig App;


        public override string GETCurrentVersion()
        {
            return CurrentVersion;
        }

        public override string GETDefaultString()
        {
            return SerializeIndented(CreateDefault());
        }

        public override void RevertToDefault()
        {
            ComponentConfig def = CreateDefault();
            this.DataDragon = def.DataDragon;
            this.PickBan = def.PickBan;
            this.Ingame = def.Ingame;
            this.Replay = def.Replay;
            this.App = def.App;
            this.PostGame = def.PostGame;
            this.FileVersion = CurrentVersion;
        }

        private ComponentConfig CreateDefault()
        {
            return new()
            {
                DataDragon = new DataDragonConfig()
                {
                    MinimumGoldCost = 2000,
                    Region = "global",
                    Locale = "en_US",
                    Patch = "latest",
                    CDN = "https://ddragon.leagueoflegends.com/cdn",
                    CDragonRaw = "https://raw.communitydragon.org"
                },
                PickBan = new PickBanConfig()
                {
                    IsActive = true,
                    DelayValue = 300,
                    UseDelay = false,
                    DefaultBlueColor = "rgb(66, 133, 244)",
                    DefaultRedColor = "rgb(234, 67, 53)"
                },
                Ingame = new IngameComponentConfig()
                {
                    IsActive = true,
                    UseLiveEvents = true,
                    DoItemCompleted = true,
                    DoLevelUp = true,
                    UseCustomScoreboard = false,
                    SeriesGameCount = 3,
                    Objectives = new IngameComponentConfig.ObjectiveConfig()
                    {
                        DoBaronKill = true,
                        DoDragonKill = true,
                        DoInhibitors = false,
                        DoObjectiveSpawnPopUp = false,
                        DoObjectiveKillPopUp = false,
                        UseCustomBaronTimer = false,
                        UseCustomDragonTimer = true,
                    },
                    Teams = new IngameComponentConfig.TeamInfoConfig()
                    {
                        DoTeamIcons = false,
                        DoTeamNames = false,
                        DoTeamScores = false
                    }
                },
                PostGame = new()
                {
                    IsActive = false
                },
                Replay = new ReplayConfig()
                {
                    IsActive = true,
                    UseAutoInitUI = true
                },
                App = new AppConfig()
                {
                    LogLevel = LogLevel.Info,
                    CheckForUpdates = true,
                    UpdateRepositoryName = @"floh22/LeagueBroadcast",
                    UpdateRepositoryUrl = "https://github.com/floh22/LeagueBroadcast",
                    LeagueInstall = new()
                    {
                        "C:\\",
                        "D:\\",
                        "E:\\",
                        "F:\\",
                        "C:\\Program Files",
                        "C:\\Program Files (x86)"
                    },
                    OffsetRepository = "https://api.github.com/repos/floh22/LeagueBroadcast/contents/Offsets?ref=v2",
                    OffsetPrefix = "Offsets-",
                    CheckForOffsets = true,
                    FrontendPort = 9001,
                }
            };
        }

        public override string GETJson()
        {
            return SerializeIndented(this);
        }

        public override bool UpdateConfigVersion(string oldVersion, string oldValues)
        {
            if (oldVersion.Equals("1.5"))
            {
                Task t = new(async () =>
                {
                    await Task.Delay(200);
                    //1.5 to 1.6

                    FileVersion = CurrentVersion;

                    App.FrontendPort = 9001;

                    JSONConfigProvider.Instance.WriteConfig(this);
                    Info($"Updated Component config from v1.5 to v{CurrentVersion}");
                });
                t.Start();
            }
            else
            {
                Log.Warn("Config too old to update");
            }
            return true;
        }

        public override void UpdateValues(string readValues)
        {
            ComponentConfig Cfg = JsonConvert.DeserializeObject<ComponentConfig>(readValues);
            this.DataDragon = Cfg.DataDragon;
            this.PickBan = Cfg.PickBan;
            this.Ingame = Cfg.Ingame;
            this.Replay = Cfg.Replay;
            this.PostGame = Cfg.PostGame;
            this.App = Cfg.App;
            this.FileVersion = Cfg.FileVersion;
        }

    }

    public class DataDragonConfig
    {
        public int MinimumGoldCost;
        public string Region;
        public string Locale;
        public string CDN;
        public string CDragonRaw;
        public string Patch;
    }

    public class PickBanConfig
    {
        public bool IsActive;
        public bool UseDelay;
        public int DelayValue;
        public string DefaultBlueColor;
        public string DefaultRedColor;
    }

    public class IngameComponentConfig
    {
        public bool IsActive;
        public bool UseLiveEvents;
        public bool DoLevelUp;
        public bool DoItemCompleted;
        public int SeriesGameCount;
        public ObjectiveConfig Objectives;
        public TeamInfoConfig Teams;
        public bool UseCustomScoreboard;

        public class ObjectiveConfig
        {
            public bool DoBaronKill;
            public bool DoDragonKill;
            public bool DoInhibitors;
            public bool DoObjectiveSpawnPopUp;
            public bool DoObjectiveKillPopUp;
            public bool UseCustomDragonTimer;
            //non functional for now
            public bool UseCustomBaronTimer;
        }

        public class TeamInfoConfig
        {
            public bool DoTeamNames;
            public bool DoTeamIcons;
            public bool DoTeamScores;
        }
    }

    public class PostGameConfig
    {
        public bool IsActive;
    }

    public class ReplayConfig
    {
        public bool IsActive;
        public bool UseAutoInitUI;
    }

    public class AppConfig
    {
        public LogLevel LogLevel;
        public bool CheckForUpdates;
        public string UpdateRepositoryUrl;
        public string UpdateRepositoryName;
        public List<string> LeagueInstall;
        public bool CheckForOffsets;
        public string OffsetRepository;
        public string OffsetPrefix;
        public int FrontendPort;

        [JsonIgnore]
        public StringVersion Version => GetSimpleLocalVersion();

        private StringVersion GetSimpleLocalVersion()
        {
            string longVersion = FileVersionInfo.GetVersionInfo("LeagueBroadcast.exe").FileVersion;
            return StringVersion.TryParse(
                FileVersionInfo.GetVersionInfo("LeagueBroadcast.exe")
                .FileVersion.Remove(longVersion.LastIndexOf('.')),
                out StringVersion version) ? version! : StringVersion.Zero;
        }
    }
}
