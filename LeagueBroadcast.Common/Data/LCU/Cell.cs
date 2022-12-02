using LeagueBroadcast.Common.Json;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.LCU
{
    public class Cell
    {
        [JsonPropertyName("cellId")]
        public int CellId { get; set; }
        [JsonPropertyName("championId")]
        public int ChampionId { get; set; }
        [JsonConverter(typeof(NumberToStringJsonConverter))]
        [JsonPropertyName("summonerId")]
        public string SummonerId { get; set; } = string.Empty;
        [JsonConverter(typeof(NumberToStringJsonConverter))]
        [JsonPropertyName("spell1Id")]
        public string Spell1Id { get; set; } = string.Empty;
        [JsonConverter(typeof(NumberToStringJsonConverter))]
        [JsonPropertyName("spell2Id")]
        public string Spell2Id { get; set; } = string.Empty;
    }
}
