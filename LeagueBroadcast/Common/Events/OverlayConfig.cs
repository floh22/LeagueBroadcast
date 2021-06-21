using LeagueBroadcast.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Common.Events
{
    public abstract class OverlayConfig : LeagueEvent
    {
        public FrontEndType type;

        public OverlayConfig() : base("OverlayConfig") { }
    }
}
