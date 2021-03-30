using LeagueBroadcastHub.Data;
using LeagueBroadcastHub.State;
using LeagueBroadcastHub.State.Game;
using LeagueIngameServer;

namespace LeagueBroadcastHub.Events.Game.FrontendEvents
{
    class HeartbeatEvent : LeagueEvent
    {
        public StateData stateData;
        public FrontendConfig config;
        public HeartbeatEvent(StateData stateData)
        {
            this.eventType = "GameHeartbeat";
            this.stateData = stateData;
            this.config = BroadcastHubController.ClientConfig.frontend;
        }
    }
}
