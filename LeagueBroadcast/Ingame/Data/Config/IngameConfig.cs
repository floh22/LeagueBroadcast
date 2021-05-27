using LeagueBroadcast.Common.Data.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LeagueBroadcast.Ingame.Data.Config
{
    public class IngameConfig : JSONConfig
    {
        public override string Name => "Ingame";

        public override string FileVersion { get => _fileVersion; set => _fileVersion = value; }

        [JsonIgnore]
        public static new string CurrentVersion => "1.0";


        public InhibitorDisplayConfig InhibDisplay;
        public ScoreDisplayConfig ScoreDisplay;
        public string BackgroundColor;

        public override string GETCurrentVersion()
        {
            return CurrentVersion;
        }

        public override string GETJson()
        {
            return SerializeIndented(this);
        }

        public override void UpdateValues(string readValues)
        {
            var Cfg = JsonConvert.DeserializeObject<IngameConfig>(readValues);
            InhibDisplay = Cfg.InhibDisplay;
            ScoreDisplay = Cfg.ScoreDisplay;
            BackgroundColor = Cfg.BackgroundColor;
            FileVersion = Cfg.FileVersion;
        }

        public override string GETDefaultString()
        {
            return JsonConvert.SerializeObject(CreateDefault(), Formatting.Indented);
        }

        public override void RevertToDefault()
        {
            var def = CreateDefault();
            this.InhibDisplay = def.InhibDisplay;
            this.ScoreDisplay = def.ScoreDisplay;
            this.BackgroundColor = def.BackgroundColor;
            this.FileVersion = CurrentVersion;
        }

        private IngameConfig CreateDefault()
        {
            return new IngameConfig() {
                FileVersion = CurrentVersion,
                InhibDisplay = new InhibitorDisplayConfig() { Location = new Vector2(0, 845), Size = new Vector2(306,118) },
                ScoreDisplay = new ScoreDisplayConfig() { Location = new Vector2(906, 0) },
                BackgroundColor = "rgb(19,24,63)"
            };
        }

        public override void UpdateConfigVersion(string oldVersion, string oldValues)
        {
            //No format change to correct
            return;
        }
        
        public class InhibitorDisplayConfig
        {
            public Vector2 Location;
            public Vector2 Size;
        }

        public class ScoreDisplayConfig
        {
            public Vector2 Location;
        }
    }
}
