using System.Text;
using System.Threading.Tasks;
using EmbedIO.WebSockets;
using LeagueBroadcast.ChampSelect.Events;
using LeagueBroadcast.ChampSelect.State;
using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.OperatingSystem;
using Newtonsoft.Json;

namespace LeagueBroadcast.Http
{
    class IngameWSServer : WebSocketModule
    {
        public IngameWSServer(string urlPath) : base(urlPath, true)
        {

        }


        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {
            //return SendToOthersAsync(context, Encoding.GetString(rxBuffer));
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
            BroadcastAsync(JsonConvert.SerializeObject(leagueEvent));
        }

        public void SendEventAsync(IWebSocketContext context, LeagueEvent leagueEvent)
        {
            SendAsync(context, JsonConvert.SerializeObject(leagueEvent));
        }
    }
}
