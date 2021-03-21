
namespace LeagueBroadcastHub.Events.FrontendEvents
{
    class ObjectiveKilled : LeagueEvent
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
