using LeagueBroadcast.Common.Data.Events;
using LeagueBroadcast.Common.Data.Pregame.State;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame.Events
{
    public class NewState : BroadcastEvent
    {
        [JsonPropertyName("state")]
        public PregameState State { get; set; }
        public NewState(PregameState State) : base("pregameState")
        {
            this.State = State;
        }
    }
}
