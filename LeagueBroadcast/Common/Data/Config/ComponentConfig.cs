using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public static new string CurrentVersion => "1.3";

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
            var def = CreateDefault();
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
                    Region = "euw",
                    Locale = "en_US",
                    Patch = "latest",
                    CDN = "https://ddragon.leagueoflegends.com/cdn"
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
                        DoObjectiveKillPopUp = false
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
                    OffsetRepository = "https://raw.githubusercontent.com/floh22/LeagueBroadcast/v2/Offsets/",
                    OffsetPrefix = "Offsets-",
                    CheckForOffsets = true
                }
            };
        }

        public override string GETJson()
        {
            return SerializeIndented(this);
        }

        public override bool UpdateConfigVersion(string oldVersion, string oldValues)
        {
            //1.0 to Current
            if (oldVersion.Equals("1.0"))
            {
                Task t = new(async () =>
                {
                    await Task.Delay(100);
                    //1.0 to 1.1
                    FileVersion = CurrentVersion;

                    PickBan.DefaultBlueColor = ConfigController.PickBan.frontend.blueTeam.color;
                    PickBan.DefaultRedColor = ConfigController.PickBan.frontend.redTeam.color;

                    Ingame.SeriesGameCount = 3;

                    //1.1 to 1.2
                    App.LogLevel = LogLevel.Info;
                    SetLogLevel(LogLevel.Info);

                    //1.2 to 1.3
                    Ingame.Objectives.DoObjectiveSpawnPopUp = false;
                    Ingame.Objectives.DoObjectiveKillPopUp = false;
                    DataDragon.Region = "euw";
                    DataDragon.Locale = "en_US";
                    DataDragon.Patch = "latest";
                    DataDragon.CDN = "https://ddragon.leagueoflegends.com/cdn";

                    JSONConfigProvider.Instance.WriteConfig(this);
                    Info($"Updated Component config from v1.0 to v{CurrentVersion}");
                });
                t.Start();
                return true;
            }

            //1.1 to Current
            if (oldVersion.Equals("1.1"))
            {
                Task t = new Task(async () =>
                {
                    await Task.Delay(100);
                    //1.1 to 1.2
                    App.LogLevel = LogLevel.Info;
                    FileVersion = CurrentVersion;

                    //1.2 to 1.3
                    Ingame.Objectives.DoObjectiveSpawnPopUp = false;
                    Ingame.Objectives.DoObjectiveKillPopUp = false;
                    DataDragon.Region = "euw";
                    DataDragon.Locale = "en_US";
                    DataDragon.Patch = "latest";
                    DataDragon.CDN = "https://ddragon.leagueoflegends.com/cdn";
                    FileVersion = CurrentVersion;

                    JSONConfigProvider.Instance.WriteConfig(this);
                    Info($"Updated Component config from v1.1 to v{CurrentVersion}");
                });
                t.Start();
                return true;
            }

            //1.2 to Current
            if (oldVersion.Equals("1.2"))
            {
                Task t = new(async () =>
                {
                    await Task.Delay(100);
                    //1.2 to 1.3
                    FileVersion = CurrentVersion;

                    Ingame.Objectives.DoObjectiveSpawnPopUp = false;
                    Ingame.Objectives.DoObjectiveKillPopUp = false;
                    DataDragon.Region = "euw";
                    DataDragon.Locale = "en_US";
                    DataDragon.Patch = "latest";
                    DataDragon.CDN = "https://ddragon.leagueoflegends.com/cdn";

                    JSONConfigProvider.Instance.WriteConfig(this);
                    Info($"Updated Component config from v1.2 to v{CurrentVersion}");
                });
                t.Start();
                return true;
            }
            return true;
        }

        public override void UpdateValues(string readValues)
        {
            var Cfg = JsonConvert.DeserializeObject<ComponentConfig>(readValues);
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
