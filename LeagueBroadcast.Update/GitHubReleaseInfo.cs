using Newtonsoft.Json;

namespace LeagueBroadcast.Update
{
    public sealed class GitHubReleaseInfo
    {
        [JsonProperty("tag_name")]
        public string Version { get; set; }

        [JsonProperty("html_url")]
        public string Url { get; set; }

        [JsonProperty("assets")]
        public GitHubReleaseAsset[] Assets { get; set; }
    }
}
