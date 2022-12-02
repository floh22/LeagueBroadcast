using LeagueBroadcast.Common.Data.CommunityDragon;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame
{
    public class Pick : PickBan
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("spell1")]
        public SummonerSpell Spell1 { get; set; } = new();
        [JsonPropertyName("spell2")]
        public SummonerSpell Spell2 { get; set; } = new();
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = "";

        public Pick(int ID)
        {
            this.ID = ID;
        }
    }
}
