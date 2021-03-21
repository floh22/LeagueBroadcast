
namespace LeagueBroadcastHub.Events
{
    public abstract class RiotEvent : LeagueEvent
    {
        public int eventID;
        public double eventTime;
        public RiotEvent(string eventType, int eventID, double eventTime)
        {
            this.eventType = eventType;
            this.eventTime = eventTime;
            this.eventID = eventID;
        }
    }
}
