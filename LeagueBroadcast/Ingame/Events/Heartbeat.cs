using LeagueBroadcast.ChampSelect.Data.Config;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Ingame.State;

namespace LeagueBroadcast.Ingame.Events
{
    public class HeartbeatEvent : LeagueEvent
    {
        public StateData stateData;
        //public FrontendConfig config;
        public HeartbeatEvent(StateData stateData)
        {
            this.eventType = "GameHeartbeat";
            this.stateData = stateData;
            //this.config = ConfigController.PickBan.frontend;
        }
    }
}
