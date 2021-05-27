using System.Text.Json.Serialization;

namespace LeagueBroadcast.Ingame.Data.Replay
{
    public class Recording
    {
        [JsonPropertyName("codec")]
        public string Codec { get; set; }

        [JsonPropertyName("currentTime")]
        public double CurrentTime { get; set; }

        [JsonPropertyName("endTime")]
        public double EndTime { get; set; }

        [JsonPropertyName("enforceFrameRate")]
        public bool EnforceFrameRate { get; set; }

        [JsonPropertyName("framesPerSecond")]
        public int FramesPerSecond { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("lossless")]
        public bool Lossless { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("recording")]
        public bool IsRecording { get; set; }

        [JsonPropertyName("replaySpeed")]
        public double ReplaySpeed { get; set; }

        [JsonPropertyName("startTime")]
        public double StartTime { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}
