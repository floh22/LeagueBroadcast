
namespace LeagueBroadcastHub.Events.RiotEvents
{
    class GameEnd : RiotEvent
    {

        public GameEnd(dynamic e) : base("GameEnd", (int)e.EventID, (double)e.EventTime) { }

        public GameEnd() : base("GameEnd", -1, 0) { }
    }
}
