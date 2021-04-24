using System.Threading.Tasks;

namespace LCUSharp.Http.Endpoints
{
    /// <summary>
    /// Handles operations relating to the process-control endpoint.
    /// </summary>
    public interface IProcessControlEndpoint
    {
        /// <summary>
        /// Quits the league client processes.
        /// </summary>
        Task QuitAsync();

        /// <summary>
        /// Restarts the league client processes.
        /// </summary>
        /// <param name="delaySeconds">The amount of time to wait before restarting.</param>
        Task RestartAsync(int delaySeconds);

        /// <summary>
        /// Restarts the league client processes.
        /// </summary>
        /// <param name="delaySeconds">The amount of time to wait before restarting.</param>
        /// <param name="restartVersion">The client version to use on restart.</param>
        Task RestartAsync(int delaySeconds, int restartVersion);

        /// <summary>
        /// Restarts the league client processes to repair the client.
        /// </summary>
        Task RestartToRepair();

        /// <summary>
        /// Restarts the league client processes to update.
        /// </summary>
        /// <param name="delaySeconds">The amount of time to wait before restarting.</param>
        /// <param name="selfUpdateUrl">The update source url.</param>
        Task RestartToUpdate(int delaySeconds, string selfUpdateUrl);
    }
}
