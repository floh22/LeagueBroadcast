using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.Ingame.Events
{
    public class GameUnpause : LeagueEvent
    {
        public double GameTime;

        public GameUnpause(double gameTime)
        {
            this.eventType = "GameUnpause";
            this.GameTime = gameTime;
        }
    }
}
