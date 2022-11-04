using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.DTO
{
    public class Champion
    {
        public string id;
        public int key;
        public string name;
        public string splashImg;
        public string splashCenteredImg;
        public string loadingImg;
        public string squareImg;
    }

    public class CDragonChampion
    {
        public static HashSet<CDragonChampion> All { get; set; } = new();

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
