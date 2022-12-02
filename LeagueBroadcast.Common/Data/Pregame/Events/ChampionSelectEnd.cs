using LeagueBroadcast.Common.Data.Events;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame.Events
{
    public class ChampionSelectEnd : BroadcastEvent
    {
        [JsonPropertyName("isFinished")]
        public bool IsFinished { get; set; }
        public ChampionSelectEnd(bool isFinished) : base("championSelectEnd")
        {
        }
    }
}
