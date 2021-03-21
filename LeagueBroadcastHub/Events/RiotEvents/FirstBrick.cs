
namespace LeagueBroadcastHub.Events.RiotEvents
{
    class FirstBrick : RiotEvent
    {
        public FirstBrick(dynamic e) : base("FirstTower", (int)e.EventID, (double)e.EventTime) { }
    }
}
