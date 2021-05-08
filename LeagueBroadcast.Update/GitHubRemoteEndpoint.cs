using LeagueBroadcast.Update.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Update
{
    public class GitHubRemoteEndpoint
    {
        private static TimeSpan RequestTimeout { get; } = TimeSpan.FromSeconds(2);
        private const string ReleaseUrl = @"https://api.github.com/repos/{0}/releases/latest";

        private static GitHubRemoteEndpoint? _instance;

        public static GitHubRemoteEndpoint Instance
        {
            get => _instance ??= new GitHubRemoteEndpoint();
        }

        private RestRequester Requester { get; }

        private GitHubRemoteEndpoint()
        {
            Requester = new RestRequester(RequestTimeout);
        }
#nullable enable
        public async Task<GitHubReleaseInfo?> GetLatestReleaseAsync(string repositoryName)
        {
            return await Requester.GetAsync<GitHubReleaseInfo?>(string.Format(ReleaseUrl, repositoryName));
        }
#nullable disable
    }
}
