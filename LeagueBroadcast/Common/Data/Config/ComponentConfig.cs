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
        public static new string CurrentVersion => "1.2";

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
            return new() {
                DataDragon = new DataDragonConfig() {
                    MinimumGoldCost = 2000
                },
                PickBan = new PickBanConfig() {
                    IsActive = true,
                    DelayValue = 300,
                    UseDelay = false,
                    DefaultBlueColor = "rgb(66, 133, 244)",
                    DefaultRedColor = "rgb(234, 67, 53)"
                },
                Ingame = new IngameComponentConfig() {
                    IsActive = true,
                    UseLiveEvents = true,
                    DoItemCompleted = true,
                    DoLevelUp = true,
                    UseCustomScoreboard = false,
                    SeriesGameCount = 3,
                    Objectives = new IngameComponentConfig.ObjectiveConfig() {
                        DoBaronKill = true,
                        DoDragonKill = true,
                        DoInhibitors = false
                    },
                    Teams = new IngameComponentConfig.TeamInfoConfig()
                    {
                        DoTeamIcons = false,
                        DoTeamNames = false,
                        DoTeamScores = false
                    }
                },
                PostGame = new() {
                    IsActive = false
                },
                Replay = new ReplayConfig() {
                    IsActive = true,
                    UseAutoInitUI = true
                },
                App = new AppConfig() {
                    LogLevel = LogLevel.Info,
                    CheckForUpdates = true,
                    UpdateRepositoryName = @"floh22/LeagueBroadcastHub",
                    UpdateRepositoryUrl = "https://github.com/floh22/LeagueBroadcastHub",
                    LeagueInstall = new() {
                        "C:\\",
                        "D:\\",
                        "E:\\",
                        "F:\\",
                        "C:\\Program Files",
                        "C:\\Program Files (x86)"
                    },
                    OffsetRepository = "https://raw.githubusercontent.com/floh22/LeagueBroadcastHub/v2/Offsets/",
                    OffsetPrefix = "Offsets-",
                    CheckForOffsets = true
                }
            };
        }

        public override string GETJson()
        {
            return SerializeIndented(this);
        }

        public override void UpdateConfigVersion(string oldVersion, string oldValues)
        {
            //1.0 to Current
            if(oldVersion.Equals("1.0"))
            {
                //1.0 to 1.1
                Task t = new Task(async () => { 
                    await Task.Delay(100); 
                    PickBan.DefaultBlueColor = ConfigController.PickBan.frontend.blueTeam.color;
                    PickBan.DefaultRedColor = ConfigController.PickBan.frontend.redTeam.color;

                    Ingame.SeriesGameCount = 3;

                    FileVersion = CurrentVersion;
                    JSONConfigProvider.Instance.WriteConfig(this);

                    //1.1 to 1.2
                    this.App.LogLevel = LogLevel.Info;
                    Log.SetLogLevel(LogLevel.Info);

                    Log.Info("Updated Component config from v1.0 to v1.2");
                });
                t.Start();
                return;
            }

            //1.1 to Current
            if(oldVersion.Equals("1.1"))
            {
                //1.1 to 1.2
                Task t = new Task(async () =>
                {
                    await Task.Delay(100);
                    this.App.LogLevel = LogLevel.Info;
                    FileVersion = CurrentVersion;
                    JSONConfigProvider.Instance.WriteConfig(this);
                    Log.Info("Updated Component config from v1.1 to v1.2");
                });
                t.Start();
                return;
            }
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
