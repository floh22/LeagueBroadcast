
namespace LeagueBroadcastHub.Events.RiotEvents
{
    class InhibRespawningSoonEvent : RiotEvent
    {
        public string InhibRespawningSoon;

        public InhibRespawningSoonEvent(dynamic e) : base("InhibRespawningSoon", (int)e.EventID, (double)e.EventTime)
        {
            this.InhibRespawningSoon = e.InhibRespawningSoon;
        }

    }
}
