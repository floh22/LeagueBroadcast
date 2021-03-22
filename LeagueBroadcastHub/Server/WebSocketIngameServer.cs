using EmbedIO.WebSockets;
using LeagueBroadcastHub.Events;
using LeagueBroadcastHub.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcastHub.Server
{
    class WebSocketIngameServer : WebSocketModule
    {

        public WebSocketIngameServer(string urlPath) : base(urlPath, true)
        {

        }


        protected override Task OnMessageReceivedAsync(IWebSocketContext context,byte[] rxBuffer,IWebSocketReceiveResult rxResult)
        {
            //return SendToOthersAsync(context, Encoding.GetString(rxBuffer));
            Logging.Info($"Message received: {Encoding.GetString(rxBuffer)}");
            return Task.CompletedTask;
        }

        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            /*
            return Task.WhenAll(
                           SendAsync(context, "Welcome to the chat room!"),
                           SendToOthersAsync(context, "Someone joined the chat room."));
            */
            Logging.Info($"New Client {context.Id} connected from {context.Origin}");
            return Task.CompletedTask;
        }

        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            //return SendToOthersAsync(context, "Someone left the chat room.");
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

        public void SendEventToAllAsync(LeagueEvent leagueEvent) {
            BroadcastAsync(JsonConvert.SerializeObject(leagueEvent));
        }
    }
}
