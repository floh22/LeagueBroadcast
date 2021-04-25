using LeagueBroadcast.ChampSelect.Data.Config;
using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.ChampSelect.Events
{
    class Heartbeat : LeagueEvent
    {
        public PickBanConfig config;
        public Heartbeat(PickBanConfig config)
        {
            this.eventType = "heartbeat";
            this.config = config;
        }
    }
}
