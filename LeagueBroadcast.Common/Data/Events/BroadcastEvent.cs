using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Events
{
    public abstract class BroadcastEvent
    {
        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonIgnore]
        public bool WasSent { get; set; }

        public BroadcastEvent(string eventType)
        {
            EventType = eventType;
        }
    }
}
