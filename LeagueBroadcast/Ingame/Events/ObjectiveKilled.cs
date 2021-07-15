using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.Ingame.Events
{
    public class ObjectiveKilled : RiotEvent
    {
        public string ObjectiveName;
        public string TeamName;
        public ObjectiveKilled(string ObjectiveName, string TeamName, double gameTime)
        {
            this.eventType = "ObjectiveKilled";
            this.ObjectiveName = ObjectiveName;
            this.TeamName = TeamName;
            this.EventTime = gameTime;
            this.EventID = -1;
        }
    }

    public class ObjectiveKilledSimple : LeagueEvent
    {
        public string ObjectiveName;
        public string TeamName;
        public ObjectiveKilledSimple(string ObjectiveName, string TeamName)
        {
            this.eventType = "ObjectiveKilled";
            this.ObjectiveName = ObjectiveName;
            this.TeamName = TeamName;
        }
    }
}
