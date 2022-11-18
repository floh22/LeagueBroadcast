
using LeagueBroadcast.Common.Utils;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.DTO
{

    //This is dumb but its a workaround
    public class SummonerSpell
    {
        public static HashSet<SummonerSpell> All { get; set; } = new();

        [JsonPropertyName("id")]
        [JsonConverter(typeof(NumberToStringJsonConverter))]
        public string ID { get; set; } = "";

        [JsonPropertyName("key")]
        public string Key => ID;

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("icon")]
        public string Icon => IconPath;

        [JsonPropertyName("iconPath")]
        public string IconPath { get; set; } = "";

        public FrontEndSummonerSpell AsFrontEndSummonerSpell()
        {
            return new FrontEndSummonerSpell
            {
                id = ID,
                name = Name,
                iconPath = IconPath,
            };
        }
    }

    public class FrontEndSummonerSpell
    {
        public string id { get; set; } = "";

        public string key => id;

        public string name { get; set; } = "";

        public string icon => iconPath;

        public string iconPath { get; set; } = "";
    }
}
