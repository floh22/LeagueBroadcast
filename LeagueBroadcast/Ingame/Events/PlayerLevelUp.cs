using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.Ingame.Events
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
