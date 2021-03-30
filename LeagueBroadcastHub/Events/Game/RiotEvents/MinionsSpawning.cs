
namespace LeagueBroadcastHub.Events.Game.RiotEvents
{
    class MinionsSpawning : RiotEvent
    {
        public MinionsSpawning(dynamic e) : base("MinionsSpawning", (int)e.EventID, (double)e.EventTime) { }
    }
}
