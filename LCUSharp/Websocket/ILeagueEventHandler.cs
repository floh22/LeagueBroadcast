using System;
using System.Threading.Tasks;

namespace LCUSharp.Websocket
{
    /// <summary>
    /// Handles operations relating to capturing and processing league events via WebSockets.
    /// </summary>
    public interface ILeagueEventHandler
    {
        /// <summary>
        /// EventHandler used for the Websocket's received messages.
        /// </summary>
        EventHandler<LeagueEvent> MessageReceived { get; set; }

        /// <summary>
        /// EventHandler used for the Websocket's error messages.
        /// </summary>
        EventHandler<string> ErrorReceived { get; set; }

        /// <summary>
        /// Connects to the WebSocket server.
        /// </summary>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects from the WebSocket server.
        /// </summary>
        Task<bool> DisconnectAsync();

        /// <summary>
        /// Initializes the web socket listener.
        /// </summary>
        /// <param name="port">The league client's port.</param>
        /// <param name="token">The user's Basic authentication token.</param>
        void ChangeSettings(int port, string token);

        /// <summary>
        /// Subscribes the event handler to the specified event uri.
        /// </summary>
        /// <param name="uri">The league client event uri.</param>
        /// <param name="eventHandler">The event handler.</param>
        void Subscribe(string uri, EventHandler<LeagueEvent> eventHandler);

        /// <summary>
        /// Unsubscribes the event handlers subscribed to the specified event uri.
        /// </summary>
        /// <param name="uri">The uri to unsubscribe from.</param>
        /// <returns>True if the uri exists and was unsubscribed successfully, otherwise false.</returns>
        bool Unsubscribe(string uri);

        /// <summary>
        /// Unsubcribes all event handlers.
        /// </summary>
        void UnsubscribeAll();
    }
}
