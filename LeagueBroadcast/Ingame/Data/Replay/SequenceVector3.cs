using System.Text.Json.Serialization;

namespace LeagueBroadcast.Ingame.Data.Replay
{
    public class SequenceVector3
    {
        [JsonPropertyName("blend")]
        public string Blend { get; set; }

        [JsonPropertyName("time")]
        public double Time { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}
