using LeagueBroadcast.Common.Data.LCU;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame.State
{
    public class CurrentSession
    {
        [JsonPropertyName("isChampSelectActive")]
        public bool IsChampSelectActive { get; set; }
        [JsonPropertyName("session")]
        public Session Session { get; set; }

        public CurrentSession(bool isChampSelectActive, Session session)
        {
            this.IsChampSelectActive = isChampSelectActive;
            this.Session = session;
        }
    }
}
