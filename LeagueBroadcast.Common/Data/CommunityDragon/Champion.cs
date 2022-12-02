using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.CommunityDragon
{
    public class Champion
    {
        public static HashSet<Champion> All { get; set; } = new();

        [JsonPropertyName("id")]
        public int ID { get; set; } = 0;

        [JsonPropertyName("alias")]
        public string Alias { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("splashImg")]
        public string? SplashImg { get; set; }

        [JsonPropertyName("splashCenteredImg")]
        public string? SplashCenteredImg { get; set; }

        [JsonPropertyName("loadingImg")]
        public string? LoadingImg { get; set; }

        [JsonPropertyName("squareImg")]
        public string? SquareImg { get; set; }
    }
}
