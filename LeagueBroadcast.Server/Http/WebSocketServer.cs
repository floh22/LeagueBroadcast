using EmbedIO.WebSockets;
using LeagueBroadcast.Common.Data.Events;
using LeagueBroadcast.Utils.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeagueBroadcast.Server.Http
{
    internal class WebSocketServer : WebSocketModule
    {
        private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

        public event EventHandler<IWebSocketContext>? ClientConnected, ClientDisconnected;
        public event EventHandler<ReceivedMessageArgs>? MessageReceived;


        public WebSocketServer(string urlPath) : base($"/{urlPath}", true)
        {

        }

        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            string message = Encoding.GetString(buffer);
            JsonElement res = JsonSerializer.Deserialize<JsonElement>(message)!;
            string type = res.GetProperty("requestType").GetString() ?? "";

            $"Request of type {type}".Debug();

            return Task.CompletedTask;
        }


        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            $"New Client {context.Id} connected from {context.Origin}".Info();
            ClientConnected?.Invoke(this, context);

            return Task.CompletedTask;
        }

        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            //return SendToOthersAsync(context, "Someone left the chat room.");
            $"Client {context.Id} disconnected".Info();
            return Task.CompletedTask;
        }

        private Task SendToOthersAsync(IWebSocketContext context, string payload)
        {
            return BroadcastAsync(payload, c => c != context);
        }

        public Task BroadcastMessageAsync(string payload)
        {
            return BroadcastAsync(payload);
        }

        
        public Task BroadcastEventAsync(BroadcastEvent leagueEvent)
        {
            leagueEvent.WasSent = true;

            return BroadcastAsync(JsonSerializer.Serialize((object)leagueEvent, SerializerOptions));
        }

        public Task SendEventAsync(IWebSocketContext context, BroadcastEvent leagueEvent)
        {
            leagueEvent.WasSent = true;
            return SendAsync(context, JsonSerializer.Serialize((object)leagueEvent, SerializerOptions));
        }

        
    }

    public class ReceivedMessageArgs
    {
        public IWebSocketContext Context;
        public byte[] Payload;

        public ReceivedMessageArgs(IWebSocketContext context, byte[] payload)
        {
            Context = context;
            Payload = payload;
        }
    }
}
