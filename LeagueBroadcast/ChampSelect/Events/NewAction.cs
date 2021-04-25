using LeagueBroadcast.Common.Events;
using static LeagueBroadcast.ChampSelect.State.StateData;

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
