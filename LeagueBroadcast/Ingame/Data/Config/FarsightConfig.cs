using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.Farsight;
using LeagueBroadcast.Update.Http;
using Newtonsoft.Json;
using System;
using System.Net.Http;
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

        public Farsight.FarsightController.Offsets GameOffsets;
        public Farsight.Object.GameObject.Offsets ObjectOffsets;

        public override string GETCurrentVersion() => CurrentVersion;

        public override string GETDefaultString()
        {
            return SerializeIndented(CreateDefault().Result);
        }

        public async Task<FarsightConfig> CreateDefault(FarsightConfig cfg = null, string forcedVersion = "")
        {
            //Download from given repo
            if(!ConfigController.Component.App.CheckForOffsets)
            {
                Log.Info("Offset Updates disabled. Cannot automatically get current values");
                return new FarsightConfig();
            }
            string remoteVersion = forcedVersion == "" ? AppStateController.LocalGameVersion : forcedVersion;
            var offsetUri = $"{ConfigController.Component.App.OffsetRepository}{ConfigController.Component.App.OffsetPrefix}{remoteVersion}.json";

            Log.Info($"Fetching new offsets from {offsetUri}");
            try
            {
                string res = await RestRequester.GetRaw(offsetUri);
                Log.Info($"Received updated offsets");

                var remote = JsonConvert.DeserializeObject<FarsightConfig>(res);
                Log.Info($"Offsets found. Updating values for game version {remote.GameVersion}");
                return remote;
            } catch(HttpRequestException)
            {
                Log.Warn("Could not fetch updated Offsets! Are they not uploaded yet? Either provide your own, change the source, or wait for an update");
                FarsightController.ShouldRun = false;
                return new FarsightConfig();
            }
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

        private void UpdateGameVersion(FarsightConfig oldVersion)
        {
                var def = CreateDefault(oldVersion).Result;
                this.FileVersion = "1.0";
                this.GameVersion = def.GameVersion;
                this.GameOffsets = def.GameOffsets;
                this.ObjectOffsets = def.ObjectOffsets;
        }

        public override bool UpdateConfigVersion(string oldVersion, string oldValues)
        {
            return true;
        }

        public override void UpdateValues(string readValues)
        {
            var Cfg = JsonConvert.DeserializeObject<FarsightConfig>(readValues);
            if(Cfg.GameVersion != AppStateController.LocalGameVersion)
            {
                Log.Info("Outdated Offsets detected. Downloading new ones");
                UpdateGameVersion(Cfg);
                if(!FarsightController.ShouldRun)
                {
                    Log.Warn("Farsight disabled");
                    return;
                }
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
