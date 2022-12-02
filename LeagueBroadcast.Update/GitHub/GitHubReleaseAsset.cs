using System.Text.Json.Serialization;

namespace LeagueBroadcast.Update.GitHub
{
    public sealed class GitHubReleaseAsset
    {
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; } = "";

        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}
