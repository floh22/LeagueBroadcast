using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.ChampSelect.Events
{
    class ChampSelectStartEvent : LeagueEvent
    {
        public ChampSelectStartEvent()
        {
            eventType = "champSelectStart";
        }
    }
}
