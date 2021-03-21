
namespace LeagueBroadcastHub.Events.FrontendEvents
{
    class GameUnpause : LeagueEvent
    {
        public double GameTime;

        public GameUnpause(double gameTime)
        {
            this.eventType = "GameUnpause";
            this.GameTime = gameTime;
        }
    }
}
