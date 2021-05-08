using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.Ingame.Events
{
    public class GameEnd : LeagueEvent
    {
        public int EventID;
        public double EventTime;
        public GameEnd()
        {
            this.eventType = "GameEnd";
            this.EventID = -1;
            this.EventTime = 0;
        }
    }
}
