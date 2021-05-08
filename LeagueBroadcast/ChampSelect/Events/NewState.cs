using LeagueBroadcast.ChampSelect.StateInfo;
using LeagueBroadcast.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.ChampSelect.Events
{
    public class NewState : LeagueEvent
    {
        public StateData state;
        public NewState(StateData State)
        {
            this.eventType = "newState";
            this.state = State;
        }
    }
}
