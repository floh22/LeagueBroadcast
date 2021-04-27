using Newtonsoft.Json;

namespace LeagueBroadcast.Update
{
    public sealed class GitHubReleaseAsset
    {
        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("browser_download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
