
namespace LeagueBroadcastHub.Events.FrontendEvents
{
    class GamePause : LeagueEvent
    {
        public double GameTime;
        public GamePause(double gameTime)
        {
            this.eventType = "GamePause";
            this.GameTime = gameTime;
        }
    }
}
