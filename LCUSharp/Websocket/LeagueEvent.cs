using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LCUSharp.Websocket
{
    /// <summary>
    /// Represents a league client event.
    /// </summary>
    public class LeagueEvent
    {
        /// <summary>
        /// The event's data.
        /// </summary>
        [JsonProperty("data")]
        public JToken Data { get; set; }

        /// <summary>
        /// The event's type.
        /// </summary>
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// The event's uri.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
