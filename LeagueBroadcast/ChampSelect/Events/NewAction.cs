using LeagueBroadcast.Common.Events;
using static LeagueBroadcast.ChampSelect.StateInfo.StateData;

namespace LeagueBroadcast.ChampSelect.Events
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
