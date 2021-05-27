using System.Text.Json.Serialization;

namespace LeagueBroadcast.Ingame.Data.Replay
{
    public class Render
    {
        [JsonPropertyName("cameraAttached")]
        public bool CameraAttached { get; set; }

        [JsonPropertyName("cameraLookSpeed")]
        public double CameraLookSpeed { get; set; }

        [JsonPropertyName("cameraMode")]
        public string CameraMode { get; set; }

        [JsonPropertyName("cameraMoveSpeed")]
        public double CameraMoveSpeed { get; set; }

        [JsonPropertyName("cameraPosition")]
        public Vector3 CameraPosition { get; set; }

        [JsonPropertyName("cameraRotation")]
        public Vector3 CameraRotation { get; set; }

        [JsonPropertyName("characters")]
        public bool Characters { get; set; }

        [JsonPropertyName("depthFogColor")]
        public Color DepthFogColor { get; set; }

        [JsonPropertyName("depthFogEnabled")]
        public bool DepthFogEnabled { get; set; }

        [JsonPropertyName("depthFogEnd")]
        public double DepthFogEnd { get; set; }

        [JsonPropertyName("depthFogIntensity")]
        public double DepthFogIntensity { get; set; }

        [JsonPropertyName("depthFogStart")]
        public double DepthFogStart { get; set; }

        [JsonPropertyName("depthOfFieldCircle")]
        public double DepthOfFieldCircle { get; set; }

        [JsonPropertyName("depthOfFieldDebug")]
        public bool DepthOfFieldDebug { get; set; }

        [JsonPropertyName("depthOfFieldEnabled")]
        public bool DepthOfFieldEnabled { get; set; }

        [JsonPropertyName("depthOfFieldFar")]
        public double DepthOfFieldFar { get; set; }

        [JsonPropertyName("depthOfFieldMid")]
        public double DepthOfFieldMid { get; set; }

        [JsonPropertyName("depthOfFieldNear")]
        public double DepthOfFieldNear { get; set; }

        [JsonPropertyName("depthOfFieldWidth")]
        public double DepthOfFieldWidth { get; set; }

        [JsonPropertyName("environment")]
        public bool Environment { get; set; }

        [JsonPropertyName("farClip")]
        public double FarClip { get; set; }

        [JsonPropertyName("fieldOfView")]
        public double FieldOfView { get; set; }

        [JsonPropertyName("floatingText")]
        public bool FloatingText { get; set; }

        [JsonPropertyName("fogOfWar")]
        public bool FogOfWar { get; set; }

        [JsonPropertyName("healthBarChampions")]
        public bool HealthBarChampions { get; set; }

        [JsonPropertyName("healthBarMinions")]
        public bool HealthBarMinions { get; set; }

        [JsonPropertyName("healthBarPets")]
        public bool HealthBarPets { get; set; }

        [JsonPropertyName("healthBarStructures")]
        public bool HealthBarStructures { get; set; }

        [JsonPropertyName("healthBarWards")]
        public bool HealthBarWards { get; set; }

        [JsonPropertyName("heightFogColor")]
        public Color HeightFogColor { get; set; }

        [JsonPropertyName("heightFogEnabled")]
        public bool HeightFogEnabled { get; set; }

        [JsonPropertyName("heightFogEnd")]
        public double HeightFogEnd { get; set; }

        [JsonPropertyName("heightFogIntensity")]
        public double HeightFogIntensity { get; set; }

        [JsonPropertyName("heightFogStart")]
        public double HeightFogStart { get; set; }

        [JsonPropertyName("interfaceAll")]
        public bool InterfaceAll { get; set; }

        [JsonPropertyName("interfaceAnnounce")]
        public bool InterfaceAnnounce { get; set; }

        [JsonPropertyName("interfaceChat")]
        public bool InterfaceChat { get; set; }

        [JsonPropertyName("interfaceFrames")]
        public bool InterfaceFrames { get; set; }

        [JsonPropertyName("interfaceMinimap")]
        public bool InterfaceMinimap { get; set; }

        [JsonPropertyName("interfaceQuests")]
        public bool InterfaceQuests { get; set; }

        [JsonPropertyName("interfaceReplay")]
        public bool InterfaceReplay { get; set; }

        [JsonPropertyName("interfaceScore")]
        public bool InterfaceScore { get; set; }

        [JsonPropertyName("interfaceScoreboard")]
        public bool InterfaceScoreboard { get; set; }

        [JsonPropertyName("interfaceTarget")]
        public bool InterfaceTarget { get; set; }

        [JsonPropertyName("interfaceTimeline")]
        public bool InterfaceTimeline { get; set; }

        [JsonPropertyName("navGridOffset")]
        public double NavGridOffset { get; set; }

        [JsonPropertyName("nearClip")]
        public double NearClip { get; set; }

        [JsonPropertyName("outlineHover")]
        public bool OutlineHover { get; set; }

        [JsonPropertyName("outlineSelect")]
        public bool OutlineSelect { get; set; }

        [JsonPropertyName("particles")]
        public bool Particles { get; set; }

        [JsonPropertyName("skyboxOffset")]
        public double SkyboxOffset { get; set; }

        [JsonPropertyName("skyboxPath")]
        public string SkyboxPath { get; set; }

        [JsonPropertyName("skyboxRadius")]
        public double SkyboxRadius { get; set; }

        [JsonPropertyName("skyboxRotation")]
        public double SkyboxRotation { get; set; }

        [JsonPropertyName("sunDirection")]
        public Vector3 SunDirection { get; set; }
    }
}
