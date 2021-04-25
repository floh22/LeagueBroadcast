using LeagueBroadcast.Common.Data.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public override void UpdateValues(dynamic readValues)
        {
            frontend = readValues.frontend;
            contentPatch = readValues.contentPatch;
            contentCdn = readValues.contentCdn;
        }

        public override string GETDefault()
        {
            return SerializeIndented(CreateDefault());
        }

        private PickBanConfig CreateDefault()
        {
            return new PickBanConfig() { FileVersion = CurrentVersion, frontend = FrontendConfig.CreateDefaultConfig(), contentPatch = "latest", contentCdn = "https://ddragon.leagueoflegends.com/cdn" };
        }

        public override void UpdateConfigVersion(string oldVersion, dynamic oldValues)
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
