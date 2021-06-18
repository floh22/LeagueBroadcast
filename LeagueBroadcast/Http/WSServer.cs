using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO.WebSockets;
using LeagueBroadcast.ChampSelect.Events;
using LeagueBroadcast.ChampSelect.StateInfo;
using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Ingame.Events;
using LeagueBroadcast.OperatingSystem;
using Newtonsoft.Json;

namespace LeagueBroadcast.Http
{
    class WSServer : WebSocketModule
    {

        private List<IngameWSClient> clients;
        public WSServer(string urlPath) : base(urlPath, true)
        {
            this.clients = new();

            //Inform clients when overlay configs change
            ConfigController.Ingame.ConfigUpdate += (s, e) => {
                clients.ForEach(c => c.UpdateFrontEnd(IngameOverlay.Instance));
            };
        }


        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {
            //return SendToOthersAsync(context, Encoding.GetString(rxBuffer));
            string message = Encoding.GetString(rxBuffer);
            dynamic res = JsonConvert.DeserializeObject<dynamic>(message);
            if(res.requestType.Equals("overlayConfig", StringComparison.OrdinalIgnoreCase))
            {
                if(clients.Any(c => c.Equals(context)))
                {
                    //Send current config
                    //TODO Change this when multiple frontends support configs like this to only send needed configs
                    SendEventAsync(context, IngameOverlay.Instance);
                }
                clients.Add(new IngameWSClient(context, res.overlayType));
            }
            Log.Info($"Message received: {Encoding.GetString(rxBuffer)}");
            return Task.CompletedTask;
        }

        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            Log.Info($"New Client {context.Id} connected from {context.Origin}");
            if (ConfigController.Component.PickBan.IsActive)
            {
                SendEventAsync(context, new NewState(State.data));
            }
            return Task.CompletedTask;
        }

        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            //return SendToOthersAsync(context, "Someone left the chat room.");
            Log.Info($"Client {context.Id} disconnected");
            return Task.CompletedTask;
        }

        private Task SendToOthersAsync(IWebSocketContext context, string payload)
        {
            return BroadcastAsync(payload, c => c != context);
        }

        public void SendMessageToAllAsync(string payload)
        {
            BroadcastAsync(payload);
        }

        public void SendEventToAllAsync(LeagueEvent leagueEvent)
        {
            BroadcastAsync(JsonConvert.SerializeObject(leagueEvent, Formatting.Indented));
        }

        public void SendEventAsync(IWebSocketContext context, LeagueEvent leagueEvent)
        {
            SendAsync(context, JsonConvert.SerializeObject(leagueEvent, Formatting.Indented));
        }
    }
}
