using Common.Http;
using LeagueBroadcast.Utils.Log;
using System.Threading.Tasks;

namespace LeagueBroadcast.Update.GitHub
{
    public class GitHubRemoteEndpoint
    {
        private const string ReleaseUrl = @"https://api.github.com/repos/{0}/releases/latest";

#nullable enable
        public static async Task<GitHubReleaseInfo?> GetLatestReleaseAsync(string repositoryName)
        {
            string releaseLocation = string.Format(ReleaseUrl, repositoryName);
            $"[Update] Getting latest release from {releaseLocation}".Info();
            return await RestRequester.GetAsync<GitHubReleaseInfo>(releaseLocation);
        }
#nullable disable
    }
}
