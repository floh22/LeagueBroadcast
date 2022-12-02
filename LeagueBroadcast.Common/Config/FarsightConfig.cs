
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Http;
using LeagueBroadcast.Common.Data.Farsight;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;

namespace LeagueBroadcast.Common.Config
{
    public class FarsightConfig : JsonConfig
    {
        #region NonSerialized
        [JsonIgnore]
        public override string Name => "Farsight.json";

        [JsonIgnore]
        public override StringVersion CurrentVersion => new(2, 0, 0);
        #endregion

        #region Serialized
        public StringVersion OffsetVersion { get; set; } = new(0, 0, 0);
        public Offsets Offsets { get; set; } = new();
        #endregion

        public async Task Init()
        {
            $"Checking offset version".Info("Farsight");
            "verifying game offset version".UpdateStartupProgressText();

            //Using this means that every time the client starts, it will check github to make sure offsets are up to date. Provides a mechanism for updating offsets mid patch
            string[] offsetInfo = await GetLatestFarsightPatchLocation();
            if (offsetInfo.Length == 2 && StringVersion.Parse(offsetInfo[1]) > OffsetVersion)
            {
                "fetching updated offsets".UpdateStartupProgressText();
                $"fetching updated offsets v{offsetInfo[1]} from {offsetInfo[0]}".Debug("Farsight");
                await RevertToDefaultAsync(offsetInfo);
            }
            else
            {
                $"Offset data up to date".Info("Farsight");
            }
        }

        public override void CheckForUpdate()
        {
            if (FileVersion < CurrentVersion)
            {
                //No file update currently
                $"Farsight Config out of date!".Warn("Farsight");
            }
        }

        private async Task<string[]> GetLatestFarsightPatchLocation()
        {
            AppConfig appCfg = await ConfigController.GetAsync<AppConfig>();

            if (!appCfg.CheckForUpdatedOffsets)
            {
                $"Offset Update disabled".Info("Farsight");
                return Array.Empty<string>();
            }

            try
            {
                StringVersion current = OffsetVersion;

                $"Attempting to retrieve updated offset file locations from {appCfg.OffsetRepository}".Debug("Farsight");
                var res = await RestRequester.GetRaw(appCfg.OffsetRepository).ConfigureAwait(false);
                if(res is null)
                {
                    $"Could not retrieve offsets. Offset repository not found".Error("Farsight");
                    return Array.Empty<string>();
                }

                JsonDocument jsonRes = JsonDocument.Parse(res);
                JsonElement root = jsonRes.RootElement;

                var offsetFiles = root.EnumerateArray();

                StringVersion latest = StringVersion.Zero;
                JsonElement latestElement = root;

                foreach(var file in offsetFiles)
                {
                    string fileName = file.GetProperty("name").GetString()??"";
                    StringVersion? fileVersion = StringVersion.Zero;
                    string version = Regex.Match(fileName, $"[\\d|\\.|\\,]+").Groups[0].Value;
                    if (StringVersion.TryParse(version, out fileVersion))
                    {
                        //Only check for current patch. This means uploads must all contain the full patch version in some way!
                        //Optimally data name will be of the form "[Anything]Major.Minor.Patch.json"
                        if (!fileName.Contains($"{StringVersion.LeagueVersion.ToString(2)}"))
                            continue;

                        if (fileVersion > latest)
                        {
                            latest = fileVersion;
                            latestElement = file;
                        }
                    }
                }

                if(latest == StringVersion.Zero || latest < StringVersion.LeagueVersion || !latestElement.TryGetProperty("download_url", out var downloadUrlProperty))
                {
                    return Array.Empty<string>();
                }

                string? downloadUrlString = downloadUrlProperty.GetString();

                if(downloadUrlString is null)
                {
                    return Array.Empty<string>();
                }

                return new string[] { downloadUrlString, latest.ToString() };

            } catch (Exception ex)
            {
                $"Could not retrieve latest offsets from GitHub".Error("Farsight");
                $"{ex.Source}: {ex.Message}\nStacktrace: {ex.StackTrace}\n{ex.InnerException}".Error();
            }

            return Array.Empty<string>();
        }

        private async Task<bool> RevertToDefaultAsync(string[] versionInfo)
        {
            AppConfig appCfg = await ConfigController.GetAsync<AppConfig>();



            if (!appCfg.CheckForUpdatedOffsets)
            {
                $"Offset Update disabled".Info("Farsight");
                return false;
            }

            if (versionInfo.Length != 2)
            {
                $"Offset Update disabled".Info("Farsight");
                return false;
            }
            string offsetDlLocation = versionInfo[0];
            StringVersion offsetVersion = StringVersion.Parse(versionInfo[1]);


            try
            {

                string remoteContent = await RestRequester.GetRaw(offsetDlLocation);
                FarsightConfig? remoteCfg = JsonSerializer.Deserialize<FarsightConfig>(remoteContent);

                if (remoteCfg is null || remoteContent.Length == 0)
                {
                    $"Updated offsets not found".Error("Farsight");
                    return false;
                }
                remoteCfg.CopyProperties(this);

                $"Offsets updated to {OffsetVersion}".Info("Farsight");

                return true;
            }

            catch (Exception e)
            {
                $"Could not update offsets: \n {e.Message}".Warn("Farsight");
                return false;
            }
        }

        public override void RevertToDefault()
        {
            "verifying game offset version".UpdateStartupProgressText();
            var result = Task.Run(() => GetLatestFarsightPatchLocation()).Result;
            "fetching updated offsets".UpdateStartupProgressText();
            Task t = Task.Run(() => RevertToDefaultAsync(result));
            t.Wait();
        }
    }

    public class Offsets
    {
        public GlobalOffsets? Global { get; set; }
        public GameObjectOffsets? GameObject { get; set; }
    }
}
