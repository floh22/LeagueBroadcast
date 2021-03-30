
namespace LeagueBroadcastHub.Events.Game.RiotEvents
{
    class Multikill : RiotEvent
    {

        public string KillerName;
        public int KillStreak;

        public Multikill(dynamic e) : base("Multikill", (int)e.EventID, (double)e.EventTime)
        {
            this.KillerName = e.KillerName;
            this.KillStreak = e.KillStreak;
        }
    }
}
