
namespace LeagueBroadcastHub.Events.RiotEvents
{
    class GameStart : RiotEvent
    {
        public GameStart(dynamic e) : base("GameStart", (int)e.EventID, (double)e.EventTime) { }

        public GameStart() : base("GameStart", -1, 0) { }
    }
}
