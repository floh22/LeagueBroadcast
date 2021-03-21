using LeagueBroadcastHub.State;

namespace LeagueBroadcastHub.Events
{
    class HeartbeatEvent : LeagueEvent
    {
        public StateData stateData;
        public HeartbeatEvent(StateData stateData)
        {
            this.eventType = "Heartbeat";
            this.stateData = stateData;
        }
    }
}
