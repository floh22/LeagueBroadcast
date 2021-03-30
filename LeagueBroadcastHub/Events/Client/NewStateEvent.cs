using LeagueBroadcastHub.Data;
using LeagueIngameServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Events.Client
{
    public class NewStateEvent : LeagueEvent
    {
        public State.Client.StateData state;
        public NewStateEvent(State.Client.StateData State)
        {
            this.eventType = "newState";
            this.state = State;
        }
    }
}
