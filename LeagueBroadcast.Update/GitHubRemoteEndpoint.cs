using LeagueBroadcast.Common;
using LeagueBroadcast.Update.Http;
using System.Threading.Tasks;

namespace LeagueBroadcast.Update
{
    public class GitHubRemoteEndpoint
    {
        private const string ReleaseUrl = @"https://api.github.com/repos/{0}/releases/latest";

#nullable enable
        public static async Task<GitHubReleaseInfo?> GetLatestReleaseAsync(string repositoryName)
        {
            string releaseLocation = string.Format(ReleaseUrl, repositoryName);
            return await RestRequester.GetAsync<GitHubReleaseInfo>(releaseLocation);
        }
#nullable disable
    }
}
