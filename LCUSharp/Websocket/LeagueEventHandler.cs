using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Websocket.Client;

namespace LCUSharp.Websocket
{
    /// <inheritdoc />
    internal class LeagueEventHandler : ILeagueEventHandler
    {
        private const int ClientEventData = 2;
        private const int ClientEventNumber = 8;

        /// <summary>
        /// Contains all event handlers that are subscribed to an event uri.
        /// </summary>
        private readonly Dictionary<string, List<EventHandler<LeagueEvent>>> _subscribers;

        /// <summary>
        /// Websocket client used to connect to the League Client's backend.
        /// </summary>
        private WebsocketClient _webSocket;

        /// <inheritdoc />
        public EventHandler<LeagueEvent> MessageReceived { get; set; }

        /// <inheritdoc />
        public EventHandler<string> ErrorReceived { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="LeagueEventHandler"/> class.
        /// </summary>
        /// <param name="port">The league client's port.</param>
        /// <param name="token">The user's Basic authentication token.</param>
        public LeagueEventHandler(int port, string token)
        {
            _subscribers = new Dictionary<string, List<EventHandler<LeagueEvent>>>();
            ChangeSettings(port, token);
        }

        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            await _webSocket.Start();
            await _webSocket.SendInstant("[5, \"OnJsonApiEvent\"]");
        }

        /// <inheritdoc />
        public async Task<bool> DisconnectAsync()
        {
            return await _webSocket.Stop(WebSocketCloseStatus.NormalClosure,
                "User requested to close the connection.");
        }

        /// <inheritdoc />
        public void ChangeSettings(int port, string token)
        {
            _webSocket = new WebsocketClient(new Uri($"wss://127.0.0.1:{port}/"), () =>
            {
                var socket = new ClientWebSocket
                {
                    Options =
                    {
                        Credentials = new NetworkCredential("riot", token),
                        RemoteCertificateValidationCallback =
                            (sender, cert, chain, sslPolicyErrors) => true,
                    }
                };

                socket.Options.AddSubProtocol("wamp");
                return socket;
            });

            _webSocket
                .MessageReceived
                .Where(msg => msg.Text != null)
                .Where(msg => msg.Text.StartsWith('['))
                .Subscribe(msg =>
                {
                    // Check if the message is json received from the client
                    var eventArray = JArray.Parse(msg.Text);
                    var eventNumber = eventArray?[0].ToObject<int>();

                    if (eventNumber != ClientEventNumber)
                    {
                        return;
                    }

                    var leagueEvent = eventArray[ClientEventData].ToObject<LeagueEvent>();
                    MessageReceived?.Invoke(this, leagueEvent);

                    if (!_subscribers.TryGetValue(leagueEvent.Uri, out var eventHandlers))
                    {
                        return;
                    }

                    eventHandlers.ForEach(eventHandler => eventHandler?.Invoke(this, leagueEvent));
                });
        }

        /// <inheritdoc />
        public void Subscribe(string uri, EventHandler<LeagueEvent> eventHandler)
        {
            if (_subscribers.TryGetValue(uri, out var eventHandlers))
            {
                eventHandlers.Add(eventHandler);
            }
            else
            {
                _subscribers.Add(uri, new List<EventHandler<LeagueEvent>> { eventHandler });
            }
        }

        /// <inheritdoc />
        public bool Unsubscribe(string uri)
        {
            return _subscribers.Remove(uri);
        }

        /// <inheritdoc />
        public void UnsubscribeAll()
        {
            _subscribers.Clear();
        }
    }
}
