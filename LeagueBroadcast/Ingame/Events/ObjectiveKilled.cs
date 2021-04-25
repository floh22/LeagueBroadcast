using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.Ingame.Events
{
    public class ObjectiveKilled : LeagueEvent
    {
        public string objective;
        public int teamId;
        public ObjectiveKilled(string objective, int team)
        {
            this.eventType = "ObjectiveKilled";
            this.objective = objective;
            this.teamId = team;
        }
    }
}
