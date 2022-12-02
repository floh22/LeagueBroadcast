using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame
{
    public class Ban : PickBan
    {
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}
