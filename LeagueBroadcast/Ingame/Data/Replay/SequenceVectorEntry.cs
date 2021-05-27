using System.Text.Json.Serialization;

namespace LeagueBroadcast.Ingame.Data.Replay
{
    public class SequenceVectorEntry
    {
        [JsonPropertyName("blend")]
        public string Blend { get; set; }

        [JsonPropertyName("time")]
        public double Time { get; set; }

        [JsonPropertyName("value")]
        public Vector3 Value { get; set; }
    }
}
