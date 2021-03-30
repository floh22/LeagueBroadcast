using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Events.Client
{
    class ChampSelectEndEvent : LeagueEvent
    {

        public ChampSelectEndEvent()
        {
            eventType = "champSelectEnd";
        }
    }
}
