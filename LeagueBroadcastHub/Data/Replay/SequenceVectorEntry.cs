using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace LeagueBroadcastHub.Data.Replay
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
