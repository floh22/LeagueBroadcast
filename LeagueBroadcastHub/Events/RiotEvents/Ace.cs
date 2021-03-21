
namespace LeagueBroadcastHub.Events.RiotEvents
{
    class Ace : RiotEvent
    {
        public string Acer;
        public string AcingTeam;
        public Ace(dynamic e) : base("Ace", (int)e.EventID, (double)e.EventTime)
        {
            this.Acer = e.Acer;
            this.AcingTeam = e.AcingTeam;
        }
    }
}
