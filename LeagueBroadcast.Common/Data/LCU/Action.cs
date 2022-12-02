using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.LCU
{
    public class Action
    {
        [JsonPropertyName("completed")]
        public bool Completed { get; set; }
        [JsonPropertyName("championId")]
        public int ChampionID { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";
        [JsonPropertyName("actorCellId")]
        public int ActorCellId { get; set; }
    }

    public class ActionType
    {
        public ActionType(string value) { Value = value; }

        public string Value { get; set; }

        public static ActionType PICK => new("pick");
        public static ActionType BAN => new("ban");
    }
}
