
namespace LeagueBroadcastHub.Events.Game.RiotEvents
{
    class FirstBlood : RiotEvent
    {
        public string Recipient;

        public FirstBlood(dynamic e) : base("FirstBlood", (int)e.EventID, (double)e.EventTime)
        {
            this.Recipient = e.Recipient;
        }
    }
}
