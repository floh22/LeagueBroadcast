using System.Text.Json.Serialization;

namespace LeagueBroadcast.Ingame.Data.Replay
{
    public class Color
    {
        [JsonPropertyName("a")]
        public double A { get; set; }

        [JsonPropertyName("b")]
        public double B { get; set; }

        [JsonPropertyName("g")]
        public double G { get; set; }

        [JsonPropertyName("r")]
        public double R { get; set; }
    }
}
