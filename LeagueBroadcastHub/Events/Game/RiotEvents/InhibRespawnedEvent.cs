
namespace LeagueBroadcastHub.Events.Game.RiotEvents
{
    class InhibRespawnedEvent : RiotEvent
    {
        public string InhibRespawned;

        public InhibRespawnedEvent(dynamic e) : base("InhibRespawned", (int)e.EventID, (double)e.EventTime)
        {
            this.InhibRespawned = e.InhibRespawned;
        }
    }
}
