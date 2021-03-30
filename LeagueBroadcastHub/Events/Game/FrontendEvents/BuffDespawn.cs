namespace LeagueBroadcastHub.Events.Game.FrontendEvents
{
    class BuffDespawn : LeagueEvent
    {
        public string objective;
        public int teamId;
        public BuffDespawn(string objective, int team)
        {
            eventType = "BuffDespawn";
            this.objective = objective;
            teamId = team;
        }
    }
}
