using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Config;
using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.Common.Utils;
using LeagueBroadcast.Farsight;
using LeagueBroadcast.Update.Http;
using Newtonsoft.Json;
using Swan.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueBroadcast.Ingame.Data.Config
{
    public class FarsightConfig : JSONConfig
    {
        [JsonIgnore]
        public override string Name => "Farsight";

        public override string FileVersion { get => _fileVersion; set => _fileVersion = value; }

        [JsonIgnore]
        public static new string CurrentVersion => "3.0";


        [JsonIgnore]
        public StringVersion OffsetStringVersion => GetOffsetVersion();

        private StringVersion GetOffsetVersion()
        {
            if (StringVersion.TryParse(OffsetVersion, out StringVersion version))
            {
                return version;
            }
            else
            {
                return StringVersion.Zero;
            }
        }

        public string OffsetVersion;

        public Farsight.FarsightController.Offsets GameOffsets;
        public Farsight.Object.GameObject.Offsets ObjectOffsets;

        public override string GETCurrentVersion() => CurrentVersion;

        public override string GETDefaultString()
        {
            return SerializeIndented(CreateDefault().Result);
        }


        private struct LatestOffsetData
        {
            public string Url;
            public StringVersion Version;


            public LatestOffsetData(string url, StringVersion version)
            {
                Url = url;
                Version = version;
            }
        }

        public async Task<FarsightConfig> CreateDefault(FarsightConfig cfg = null, StringVersion? forcedVersion = null)
        {
            //Download from given repo
            if(cfg == null)
            {
                cfg = new FarsightConfig();
            }
            if(!ConfigController.Component.App.CheckForOffsets)
            {
                Log.Info("Offset Updates disabled. Cannot automatically get current values");
                return cfg;
            }
            StringVersion localVersion = (forcedVersion == StringVersion.Zero || forcedVersion is null) ? AppStateController.LocalGameVersion : forcedVersion;

            Log.Info("Local Offset Version: " + cfg.OffsetVersion);

            LatestOffsetData? offsetInfo = await GetLatestFarsightPatchLocation(localVersion);

            if (offsetInfo.HasValue && offsetInfo.Value.Version > cfg.OffsetStringVersion)
            {
                var data = offsetInfo.Value;
                Log.Info($"fetching updated offsets v{data.Version} from {data.Url}");


                string offsetDlLocation = data.Url;
                cfg.OffsetVersion = data.Version.ToString();

                try
                {
                    string remoteContent = await RestRequester.GetRaw(offsetDlLocation);
                    FarsightConfig remoteCfg = JsonConvert.DeserializeObject<FarsightConfig>(remoteContent);

                    if (remoteCfg is null || remoteContent.Length == 0 || StringVersion.Parse(remoteCfg.FileVersion) < StringVersion.Parse(CurrentVersion))
                    {
                        Log.Warn($"Updated offsets not found");
                        FarsightController.ShouldRun = false;
                        return cfg;
                    }

                    return remoteCfg;
                }

                catch
                {
                    Log.Warn("Could not fetch updated Offsets! Are they not uploaded yet? Either provide your own, change the source, or wait for an update");
                    FarsightController.ShouldRun = false;
                    return cfg;
                }
            }
            else
            {
                Log.Info($"Offset data up to date");
                return cfg;
            }
        }

        private async Task<LatestOffsetData?> GetLatestFarsightPatchLocation(StringVersion current)
        {
            try
            {
                Log.Info($"Attempting to retrieve updated offset file locations from {ConfigController.Component.App.OffsetRepository}");
                var res = await RestRequester.GetRaw(ConfigController.Component.App.OffsetRepository).ConfigureAwait(false);
                if (res is null)
                {
                    Log.Warn($"Could not retrieve offsets. Offset repository not found");
                    return null;
                }

                JsonDocument jsonRes = JsonDocument.Parse(res);
                JsonElement root = jsonRes.RootElement;

                var offsetFiles = root.EnumerateArray();

                StringVersion latest = StringVersion.Zero;
                JsonElement latestElement = root;

                foreach (var file in offsetFiles)
                {
                    string fileName = file.GetProperty("name").GetString() ?? "";
                    StringVersion? fileVersion = StringVersion.Zero;
                    string version = Regex.Match(fileName, $"[\\d|\\.|\\,]+").Groups[0].Value;
                    if (StringVersion.TryParse(version, out fileVersion))
                    {
                        //Only check for current patch. This means uploads must all contain the full patch version in some way!
                        //Optimally data name will be of the form "[Anything]Major.Minor.Patch.json"
                        if (!fileName.Contains($"{current.ToString(2)}"))
                            continue;

                        if (fileVersion > latest)
                        {
                            latest = fileVersion;
                            latestElement = file;
                        }
                    }
                }

                if (latest == StringVersion.Zero || latest < current || !latestElement.TryGetProperty("download_url", out var downloadUrlProperty))
                {
                    return new LatestOffsetData();
                }

                string? downloadUrlString = downloadUrlProperty.GetString();

                if (downloadUrlString is null)
                {
                    return null;
                }

                Log.Info("Latest Offset file: " + downloadUrlString + " (v" + latest.ToString() + ")");

                return new LatestOffsetData(downloadUrlString, latest);

            }
            catch (Exception ex)
            {
                Log.Warn($"Could not retrieve latest offsets from GitHub");
                Log.Warn($"{ex.Source}: {ex.Message}\nStacktrace: {ex.StackTrace}\n{ex.InnerException}");
            }

            return null;
        }

        public override string GETJson()
        {
            return SerializeIndented(this);
        }


        public override void RevertToDefault()
        {
            var def = CreateDefault().Result;
            this.FileVersion = CurrentVersion;
            this.OffsetVersion = def.OffsetVersion;
            this.GameOffsets = def.GameOffsets;
            this.ObjectOffsets = def.ObjectOffsets;
        }

        private void UpdateGameVersion(FarsightConfig oldVersion)
        {
            var def = CreateDefault(oldVersion).Result;
            this.FileVersion = CurrentVersion;
            this.OffsetVersion = def.OffsetVersion;
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


            if(!FarsightController.ShouldRun)
                {
                    Log.Warn("Farsight disabled");
                    return;
                }

            UpdateGameVersion(Cfg);
            ConfigController.UpdateConfigFile(this);


            this.FileVersion = Cfg.FileVersion;
            this.OffsetVersion = Cfg.OffsetVersion;
            this.GameOffsets = Cfg.GameOffsets;
            this.ObjectOffsets = Cfg.ObjectOffsets;

        }
    }
}
