using EmbedIO.WebSockets;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.OperatingSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Http
{
    class IngameWSClient
    {
        private IWebSocketContext ctx;
        private FrontEndType type;


        public IngameWSClient(IWebSocketContext ctx, List<string> types)
        {
            this.ctx = ctx;
            types.ForEach(t => {
                if(Enum.TryParse(typeof(FrontEndType), t, true, out var res)) {
                    FlagsHelper.Set(ref type, (FrontEndType) res);
                }
            });
        }
        
        public void UpdateFrontEnd(OverlayConfig config)
        {
            if(this.type.HasFlag(config.type))
            {
                EmbedIOServer.SocketServer.SendEventAsync(ctx, config);
            }
        }

        public bool Equals(IWebSocketContext ctx)
        {
            return this.ctx.Equals(ctx);
        }
    }


    [Flags]
    public enum FrontEndType
    {
        ChampSelect,
        Ingame,
        PostGame
    }
}
