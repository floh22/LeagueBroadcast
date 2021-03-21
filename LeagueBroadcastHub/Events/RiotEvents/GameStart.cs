
namespace LeagueBroadcastHub.Events.RiotEvents
{
    class GameStart : RiotEvent
    {
        public GameStart(dynamic e) : base("GameStart", (int)e.EventID, (double)e.EventTime) { }
    }
}
