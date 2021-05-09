using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.Common.Data.Provider;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace LeagueBroadcast.Ingame.Data.Config
{
    public class FarsightConfig : JSONConfig
    {
        [JsonIgnore]
        public override string Name => "Farsight";

        public override string FileVersion { get => _fileVersion; set => _fileVersion = value; }

        [JsonIgnore]
        public static new string CurrentVersion => "1.0";

        public string GameVersion;

        public Farsight.FarsightController.Offsets GameOffsets
        {
            get { return Farsight.FarsightController.GameOffsets; }
            set { Farsight.FarsightController.GameOffsets = value; }
        }
        public Farsight.Object.GameObject.Offsets ObjectOffsets
        {
            get { return Farsight.FarsightController.ObjectOffsets; }
            set { Farsight.FarsightController.ObjectOffsets = value; }
        }

        public override string GETCurrentVersion() => CurrentVersion;

        public override string GETDefaultString()
        {
            return SerializeIndented(CreateDefault().Result);
        }

        public async Task<FarsightConfig> CreateDefault()
        {
            //Download from given repo
            if(!ConfigController.Component.App.CheckForOffsets)
            {
                Log.Info("Offset Updates disabled. Cannot automatically get current values");
                return new FarsightConfig();
            }
            var offsetUri = $"{ConfigController.Component.App.OffsetRepository}{ConfigController.Component.App.OffsetPrefix}{DataDragon.version.Champion}.json";
            Log.Info($"Fetching new offsets from {offsetUri}");
            string res = await DataDragonUtils.GetAsync(offsetUri);
            if(res.Length == 0)
            {
                Log.Warn("Could not fetch updated Offsets! Are they not uploaded yet? Either provide your own, change the source, or wait for an update");
                return new FarsightConfig();
            }
            var remote = JsonConvert.DeserializeObject<FarsightConfig>(res);
            Log.Info($"Offsets found. Updating values for game version {remote.GameVersion}");
            return remote;
        }

        public override string GETJson()
        {
            return SerializeIndented(this);
        }

        public override void RevertToDefault()
        {
            var def = CreateDefault().Result;
            this.FileVersion = "1.0";
            this.GameVersion = def.GameVersion;
            this.GameOffsets = def.GameOffsets;
            this.ObjectOffsets = def.ObjectOffsets;
        }

        public override void UpdateConfigVersion(string oldVersion, dynamic oldValues)
        {
            return;
        }

        public override void UpdateValues(string readValues)
        {
            var Cfg = JsonConvert.DeserializeObject<FarsightConfig>(readValues);
            if(Cfg.GameVersion != DataDragon.version.Champion)
            {
                Log.Info("Outdated Offsets detected. Downloading new ones");
                RevertToDefault();
                Log.Info("Saving new Offsets to file");
                ConfigController.UpdateConfigFile(this);
                return;
            }
            this.FileVersion = Cfg.FileVersion;
            this.GameVersion = Cfg.GameVersion;
            this.GameOffsets = Cfg.GameOffsets;
            this.ObjectOffsets = Cfg.ObjectOffsets;
        }
    }
}
