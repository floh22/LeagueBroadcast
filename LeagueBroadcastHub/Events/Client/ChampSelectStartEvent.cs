using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Events.Client
{
    class ChampSelectStartEvent : LeagueEvent
    {

        public ChampSelectStartEvent()
        {
            eventType = "champSelectStart";
        }
    }
}
