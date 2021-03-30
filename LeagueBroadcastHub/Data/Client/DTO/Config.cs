using LeagueBroadcastHub.Data.Provider;
using LeagueIngameServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Data.Client.DTO
{
    public class Config
    {
        [JsonIgnore]
        public const string FileVersion = "1.0";

        public string fileVersion;

        public FrontendConfig frontend = FrontendConfig.CreateDefaultConfig();
        public string contentPatch = "";
        public string contentCdn = "";

        public static Config CreateDefaultConfig()
        {
            return new Config() { fileVersion = FileVersion, frontend = FrontendConfig.CreateDefaultConfig(), contentPatch = "latest", contentCdn= "https://ddragon.leagueoflegends.com/cdn" };
        }
    }
}
