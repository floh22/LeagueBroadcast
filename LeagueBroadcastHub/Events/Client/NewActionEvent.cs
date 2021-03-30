using System;
using System.Collections.Generic;
using System.Text;
using static LeagueBroadcastHub.State.Client.StateData;

namespace LeagueBroadcastHub.Events.Client
{
    class NewActionEvent : LeagueEvent
    {
        public CurrentAction action;
        public NewActionEvent(CurrentAction action)
        {
            eventType = "newAction";
            this.action = action;
        }
    }
}
