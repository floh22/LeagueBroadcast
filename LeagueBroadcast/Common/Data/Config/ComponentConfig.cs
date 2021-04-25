using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static LeagueBroadcast.OperatingSystem.Log;

namespace LeagueBroadcast.Common.Data.Config
{
    class ComponentConfig : JSONConfig
    {
        [JsonIgnore]
        public override string Name => "Component";

        public override string FileVersion { get => _fileVersion; set => _fileVersion = value; }

        [JsonIgnore]
        public static new string CurrentVersion => "1.0";

        public DataDragonConfig DataDragon;

        public PickBanConfig PickBan;

        public IngameConfig Ingame;

        public ReplayConfig Replay;

        public AppConfig App;


        public override string GETCurrentVersion()
        {
            return CurrentVersion;
        }

        public override string GETDefault()
        {
            return SerializeIndented(CreateDefault());
        }

        private ComponentConfig CreateDefault()
        {
            return new() {
                DataDragon = new DataDragonConfig() { MinimumGoldCost = 2000 },
                PickBan = new PickBanConfig() { IsActive = true, DelayValue = 300, UseDelay = false },
                Ingame = new IngameConfig() { 
                    IsActive = true, 
                    UseLiveEvents = true, 
                    DoItemCompleted = true, 
                    DoLevelUp = true,
                    Objectives = new IngameConfig.ObjectiveConfig() { 
                        DoBaronKill = true, 
                        DoDragonKill = true
                    }
                },
                Replay = new ReplayConfig() { IsActive = true, UseAutoInitUI = true },
                App = new AppConfig() { LogLevel = LogLevel.Verbose }
            };
        }

        public override string GETJson()
        {
            return SerializeIndented(this);
        }

        public override void UpdateConfigVersion(string oldVersion, dynamic oldValues)
        {
            //Currently v1.0
            return;
        }

        public override void UpdateValues(dynamic readValues)
        {
            throw new NotImplementedException();
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
    }

    public class IngameConfig
    {
        public bool IsActive;
        public bool UseLiveEvents;
        public bool DoLevelUp;
        public bool DoItemCompleted;
        public ObjectiveConfig Objectives;

        public class ObjectiveConfig
        {
            public bool DoBaronKill;
            public bool DoDragonKill;
        }
    }

    public class ReplayConfig
    {
        public bool IsActive;
        public bool UseAutoInitUI;
    }

    public class AppConfig
    {
        public LogLevel LogLevel;
    }
}
