using LeagueBroadcast.Common.Data.Events;
using LeagueBroadcast.Common.Data.Pregame.State;
using LeagueBroadcast.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Data.Pregame.Events
{
    public class NewAction : BroadcastEvent
    {
        [JsonPropertyName("action")]
        public CurrentAction Action { get; set; }
        public NewAction(CurrentAction action) : base("pregameAction")
        {
            this.Action = action;
        }
    }
}
