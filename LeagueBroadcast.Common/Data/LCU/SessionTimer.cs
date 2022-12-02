using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.LCU
{
    public class SessionTimer
    {
        [JsonPropertyName("adjustedTimeLeftInPhase")]
        public int AdjustedTimeLeftInPhase { get; set; }
        [JsonPropertyName("adjustedTimeLeftInPhaseInSec")]
        public int AdjustedTimeLeftInPhaseInSec { get; set; }
        [JsonPropertyName("internalNowInEpochMs")]
        public long InternalNowInEpochMs { get; set; }
        [JsonPropertyName("phase")]
        public string Phase { get; set; } = "";
        [JsonPropertyName("timeLeftInPhase")]
        public int TimeLeftInPhase { get; set; }
        [JsonPropertyName("timeLeftInPhaseInSec")]
        public int TimeLeftInPhaseInSec { get; set; }
        [JsonPropertyName("totaTimeInPhase")]
        public int TotalTimeInPhase { get; set; }
    }
}
