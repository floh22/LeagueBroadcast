
using LeagueBroadcast.Common.Data.Config;
using Newtonsoft.Json;

namespace LeagueBroadcast.ChampSelect.Data.Config
{
    public class PickBanConfig : JSONConfig
    {
        public FrontendConfig frontend = FrontendConfig.CreateDefaultConfig();
        public string contentPatch = "";
        public string contentCdn = "";

        [JsonIgnore]
        public override string Name => "PickBan";
        public override string FileVersion { get => _fileVersion; set => _fileVersion = value; }

        [JsonIgnore]
        public static new string CurrentVersion => "1.0";

        public override string GETJson()
        {
            return SerializeIndented(this);
        }

        public override void UpdateValues(string readValues)
        {
            var Cfg = JsonConvert.DeserializeObject<PickBanConfig>(readValues);
            frontend = Cfg.frontend;
            contentPatch = Cfg.contentPatch;
            contentCdn = Cfg.contentCdn;
            FileVersion = Cfg.FileVersion;
        }

        public override string GETDefaultString()
        {
            return JsonConvert.SerializeObject(CreateDefault(), Formatting.Indented);
        }

        public override void RevertToDefault()
        {
            var def = CreateDefault();
            this.contentCdn = def.contentCdn;
            this.contentPatch = def.contentPatch;
            this.frontend = def.frontend;
            this.FileVersion = CurrentVersion;
        }

        private PickBanConfig CreateDefault()
        {
            return new PickBanConfig() { FileVersion = CurrentVersion, frontend = FrontendConfig.CreateDefaultConfig(), contentPatch = "latest", contentCdn = "https://ddragon.leagueoflegends.com/cdn" };
        }

        public override void UpdateConfigVersion(string oldVersion, string oldValues)
        {
            //No format change to correct
            return;
        }

        public override string GETCurrentVersion()
        {
            return CurrentVersion;
        }
    }
}
