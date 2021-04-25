using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.ChampSelect.Events
{
    class ChampSelectEndEvent : LeagueEvent
    {
        public ChampSelectEndEvent()
        {
            eventType = "champSelectEnd";
        }
    }
}
