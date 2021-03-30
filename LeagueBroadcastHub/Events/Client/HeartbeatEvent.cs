using LeagueBroadcastHub.Data.Client.DTO;
using LeagueBroadcastHub.State;
using LeagueBroadcastHub.State.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Events.Client
{
    class HeartbeatEvent : LeagueEvent
    {
        public Config config;
        public HeartbeatEvent(Config config)
        {
            this.eventType = "heartbeat";
            this.config = config;
        }
    }
}
