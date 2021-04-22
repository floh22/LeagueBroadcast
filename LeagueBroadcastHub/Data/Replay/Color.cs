using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace LeagueBroadcastHub.Data.Replay
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
