using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace LeagueBroadcastHub.Data.Replay
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
