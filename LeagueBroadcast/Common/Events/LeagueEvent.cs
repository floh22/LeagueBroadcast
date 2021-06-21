namespace LeagueBroadcast.Common.Events
{
    public abstract class LeagueEvent
    {
        public string eventType { get; set; }

        public LeagueEvent(string evenType)
        {
            this.eventType = evenType;
        }

        public LeagueEvent(){ }
    }
}
