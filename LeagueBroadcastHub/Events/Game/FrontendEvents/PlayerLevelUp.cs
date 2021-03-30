
namespace LeagueBroadcastHub.Events.Game.FrontendEvents
{
    public class PlayerLevelUp : LeagueEvent
    {
        public int playerId;
        public int level;
        public PlayerLevelUp(int playerId, int level)
        {
            this.eventType = "PlayerLevelUp";
            this.playerId = playerId;
            this.level = level;
        }
    }
}
