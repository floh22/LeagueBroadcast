using LCUSharp.Http;
using LCUSharp.Http.Endpoints;
using LCUSharp.Utility;
using LCUSharp.Websocket;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LCUSharp
{
    /// <inheritdoc />
    public class LeagueClientApi : ILeagueClientApi
    {
        private static readonly LeagueProcessHandler _processHandler;
        private static readonly LockFileHandler _lockFileHandler;

        /// <inheritdoc />
        public event EventHandler Disconnected;

        /// <inheritdoc />
        public ILeagueRequestHandler RequestHandler { get; }

        /// <inheritdoc />
        public ILeagueEventHandler EventHandler { get; }

        /// <inheritdoc />
        public IRiotClientEndpoint RiotClientEndpoint { get; }

        /// <inheritdoc />
        public IProcessControlEndpoint ProcessControlEndpoint { get; }

        /// <summary>
        /// Initializes the necessary handlers.
        /// </summary>
        static LeagueClientApi()
        {
            _processHandler = new LeagueProcessHandler();
            _lockFileHandler = new LockFileHandler();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeagueClientApi"/> class.
        /// </summary>
        /// <param name="port">The league client API's port.</param>
        /// <param name="token">The authentication token.</param>
        /// <param name="eventHandler">The event handler.</param>
        private LeagueClientApi(int port, string token, ILeagueEventHandler eventHandler)
        {
            EventHandler = eventHandler;
            RequestHandler = new LeagueRequestHandler(port, token);
            RiotClientEndpoint = new RiotClientEndpoint(RequestHandler);
            ProcessControlEndpoint = new ProcessControlEndpoint(RequestHandler);
            _processHandler.Closed += OnDisconnected;
        }

        /// <summary>
        /// Connects to the league client api.
        /// </summary>
        /// <returns>A new instance of <see cref="LeagueClientApi" /> that's connected to the client api.</returns>
        public static async Task<LeagueClientApi> ConnectAsync()
        {
            var (port, token) = await GetAuthCredentialsAsync().ConfigureAwait(false);
            var eventHandler = new LeagueEventHandler(port, token);
            var api = new LeagueClientApi(port, token, eventHandler);
            return await EnsureConnectionAsync(api).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task ReconnectAsync()
        {
            var (port, token) = await GetAuthCredentialsAsync().ConfigureAwait(false);

            RequestHandler.ChangeSettings(port, token);
            EventHandler.ChangeSettings(port, token);

            await EnsureConnectionAsync(this).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            OnDisconnected(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the league client api authentication credentials.
        /// </summary>
        /// <returns>The port and auth token.</returns>
        private static async Task<(int port, string token)> GetAuthCredentialsAsync()
        {
            await _processHandler.WaitForProcessAsync().ConfigureAwait(false);
            return await _lockFileHandler.ParseLockFileAsync(_processHandler.ExecutablePath).ConfigureAwait(false);
        }

        /// <summary>
        /// Ensures the connection is successful by sending a test request.
        /// </summary>
        /// <param name="api">The league client api.</param>
        /// <returns>The league client api.</returns>
        private static async Task<LeagueClientApi> EnsureConnectionAsync(LeagueClientApi api)
        {
            while (true)
            {
                try
                {
                    await api.RequestHandler
                        .GetResponseAsync<string>(HttpMethod.Get, "/riotclient/app-name")
                        .ConfigureAwait(false);

                    var delay = Task.Delay(200);
                    var connect = api.EventHandler.ConnectAsync();

                    var finished = await Task.WhenAny(delay, connect).ConfigureAwait(false);
                    if (finished == delay)
                    {
                        continue;
                    }

                    return api;
                }
                catch (Exception)
                {
                    await Task.Delay(200).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Invoked when the client is disconnected from the api.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnDisconnected(object sender, EventArgs e)
        {
            await EventHandler.DisconnectAsync().ConfigureAwait(false);
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}
