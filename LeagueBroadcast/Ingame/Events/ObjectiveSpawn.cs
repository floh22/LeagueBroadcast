using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.Ingame.Events
{
    public class ObjectiveSpawn : RiotEvent
    {
        public string ObjectiveName;
        public ObjectiveSpawn(string ObjectiveName, double EventTime)
        {
            this.eventType = "ObjectiveSpawn";
            this.ObjectiveName = ObjectiveName;
            this.EventTime = EventTime;
            this.EventID = -1;
        }
    }

    public class ObjectiveSpawnSimple : LeagueEvent
    {
        public string ObjectiveName;
        public ObjectiveSpawnSimple(string ObjectiveName)
        {
            this.eventType = "ObjectiveSpawn";
            this.ObjectiveName = ObjectiveName;
        }
    }
}
