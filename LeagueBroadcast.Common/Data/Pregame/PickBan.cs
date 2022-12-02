using LeagueBroadcast.Common.Data.CommunityDragon;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame
{
    public class PickBan
    {
        [JsonPropertyName("champion")]
        public Champion? Champion { get; set; }
    }
}
