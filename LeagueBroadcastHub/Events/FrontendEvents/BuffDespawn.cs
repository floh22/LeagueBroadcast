
namespace LeagueBroadcastHub.Events.FrontendEvents
{
    class BuffDespawn : LeagueEvent
    {
        public string objective;
        public int teamId;
        public BuffDespawn(string objective, int team)
        {
            this.eventType = "BuffDespawn";
            this.objective = objective;
            this.teamId = team;
        }
    }
}
