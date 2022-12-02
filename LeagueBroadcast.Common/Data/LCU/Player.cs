using LeagueBroadcast.Common.Json;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.LCU
{
    public class Player
    {
        [JsonPropertyName("accountId")]
        public int AccountId { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = "";

        [JsonPropertyName("internalName")]
        public string InternalName { get; set; } = "";

        [JsonPropertyName("nameChangeFlag")]
        public bool NameChangeFlag { get; set; }

        [JsonPropertyName("percentCompleteForNextLevel")]
        public int PercentCompleteForNextLevel { get; set; }

        [JsonPropertyName("privacy")]
        public string Privacy { get; set; } = "PRIVATE";

        [JsonPropertyName("profileIconId")]
        public int ProfileIconId { get; set; }

        [JsonPropertyName("puuid")]
        public string Puuid { get; set; } = "";

        [JsonPropertyName("rerollPoints")]
        public RerollPoints RerollPoints { get; set; } = new();


        [JsonConverter(typeof(NumberToStringJsonConverter))]
        [JsonPropertyName("summonerId")]
        public string SummonerId { get; set; } = "";

        [JsonPropertyName("summonerLevel")]
        public int SummonerLevel { get; set; }

        [JsonPropertyName("unnamed")]
        public bool Unnamed { get; set; }

        [JsonPropertyName("xpSinceLastLevel")]
        public int XpSinceLastLevel { get; set; }

        [JsonPropertyName("xpUntilNextLevel")]
        public int XpUntilNextLevel { get; set; }
    }


    public class RerollPoints
    {
        [JsonPropertyName("currentPoints")]
        public int CurrentPoints { get; set; }

        [JsonPropertyName("maxRolls")]
        public int MaxRolls { get; set; }

        [JsonPropertyName("numberOfRolls")]
        public int NumberOfRolls { get; set; }

        [JsonPropertyName("pointsCostToRoll")]
        public int PointsCostToRoll { get; set; }

        [JsonPropertyName("pointsToReroll")]
        public int PointsToReroll { get; set; }
    }
}
