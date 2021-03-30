using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LCUSharp.Http.Endpoints
{
    /// <inheritdoc cref="IProcessControlEndpoint"/>
    internal class ProcessControlEndpoint : EndpointBase, IProcessControlEndpoint
    {
        private const string BaseUrl = "process-control/v1/process/";

        /// <summary>
        /// Initializes a new instance of the <see cref="RiotClientEndpoint"/> class.
        /// </summary>
        public ProcessControlEndpoint(ILeagueRequestHandler requestHandler)
            : base(requestHandler)
        {
        }

        /// <inheritdoc />
        public async Task QuitAsync()
        {
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}quit").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task RestartAsync(int delaySeconds)
        {
            var queryParameters = new List<string>();
            await RestartAsync(delaySeconds, queryParameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task RestartAsync(int delaySeconds, int restartVersion)
        {
            var queryParameters = new List<string> { $"restartVersion={restartVersion}" };
            await RestartAsync(delaySeconds, queryParameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task RestartToRepair()
        {
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}restart-to-repair").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task RestartToUpdate(int delaySeconds, string selfUpdateUrl)
        {
            var queryParameters = new string[]
            {
                $"delaySeconds={delaySeconds}",
                $"selfUpdateUrl={selfUpdateUrl}"
            };
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}restart-to-update", queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Restarts the league client process.
        /// </summary>
        /// <param name="delaySeconds">The amount of time to wait before restarting.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <returns></returns>
        private async Task RestartAsync(int delaySeconds, ICollection<string> queryParameters)
        {
            queryParameters.Add($"delaySeconds={delaySeconds}");
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}restart", queryParameters).ConfigureAwait(false);
        }
    }
}
