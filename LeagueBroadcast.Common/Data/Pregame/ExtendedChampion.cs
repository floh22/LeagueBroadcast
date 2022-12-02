using LeagueBroadcast.Common.Data.CommunityDragon;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame
{
    public class ExtendedChampion : Champion
    {
        [JsonPropertyName("role")]
        public Role Role { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("tier")]
        public int Tier { get; set; }

        [JsonPropertyName("winrate")]
        public float Winrate { get; set; }

        [JsonPropertyName("pickrate")]
        public float Pickrate { get; set; }

        [JsonPropertyName("banrate")]
        public float Banrate { get; set; }
    }


    public static class ChampionExtensions
    {
        public static ExtendedChampion ExtendWithPlayRateData(this Champion c, Role role)
        {
            var ec = (ExtendedChampion)c;

            //scrape op.gg in role for champion. If they dont want to provide an API guess ill make one myself

            return ec;
        }
    }

}
