using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace LeagueBroadcastHub.Data.Replay
{
    public class Vector3
    {
        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("z")]
        public double Z { get; set; }
    }
}
