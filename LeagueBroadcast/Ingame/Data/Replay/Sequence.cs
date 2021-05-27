using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Ingame.Data.Replay
{
    public class DepthFogColor
    {
        [JsonPropertyName("blend")]
        public string Blend { get; set; }

        [JsonPropertyName("time")]
        public double Time { get; set; }

        [JsonPropertyName("value")]
        public Color Value { get; set; }
    }

    public class DepthFogEnabled
    {
        [JsonPropertyName("blend")]
        public string Blend { get; set; }

        [JsonPropertyName("time")]
        public double Time { get; set; }

        [JsonPropertyName("value")]
        public bool Value { get; set; }
    }

    public class DepthOfFieldEnabled
    {
        [JsonPropertyName("blend")]
        public string Blend { get; set; }

        [JsonPropertyName("time")]
        public double Time { get; set; }

        [JsonPropertyName("value")]
        public bool Value { get; set; }
    }

    public class HeightFogColor
    {
        [JsonPropertyName("blend")]
        public string Blend { get; set; }

        [JsonPropertyName("time")]
        public double Time { get; set; }

        [JsonPropertyName("value")]
        public Color Value { get; set; }
    }

    public class HeightFogEnabled
    {
        [JsonPropertyName("blend")]
        public string Blend { get; set; }

        [JsonPropertyName("time")]
        public double Time { get; set; }

        [JsonPropertyName("value")]
        public bool Value { get; set; }
    }

    public class Sequence
    {
        [JsonPropertyName("cameraPosition")]
        public List<SequenceVectorEntry> CameraPosition { get; set; }

        [JsonPropertyName("cameraRotation")]
        public List<SequenceVectorEntry> CameraRotation { get; set; }

        [JsonPropertyName("depthFogColor")]
        public List<DepthFogColor> DepthFogColor { get; set; }

        [JsonPropertyName("depthFogEnabled")]
        public List<DepthFogEnabled> DepthFogEnabled { get; set; }

        [JsonPropertyName("depthFogEnd")]
        public List<SequenceVector3> DepthFogEnd { get; set; }

        [JsonPropertyName("depthFogIntensity")]
        public List<SequenceVector3> DepthFogIntensity { get; set; }

        [JsonPropertyName("depthFogStart")]
        public List<SequenceVector3> DepthFogStart { get; set; }

        [JsonPropertyName("depthOfFieldCircle")]
        public List<SequenceVector3> DepthOfFieldCircle { get; set; }

        [JsonPropertyName("depthOfFieldEnabled")]
        public List<DepthOfFieldEnabled> DepthOfFieldEnabled { get; set; }

        [JsonPropertyName("depthOfFieldFar")]
        public List<SequenceVector3> DepthOfFieldFar { get; set; }

        [JsonPropertyName("depthOfFieldMid")]
        public List<SequenceVector3> DepthOfFieldMid { get; set; }

        [JsonPropertyName("depthOfFieldNear")]
        public List<SequenceVector3> DepthOfFieldNear { get; set; }

        [JsonPropertyName("depthOfFieldWidth")]
        public List<SequenceVector3> DepthOfFieldWidth { get; set; }

        [JsonPropertyName("farClip")]
        public List<SequenceVector3> FarClip { get; set; }

        [JsonPropertyName("fieldOfView")]
        public List<SequenceVector3> FieldOfView { get; set; }

        [JsonPropertyName("heightFogColor")]
        public List<HeightFogColor> HeightFogColor { get; set; }

        [JsonPropertyName("heightFogEnabled")]
        public List<HeightFogEnabled> HeightFogEnabled { get; set; }

        [JsonPropertyName("heightFogEnd")]
        public List<SequenceVector3> HeightFogEnd { get; set; }

        [JsonPropertyName("heightFogIntensity")]
        public List<SequenceVector3> HeightFogIntensity { get; set; }

        [JsonPropertyName("heightFogStart")]
        public List<SequenceVector3> HeightFogStart { get; set; }

        [JsonPropertyName("navGridOffset")]
        public List<SequenceVector3> NavGridOffset { get; set; }

        [JsonPropertyName("nearClip")]
        public List<SequenceVector3> NearClip { get; set; }

        [JsonPropertyName("playbackSpeed")]
        public List<SequenceVector3> PlaybackSpeed { get; set; }

        [JsonPropertyName("skyboxOffset")]
        public List<SequenceVector3> SkyboxOffset { get; set; }

        [JsonPropertyName("skyboxRadius")]
        public List<SequenceVector3> SkyboxRadius { get; set; }

        [JsonPropertyName("skyboxRotation")]
        public List<SequenceVector3> SkyboxRotation { get; set; }

        [JsonPropertyName("sunDirection")]
        public List<Vector3> SunDirection { get; set; }
    }
}
