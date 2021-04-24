using System.Threading.Tasks;

namespace LCUSharp.Http.Endpoints
{
    /// <summary>
    /// Handles operations relating to the riotclient endpoint.
    /// </summary>
    public interface IRiotClientEndpoint
    {
        /// <summary>
        /// Minimizes the client.
        /// </summary>
        Task MinimizeUxAsync();

        /// <summary>
        /// Show the client.
        /// </summary>
        Task ShowUxAsync();

        /// <summary>
        /// Flashes the client's icon on the taskbar.
        /// </summary>
        Task FlashUxAsync();

        /// <summary>
        /// Kills the client ux.
        /// </summary>
        Task KillUxAsync();

        /// <summary>
        /// Kills and restarts the client ux.
        /// </summary>
        Task KillAndRestartUxAsync();

        /// <summary>
        /// Kills the client ux and resets the state (logs out user, etc.).
        /// </summary>
        Task UnloadUxAsync();

        /// <summary>
        /// Launches the client ux.
        /// </summary>
        Task LaunchUxAsync();

        /// <summary>
        /// Gets the client ux zoom scale.
        /// </summary>
        /// <returns>The zoom scale.</returns>
        Task<double> GetZoomScaleAsync();

        /// <summary>
        /// Sets the client ux zoom scale.
        /// </summary>
        /// <param name="scale">The zoom scale.</param>
        /// <returns></returns>
        Task SetZoomScaleAsync(double scale);
    }
}
