using LeagueBroadcast.Common.Events;

namespace LeagueBroadcast.Ingame.Events
{
    public class GamePause : LeagueEvent
    {
        public double GameTime;
        public GamePause(double gameTime)
        {
            this.eventType = "GamePause";
            this.GameTime = gameTime;
        }
    }
}
