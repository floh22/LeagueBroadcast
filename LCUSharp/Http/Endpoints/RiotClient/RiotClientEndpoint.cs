using System.Net.Http;
using System.Threading.Tasks;

namespace LCUSharp.Http.Endpoints
{
    /// <inheritdoc cref="IRiotClientEndpoint"/>
    internal class RiotClientEndpoint : EndpointBase, IRiotClientEndpoint
    {
        private const string BaseUrl = "riotclient/";

        /// <summary>
        /// Initializes a new instance of the <see cref="RiotClientEndpoint"/> class.
        /// </summary>
        public RiotClientEndpoint(ILeagueRequestHandler requestHandler)
            : base(requestHandler)
        {
        }

        /// <inheritdoc />
        public async Task MinimizeUxAsync()
        {
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}ux-minimize").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task ShowUxAsync()
        {
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}ux-show").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task FlashUxAsync()
        {
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}ux-flash").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task KillUxAsync()
        {
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}kill-ux").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task KillAndRestartUxAsync()
        {
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}kill-and-restart-ux").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task UnloadUxAsync()
        {
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}unload").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task LaunchUxAsync()
        {
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}launch-ux").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<double> GetZoomScaleAsync()
        {
            return double.Parse(await RequestHandler.GetJsonResponseAsync(HttpMethod.Get, $"{BaseUrl}zoom-scale").ConfigureAwait(false));
        }

        /// <inheritdoc />
        public async Task SetZoomScaleAsync(double scale)
        {
            var queryParameters = new string[] { $"newZoomScale={scale}" };
            await RequestHandler.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}zoom-scale",   queryParameters).ConfigureAwait(false);
        }
    }
}
