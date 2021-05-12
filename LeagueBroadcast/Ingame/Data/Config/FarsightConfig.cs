using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.Farsight;
using Newtonsoft.Json;
using System;
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
            string remoteVersion = forcedVersion == "" ? DataDragon.version.Champion : forcedVersion;
            var offsetUri = $"{ConfigController.Component.App.OffsetRepository}{ConfigController.Component.App.OffsetPrefix}{remoteVersion}.json";

            Log.Info($"Fetching new offsets from {offsetUri}");
            string res = await DataDragonUtils.GetAsync(offsetUri);
            if(res == "")
            {
                //Wednesday at 4AM is usually patch time, so if its earlier, look one patch earlier
                Log.Info("New Offsets not yet online. Checking to make sure if new patch should already be used");
                var versionComponents = DataDragon.version.Champion.Split('.');
                int patch = Int32.Parse(versionComponents[1]);
                if (cfg != null && 
                     patch - 1 == Int32.Parse(cfg.GameVersion.Substring(3, 2).Replace(".", "")) && 
                    (DateTime.Now.Hour < 4 && DateTime.Now.DayOfWeek <= DayOfWeek.Wednesday && DateTime.Now.DayOfWeek >= DayOfWeek.Tuesday))
                {
                    Log.Info("Night before patch. Using old offsets");
                    return cfg;
                }

                //We do not have a local version available. Incase this has been started for the first time on a tuesday before a patch, get old offsets
                //This is in theory a moot point since this means Essence will stop working in a couple of hours, but its better than not working
                if(DateTime.Now.Hour < 4 && DateTime.Now.DayOfWeek <= DayOfWeek.Wednesday && patch >= 0 && DateTime.Now.DayOfWeek >= DayOfWeek.Tuesday)
                {
                    return CreateDefault(null, $"{versionComponents[0]}.{patch - 1}.{versionComponents[2]}").Result;
                }

                //Its not before a patch and offsets could not be found. This means that its probably Wednesday after a patch
                //Disable memory component of essence. It's better than crashing :)
                Log.Warn("Could not fetch updated Offsets! Are they not uploaded yet? Either provide your own, change the source, or wait for an update");
                FarsightController.ShouldRun = false;
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

        private void UpdateGameVersion(FarsightConfig oldVersion)
        {
            var def = CreateDefault(oldVersion).Result;
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
                UpdateGameVersion(Cfg);
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
